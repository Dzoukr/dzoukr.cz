module DzoukrCz.WebServer.DataReaders.Talks

open System

type TalkLang = CZ | EN

type private TalkRaw = {
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

module private Talk =
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

