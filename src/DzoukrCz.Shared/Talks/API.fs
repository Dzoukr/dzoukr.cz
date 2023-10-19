module DzoukrCz.Shared.Talks.API

open System

type TalkLang = CZ | EN

module Response =
    type Talk = {
        Date : DateTime
        Event : string
        Title : string
        Location : string
        Link : string option
        Lang : TalkLang
        Logo : string option
    }


type TalksAPI = {
    GetTalks : unit -> Async<Response.Talk list>
}
with
    static member RouteBuilder _ m = sprintf "/api/talks/%s" m