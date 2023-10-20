module DzoukrCz.Client.Pages.Index

open System
open DzoukrCz.Shared.Stats.API
open Feliz
open Feliz.DaisyUI
open Elmish
open Feliz.UseElmish
open DzoukrCz.Client.Server
open DzoukrCz.Client.Router
open DzoukrCz.Client

type private State = {
    Stats : Response.IndexStats
}

type private Msg =
    | LoadStats
    | StatsLoaded of ServerResult<Response.IndexStats>

let private init () =
    {
        Stats = Response.IndexStats.init
    }, Cmd.ofMsg LoadStats

let private update (msg:Msg) (state:State) : State * Cmd<Msg> =
    match msg with
    | LoadStats -> state, Cmd.OfAsync.eitherAsResult (fun _ -> statsAPI.GetStats()) StatsLoaded
    | StatsLoaded (Ok stats) -> { state with Stats = stats }, Cmd.none
    | StatsLoaded (Error _) -> state, Cmd.none

let private stats (state:State) =
    let years = DateTimeOffset.Now.Year - 2000
    Daisy.stats [
        prop.className "shadow w-full"
        prop.children [
            Daisy.stat [
                Daisy.statFigure [
                    Html.faIcon "fa-solid fa-calendar-days fa-2x opacity-80"
                ]
                Daisy.statTitle "Years of experience"
                Daisy.statValue years
                Daisy.statDesc "and counting..."
            ]
            Html.a [
                yield! prop.hrefRouted Page.Talks
                prop.children [
                    Daisy.stat [
                        Daisy.statFigure [
                            Html.faIcon "fa-solid fa-microphone fa-2x opacity-80"
                        ]
                        Daisy.statTitle "Public talks"
                        Daisy.statValue state.Stats.Talks
                        Daisy.statDesc "online, offline"
                    ]
                ]
            ]
            Html.a [
                prop.href "https://www.podvocasem.cz"
                prop.children [
                    Daisy.stat [
                        Daisy.statFigure [
                            Html.faIcon "fa-solid fa-podcast fa-2x opacity-80"
                        ]
                        Daisy.statTitle "Episodes"
                        Daisy.statValue state.Stats.Episodes
                        Daisy.statDesc "PodVocasem podcast"
                    ]
                ]
            ]
            Html.a [
                prop.href "https://www.nuget.org/profiles/dzoukr"
                prop.children [
                    Daisy.stat [
                        Daisy.statFigure [
                            Html.faIcon "fa-brands fa-github fa-2x opacity-80"
                        ]
                        Daisy.statTitle "Total downloads"
                        Daisy.statValue state.Stats.Downloads
                        Daisy.statDesc "open source libraries"
                    ]
                ]
            ]
        ]
    ]

let private leftCol =
    let iconLink (icon:string) (text:string) (href:string) =
        Html.divClassed "flex gap-1 items-center" [
            Html.faIcon icon
            Daisy.link [
                prop.href href
                prop.text text
            ]
        ]

    Html.divClassed "flex flex-col gap-8 items-center p-4" [

        Html.divClassed "flex flex-col gap-4 items-center" [
            Html.img [ prop.src "img/profile.png"; prop.className "mask mask-circle w-48 lg:w-64" ]
            Html.divClassed "text-xl" [ Html.text "Building software with ❤️ + 🧠" ]
        ]
        Html.divClassed "flex flex-col w-full gap-1 self-start" [
            Html.divClassed "text-lg font-medium my-2" [ Html.text "CONTACT" ]
            iconLink "fa-regular fa-envelope" "dzoukr@dzoukr.cz" "mailto:dzoukr@dzoukr.cz"
            iconLink "fa-brands fa-twitter" "@dzoukr" "https://twitter.com/dzoukr"
            iconLink "fa-brands fa-github" "dzoukr" "https://github.com/dzoukr"
            iconLink "fa-brands fa-linkedin" "Roman Provazník" "https://www.linkedin.com/in/dzoukr/"
        ]


    ]

let private rightCol (state:State) =
    Html.divClassed "flex flex-col gap-16" [
        Html.divClassed "prose prose-base lg:prose-xl min-w-full" [
            Html.h1 "👋 Hi, I'm Roman Provazník"
            Html.p [
                Html.text "I am the Principal Technical Lead .NET / Architect at Ciklum Western Europe, a melomaniac, "
                Html.a [
                    prop.text "a  speaker"
                    yield! prop.hrefRouted Page.Talks
                ]
                Html.text ", and a terrible drummer."
            ]
            Html.p "I have more than 20 years of experience with software development using languages like Pascal, Delphi, Prolog, PHP, C#, or F#, but I successfully managed to erase most of these languages from my brain."
            Html.p [
                Html.text "As a big fan of functional-first .NET language "
                Html.a [ prop.text "F#"; prop.href "https://fsharp.org" ]
                Html.text ", I founded the Czech F# community called FSharping which I love to maintain and grow. I also somehow sneaked into F# Software Foundation Board of Trustees for one year."
            ]
            Html.p "If you were listening to Kiss Radio between 2002 and 2005, I'm the guy who annoyed you during the morning show."
            Html.p [
                Html.text "Since October 2021, I am co-hosting a Czech IT podcast called "
                Html.a [ prop.text "PodVocasem"; prop.href "https://www.podvocasem.cz" ]
                Html.text "."
            ]
        ]

        Html.divClassed "flex self-center" [
            Daisy.button.a [
                button.neutral
                prop.children [
                    Html.faIcon "fa-solid fa-envelope"
                    Html.text "Say Hello World!"
                ]
                prop.href "mailto:dzoukr@dzoukr.cz"
            ]
        ]
        stats state
    ]

[<ReactComponent>]
let IndexView () =
    let state, dispatch = React.useElmish(init, update, [| |])

    Html.divClassed "flex flex-col lg:flex-row gap-8 my-8" [
        Html.divClassed "basis-1/4" [ leftCol ]
        Html.divClassed "basis-3/4" [ rightCol state ]
    ]


