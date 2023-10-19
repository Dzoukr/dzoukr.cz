module DzoukrCz.Server.Stats.StatsAPI


open System
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

let private getStats () : Task<Response.IndexStats> =
    task {
        return {
            Talks = "26"
            Episodes = "40"
            Downloads = "611k+"
        }
    }

let private service : StatsAPI =
    {
        GetStats = getStats >> Async.AwaitTask
    }

let api : HttpHandler =
    Require.services<ILogger<_>> (fun logger ->
        Remoting.createApi()
        |> Remoting.withRouteBuilder StatsAPI.RouteBuilder
        |> Remoting.fromValue (service)
        |> Remoting.withErrorHandler (Remoting.errorHandler logger)
        |> Remoting.buildHttpHandler
    )