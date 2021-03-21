module DzoukrCz.Client.View

open Feliz
open Router

[<ReactComponent>]
let AppView () =
    let page,setPage = React.useState(Router.currentPath() |> Page.parseFromUrlSegments)

    // routing for full refreshed page (to fix wrong urls)
    React.useEffectOnce (fun _ -> Router.navigatePage page)

    let content =
        match page with
        | Page.AboutMe -> Pages.AboutMe.AboutMeView ()
        | Page.Blog -> Html.text "BLOG"
        | Page.Talks -> Pages.Talks.TalksView()
    React.router [
        router.pathMode
        router.onUrlChanged (Page.parseFromUrlSegments >> setPage)
        router.children [
            content |> Pages.Layout.basic page
        ]
    ]