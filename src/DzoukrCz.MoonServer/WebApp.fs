module DzoukrCz.MoonServer.WebApp

open Giraffe

let webApp : HttpHandler =
    choose [
        MoonServerAPI.api
    ]