module DzoukrCz.Server.MoonServer.StoragePublisher

open System
open Azure.Data.Tables
open Azure.Data.Tables.FSharp
open Azure.Storage.Blobs
open FSharp.Control
open Newtonsoft.Json
open Newtonsoft.Json.Linq

let private key (s:string) =
    ["/";"\\";"#";"?"]
    |> List.fold (fun (acc:string) item -> acc.Replace(item, "")) s
    |> (fun x -> x.ToLowerInvariant().Trim())

type PublishData = {
    Id : string
    Name : string
    Path : string
    Content : string
    Metadata : (string * JToken) list
    Attachments : (string * string) list
}

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

    let private getNewName (link:MarkdownLink) (file:string,base64:string) =
        {
            Link = link
            Base64Payload = base64
            NewFilename = Guid.NewGuid().ToString("N") + Path.GetExtension(file)
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
    let toEntity (data:PublishData) =
        let id = data.Id |> key
        let entity = TableEntity(id, id)
        entity.Add("Name", data.Name)
        entity.Add("Path", data.Path)
        entity.Add("Content", data.Content)
        for k,v in data.Metadata |> List.filter (fun (x,_) -> x.ToLowerInvariant() <> "id") do
            let name = $"meta_{k}"
            entity.Add(name, v.ToString(Formatting.None))
        entity


type Configuration = {
    ConnectionString : string
    TableName : string
    ContainerName : string
    PathPrefix : string
}
with
    member this.SafeContainerName = this.ContainerName |> key

type Publisher(cfg:Configuration) =
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

    member _.Publish(data:PublishData) =
        task {
            // table
            let! _ = tableClient.CreateIfNotExistsAsync()
            let replaces = data.Content |> LinkParser.parse |> LinkParser.withReplacement data.Attachments
            let newData = { data with Content = replaces |> LinkParser.applyReplacements (idBasedPrefix data.Id) data.Content }
            let entity = newData |> TableStorage.toEntity
            let! _ = tableClient.UpsertEntityAsync(entity, TableUpdateMode.Replace)

            // blobs
            let! _ = blobClient.CreateIfNotExistsAsync()
            do! cleanContainer data.Id
            do! uploadFiles data.Id replaces
            return ()
        }