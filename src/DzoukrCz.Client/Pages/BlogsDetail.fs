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
open Fable.SimpleJson
open Fable.SimpleHttp

type internal BlogPostRaw = {
    Id : string
    Title : string
    Url : string
    Publish : string
    Content : string
    Tags : string []
    Lang : string
}

type internal BlogPost = {
    Id : string
    Title : string
    Url : string
    Publish : DateTimeOffset
    Content : string
    Tags : string list
    Lang : string
}

module BlogPost =
    let internal ofRaw (p:BlogPostRaw) : BlogPost =
        {
            Id = p.Id
            Title = p.Title
            Url = p.Url
            Publish = p.Publish |> DateTimeOffset.Parse
            Content = p.Content
            Tags = p.Tags |> Array.map Blogs.BlogPostItem.cleanTag |> Array.toList
            Lang = p.Lang
        }

type private State = {
    Url : string
    Posts : RemoteData<Blogs.BlogPostItem list>
    PostDetail : RemoteData<BlogPost option>
}

type private Msg =
    | LoadPosts
    | PostsLoaded of ServerResult<Blogs.BlogPostItem list>
    | LoadSinglePost of url:string
    | SinglePostLoaded of ServerResult<BlogPost option>

let private init rewrite () =
    {
        Url = rewrite
        Posts = RemoteData.Idle
        PostDetail = RemoteData.Idle
    }, Cmd.ofMsg LoadPosts

let private getSingleBlogPost (allPosts:Blogs.BlogPostItem list) (url:string) =
    async {
        let item = allPosts |> List.tryFind (fun x -> x.Url = url)
        match item with
        | Some i ->
            let! (_, responseText) = Http.get $"https://media.dzoukr.cz/blog/{i.Id}.json"
            return responseText |> Json.parseNativeAs<BlogPostRaw> |> BlogPost.ofRaw |> Some
        | None -> return None
    }

let private update (msg:Msg) (state:State) : State * Cmd<Msg> =
    match msg with
    | LoadPosts -> { state with Posts = RemoteData.InProgress }, Cmd.OfAsync.eitherAsResult Blogs.getBlogPosts PostsLoaded
    | PostsLoaded posts -> { state with Posts = RemoteData.setResponse posts }, Cmd.ofMsg (LoadSinglePost state.Url)
    | LoadSinglePost u ->
        let all = state.Posts |> RemoteData.tryGetFinishedOk |> Option.defaultValue []
        { state with PostDetail = RemoteData.InProgress }, Cmd.OfAsync.eitherAsResult (fun _ -> getSingleBlogPost all u) SinglePostLoaded
    | SinglePostLoaded p -> { state with PostDetail = RemoteData.setResponse p }, Cmd.none

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

let private showPost (px:BlogPost) =
    Html.divClassed "markdown-viewer formatted-text" [
        Html.h1 [
            prop.text px.Title
            prop.className "text-center"
        ]
        Html.divClassed "flex flex-col gap-2 items-center mb-10 lg:mb-12 text-lg" [
            Html.div ("Published " + px.Publish.ToString("dd. MM. yyyy"))
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
    match state.PostDetail with
    | Idle
    | InProgress -> loadingPost
    | Finished (Error err) -> Html.div (string err)
    | Finished (Ok post) -> post |> Option.map showPost |> Option.defaultValue noPost