module DzoukrCz.Client.Pages.Blogs

open System
open Feliz
open Feliz.DaisyUI
open Elmish
open Feliz.UseElmish
open DzoukrCz.Client.Server
open DzoukrCz.Client.Router
open DzoukrCz.Client
open Fable.SimpleJson
open Fable.SimpleHttp

type internal BlogPostItemRaw = {
    Id : string
    Title : string
    Url : string
    Publish : string
    Tags : string []
    Lang : string
}

type BlogPostItem = {
    Id : string
    Title : string
    Url : string
    Publish : DateTimeOffset
    Tags : string list
}

module BlogPostItem =
    let cleanTag (s:string) : string =
        s.Replace("blog/","")

    let internal ofRaw (p:BlogPostItemRaw) =
        {
            Id = p.Id
            Title = p.Title
            Url = p.Url
            Publish = p.Publish |> DateTimeOffset.Parse
            Tags = p.Tags |> Array.map cleanTag |> Array.toList
        }

type private State = {
    Posts : RemoteData<BlogPostItem list>
}

type private Msg =
    | LoadPosts
    | PostsLoaded of ServerResult<BlogPostItem list>

let private init (bf:BlogsFilter) () =
    {
        Posts = RemoteData.Idle
    }, Cmd.ofMsg LoadPosts

let getBlogPosts () =
    async {
        let! (_, responseText) = Http.get "https://media.dzoukr.cz/blogposts.json"
        return responseText |> Json.parseNativeAs<BlogPostItemRaw []> |> Array.toList |> List.map BlogPostItem.ofRaw
    }

let private update (msg:Msg) (state:State) : State * Cmd<Msg> =
    match msg with
    | LoadPosts -> { Posts = RemoteData.InProgress }, Cmd.OfAsync.eitherAsResult getBlogPosts PostsLoaded
    | PostsLoaded posts -> { Posts = RemoteData.setResponse posts }, Cmd.none

let private showPostsWithYear (year:int, p:BlogPostItem list) =
    Html.divClassed "formatted-text" [
        Html.div [
            prop.text year
            prop.className "text-3xl lg:text-4xl font-bold mb-4"
        ]
        Html.divClassed "flex flex-col gap-2" [
            for post in p do
                Html.a [
                    yield! prop.hrefRouted(Page.Web(BlogsDetail post.Url))
                    prop.text post.Title
                ]
        ]
    ]

let private loadingPosts =
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

let private showPosts (px:BlogPostItem list) =
    match px with
    | [] ->
        Html.divClassed "formatted-text flex flex-col h-full justify-center" [
            Html.divClassed "" [
                Html.faIcon "fa-regular fa-face-sad-tear mr-2"
                Html.span "No posts published yet..."
            ]
        ]
    | px ->
        Html.divClassed "formatted-text flex flex-col gap-12 text-center" [
            yield!
                px
                |> List.groupBy (_.Publish.Year)
                |> List.sortByDescending fst
                |> List.map showPostsWithYear
        ]

let private showFilter (bf:BlogsFilter) =
    match bf with
    | { Tag = Some str } ->
        Html.divClassed "pb-8 formatted-text" [
            Daisy.alert [
                prop.children [
                    Html.faIcon "fa-solid fa-circle-info"
                    Html.span [
                        Html.text $"Displaying only posts tagged with "
                        Html.strong $"#{str}"
                    ]
                    Daisy.button.div [
                        prop.text "Show all"
                        yield! prop.hrefRouted(Page.Web <| Blogs BlogsFilter.empty)
                    ]
                ]
            ]
        ]
    | { Tag = None } -> Html.none

let private show (bf:BlogsFilter) (px:BlogPostItem list) =
    React.fragment [
        showFilter bf
        showPosts px
    ]


let private filterPosts (f:BlogsFilter) (px:BlogPostItem list) : BlogPostItem list =
    match f with
    | { Tag = Some stringOption } -> px |> List.filter (fun x -> x.Tags |> List.contains stringOption)
    | { Tag = None } -> px

[<ReactComponent>]
let BlogView (bf:BlogsFilter) =
    let state, dispatch = React.useElmish(init bf, update, [| box bf |])
    match state.Posts with
    | Idle
    | InProgress -> loadingPosts
    | Finished (Error err) -> Html.div (string err)
    | Finished (Ok posts) ->
        posts
        |> filterPosts bf
        |> show bf