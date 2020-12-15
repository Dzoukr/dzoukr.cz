module DzoukrCz.Client.Pages.Layout

open System
open Feliz
open Feliz.Bulma
open Feliz.Bulma.Divider
open DzoukrCz.Client.SharedView

let icons =
    Html.divClassed "icons" [
        Html.faIconLink "fab fa-github fa-2x" "https://github.com/dzoukr"
        Html.faIconLink "fab fa-twitter fa-2x" "https://twitter.com/rprovaznik"
        Html.faIconLink "fab fa-linkedin fa-2x" "https://www.linkedin.com/in/romanprovaznik/"
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

let letfCol =
    Html.divClassed "profile" [
        Bulma.image [
            prop.children [
                Html.img [
                    prop.src "img/profile.png"
                    image.isRounded
                ]
            ]
        ]
        Bulma.container [
            Bulma.title.h1 [
                title.is3
                prop.text "Roman Provazník"
            ]
            Bulma.subtitle.p [
                prop.children [
                    Html.text "F# |> I Love"
                ]
            ]
        ]
        Bulma.container [
            Divider.divider [ divider.text "Bio" ]
            Html.div [ prop.children [ Html.text "F# Team Leader @ "; Html.a [ prop.text "CN Group CZ"; prop.href "https://www.cngroup.dk" ] ] ]
            Html.div [ prop.children [ Html.text "Founder @ "; Html.a [ prop.text "FSharping"; prop.href "https://fsharping.com" ] ] ]
            Html.div "Speaker, melomaniac & bad drummer"
        ]
        Bulma.container [
            Divider.divider [ divider.text "Contact" ]
            Html.div [
                Html.faIconTextLink "fab fa-github" "https://github.com/dzoukr" "github.com/dzoukr" |> Html.div
                Html.faIconTextLink "fab fa-twitter" "https://twitter.com/rprovaznik" "twitter.com/rprovaznik" |> Html.div
                Html.faIconTextLink "fab fa-linkedin" "https://www.linkedin.com/in/romanprovaznik/" "linkedin.com/in/romanprovaznik" |> Html.div
            ]
        ]
        Bulma.container [
            Divider.divider [ divider.text "Home Team" ]
            Html.divClassed "home-team" [
                Html.a [
                    prop.children [
                        Html.img [ prop.src "https://www.cngroup.dk/static/media/CN-logo-2019.d720839f.svg" ]
                    ]
                    prop.href "https://www.cngroup.dk"
                ]
            ]
        ]
    ]

let basic (middle:ReactElement) =
    [
        Bulma.section [
            Bulma.container [
                Bulma.columns [
                    Bulma.column [
                        column.isOneQuarter
                        prop.children [ letfCol ]
                    ]
                    Bulma.column [ middle ]
                ]

            ]
        ]
        footer
    ]
    |> React.fragment