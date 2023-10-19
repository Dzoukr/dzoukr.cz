module DzoukrCz.Server.MoonServer.MoonServerAPI

open System
open DzoukrCz.Server.MoonServer.StoragePublisher
open Giraffe
open Giraffe.GoodRead
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.Logging
open Newtonsoft.Json.Linq

type PublishRequest = {
    metadata : JObject
    content : string
    name : string
    path : string
    attachments : JObject
}

let private toKeyValue (j:JObject) =
    j.Properties()
    |> Seq.map (fun v -> v.Name, v.Value)
    |> Seq.toList

let private tryFindId (data:(string * JToken) list) =
    data
    |> List.tryFind (fun (k,v) -> k = "id")
    |> Option.map snd
    |> Option.bind (fun x ->
        match x with
        | :? JValue -> Some <| x.Value<string>()
        | _ -> None
    )

let private publish (i:string option) (publisher:Publisher) (next:HttpFunc) (ctx:HttpContext) =
    task {
        let! j = ctx.BindJsonAsync<PublishRequest>()
        let metadata = j.metadata |> toKeyValue
        let attachments = j.attachments |> toKeyValue |> List.map (fun (k,v) -> k, v.Value<string>())
        let pubId = i |> Option.orElse (tryFindId metadata) |> Option.defaultWith (fun _ -> Guid.NewGuid().ToString("N"))
        let! _ = publisher.Publish({ Id = pubId; Metadata = metadata; Content = j.content; Name = j.name; Path = j.path; Attachments = attachments })
        return! json {| id = pubId |} next ctx
    }

let private unpublish (pubId:string) (next:HttpFunc) (ctx:HttpContext) =
    task {
        return! json {| id = null |} next ctx
    }

let api : HttpHandler =
    Require.services<ILogger<_>, Publisher> (fun logger publisher ->
        subRouteCi "/moonserver" (
            choose [
                POST >=> routeCif "/publish/%s"  (fun (s:string) -> publish (Some s) publisher)
                POST >=> routeCi "/publish" >=> publish None publisher
                POST >=> routeCif "/unpublish/%s" (fun (s:string) -> unpublish s)
                GET >=> routeCif "/detail/%s" (fun (s:string) -> text $"detail {s}")
            ]
        )
    )
