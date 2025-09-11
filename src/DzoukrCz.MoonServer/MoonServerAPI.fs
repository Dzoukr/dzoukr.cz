module DzoukrCz.MoonServer.MoonServerAPI

open System
open System.IO
open System.Text
open System.Text.Json
open System.Text.Json.Nodes
open DzoukrCz.MoonServer.Security
open DzoukrCz.Libraries.Publisher
open FsToolkit.ErrorHandling
open Giraffe
open Giraffe.GoodRead
open Giraffe.EndpointRouting
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.Logging
open System.Linq

type PublishData = {
    metadata : JsonObject
    content : string
    name : string
    path : string
    attachments : JsonObject
}

type Metadata = (string * JsonNode) list

let private toMetadata (j:JsonObject) =
    j.AsEnumerable()
    |> Seq.map (fun kv -> kv.Key, kv.Value)
    |> Seq.toList

let private toJObject (data:Metadata) =
    let j = JsonObject()
    for (k,v) in data do
        j[k] <- v
    j

let private publish (somePubId:string option) (publisher:Publisher) (next:HttpFunc) (ctx:HttpContext) =
    task {
        let! j = ctx.BindJsonAsync<PublishData>()
        let metadata = j.metadata |> toMetadata
        let attachments = j.attachments |> toMetadata |> List.map (fun (k,v) -> k, v.ToString())
        let! pubId = publisher.Publish({ Id = somePubId; Metadata = metadata; Content = j.content; Name = j.name; Path = j.path; Attachments = attachments })
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
                    attachments = JsonObject()
                }
            return! json data next ctx
        | None ->
            ctx.SetStatusCode 404
            return Some ctx
    }

let private onlyWithKeyAndSecret (p:ApiSecurity) : HttpHandler =
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

let private withSecuredPublisher handler : HttpHandler =
    Require.services<Publisher, ApiSecurity> (fun publisher security ->
        onlyWithKeyAndSecret security >=> handler publisher)

let api =
    [
        POST [
            routef "/unpublish/%s" (fun i -> withSecuredPublisher (unpublish i))
            routef "/publish/%s" (fun i -> withSecuredPublisher (publish (Some i)))
            route "/publish" (withSecuredPublisher (publish None))
        ]
        GET [
            routef "/detail/%s" (fun i -> withSecuredPublisher (detail i))
        ]
    ]