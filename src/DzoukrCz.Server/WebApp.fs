module DzoukrCz.Server.WebApp

open DzoukrCz.Server.MoonServer
open DzoukrCz.Server.Stats
open Giraffe
open Giraffe.GoodRead
open Fable.Remoting.Server
open Fable.Remoting.Giraffe
open Microsoft.Extensions.Logging

let webApp : HttpHandler =
    choose [
        MoonServerAPI.api
        StatsAPI.api
        htmlFile "public/index.html"
    ]