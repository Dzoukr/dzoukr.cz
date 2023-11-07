module DzoukrCz.Shared.Shares.API

open System

module Response =
    type Share = {
        Id : string
        Name : string
        Content : string
        CreatedAt : DateTimeOffset
        ExpiresAt : DateTimeOffset option
    }

type SharesAPI = {
    GetShare : string -> Async<Response.Share option>
}
with
    static member RouteBuilder _ m = sprintf "/api/shares/%s" m