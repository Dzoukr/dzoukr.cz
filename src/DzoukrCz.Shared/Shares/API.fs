module DzoukrCz.Shared.Shares.API

open System

module Request =
    type UnlockShare = {
        Id : string
        Password : string
    }

module Response =
    type Share = {
        Id : string
        Name : string
        Content : string
        CreatedAt : DateTimeOffset
        ExpiresAt : DateTimeOffset option
    }
    type ShareWrapper =
        | Public of Share
        | Locked
        | Unlocked of Share

type SharesAPI = {
    GetShare : string -> Async<Response.ShareWrapper option>
    Unlock : Request.UnlockShare -> Async<Response.ShareWrapper option>
}
with
    static member RouteBuilder _ m = sprintf "/api/shares/%s" m