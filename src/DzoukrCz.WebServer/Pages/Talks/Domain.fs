module DzoukrCz.WebServer.Pages.Talks.Domain

open System
open System.Threading.Tasks

type TalkLang = CZ | EN

module Queries =
    type Talk = {
        Date : DateTime
        Event : string
        Title : string
        Location : string
        Link : string option
        Lang : TalkLang
        Logo : string option
    }

type TalksQueries =
    abstract member GetTalks : unit -> Task<Queries.Talk list>
