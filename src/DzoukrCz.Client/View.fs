module DzoukrCz.Client.View

open Feliz
open Feliz.DaisyUI
open Router
open Elmish
open Feliz.UseElmish

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

[<ReactComponent>]
let private Navbar (currentPage:Page) =

    let btn (text:string) (icon:string) (nextPage:Page) =
        let isActive = currentPage = nextPage
        Daisy.button.label [
            button.ghost
            if isActive then color.textInfo
            prop.children [
                Html.faIcon icon
                Html.span [ prop.text text ]
            ]
            yield! prop.hrefRouted nextPage
        ]

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
                    prop.text "Roman Provazník"
                    yield! prop.hrefRouted Page.Index
                ]
            ]
            Daisy.navbarCenter [

            ]
            Daisy.navbarEnd [
                Html.divClassed "flex gap-2 hidden lg:flex" [
                    // btn "Blog" "fa-solid fa-blog" Page.Contact
                    btn "Talks & Events" "fa-solid fa-podcast" Page.Talks
                    // btn "Projects" "fa-solid fa-project-diagram" Page.Contact
                    btn "Contact" "fa-solid fa-envelope" Page.Contact
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
let private MainLayout state dispatch =

    Daisy.drawer [
        prop.children [
            Daisy.drawerToggle [ prop.id "drawer" ]
            Daisy.drawerContent [
                prop.className "h-screen flex flex-col overflow-y-auto"
                prop.children [
                    Navbar state.Page
                    Html.divClassed (padding + "grow") [
                        match state.Page with
                        | Page.Index -> Pages.Index.IndexView ()
                        | Talks -> Html.div "TODO"
                        | Contact -> Html.div "TODO"
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
                            // // ctxMenu
                            // state.CurrentWorkspace
                            // |> Option.map (leftMenu page)
                            // |> Option.defaultValue Html.none
                        ]
                    ]

                ]
            ]

        ]
    ]



[<ReactComponent>]
let AppView () =
    let state,dispatch = React.useElmish(init, update)
    React.router [
        router.pathMode
        router.onUrlChanged (Page.parseFromUrlSegments >> UrlChanged >> dispatch)
        router.children [
            MainLayout state dispatch
        ]
    ]