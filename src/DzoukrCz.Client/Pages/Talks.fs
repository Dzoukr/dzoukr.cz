module DzoukrCz.Client.Pages.Talks

open System
open Feliz
open Feliz.DaisyUI
open Elmish
open Feliz.UseElmish
open DzoukrCz.Client.Server
open DzoukrCz.Client.Router
open DzoukrCz.Client
open Fable.SimpleJson
open Fable.SimpleHttp

//[{"Date":"2018-03-29","Event":"SYMA 2018","Title":"SW Quality Discussion Panel","Place":"Prague, CZ","Lang":"CZ","Link":"[https://www.tpconsulting.cz/konference-syma-2018-csj-n24695.htm](https://www.tpconsulting.cz/konference-syma-2018-csj-n24695.htm)","Logo":"![syma_logo.png](https://media.dzoukr.cz/datatable-3cc5022532b14f0d9b4181f8fde9c519/e42a6d607f034726b9210c7bb7e235f1.png)"}
type TalkLang = CZ | EN

type TalkRaw = {
    Date : string
    Event : string
    Title : string
    Place : string
    Link : string
    Lang : string
    Logo : string
}

type Talk = {
    Date : DateTime
    Event : string
    Title : string
    Location : string
    Link : string option
    Lang : TalkLang
    Logo : string option
}

module Talk =
    open System.Text.RegularExpressions
    let pattern = @"\((.+)\)"

    let private parseUrl (s:string) =
        let m = Regex.Match(s, pattern)
        if m.Groups.Count = 0 then s
        else m.Groups.[1].Value

    let private someString (s:string) = if String.IsNullOrWhiteSpace s then None else Some s

    let ofRaw (r:TalkRaw) : Talk =
        {
            Date = DateTime.Parse(r.Date)
            Event = r.Event
            Title = r.Title
            Location = r.Place
            Link = r.Link |> someString |> Option.map parseUrl
            Lang = if r.Lang = "CZ" then CZ else EN
            Logo = r.Logo  |> someString |> Option.map parseUrl
        }

type private State = {
    Talks : RemoteData<Talk list>
}

type private Msg =
    | LoadTalks
    | TalksLoaded of ServerResult<Talk list>

let private init () =
    {
        Talks = RemoteData.Idle
    }, Cmd.ofMsg LoadTalks

let private getTalks () =
    async {
        let! (_, responseText) = Http.get "https://media.dzoukr.cz/talks.json"
        return responseText |> Json.parseNativeAs<TalkRaw []> |> Array.toList |> List.map Talk.ofRaw
    }


let private update (msg:Msg) (state:State) : State * Cmd<Msg> =
    match msg with
    | LoadTalks -> { Talks = RemoteData.InProgress }, Cmd.OfAsync.eitherAsResult getTalks TalksLoaded
    | TalksLoaded t -> { Talks = RemoteData.setResponse t }, Cmd.none

let private talkCard (t:Talk) =
    let isFuture = t.Date > DateTime.Now
    let shadow = if isFuture then "shadow-xl" else "shadow"
    Daisy.card [
        prop.className $"{shadow} w-full lg:w-72"
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
                                button.warning
                                button.sm
                                prop.children [
                                    Html.div "More info"
                                    Html.faIcon "fa-solid fa-arrow-up-right-from-square"
                                ]
                                prop.href l
                            ]
                        | None -> Html.none
                    ]
                ]
            ]
        ]
    ]

let private loadingTalk =
    Daisy.card [
        prop.className "shadow w-full lg:w-72 animate-pulse"
        card.bordered
        card.compact
        prop.children [
            Daisy.cardBody [
                Daisy.cardTitle [
                    prop.className "w-48 bg-slate-200 rounded"
                    prop.dangerouslySetInnerHTML "&nbsp;"
                ]
                Html.p [
                    prop.className "text-lg h-6 w-32 bg-slate-200 rounded"
                    prop.dangerouslySetInnerHTML "&nbsp;"
                ]
                Html.p [
                    prop.className "text-lg h-4 w-36 bg-slate-200 rounded"
                    prop.dangerouslySetInnerHTML "&nbsp;"
                ]
                Html.p [
                    prop.className "text-lg h-4 w-36 bg-slate-200 rounded"
                    prop.dangerouslySetInnerHTML "&nbsp;"
                ]
                Daisy.cardActions [
                    prop.className "flex flex-row self-end w-16"
                    prop.children [
                        Html.p [
                            prop.className "text-lg w-8 bg-slate-200 rounded"
                            prop.dangerouslySetInnerHTML "&nbsp;"
                        ]
                    ]
                ]
            ]
        ]
    ]

[<ReactComponent>]
let TalksView () =
    let state, dispatch = React.useElmish(init, update, [| |])

    Html.divClassed "flex flex-col gap-8" [
        Html.divClassed "formatted-text text-center" [
            Html.text "Here you can find a list of my talks and events I have attended as a speaker:"
        ]
        Html.divClassed "flex flex-row gap-4 flex-wrap justify-center lg:justify-start" [
            match state.Talks with
            | Idle
            | Finished (Error _)
            | InProgress -> yield! [1..9 ] |> List.map (fun _ -> loadingTalk)
            | Finished (Ok t) ->
                yield!
                    t
                    |> List.sortByDescending (fun x -> x.Date)
                    |> List.map talkCard
        ]
    ]