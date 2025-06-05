module DzoukrCz.MoonServer.MoonServerAPI

open System
open System.IO
open System.Text
open DzoukrCz.MoonServer.Security
open DzoukrCz.MoonServer.StoragePublisher
open FsToolkit.ErrorHandling
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

type BlogPost = {
    Title : string
    Url : string
    Publish : DateTimeOffset
    Tags : string list
    Lang : string
}

type DataType =
    | DataTable of DataTable
    | BlogPost of BlogPost

type Metadata = (string * JToken) list

module Metadata =
    let tryGet (m:Metadata) (k:string) =
        m |> List.tryFind (fun (x,_) -> x.ToLowerInvariant() = k.ToLowerInvariant())
        |> Option.map (fun (_,v) -> v)

    let tryGetString (m:Metadata) (k:string) =
        tryGet m k |> Option.map (fun x -> x.Value<string>())

    let private tryToStringList (xs:JToken seq) =
        try
            xs |> Seq.map (fun x -> x.Value<string>()) |> Seq.toList |> Some
        with _ -> None

    let tryGetStrings (m:Metadata) (k:string) =
        tryGet m k
        |> Option.bind (fun x ->
            if x.HasValues then x.Values() |> tryToStringList
            else None)
        |> Option.defaultValue []

[<RequireQualifiedAccess>]
module DataType =
    [<Literal>]
    let DTKey = "datatable"

    [<Literal>]
    let BPKey = "blog"

    let private tryDataTable (m:Metadata) =
        m
        |> List.tryFind (fun (x,_) -> x = DTKey)
        |> Option.map snd
        |> Option.map (fun x -> DataTable { FileName = x.Value<string>() })

    let private tryBlogPost (m:Metadata) =
        let someTitle = "title" |> Metadata.tryGetString m
        let someUrl = "url" |> Metadata.tryGetString m
        let publish = "publish" |> Metadata.tryGetString m |> Option.map DateTimeOffset.Parse |> Option.defaultWith (fun _ -> DateTimeOffset.Now)
        let lang = "lang" |> Metadata.tryGetString m |> Option.defaultValue "en"
        let tags = "tags" |> Metadata.tryGetStrings m

        Option.map2 (fun title url ->
            {
                Title = title
                Url = url
                Publish = publish
                Tags = tags
                Lang = lang
            }
        ) someTitle someUrl
        |> Option.map BlogPost

    let getPrefix = function
        | DataTable _ -> DTKey
        | BlogPost _ -> BPKey

    let ofMetadata (m:Metadata) : DataType =
        m
        |> tryDataTable
        |> Option.orElseWith (fun _ -> tryBlogPost m)
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

// [<RequireQualifiedAccess>]
// module PostPublisher =
//
//     let private tableToJArray (tr:MarkdownTools.TableReader) : JArray =
//         let arr = JArray()
//         for kv in tr.GetKeyValues() do
//             let j = JObject()
//             for (k,v) in kv do
//                 j.Add(k, JValue(v))
//             arr.Add(j)
//         arr
//
//     let private writeToFile (publisher:Publisher) (filename:string) (jt:JToken) =
//         task {
//             use ms = new MemoryStream()
//             use sw = new StreamWriter(ms, Encoding.UTF8)
//             use jw = new JsonTextWriter(sw)
//             do! jt.WriteToAsync(jw)
//             jw.Flush()
//             ms.Position <- 0L
//             do! publisher.UpsertFile(filename, ms)
//         }
//
//     let private setSharePropsOfBlogPost (i:string) (post:BlogPost) (o:JObject) =
//         o.["Id"] <- i
//         o.["Title"] <- post.Title
//         o.["Url"] <- post.Url
//         o.["Publish"] <- post.Publish.UtcDateTime.ToString("O")
//         o.["Tags"] <- post.Tags |> Array.ofList |> JArray
//         o.["Lang"] <- post.Lang
//
//     let private blogPostToJObject (i:string) (post:BlogPost) (content:string) =
//         let o = JObject()
//         o |> setSharePropsOfBlogPost i post
//         o.["Content"] <- content
//         o
//
//     let private blogPostToJObjectList (i:string) (post:BlogPost) =
//         let o = JObject()
//         o |> setSharePropsOfBlogPost i post
//         o
//
//     let private tryToBlogPostJObject (pr:PublishResponse) =
//         let m = pr.Metadata |> DataType.ofMetadata
//         match m with
//         | DataType.DataTable _ -> None
//         | DataType.BlogPost pb -> Some (blogPostToJObjectList pr.Id pb)
//
//     let private reindexPosts (publisher:Publisher) (idToFilter:string option) =
//         task {
//             // create index
//             let! all =
//                 publisher.FindByPartition DataType.BPKey
//                 |> Task.map (fun x ->
//                     match idToFilter with
//                     | Some i -> x |> List.filter (fun y -> y.Id <> i)
//                     | None -> x
//                 )
//                 |> Task.map (List.choose tryToBlogPostJObject)
//                 |> Task.map (fun x ->
//                     let arr = JArray()
//                     arr.Add(x)
//                     arr
//                 )
//             do! writeToFile publisher $"blogposts.json" all
//         }
//     let private _postPublish (publisher:Publisher) (i:string) (detail:PublishResponse) (dataType:DataType) =
//         task {
//             match dataType with
//             | DataTable data ->
//                 let tables = detail.Content |> MarkdownTools.findTables
//                 let arr = tables.[0] |> tableToJArray
//                 do! writeToFile publisher $"{data.FileName}.json" arr
//             | BlogPost post ->
//                 let ob = detail.Content |> blogPostToJObject i post
//                 do! writeToFile publisher $"blog/{i}.json" ob
//                 do! reindexPosts publisher None
//         }
//
//



let private publish (i:string option) (publisher:Publisher) (next:HttpFunc) (ctx:HttpContext) =
    task {
        let! j = ctx.BindJsonAsync<PublishData>()
        let metadata = j.metadata |> toMetadata
        let attachments = j.attachments |> toMetadata |> List.map (fun (k,v) -> k, v.Value<string>())
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

let api : HttpHandler =
    Require.services<ILogger<_>, Publisher, ApiSecurity> (fun logger publisher security ->
        choose [
            POST >=> onlyWithKeyAndSecret security >=> choose [
                routeCif "/unpublish/%s" (fun (s:string) -> unpublish s publisher)
                routeCif "/publish/%s"  (fun (s:string) -> publish (Some s) publisher)
                routeCi "/publish" >=> publish None publisher
            ]
            GET >=> routeCif "/detail/%s" (fun (s:string) -> detail s publisher)
        ]
    )