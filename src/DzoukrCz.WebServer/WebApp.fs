module DzoukrCz.WebServer.WebApp

open DzoukrCz.WebServer.Site
open Giraffe
open DzoukrCz
open DzoukrCz.WebServer
open Feliz.ViewEngine

let private render (el:ReactElement) = el |> Render.htmlDocument |> htmlString

let webApp : HttpHandler =
    choose [
        GET >=> choose [
            route SiteUrls.SpeakingUrl >=> text "TODO: Speaking"
            route SiteUrls.IndexUrl >=> render Pages.Index.page
        ]
    ]