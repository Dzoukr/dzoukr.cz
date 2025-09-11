module DzoukrCz.MoonServer.DataAPI

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
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.Logging
open System.Linq
open Giraffe.EndpointRouting
open DzoukrCz.Libraries


let private tableToJsonArray (tr:MarkdownTools.TableReader) =
    let arr = JsonArray()
    for kv in tr.GetKeyValues() do
        let j = JsonObject()
        for (k,v) in kv do
            j.Add(k, JsonValue.Create(v))
        arr.Add(j)
    arr

let private replaceMarkdownLinks (arr:JsonArray) : JsonArray =
    let newArr = JsonArray()
    for item in arr do
        if item :? JsonObject then
            let obj = item :?> JsonObject
            for kv in obj do
                match kv.Value.GetValue() |> MarkdownTools.trySrcAndAlt with
                | Some (src, _) -> obj[kv.Key] <- JsonValue.Create(src)
                | None -> ()
            newArr.Add(obj.DeepClone())
        else
            newArr.Add(item)
    newArr


let private getTalks (publisher:Publisher) (next:HttpFunc) (ctx:HttpContext) =
    task {
        let! items = publisher.FindByMetadataEq(Some "data", "source", "talks")
        let resp = 
            items
            |> List.tryHead
            |> Option.map (fun i -> i.Content)
            |> Option.bind (fun c -> MarkdownTools.findTables c |> List.tryHead)
            |> Option.map tableToJsonArray
            |> Option.map replaceMarkdownLinks
            |> Option.defaultValue (JsonArray())
        return! json resp next ctx
    }

let private withPublisher (handler:Publisher -> HttpHandler) : HttpHandler = Require.services<Publisher> (fun publisher -> handler publisher)

let api  =
    [
        subRoute "/data" [
            GET [
                route "/talks" (withPublisher getTalks)
            ]
        ]
    ]    