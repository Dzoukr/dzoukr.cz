module DzoukrCz.Libraries.StoragePublisher

open System
open System.Collections.Generic
open System.Globalization
open System.Threading.Tasks
open Azure.Data.Tables
open Azure.Data.Tables.FSharp
open Azure.Storage.Blobs
open FSharp.Control
open System.Text.Json
open System.Text.Json.Nodes
open DzoukrCz.Libraries.Publisher

let jsonOptions = JsonSerializerOptions(WriteIndented = false)

let private key (s:string) =
    ["/";"\\";"#";"?"]
    |> List.fold (fun (acc:string) item -> acc.Replace(item, "")) s
    |> _.ToLowerInvariant().Trim()

let pkDelimiter = "-"
let pkKey = "pk"
let pkDefault = "default"

let private partitionAndRow (s:string) =
    let parts = s.Split(pkDelimiter)
    if parts.Length = 1 then
        parts.[0], parts.[0]
    else
        parts.[0], parts.[1]
    |> fun (x,y) -> key x, key y

let private getNewId (metadata:Metadata) =
    let pk =
        metadata
        |> List.tryFind (fun (k,_) -> String.Compare(k, pkKey, StringComparison.InvariantCultureIgnoreCase) = 0)
        |> Option.map (fun (_,v) -> v.ToString())
        |> Option.defaultValue pkDefault
    let i = Guid.NewGuid().ToString("N")
    $"{pk}{pkDelimiter}{i}"

module LinkParser =
    open System.IO
    open System.Text.RegularExpressions

    type MarkdownLinkKind =
        | Image
        | Link

    type MarkdownLink = {
        Outer: string
        InnerText: string
        InnerAlt: string option
        InnerSrc: string
        Filename : string
        Kind: MarkdownLinkKind
    }

    let private getFilename (s:string) =
        s
        |> Path.GetFileName
        |> (fun x -> x.Split("|").[0])

    let private getObsidianKind (f:string) =
        match Path.GetExtension(f).ToLowerInvariant() with
        | ".png" | ".webp" | ".jpg" | ".jpeg" | ".gif" | ".bmp" | ".svg" -> Image
        | _ -> Link

    let private getObsidianLinks (s:string) =
        let pattern = @"\!\[\[(.+)\]\]"
        let options = RegexOptions.Multiline
        let matches = Regex.Matches(s, pattern, options)
        matches |> Seq.map (fun x ->
            let filename = x.Groups.[1].Value |> getFilename
            {
                Outer = x.Groups.[0].Value
                InnerText = ""
                InnerAlt = None
                InnerSrc = x.Groups.[1].Value
                Filename = filename
                Kind = filename |> getObsidianKind
            }
        )
        |> Seq.toList

    let private getSrcAndAlt (s:string) =
        let pattern = "(.+)(\s\"(.+)\")"
        let options = RegexOptions.Multiline
        let matches = Regex.Matches(s, pattern, options)
        match matches.Count with
        | 0 -> (s, None)
        | _ -> (matches.[0].Groups.[1].Value, Some(matches.[0].Groups.[3].Value))

    let private getMarkdownLinks (s:string) =
        let pattern = @"!?\[(.*)\]\((.+)\)"
        let options = RegexOptions.Multiline
        let matches = Regex.Matches(s, pattern, options)
        matches |> Seq.map (fun x ->
            let src, alt = getSrcAndAlt(x.Groups.[2].Value)
            let outer = x.Groups.[0].Value
            {
                Outer = outer
                InnerText = x.Groups.[1].Value
                InnerSrc = src
                InnerAlt = alt
                Filename = src |> getFilename
                Kind = if outer.StartsWith("!") then Image else Link
            }
        )
        |> Seq.toList

    let parse (s:string) =
        let obsidianLinks = getObsidianLinks s
        let markdownLinks = getMarkdownLinks s
        obsidianLinks @ markdownLinks
        |> List.distinctBy (fun x -> x.Filename)

    type FileReplacement = {
        Link : MarkdownLink
        Base64Payload : string
        NewFilename: string
    }

    module FileReplacement =
        let getOuterMarkdown (prefix:string) (item:FileReplacement) =
            let safePrefix = if prefix.EndsWith("/") then prefix else prefix + "/"
            let safeName = if String.IsNullOrWhiteSpace(item.Link.InnerText) then item.Link.Filename else item.Link.InnerText
            match item.Link.Kind with
            | Image ->
                let hover = if item.Link.InnerAlt.IsSome then $" \"{item.Link.InnerAlt.Value}\"" else ""
                $"![{safeName}]({safePrefix}{item.NewFilename}{hover})"
            | Link -> $"[{safeName}]({safePrefix}{item.NewFilename})"
    let private getMd5Hash (input:string) =
        use md5 = System.Security.Cryptography.MD5.Create()
        input
        |> System.Text.Encoding.ASCII.GetBytes
        |> md5.ComputeHash
        |> Array.map (fun x -> x.ToString("X2"))
        |> String.concat String.Empty
        |> _.ToLowerInvariant()

    let private getNewName (link:MarkdownLink) (file:string,base64:string) =
        {
            Link = link
            Base64Payload = base64
            NewFilename = (getMd5Hash base64) + Path.GetExtension(file)
        }

    let withReplacement (files:(string * string) list) (links:MarkdownLink list) =
        links |> List.choose (fun x ->
            files
            |> List.tryFind (fun (y,_) -> y = x.Filename)
            |> Option.map (getNewName x)
        )

    let applyReplacements (prefix:string) (markdown:string) (replaces:FileReplacement list) =
        let folder (acc:string) (item:FileReplacement) =
            acc.Replace(item.Link.Outer, (FileReplacement.getOuterMarkdown prefix item))
        replaces |> List.fold folder markdown

module TableStorage =
    let private ignoredMetadata = ["id"; pkKey]
    
    let toEntity id name path content (meta:Metadata) =
        let partition,row = id |> partitionAndRow
        let entity = TableEntity(partition, row)
        entity.Add("Name", name)
        entity.Add("Path", path)
        entity.Add("Content", content)

        let filteredMeta = meta |> List.filter (fun (k,_) -> not (List.contains (k.ToLowerInvariant()) ignoredMetadata))

        for k,v in filteredMeta do
            let name = $"meta_{k}"
            entity.Add(name, v.ToJsonString(jsonOptions))
        entity
    let toData (e:TableEntity) : PublishResponse =
        let meta =
            (e :> IDictionary<string,obj>)
            |> Seq.filter (fun x -> x.Key.StartsWith("meta_"))
            |> Seq.map (fun x ->
                let key = x.Key.Replace("meta_", "")
                let value = x.Value.ToString()
                key, JsonNode.Parse(value)
            )
            |> Seq.toList
            |> List.append [("id", JsonValue.Create(e.PartitionKey))]

        {
            Id = e.PartitionKey + pkDelimiter + e.RowKey
            Name = e.GetString("Name")
            Path = e.GetString("Path")
            Content = e.GetString("Content")
            Metadata = meta
            CreatedAt = e.Timestamp |> Option.ofNullable |> Option.defaultWith (fun _ -> DateTimeOffset.Now)
        }

type StoragePublisherConfiguration = {
    ConnectionString : string
    TableName : string
    ContainerName : string
    PathPrefix : string
}
with
    member this.SafeContainerName = this.ContainerName |> key

type StoragePublisher(cfg:StoragePublisherConfiguration) =
    let tableClient = TableClient(cfg.ConnectionString, cfg.TableName)
    let blobClient = BlobContainerClient(cfg.ConnectionString, cfg.SafeContainerName)

    let idBasedPrefix (i:string) =
        let safePrefix = if cfg.PathPrefix.EndsWith("/") then cfg.PathPrefix else cfg.PathPrefix + "/"
        $"{safePrefix}{i}/"

    let cleanContainer (i:string) =
        task {
            let! blobs =
                blobClient.GetBlobsAsync(prefix = i).AsPages()
                |> AsyncSeq.ofAsyncEnum
                |> AsyncSeq.map (fun x -> x.Values)
                |> AsyncSeq.concatSeq
                |> AsyncSeq.toListAsync

            for b in blobs do
                let! _ = blobClient.GetBlobClient(b.Name).DeleteAsync()
                ()
        }

    let uploadFiles (i:string) (replaces:LinkParser.FileReplacement list) =
        task {
            for f in replaces do
                let filename = $"{i}/{f.NewFilename}"
                let! _ = blobClient.UploadBlobAsync(filename, BinaryData.FromBytes(Convert.FromBase64String(f.Base64Payload)))
                ()
        }

    interface Publisher with
        member _.Publish(data:PublishRequest) =
            task {
                // table
                let! _ = tableClient.CreateIfNotExistsAsync()
                let id = data.Id |> Option.defaultWith (fun _ -> getNewId data.Metadata)
                let replaces = data.Content |> LinkParser.parse |> LinkParser.withReplacement data.Attachments
                let newData = { data with Content = replaces |> LinkParser.applyReplacements (idBasedPrefix id) data.Content }
                let entity = TableStorage.toEntity id newData.Name newData.Path newData.Content newData.Metadata
                let! _ = tableClient.UpsertEntityAsync(entity, TableUpdateMode.Replace)

                // blobs
                let! _ = blobClient.CreateIfNotExistsAsync()
                do! cleanContainer id
                do! uploadFiles id replaces
                return id
            }

        member _.Unpublish(i:string) =
            task {
                // table
                let partition,row = i |> partitionAndRow
                let! _ = tableClient.CreateIfNotExistsAsync()
                let! _ = tableClient.DeleteEntityAsync(key partition, key row)
                // blobs
                let! _ = blobClient.CreateIfNotExistsAsync()
                do! cleanContainer i
                return ()
            }

        member _.TryDetail (i:string) =
            task {
                let! _ = tableClient.CreateIfNotExistsAsync()
                let partition,row = i |> partitionAndRow
                return
                    tableQuery {
                        filter (pk partition + rk row)
                    }
                    |> tableClient.Query<TableEntity>
                    |> Seq.map TableStorage.toData
                    |> Seq.tryHead
            }

        member _.FindByMetadataEq (partitionKey:string option, name:string, value:JsonNode) : Task<PublishResponse list> =
            task {
                let! _ = tableClient.CreateIfNotExistsAsync()
                let partitionKey = partitionKey |> Option.defaultValue pkDefault
                return
                    tableQuery {
                        filter (pk partitionKey + eq $"meta_{name}" (value.ToJsonString(jsonOptions)))
                    }
                    |> tableClient.Query<TableEntity>
                    |> Seq.map TableStorage.toData
                    |> Seq.toList
            }

        member _.FindByPartition (partitionKey:string option) : Task<PublishResponse list> =
            task {
                let! _ = tableClient.CreateIfNotExistsAsync()
                let partitionKey = partitionKey |> Option.defaultValue pkDefault
                return
                    tableQuery {
                        filter (pk partitionKey)
                    }
                    |> tableClient.Query<TableEntity>
                    |> Seq.map TableStorage.toData
                    |> Seq.toList
            }