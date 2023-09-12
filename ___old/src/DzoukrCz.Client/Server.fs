module DzoukrCz.Client.Server

open Fable.Remoting.Client
open DzoukrCz.Shared.API

let service =
    Remoting.createApi()
    |> Remoting.withRouteBuilder Service.RouteBuilder
    |> Remoting.buildProxy<Service>