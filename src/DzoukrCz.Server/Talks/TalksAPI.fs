module DzoukrCz.Server.Talks.TalksAPI

open System
open DzoukrCz.Server.MarkdownTools
open FsToolkit.ErrorHandling
open Giraffe
open Giraffe.GoodRead
open Fable.Remoting.Server
open Fable.Remoting.Giraffe
open Microsoft.AspNetCore.Authentication.Cookies
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.Logging
open Microsoft.AspNetCore.Authentication
open System.Threading.Tasks
open DzoukrCz.Shared.Talks.API
open DzoukrCz.Server.MoonServer.StoragePublisher
open Newtonsoft.Json.Linq

let private someString (s:string) = if String.IsNullOrWhiteSpace s then None else Some s
let private toLang (s:string) : TalkLang =
    match s.ToLowerInvariant() with
    | "cz" -> TalkLang.CZ
    | _ -> TalkLang.EN

let private getTalks (publisher:Publisher) () : Task<Response.Talk list> =
    task {
        let! data = publisher.FindByMetadataEq("datasource", JValue("talks")) |> Task.map List.tryHead
        return
            data
            |> Option.bind (fun x ->
                x.Content
                |> MarkdownTools.findTables
                |> List.tryHead
                |> Option.map (fun x ->
                    x.GetDataRowsOrEmpty()
                    |> List.map (fun get ->
                        {
                            Date = get "Date" |> DateTime.Parse
                            Event = get "Event"
                            Title = get "Title"
                            Location = get "Place"
                            Link = get "Link" |> someString |> Option.bind MarkdownTools.trySrcAndAlt |> Option.map fst
                            Lang = get "Lang" |> toLang
                            Logo = get "Logo" |> someString |> Option.bind MarkdownTools.trySrcAndAlt |> Option.map fst
                        } : Response.Talk
                    )
                )
            )
            |> Option.defaultValue []
    }

let private service (publisher:Publisher) : TalksAPI =
    {
        GetTalks = getTalks publisher >> Async.AwaitTask
    }

let api : HttpHandler =
    Require.services<ILogger<_>, Publisher> (fun logger publisher ->
        Remoting.createApi()
        |> Remoting.withRouteBuilder TalksAPI.RouteBuilder
        |> Remoting.fromValue (service publisher)
        |> Remoting.withErrorHandler (Remoting.errorHandler logger)
        |> Remoting.buildHttpHandler
    )