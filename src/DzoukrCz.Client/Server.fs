module DzoukrCz.Client.Server

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
