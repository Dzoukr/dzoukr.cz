module DzoukrCz.Client.Pages.Blogs

open System
open Feliz
open Feliz.DaisyUI
open Elmish
open Feliz.UseElmish
open DzoukrCz.Client.Server
open DzoukrCz.Client.Router
open DzoukrCz.Client

type private State = {
    Posts : RemoteData<Response.Posts>
}

type private Msg =
    | LoadPosts
    | PostsLoaded of ServerResult<Response.Posts>

let private init (bf:BlogsFilter) () =
    {
        Posts = RemoteData.Idle
    }, Cmd.ofMsg LoadPosts

let private update (msg:Msg) (state:State) : State * Cmd<Msg> =
    match msg with
    // | LoadPosts -> { Posts = RemoteData.InProgress }, Cmd.OfAsync.eitherAsResult (fun _ -> blogsAPI.GetPosts()) PostsLoaded
    | PostsLoaded posts -> { Posts = RemoteData.setResponse posts }, Cmd.none

let private showPostsWithYear (year:int, p:Response.Posts) =
    Html.divClassed "formatted-text" [
        Html.div [
            prop.text year
            prop.className "text-3xl lg:text-4xl font-bold mb-4"
        ]
        Html.divClassed "flex flex-col gap-2" [
            for post in p do
                Html.a [
                    yield! prop.hrefRouted(Page.Web(BlogsDetail post.Rewrite))
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

let private showPosts (px:Response.Posts) =
    Html.divClassed "flex flex-col gap-12 text-center" [
        yield!
            px
            |> List.groupBy (_.PublishedAt.Year)
            |> List.sortByDescending fst
            |> List.map showPostsWithYear
    ]

[<ReactComponent>]
let BlogView (bf:BlogsFilter) =
    let state, dispatch = React.useElmish(init bf, update, [| box bf |])
    match state.Posts with
    | Idle
    | InProgress -> loadingPosts
    | Finished (Error err) -> Html.div (string err)
    | Finished (Ok posts) -> posts |> showPosts