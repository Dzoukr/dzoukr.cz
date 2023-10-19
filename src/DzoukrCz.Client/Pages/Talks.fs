module DzoukrCz.Client.Pages.Talks

open System
open DzoukrCz.Shared.Talks.API
open Feliz
open Feliz.DaisyUI
open Elmish
open Feliz.UseElmish
open DzoukrCz.Client.Server
open DzoukrCz.Client.Router
open DzoukrCz.Client

type private State = {
    Talks : Response.Talk list
}

type private Msg =
    | LoadTalks
    | TalksLoaded of ServerResult<Response.Talk list>

let private init () =
    {
        Talks = []
    }, Cmd.ofMsg LoadTalks

let private update (msg:Msg) (state:State) : State * Cmd<Msg> =
    match msg with
    | LoadTalks -> state, Cmd.OfAsync.eitherAsResult (fun _ -> talksAPI.GetTalks()) TalksLoaded
    | TalksLoaded (Ok talks) -> { state with Talks = talks }, Cmd.none
    | TalksLoaded (Error _) -> state, Cmd.none

let private talkCard (t:Response.Talk) =
    let isFuture = t.Date > DateTime.Now
    let shadow = if isFuture then "shadow-xl" else "shadow"
    Daisy.card [
        prop.className $"{shadow} w-80"
        card.bordered
        card.compact
        prop.children [
            match t.Logo with
            | Some l ->
                Html.figure [
                    prop.className "h-48"
                    prop.children [
                        Html.img [ prop.src l; prop.className "w-80" ]
                    ]
                ]
            | None -> Html.none
            Daisy.cardBody [
                if isFuture then
                    Daisy.badge [
                        badge.warning
                        badge.lg
                        prop.className "font-medium"
                        prop.children [
                            Html.faIcon "fa-solid fa-triangle-exclamation mr-2"
                            Html.div "Seats still available"
                        ]
                    ]

                Daisy.cardTitle t.Title

                Html.p [ prop.className "text-lg"; prop.text t.Event ]

                Html.divClassed "flex gap-2 items-center" [
                    Html.faIcon "fa-solid fa-calendar-days opacity-80"
                    Html.div $"{t.Date.Day}. {t.Date.Month}. {t.Date.Year}"
                ]
                Html.divClassed "flex gap-2 items-center" [
                    Html.faIcon "fa-solid fa-location-dot opacity-80"
                    Html.div t.Location
                ]
                Html.divClassed "flex gap-2 items-center" [
                    match t.Lang with
                    | CZ ->
                        Html.div [ Html.img [ prop.src "/img/cz.png"; prop.className "w-4" ] ]
                        Html.div "Czech"
                    | EN ->
                        Html.div [ Html.img [ prop.src "/img/en.png"; prop.className "w-4" ] ]
                        Html.div "English"
                ]

                Daisy.cardActions [
                    prop.className "justify-end"
                    prop.children [
                        match t.Link with
                        | Some l ->
                            Daisy.button.a [
                                button.outline
                                button.sm
                                prop.children [
                                    Html.div "More info"
                                    Html.faIcon "fa-solid fa-film"
                                ]
                                prop.href l
                            ]
                        | None -> Html.none
                    ]
                ]
            ]
        ]
    ]

[<ReactComponent>]
let TalksView () =
    let state, dispatch = React.useElmish(init, update, [| |])

    Html.divClassed "flex flex-col gap-8 my-8" [
        Html.divClassed "prose prose-base lg:prose-xl min-w-full" [
            Html.h1 [
                prop.text "Talks & Events"
            ]

        ]
        Html.divClassed "flex flex-row gap-8 flex-wrap justify-center lg:justify-start" [
            yield!
                state.Talks
                |> List.sortByDescending (fun x -> x.Date)
                |> List.map talkCard
        ]
    ]


