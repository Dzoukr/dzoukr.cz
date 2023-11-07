module DzoukrCz.Client.Router

open Browser.Types
open Fable.Core.JsInterop
open Feliz.Router

type WebPage =
    | Index
    | Talks

type ToolPage =
    | Share of string

type Page =
    | Web of WebPage
    | Tool of ToolPage

[<RequireQualifiedAccess>]
module Page =
    let defaultPage = Page.Web Index

    let parseFromUrlSegments = function
        | [ "share"; str ] -> Page.Tool (Share str)
        | [ "talks" ] -> Page.Web Talks
        | [ ] -> Page.Web Index
        | _ -> defaultPage

    let noQueryString segments : string list * (string * string) list = segments, []

    let toUrlSegments = function
        | Page.Web Index -> [ ] |> noQueryString
        | Page.Web Talks -> [ "talks" ] |> noQueryString
        | Page.Tool (Share str) -> [ "share"; str ] |> noQueryString

[<RequireQualifiedAccess>]
module Router =
    let goToUrl (e:MouseEvent) =
        e.preventDefault()
        let href : string = !!e.currentTarget?attributes?href?value
        Router.navigatePath href

    let navigatePage (p:Page) = p |> Page.toUrlSegments |> Router.navigatePath

[<RequireQualifiedAccess>]
module Cmd =
    let navigatePage (p:Page) = p |> Page.toUrlSegments |> Cmd.navigatePath