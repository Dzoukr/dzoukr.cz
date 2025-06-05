module DzoukrCz.WebServer.Pages.Talks.Queries

open System
open System.Threading.Tasks
open DzoukrCz.WebServer.Pages.Talks.Domain

type StorageTalksQueries () =
    interface TalksQueries with
        member _.GetTalks () = [] |> Task.FromResult
            // select {
            //     for d in drops do
            //     where (d.PoolId = args.Args)
            // }
            // |> conn.SelectAsync<DropsTable>
            // |> Task.map (Seq.map toDrop >> Seq.toList)
