module DzoukrCz.Client.Pages.AboutMe

open System
open DzoukrCz.Client.Router
open Feliz
open Feliz.Bulma
open DzoukrCz.Client.SharedView

let leftCol =
    Html.divClassed "left-col" [
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
            Bulma.subtitle "F# |> I ❤️"
        ]
        Bulma.block [
            Html.div [
                Html.faIconTextLink "fab fa-github" "https://github.com/dzoukr" "github.com/dzoukr" |> Html.div
                Html.faIconTextLink "fab fa-twitter" "https://twitter.com/dzoukr" "twitter.com/dzoukr" |> Html.div
                Html.faIconTextLink "fab fa-linkedin" "https://www.linkedin.com/in/dzoukr/" "linkedin.com/in/dzoukr" |> Html.div
                Html.faIconTextLink "far fa-envelope" "mailto:dzoukr@dzoukr.cz" "dzoukr@dzoukr.cz" |> Html.div
            ]
        ]
        Bulma.block [
            Divider.divider [ divider.text "Home Team" ]
            Html.divClassed "home-team" [
                Html.a [
                    prop.children [
                        Html.img [ prop.src "https://www.cngroup.dk/static/media/CN_Group+Ciklum-logo-2021.b00bee63.svg" ]
                    ]
                    prop.href "https://www.cngroup.dk"
                ]
            ]
        ]
    ]


let rightCol =
    Html.divClassed "right-col" [
        Bulma.block [
            Bulma.title.h1 [
                prop.children [
                    Html.text "Hi, I'm "
                    Html.strong "Roman Provazník"
                ]
            ]
        ]
        Bulma.block [
            Html.p [
                Html.text "I am Principal Technical Lead .NET / Architect at "
                Html.a [ prop.text "CN Group CZ"; prop.href "https://www.cngroup.dk" ]
                Html.text ", melomaniac, "
                Html.a ("speaker", Page.Talks)
                Html.text " and a terrible drummer."
            ]
        ]
        Bulma.block [
            Html.p [
                Html.text "I have more than 20 years of experience with software development using languages like Pascal, Delphi, Prolog, PHP, C#, or F#, "
                Html.text "but I successfully managed to erase most of these languages out of my brain."
            ]
        ]
        Bulma.block [
            Html.p [
                Html.text "As a big fan of functional-first .NET language "
                Html.a [ prop.text "F#"; prop.href "https://www.fsharp.org" ]
                Html.text ", I founded the Czech F# community called "
                Html.a [ prop.text "FSharping"; prop.href "https://fsharping.com" ]
                Html.text " which I love to maintain and grow. I also somehow sneaked in F# Software Foundation Board of Trustees for one year."
            ]
        ]
        Bulma.block [
            Html.p [
                Html.text "If you were listening to Kiss Radio between 2002 and 2005, I'm the guy who annoyed you during the morning show."
            ]
        ]
    ]

[<ReactComponent>]
let AboutMeView () =
    Html.divClassed "about-me" [
        Bulma.section [
            Bulma.columns [
                Bulma.column [
                    column.isOneThird
                    prop.children [
                        leftCol
                    ]
                ]
                Bulma.column [
                    column.isTwoThirds
                    prop.children [
                        rightCol
                    ]
                ]
            ]
        ]
    ]