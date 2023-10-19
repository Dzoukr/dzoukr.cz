module DzoukrCz.Server.Stats.StatsAPI


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
open DzoukrCz.Shared.Stats.API
open DzoukrCz.Server.MoonServer.StoragePublisher
open Newtonsoft.Json.Linq

let private getStats (publisher:Publisher) () : Task<Response.IndexStats> =
    task {
        let! stats = publisher.FindByMetadataEq("datasource", JValue("stats")) |> Task.map List.tryHead
        return
            stats
            |> Option.bind (fun x ->
                x.Content
                |> MarkdownTools.findTables
                |> List.tryHead
                |> Option.map (fun x ->
                    {
                        Talks = x.GetCellValueOrEmpty(0, "Talks")
                        Episodes = x.GetCellValueOrEmpty(0, "Episodes")
                        Downloads = x.GetCellValueOrEmpty(0, "Downloads")
                    } : Response.IndexStats
                )
            )
            |> Option.defaultValue Response.IndexStats.init
    }

let private service (publisher:Publisher) : StatsAPI =
    {
        GetStats = getStats publisher >> Async.AwaitTask
    }

let api : HttpHandler =
    Require.services<ILogger<_>, Publisher> (fun logger publisher ->
        Remoting.createApi()
        |> Remoting.withRouteBuilder StatsAPI.RouteBuilder
        |> Remoting.fromValue (service publisher)
        |> Remoting.withErrorHandler (Remoting.errorHandler logger)
        |> Remoting.buildHttpHandler
    )