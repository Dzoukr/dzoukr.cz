module DzoukrCz.Client.Pages.Index

open System
open Feliz
open Feliz.DaisyUI
open Elmish
open Feliz.UseElmish
open Fable.SimpleHttp
open Fable.SimpleJson
open DzoukrCz.Client.Server
open DzoukrCz.Client.Router
open DzoukrCz.Client

type IndexStats = {
    Talks : string
    Episodes : string
    Downloads : string
}

type private State = {
    Stats : IndexStats
}

type private Msg =
    | LoadStats
    | StatsLoaded of ServerResult<IndexStats>

let private init () =
    {
        Stats = { Talks = "27"; Episodes = "48"; Downloads = "693k+" }
    }, Cmd.ofMsg LoadStats

let private getEpisodes () =
    async {
        let! (_, responseText) = Http.get "https://media.dzoukr.cz/stats.json"
        return responseText |> Json.parseNativeAs<IndexStats []> |> Array.head
    }

let private update (msg:Msg) (state:State) : State * Cmd<Msg> =
    match msg with
    | LoadStats -> state, Cmd.OfAsync.eitherAsResult (fun _ -> getEpisodes()) StatsLoaded
    | StatsLoaded (Ok stats) -> { Stats = stats }, Cmd.none
    | StatsLoaded (Error _) -> state, Cmd.none

let private stats (state:State) =
    let years = DateTimeOffset.Now.Year - 2000
    Daisy.stats [
        prop.className "shadow stats-vertical lg:stats-horizontal"
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
                yield! prop.hrefRouted (Web Talks)
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

[<ReactComponent>]
let IndexView () =
    let state, dispatch = React.useElmish(init, update, [| |])
    Html.divClassed "flex flex-col gap-16" [
        Html.divClassed "formatted-text" [
            Html.divClassed "grid grid-cols-1 gap-10 lg:gap-8 lg:grid-cols-4" [
                Html.divClassed "mx-auto" [
                    Html.img [ prop.src "img/profile.png"; prop.className "not-prose rounded-xl" ]
                ]
                Html.divClassed "lg:col-span-3" [
                    Html.h1 "👋 Hi, I'm Roman"

                    Html.text "I am .NET Center of Excellence Director at "
                    Html.a [
                        prop.text "Ciklum Western Europe"
                        prop.href "https://www.ciklum.com/we"
                    ]
                    Html.text ", melomaniac, "
                    Html.a [
                        prop.text "speaker"
                        yield! prop.hrefRouted (Web Talks)
                    ]
                    Html.text ", "
                    Html.a [ prop.text "podcaster"; prop.href "https://www.podvocasem.cz" ]
                    Html.text ", and a terrible drummer."
                ]
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
                button.warning
                prop.children [
                    Html.faIcon "fa-solid fa-envelope"
                    Html.text "GET IN TOUCH"
                ]
                prop.href "mailto:dzoukr@dzoukr.cz"
            ]
        ]
        stats state
    ]
