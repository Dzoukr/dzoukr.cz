module DzoukrCz.Client.Router

open Browser.Types
open Feliz.Router
open Fable.Core.JsInterop

type Page =
    | AboutMe
    | Blog
    | Talks
    | Projects

[<RequireQualifiedAccess>]
module Page =
    let defaultPage = Page.AboutMe

    let parseFromUrlSegments = function
        | [ ] -> Page.AboutMe
        | [ "blog" ] -> Page.Blog
        | [ "talks" ] -> Page.Talks
        | [ "projects" ] -> Page.Projects
        | _ -> defaultPage

    let noQueryString segments : string list * (string * string) list = segments, []

    let toUrlSegments = function
        | Page.AboutMe -> [ ] |> noQueryString
        | Page.Blog -> [ "blog" ] |> noQueryString
        | Page.Talks -> [ "talks" ] |> noQueryString
        | Page.Projects -> [ "projects" ] |> noQueryString

[<RequireQualifiedAccess>]
module Router =
    let goToUrl (e:MouseEvent) =
        e.preventDefault()
        let href : string = !!e.currentTarget?attributes?href?value
        Router.navigatePath href

    let navigatePage (p:Page) = p |> Page.toUrlSegments |> Router.navigatePath