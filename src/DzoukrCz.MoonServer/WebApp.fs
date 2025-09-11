module DzoukrCz.MoonServer.WebApp

let webApp =
    [
        MoonServerAPI.api
        DataAPI.api
    ]
    |> List.concat