module DzoukrCz.Client.Server

open DzoukrCz.Shared.Stats.API
open DzoukrCz.Shared.Talks.API
open Fable.SimpleJson
open Fable.Remoting.Client

type ServerError =
    | Exception of string

type ServerResult<'a> = Result<'a,ServerError>

let exnToError (e:exn) : ServerError =
    match e with
    | :? ProxyRequestException as ex ->
        try
            let serverError = Json.parseAs<{| error: ServerError |}>(ex.Response.ResponseBody)
            serverError.error
        with _ -> ServerError.Exception(e.Message)
    | _ -> (ServerError.Exception(e.Message))

module Cmd =
    open Elmish

    module OfAsync =
        let eitherAsResult fn resultMsg =
            Cmd.OfAsync.either fn () (Result.Ok >> resultMsg) (exnToError >> Result.Error >> resultMsg)

let statsAPI =
    Remoting.createApi()
    |> Remoting.withRouteBuilder StatsAPI.RouteBuilder
    |> Remoting.buildProxy<StatsAPI>

let talksAPI =
    Remoting.createApi()
    |> Remoting.withRouteBuilder TalksAPI.RouteBuilder
    |> Remoting.buildProxy<TalksAPI>