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
        Html.faIconLink "fab fa-linkedin fa-2x" "https://www.linkedin.com/in/rprovaznik/"
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
        Bulma.block [
            Bulma.image [
                prop.children [
                    Html.img [
                        prop.src "img/profile.png"
                        image.isRounded
                    ]
                ]
            ]
        ]
        Bulma.block [
            Bulma.title "Roman Provazník"
            Bulma.subtitle "F# |> I ❤️"
        ]
        Bulma.block [
            Bulma.title.h2 "Bio"
            Html.div [ prop.children [ Html.text "F# Team Leader @ "; Html.a [ prop.text "CN Group CZ"; prop.href "https://www.cngroup.dk" ] ] ]
            Html.div [ prop.children [ Html.text "Founder @ "; Html.a [ prop.text "FSharping"; prop.href "https://fsharping.com" ] ] ]
            Html.div "Speaker, melomaniac & terrible drummer"
        ]
        Bulma.block [
            Bulma.title.h2 "Contact"
            Html.div [
                Html.faIconTextLink "fab fa-github" "https://github.com/dzoukr" "github.com/dzoukr" |> Html.div
                Html.faIconTextLink "fab fa-twitter" "https://twitter.com/rprovaznik" "twitter.com/rprovaznik" |> Html.div
                Html.faIconTextLink "fab fa-linkedin" "https://www.linkedin.com/in/rprovaznik/" "linkedin.com/in/rprovaznik" |> Html.div
            ]
        ]
        Bulma.block [
            Divider.divider [ divider.text "Home Team" ]
            //Bulma.title.h2 "# Home Team"
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
                        prop.children [
                            Bulma.box [ letfCol ]
                        ]
                    ]
                    Bulma.column [ middle ]
                ]

            ]
        ]
        footer
    ]
    |> React.fragment