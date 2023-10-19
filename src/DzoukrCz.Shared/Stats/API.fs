module DzoukrCz.Shared.Stats.API

module Response =
    type IndexStats = {
        Talks : string
        Episodes : string
        Downloads : string
    }
    module IndexStats =
        let init = {
            Talks = "26"
            Episodes = "40"
            Downloads = "611k+"
        }

type StatsAPI = {
    GetStats : unit -> Async<Response.IndexStats>
}
with
    static member RouteBuilder _ m = sprintf "/api/stats/%s" m