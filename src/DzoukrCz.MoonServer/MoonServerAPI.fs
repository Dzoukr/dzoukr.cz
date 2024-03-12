module DzoukrCz.MoonServer.MoonServerAPI

open System
open System.IO
open System.Text
open DzoukrCz.MoonServer.StoragePublisher
open Giraffe
open Giraffe.GoodRead
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.Logging
open Newtonsoft.Json
open Newtonsoft.Json.Linq

type PublishData = {
    metadata : JObject
    content : string
    name : string
    path : string
    attachments : JObject
}

type DataTable = {
    FileName : string
}

type DataType =
    | DataTable of DataTable

type Metadata = (string * JToken) list

[<RequireQualifiedAccess>]
module DataType =
    [<Literal>]
    let private dtKey = "datatable"

    let private tryDataTable (m:Metadata) =
        m
        |> List.tryFind (fun (x,_) -> x = dtKey)
        |> Option.map snd
        |> Option.map (fun x -> DataTable { FileName = x.Value<string>() })

    let getPrefix = function
        | DataTable _ -> dtKey

    let ofMetadata (m:Metadata) : DataType =
        m
        |> tryDataTable
        |> Option.defaultWith (fun _ -> failwith "Cannot find datatype from metadata properties")

let private toMetadata (j:JObject) =
    j.Properties()
    |> Seq.map (fun v -> v.Name, v.Value)
    |> Seq.toList

let private toJObject (data:Metadata) =
    let j = JObject()
    for (k,v) in data do
        j.Add(k,v)
    j

let private tryFindId (data:Metadata) =
    data
    |> List.tryFind (fun (k,v) -> k = "id")
    |> Option.map snd
    |> Option.bind (fun x ->
        match x with
        | :? JValue -> Some <| x.Value<string>()
        | _ -> None
    )

let private getNewId (metadata:Metadata) =
    let dt = DataType.ofMetadata metadata
    let prefix = (dt |> DataType.getPrefix) + pkDelimiter
    let i = Guid.NewGuid().ToString("N")
    $"{prefix}{i}"

[<RequireQualifiedAccess>]
module PostPublisher =

    let private tableToJArray (tr:MarkdownTools.TableReader) : JArray =
        let arr = JArray()
        for kv in tr.GetKeyValues() do
            let j = JObject()
            for (k,v) in kv do
                j.Add(k, JValue(v))
            arr.Add(j)
        arr

    let postPublish (publisher:Publisher) (i:string) (d:DataType) =
        task {
            match d with
            | DataTable data ->
                let! d = publisher.TryDetail i
                let detail = d.Value
                let tables = detail.Content |> MarkdownTools.findTables
                let arr = tables.[0] |> tableToJArray
                use ms = new MemoryStream()
                use sw = new StreamWriter(ms, Encoding.UTF8)
                use jw = new JsonTextWriter(sw)
                do! arr.WriteToAsync(jw)
                jw.Flush()
                ms.Position <- 0L
                do! publisher.UpsertFile(data.FileName + ".json", ms)
            return ()
        }

let private publish (i:string option) (publisher:Publisher) (next:HttpFunc) (ctx:HttpContext) =
    task {
        let! j = ctx.BindJsonAsync<PublishData>()
        let metadata = j.metadata |> toMetadata
        let dataType = metadata |> DataType.ofMetadata
        let attachments = j.attachments |> toMetadata |> List.map (fun (k,v) -> k, v.Value<string>())
        let pubId = i |> Option.orElse (tryFindId metadata) |> Option.defaultWith (fun _ -> getNewId metadata)
        let! _ = publisher.Publish({ Id = pubId; Metadata = metadata; Content = j.content; Name = j.name; Path = j.path; Attachments = attachments })
        let! _ = PostPublisher.postPublish publisher pubId dataType
        return! json {| id = pubId |} next ctx
    }

let private unpublish (pubId:string) (publisher:Publisher) (next:HttpFunc) (ctx:HttpContext) =
    task {
        let! _ = publisher.Unpublish(pubId)
        return! json {| id = null |} next ctx
    }

let private detail (pubId:string) (publisher:Publisher) (next:HttpFunc) (ctx:HttpContext) =
    task {
        match! publisher.TryDetail(pubId) with
        | Some v ->
            let data =
                {
                    metadata = v.Metadata |> toJObject
                    content = v.Content
                    name = v.Name
                    path = v.Path
                    attachments = JObject.FromObject(obj())
                }
            return! json data next ctx
        | None ->
            ctx.SetStatusCode 404
            return Some ctx
    }

let private onlyWithKeyAndSecret (p:Publisher) : HttpHandler =
    fun (next:HttpFunc) (ctx:HttpContext) ->
        task {
            match ctx.TryGetRequestHeader("Api-Key"), ctx.TryGetRequestHeader("Api-Secret") with
            | Some key, Some secret ->
                if key = p.ApiKey && secret = p.ApiSecret then
                    return! next ctx
                else
                    ctx.SetStatusCode 401
                    return Some ctx
            | _ ->
                ctx.SetStatusCode 401
                return Some ctx
        }

let api : HttpHandler =
    Require.services<ILogger<_>, Publisher> (fun logger publisher ->
        choose [
            POST >=> onlyWithKeyAndSecret publisher >=> choose [
                routeCif "/unpublish/%s" (fun (s:string) -> unpublish s publisher)
                routeCif "/publish/%s"  (fun (s:string) -> publish (Some s) publisher)
                routeCi "/publish" >=> publish None publisher
            ]
            GET >=> routeCif "/detail/%s" (fun (s:string) -> detail s publisher)
        ]
    )