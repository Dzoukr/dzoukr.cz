module DzoukrCz.WebServer.Pages.Talks.Queries

open System
open System.Text.Json.Nodes
open System.Threading.Tasks
open DzoukrCz.WebServer.Pages.Talks.Domain
open DzoukrCz.Libraries.Publisher
open DzoukrCz.Libraries.MarkdownTools
open System.Text.Json

type StorageTalksQueries (publisher:Publisher) =
    interface TalksQueries with
        member _.GetTalks () =
            task {
                let! talks = publisher.FindByMetadataEq(Some "datatable", "datatable", JsonValue.Create("talks"))
                return
                    talks
                    |> List.tryHead
                    |> Option.map (_.Content >> findTables)
                    |> Option.bind List.tryHead
                    |> Option.map (fun tr ->
                        [
                            for v in tr.GetValues() do
                                {
                                    Date = v.[0] |> DateTime.Parse
                                    Event = v.[1]
                                    Title = v.[2]
                                    Location = v.[3]
                                    Lang = if v.[4] = "en" then EN else CZ
                                    Link = v.[5] |> Option.ofObj |> Option.bind trySrcAndAlt |> Option.map fst
                                    Logo = v.[6] |> Option.ofObj |> Option.bind trySrcAndAlt |> Option.map fst
                                } : Queries.Talk
                        ]
                    )
                    |> Option.defaultValue []
                    |> List.sortByDescending _.Date
            }