module DzoukrCz.Client.View

open DzoukrCz.Client.Router
open Feliz
open Feliz.DaisyUI
open Feliz.UseElmish
open Feliz.Router
open Elmish

type private Msg =
    | UrlChanged of Page

type private State = {
    Page : Page
}

let private init () =
    let nextPage = Router.currentPath() |> Page.parseFromUrlSegments
    { Page = nextPage }, Cmd.navigatePage nextPage

let private update (msg:Msg) (state:State) : State * Cmd<Msg> =
    match msg with
    | UrlChanged page -> { state with Page = page }, Cmd.none

let private padding = "px-4 lg:px-64 "

let private menuLinks (currentPage:WebPage) =
    let btn (text:string) (icon:string) (nextPage:WebPage) =
        let isActive = currentPage = nextPage
        Daisy.button.a [
            prop.className "content-center"
            button.ghost
            if isActive then color.textInfo
            prop.children [
                Html.faIcon icon
                Html.span [ prop.text text ]
            ]
            yield! prop.hrefRouted (Web nextPage)
        ]
    [
        btn "About me" "fa-solid fa-user" Index
        btn "Talks & Events" "fa-solid fa-podcast" Talks
    ]

[<ReactComponent>]
let private Navbar (currentPage:WebPage) =

    Daisy.navbar [
        color.textNeutralContent
        color.bgNeutral
        prop.className padding
        prop.children [
            Daisy.navbarStart [
                Daisy.button.label [
                    button.square
                    button.ghost
                    prop.className "lg:hidden"
                    prop.htmlFor "drawer"
                    prop.children [
                        Html.faIcon "fa-solid fa-bars"
                    ]
                ]
                Daisy.button.label [
                    button.ghost
                    prop.className "normal-case"
                    prop.text "Roman \"Džoukr\" Provazník"
                    yield! prop.hrefRouted (Web Index)
                ]
            ]
            Daisy.navbarCenter [

            ]
            Daisy.navbarEnd [
                Html.divClassed "flex gap-2 hidden lg:flex" [
                    yield! menuLinks currentPage
                ]
            ]
        ]
    ]

let private footer =
    let iconLink (icon:string) (text:string) (href:string) =
        Html.divClassed "flex gap-1 items-center" [
            Html.faIcon icon
            Daisy.link [
                prop.href href
                prop.text text
            ]
        ]

    Daisy.footer [
        prop.className (padding + "p-5 bg-neutral text-neutral-content")
        prop.children [

            Html.nav [
                Html.p "Roman \"Džoukr\" Provazník"
                Html.p [ prop.text "Building software with ❤️ + 🧠"; prop.className "text-xs opacity-90" ]
            ]
            Html.nav [
                Daisy.footerTitle "Projects"
                iconLink "fa-solid fa-podcast" "PodVocasem" "https://www.podvocasem.cz"
                // iconLink "fa-brands fa-twitter" "FSharping" "https://twitter.com/fsharping"
                iconLink "fa-solid fa-code" "fsharpConf" "https://fsharpconf.com"
            ]
            Html.nav [
                Daisy.footerTitle "Social"
                iconLink "fa-brands fa-twitter" "@dzoukr" "https://twitter.com/dzoukr"
                iconLink "fa-brands fa-github" "dzoukr" "https://github.com/dzoukr"
                iconLink "fa-brands fa-linkedin" "Roman Provazník" "https://www.linkedin.com/in/dzoukr/"
            ]
        ]
    ]

[<ReactComponent>]
let private WebLayout (page:WebPage) =

    Daisy.drawer [
        prop.children [
            Daisy.drawerToggle [ prop.id "drawer" ]
            Daisy.drawerContent [
                prop.className "h-screen flex flex-col overflow-y-auto"
                prop.children [
                    Navbar page
                    Html.divClassed (padding + "grow") [
                        match page with
                        | Index -> Pages.Index.IndexView ()
                        | Talks -> Pages.Talks.TalksView ()
                    ]
                    footer
                ]
            ]
            Daisy.drawerSide [
                prop.className "z-40"
                prop.children [
                    Daisy.drawerOverlay [ prop.htmlFor "drawer" ]
                    Html.aside [
                        prop.className "w-80 h-screen border-r border-base-300 bg-base-100"
                        prop.children [
                            Daisy.menu [
                                prop.children [
                                    for m in menuLinks page do
                                        Html.li [
                                            //prop.className "flex flex-grow content-center"
                                            prop.children m
                                        ]
                                ]
                            ]
                        ]
                    ]

                ]
            ]

        ]
    ]

[<ReactComponent>]
let private ToolLayout (page:ToolPage) =
    match page with
    | Share i -> Pages.Shares.SharesView i

[<ReactComponent>]
let AppView () =
    let state,dispatch = React.useElmish(init, update)
    React.router [
        router.pathMode
        router.onUrlChanged (Page.parseFromUrlSegments >> UrlChanged >> dispatch)
        router.children [
            match state.Page with
            | Web page -> WebLayout page
            | Tool page -> ToolLayout page
        ]
    ]