module DzoukrCz.Server.MoonServer.MoonServerAPI

open System
open DzoukrCz.Server.MoonServer.StoragePublisher
open Giraffe
open Giraffe.GoodRead
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.Logging
open Newtonsoft.Json.Linq

type PublishData = {
    metadata : JObject
    content : string
    name : string
    path : string
    attachments : JObject
}

type Partitioner = {
    PartitionPrefix : string
    MetadataKey : string
}

[<RequireQualifiedAccess>]
module MetadataKeys =
    let DataSource = "datasource"
    let Share = "share"

module Partitioner =
    let data = { PartitionPrefix = "data"; MetadataKey = MetadataKeys.DataSource }
    let share = { PartitionPrefix = "share"; MetadataKey = MetadataKeys.Share }
    let all = [ data; share ]
    let tryFind (metadata:(string * JToken) list) =
        all
        |> List.tryFind (fun p -> metadata |> List.exists (fun (x,_) -> x = p.MetadataKey))

let private toKeyValue (j:JObject) =
    j.Properties()
    |> Seq.map (fun v -> v.Name, v.Value)
    |> Seq.toList

let private fromKeyValue (data:(string * JToken) list) =
    let j = JObject()
    for (k,v) in data do
        j.Add(k,v)
    j

let private tryFindId (data:(string * JToken) list) =
    data
    |> List.tryFind (fun (k,v) -> k = "id")
    |> Option.map snd
    |> Option.bind (fun x ->
        match x with
        | :? JValue -> Some <| x.Value<string>()
        | _ -> None
    )

let private getNewId (metadata:(string * JToken) list) =
    let prefix = Partitioner.tryFind metadata |> Option.map (fun p -> p.PartitionPrefix + pkDelimiter) |> Option.defaultWith (fun _ -> String.Empty)
    let i = Guid.NewGuid().ToString("N")
    $"{prefix}{i}"

let private publish (i:string option) (publisher:Publisher) (next:HttpFunc) (ctx:HttpContext) =
    task {
        let! j = ctx.BindJsonAsync<PublishData>()
        let metadata = j.metadata |> toKeyValue
        let attachments = j.attachments |> toKeyValue |> List.map (fun (k,v) -> k, v.Value<string>())
        let pubId = i |> Option.orElse (tryFindId metadata) |> Option.defaultWith (fun _ -> getNewId metadata)
        let! _ = publisher.Publish({ Id = pubId; Metadata = metadata; Content = j.content; Name = j.name; Path = j.path; Attachments = attachments })
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
                    metadata = v.Metadata |> fromKeyValue
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
        subRouteCi "/moonserver" (
            choose [
                POST >=> onlyWithKeyAndSecret publisher >=> choose [
                    routeCif "/unpublish/%s" (fun (s:string) -> unpublish s publisher)
                    routeCif "/publish/%s"  (fun (s:string) -> publish (Some s) publisher)
                    routeCi "/publish" >=> publish None publisher
                ]
                GET >=> routeCif "/detail/%s" (fun (s:string) -> detail s publisher)
            ]
        )
    )