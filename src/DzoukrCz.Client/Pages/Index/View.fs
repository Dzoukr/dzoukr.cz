module DzoukrCz.Client.Pages.Index.View

open System
open Feliz
open Feliz.Bulma
open State
open Feliz.UseElmish
open DzoukrCz.Client.SharedView

let footer =
    Bulma.footer [
        Bulma.container [
            Html.divClassed "icons" [
                Html.faIconLink "fab fa-github fa-2x" "https://github.com/dzoukr"
                Html.faIconLink "fab fa-twitter fa-2x" "https://twitter.com/rprovaznik"
                Html.faIconLink "fab fa-linkedin fa-2x" "https://www.linkedin.com/in/romanprovaznik/"
            ]
            Html.hr []
            Html.divClassed "copyright" [
                Html.text "Made with ❤️ and F#. "
                Html.text $"Roman Provazník © {DateTime.UtcNow.Year}"
            ]
        ]
    ]

[<ReactComponent>]
let IndexView () =
    let model, dispatch = React.useElmish(State.init, State.update, [| |])
    [
        Bulma.section [
            Bulma.container [
                Bulma.columns [
                    Bulma.column [
                        column.isOneThird
                        prop.children [
                            Html.text "AAAA"
                        ]
                    ]
                    Bulma.column [
                        Html.text "BBBB"
                    ]
                ]

            ]
        ]
        footer
    ]
    |> React.fragment
