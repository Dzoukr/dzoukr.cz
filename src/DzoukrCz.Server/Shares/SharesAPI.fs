module DzoukrCz.Server.Shares.SharesAPI


open System
open DzoukrCz.Server.MarkdownTools
open DzoukrCz.Server.MoonServer.MoonServerAPI
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
open DzoukrCz.Shared.Shares.API
open DzoukrCz.Server.MoonServer.StoragePublisher
open Newtonsoft.Json.Linq

let private getShare (publisher:Publisher) (i:string) : Task<Response.Share option> =
    task {
        let! share = publisher.FindByMetadataEq(Partitioner.share.PartitionPrefix, MetadataKeys.Share, JValue("true")) |> Task.map List.tryHead
        return
            share
            |> Option.map (fun x ->
                {
                    Id = x.Id
                    Name = x.Name
                    Content = x.Content
                    CreatedAt = x.CreatedAt
                    ExpiresAt = None // TODO:
                } : Response.Share
            )
    }

let private service (publisher:Publisher) : SharesAPI =
    {
        GetShare = getShare publisher >> Async.AwaitTask
    }

let api : HttpHandler =
    Require.services<ILogger<_>, Publisher> (fun logger publisher ->
        Remoting.createApi()
        |> Remoting.withRouteBuilder SharesAPI.RouteBuilder
        |> Remoting.fromValue (service publisher)
        |> Remoting.withErrorHandler (Remoting.errorHandler logger)
        |> Remoting.buildHttpHandler
    )