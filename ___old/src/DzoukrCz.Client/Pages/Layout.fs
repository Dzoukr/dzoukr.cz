module DzoukrCz.Client.Pages.Layout

open System
open DzoukrCz.Client.Router
open Feliz
open Feliz.Bulma
open DzoukrCz.Client.SharedView

let icons =
    Html.divClassed "icons" [
        Html.faIconLink "fab fa-github fa-2x" "https://github.com/dzoukr"
        Html.faIconLink "fab fa-twitter fa-2x" "https://twitter.com/dzoukr"
        Html.faIconLink "fab fa-linkedin fa-2x" "https://www.linkedin.com/in/dzoukr/"
    ]

let footer =
    Bulma.footer [
        Bulma.container [
            icons
            Html.hr []
            Html.divClassed "copyright" [
                Html.text "Made with ❤️ and F#. "
                Html.text $"Roman Provazník © {DateTime.UtcNow.Year}"
            ]
        ]
    ]



let menu (p:Page) =
    let isActive,setIsActive = React.useState(false)
    let item (name:string) (mp:Page) =
        Bulma.navbarItem.a [
            prop.text name
            prop.href mp
            prop.onClick Router.goToUrl
            if p = mp then navbarItem.isActive
        ]

    Bulma.navbar [
        prop.children [
            Bulma.container [
                Bulma.navbarBrand.div [
                    Bulma.navbarBurger [
                        if isActive then navbarBurger.isActive
                        prop.children ([1..3] |> List.map (fun _ -> Html.span []))
                        prop.onClick (fun _ -> if isActive then setIsActive(false) else setIsActive(true))
                    ]
                ]

                Bulma.navbarMenu [
                    if isActive then navbarMenu.isActive
                    prop.children [
                        Bulma.navbarEnd.div [
                            item "About me" Page.AboutMe
                            //item "Blog" Page.Blog
                            item "Talks & Events" Page.Talks
                            item "Projects" Page.Projects
                        ]
                    ]
                ]
            ]
        ]
    ]

let basic page (middle:ReactElement) =
    [
        menu page
        [ middle ] |> Bulma.container
        footer
    ]
    |> React.fragment