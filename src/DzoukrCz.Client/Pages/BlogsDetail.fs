module DzoukrCz.Client.Pages.BlogsDetail

open System
open DzoukrCz.Client.Components
open Feliz
open Feliz.DaisyUI
open Elmish
open Feliz.UseElmish
open DzoukrCz.Client.Server
open DzoukrCz.Client.Router
open DzoukrCz.Client

type private State = {
    Rewrite : string
    Posts : RemoteData<Response.PostDetail option>
}

type private Msg =
    | LoadPost
    | PostLoaded of ServerResult<Response.PostDetail option>

let private init rewrite () =
    {
        Rewrite = rewrite
        Posts = RemoteData.Idle
    }, Cmd.ofMsg LoadPost

let private update (msg:Msg) (state:State) : State * Cmd<Msg> =
    match msg with
    // | LoadPost -> { state with Posts = RemoteData.InProgress }, Cmd.OfAsync.eitherAsResult (fun _ -> blogsAPI.GetPost state.Rewrite) PostLoaded
    | PostLoaded posts -> { state with Posts = RemoteData.setResponse posts }, Cmd.none

let private loadingPost =
    [0..1]
    |> List.map (fun _ ->
        Html.divClassed "formatted-text w-[56rem]" [
            Html.loading "w-4/12 mx-auto"
            Html.loading "w-8/12 mx-auto"
            Html.loading "w-7/12 mx-auto"
            Html.loading "w-9/12 mx-auto"
        ]
    )
    |> React.fragment

let private showPost (px:Response.PostDetail) =
    Html.divClassed "markdown-viewer formatted-text" [
        Html.h1 [
            prop.text px.Title
            prop.className "text-center"
        ]
        Html.divClassed "flex flex-col gap-2 items-center mb-10 lg:mb-12 text-lg" [
            Html.div ("Published " + px.PublishedAt.ToString("dd. MM. yyyy"))
            Html.divClassed "flex flex-row gap-2 items-center text-lg" [
                for t in px.Tags do
                    Html.a [
                        prop.text $"#{t}"
                        yield! prop.hrefRouted (Page.Web (WebPage.Blogs (BlogsFilter.empty |> BlogsFilter.withTag t)))
                    ]
            ]
        ]
        MarkdownViewer.ViewMarkdown px.Content
    ]

let private noPost =
    Html.div "NO DETAIL"

[<ReactComponent>]
let BlogsDetailView (rewrite:string) =
    let state, dispatch = React.useElmish(init rewrite, update, [| box rewrite |])
    match state.Posts with
    | Idle
    | InProgress -> loadingPost
    | Finished (Error err) -> Html.div (string err)
    | Finished (Ok post) -> post |> Option.map showPost |> Option.defaultValue noPost