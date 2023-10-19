module DzoukrCz.Shared.Stats.API

module Response =
    type IndexStats = {
        Talks : string
        Episodes : string
        Downloads : string
    }

type StatsAPI = {
    GetStats : unit -> Async<Response.IndexStats>
}
with
    static member RouteBuilder _ m = sprintf "/api/stats/%s" m