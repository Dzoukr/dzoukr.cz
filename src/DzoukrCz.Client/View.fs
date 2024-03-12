module DzoukrCz.Client.View

open DzoukrCz.Client.Router
open Feliz
open Feliz.DaisyUI
open Feliz.DaisyUI.Operators
open Feliz.UseElmish
open Feliz.Router
open Elmish

type private Msg =
    | UrlChanged of Page
    | OpenMenu of bool

type private State = {
    Page : Page
    MenuOpened : bool
}

let private init () =
    let nextPage = Router.currentPath() |> Page.parseFromUrlSegments
    { Page = nextPage; MenuOpened = false }, Cmd.navigatePage nextPage

let private update (msg:Msg) (state:State) : State * Cmd<Msg> =
    match msg with
    | UrlChanged page -> { state with Page = page; MenuOpened = false }, Cmd.none
    | OpenMenu opened -> { state with MenuOpened = opened }, Cmd.none

let private footer =
    let iconLink (icon:string) (text:string) (href:string) =
        Html.divClassed "flex gap-1 items-center" [
            Html.faIcon icon
            Daisy.link [
                prop.href href
                prop.text text
            ]
        ]

    Html.div [
        color.bgNeutral
        prop.children [
            Daisy.footer [
                prop.className "max-w-4xl mx-auto py-5 bg-neutral text-neutral-content px-5 lg:px-0"
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
                        iconLink "fa-solid fa-hockey-puck" "Hlášky" "https://hlasky.dzoukr.cz"
                    ]
                    Html.nav [
                        Daisy.footerTitle "Contact"
                        iconLink "fa-regular fa-envelope" "dzoukr@dzoukr.cz" "mailto:dzoukr@dzoukr.cz"
                        iconLink "fa-brands fa-twitter" "@dzoukr" "https://twitter.com/dzoukr"
                        iconLink "fa-brands fa-github" "dzoukr" "https://github.com/dzoukr"
                        iconLink "fa-brands fa-linkedin" "Roman Provazník" "https://www.linkedin.com/in/dzoukr/"
                    ]
                ]
            ]
        ]
    ]

let private webMenu currentPage menuOpened dispatch =
    let icon = if menuOpened then "fa-circle-xmark" else "fa-bars"

    let btn (text:string) (nextPage:WebPage) =
        let isActive = currentPage = nextPage
        Html.a [
            if isActive then prop.className "text-warning font-bold" else prop.className "hover:font-bold"
            prop.children [
                Html.span [ prop.text text ]
            ]
            yield! prop.hrefRouted (Web nextPage)
        ]

    Html.divClassed "flex flex-col gap-4 items-center" [
        Html.div [
            prop.onClick (fun _ -> dispatch (OpenMenu (not menuOpened)))
            prop.className "cursor-pointer"
            prop.children [
                Html.faIcon $"fa-solid {icon} fa-2x"
            ]
        ]
        if menuOpened then
            Html.divClassed "text-2xl flex flex-col gap-3 items-center" [
                btn "ABOUT ME" Index
                //btn "BLOG" (Blogs BlogsFilter.empty)
                btn "TALKS & EVENTS" Talks
            ]
    ]

let private logo =
    Html.a [
        prop.className "text-3xl lg:text-4xl not-a"
        prop.children [
            Html.span "<ROMAN"
            Html.span [ prop.className "font-bold" ++ color.textWarning; prop.text "PROVAZNÍK/>" ]
        ]
        yield! prop.hrefRouted (Web WebPage.Index)
    ]

let private slogan =
    Html.div "Building software with ❤️ + 🧠"

let private header page menuOpened dispatch =
    Html.divClassed "bg-neutral text-neutral-content" [
        Html.divClassed "max-w-4xl mx-auto py-5 flex flex-col gap-5 items-center" [
            logo
            slogan
            webMenu page menuOpened dispatch
        ]
    ]

[<ReactComponent>]
let private WebLayout (page:WebPage) state dispatch =
    Html.divClassed "h-screen flex flex-col gap-4 overflow-y-auto" [
        header page state.MenuOpened dispatch
        Html.divClassed "grow max-w-4xl mx-auto py-5 px-5 lg:px-0" [
            match page with
            | Index -> Pages.Index.IndexView ()
            | Talks -> Pages.Talks.TalksView ()
            // | Blogs f -> Pages.Blogs.BlogView f
            // | BlogsDetail rewrite -> Pages.BlogsDetail.BlogsDetailView rewrite
        ]
        footer
    ]


[<ReactComponent>]
let AppView () =
    let state,dispatch = React.useElmish(init, update)
    React.router [
        router.pathMode
        router.onUrlChanged (Page.parseFromUrlSegments >> UrlChanged >> dispatch)
        router.children [
            match state.Page with
            | Web page -> WebLayout page state dispatch
        ]
    ]