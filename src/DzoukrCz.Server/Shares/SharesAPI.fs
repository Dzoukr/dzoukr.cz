module DzoukrCz.Server.Shares.SharesAPI


open System
open System.Globalization
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

let private tryDateTimeOffsetUTC (s:string) =
    match DateTimeOffset.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal ||| DateTimeStyles.AdjustToUniversal) with
    | true, dt -> Some dt
    | _ -> None

let private noneIfExpired (share:Response.Share) =
    match share.ExpiresAt with
    | Some x when x < DateTimeOffset.UtcNow -> None
    | _ -> Some share

let private getUnexpiredShare (publisher:Publisher) (i:string) =
    task {
        let fullId = $"{Partitioner.share.PartitionPrefix}{pkDelimiter}{i}"
        let! share =
            publisher.TryDetail fullId
            |> Task.map (Option.bind (fun x ->
                let k = Metadata.tryGet x.Metadata MetadataKeys.Share
                if k = Some(JValue("true")) then Some x
                else None
            ))
        let pwd = share |> Option.bind (fun x -> Metadata.tryGet x.Metadata "share_password") |> Option.map (fun x -> x.Value<string>())

        return
            share
            |> Option.map (fun x ->
                let expiry =
                    Metadata.tryGet x.Metadata "share_expiry"
                    |> Option.bind (fun x -> x.Value<string>() |> tryDateTimeOffsetUTC)
                ({
                    Id = x.Id
                    Name = x.Name
                    Content = x.Content
                    CreatedAt = x.CreatedAt
                    ExpiresAt = expiry
                } : Response.Share)
            )
            |> Option.bind noneIfExpired
            |> Option.map (fun x -> {| Password = pwd; Share = x |})
    }

let private getShare (publisher:Publisher) (i:string) : Task<Response.ShareWrapper option> =
    task {
        let! rawShare = getUnexpiredShare publisher i
        return
            rawShare

            |> Option.map (fun x ->
                if x.Password.IsSome then Response.ShareWrapper.Locked
                else x.Share |> Response.Public
            )
    }

let private unlock (publisher:Publisher) (u:Request.UnlockShare) : Task<Response.ShareWrapper option> =
    task {
        let! rawShare = getUnexpiredShare publisher u.Id
        return
            rawShare
            |> Option.map (fun x ->
                if x.Password = Some u.Password then
                    x.Share |> Response.Unlocked
                else Response.Locked
            )
    }

let private service (publisher:Publisher) : SharesAPI =
    {
        GetShare = getShare publisher >> Async.AwaitTask
        Unlock = unlock publisher >> Async.AwaitTask
    }

let api : HttpHandler =
    Require.services<ILogger<_>, Publisher> (fun logger publisher ->
        Remoting.createApi()
        |> Remoting.withRouteBuilder SharesAPI.RouteBuilder
        |> Remoting.fromValue (service publisher)
        |> Remoting.withErrorHandler (Remoting.errorHandler logger)
        |> Remoting.buildHttpHandler
    )