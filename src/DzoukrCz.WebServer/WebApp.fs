module DzoukrCz.WebServer.WebApp

open System.Threading.Tasks
open DzoukrCz.WebServer.Site
open Giraffe
open Giraffe.GoodRead
open DzoukrCz
open DzoukrCz.WebServer
open Feliz.ViewEngine
open Microsoft.AspNetCore.Http

let private render (el:ReactElement) = el |> Render.htmlDocument |> htmlString
let private renderAsync<'dep> (elFn:'dep -> Task<ReactElement>) =
    Require.services<'dep>(fun dep ->
        fun (_:HttpFunc) (ctx:HttpContext) ->
            task {
                let! elm = elFn dep
                let html = elm |> Render.htmlDocument
                return! ctx.WriteHtmlStringAsync html
            }
    )

let webApp : HttpHandler =
    choose [
        GET >=> choose [
            route SiteUrls.SpeakingUrl >=> renderAsync Pages.Talks.WebPage.talksWebPage
            route SiteUrls.IndexUrl >=> render Pages.Index.WebPage.indexWebPage
        ]
    ]