module DzoukrCz.WebServer.Pages.Talks.WebPage

open System
open Feliz.ViewEngine
open DzoukrCz.WebServer
open DzoukrCz.WebServer.Templates
open DzoukrCz.WebServer.Site
open DzoukrCz.WebServer.Pages.Talks.Domain


let private body (talks:Queries.Talk list) =
    Html.div $"TODO {talks}"

let talksWebPage (queries:TalksQueries)=
    task {
        let! talks = queries.GetTalks()
        return
            Document.html Header.head SiteUrls.SpeakingUrl (body talks)
    }
