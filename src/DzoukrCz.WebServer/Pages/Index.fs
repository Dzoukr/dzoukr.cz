module DzoukrCz.WebServer.Pages.Index

open Feliz.ViewEngine
open DzoukrCz.WebServer
open DzoukrCz.WebServer.Templates
open DzoukrCz.WebServer.Site

let private body =
    Html.divClassed "grow max-w-4xl mx-auto py-5 px-5 lg:px-0" [
        Menu.menu SiteUrls.IndexUrl
        Html.divClassed "mt-5" [
            Html.h1 "Hello, Index!!!"
            Html.faIcon "fa-solid fa-code"
        ]
    ]

let page = Document.html Header.head body
