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

type RemoteData<'a> =
    | Idle
    | InProgress
    | Finished of ServerResult<'a>

module RemoteData =
    let isInProgress = function
        | InProgress -> true
        | _ -> false
    let isFinished = function
        | Finished _ -> true
        | _ -> false
    let setResponse r = Finished r
    let tryGetFinishedOk = function
        | Finished (Ok v) -> Some v
        | _ -> None

    let map mapFn = function
        | Idle -> Idle
        | InProgress -> InProgress
        | Finished (Error e) -> Finished (Error e)
        | Finished (Ok data) -> data |> mapFn |> Ok |> Finished

    let bind (bindFn:'a -> RemoteData<'b>) = function
        | Idle -> Idle
        | InProgress -> InProgress
        | Finished (Error e) -> Finished (Error e)
        | Finished (Ok data) -> data |> bindFn