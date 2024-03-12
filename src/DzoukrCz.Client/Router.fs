module DzoukrCz.Client.Router

open Browser.Types
open Fable.Core.JsInterop
open Feliz.Router

module private Paths =
    // let [<Literal>] Share = "share"
    let [<Literal>] Talks = "talks"
    let [<Literal>] Blog = "blog"

type BlogsFilter = {
    Tag : string option
}

module BlogsFilter =
    let empty = {
        Tag = None
    }
    let withTag (s:string) (f:BlogsFilter) =
        { f with Tag = s |> Option.ofObj }


type WebPage =
    | Index
    | Talks
    | Blogs of BlogsFilter
    | BlogsDetail of string

type ToolPage =
    | Share of string

type Page =
    | Web of WebPage
    // | Tool of ToolPage

[<RequireQualifiedAccess>]
module Page =
    let defaultPage = Page.Web Index

    let filterToQueryString (f:BlogsFilter) =
        f.Tag
        |> Option.map (fun x -> ["tag",x])
        |> Option.defaultValue []

    let parseFromUrlSegments = function
        // | [ Paths.Share; str ] -> Page.Tool (Share str)
        | [ Paths.Talks ] -> Page.Web Talks
        | [ Paths.Blog; Route.Query [ "tag", tag ] ] -> Page.Web (Blogs (BlogsFilter.empty |> BlogsFilter.withTag tag))
        | [ Paths.Blog; str ] -> Page.Web (BlogsDetail str)
        | [ Paths.Blog ] -> Page.Web (Blogs BlogsFilter.empty)
        | [ ] -> Page.Web Index
        | _ -> defaultPage

    let noQueryString segments : string list * (string * string) list = segments, []

    let toUrlSegments = function
        | Page.Web Index -> [ ] |> noQueryString
        | Page.Web Talks -> [ Paths.Talks ] |> noQueryString
        | Page.Web (Blogs f) -> [ Paths.Blog ], (filterToQueryString f)
        | Page.Web (BlogsDetail str) -> [ Paths.Blog; str ] |> noQueryString
        // | Page.Tool (Share str) -> [ Paths.Share; str ] |> noQueryString

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