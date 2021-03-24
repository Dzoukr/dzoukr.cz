module DzoukrCz.Client.Pages.Projects

open System
open DzoukrCz.Client.Router
open Feliz
open Feliz.Bulma
open DzoukrCz.Client.SharedView

type Project = {
    Name : string
    Url : string
    Description : string
    Logo : string option
    Tags : string list
}

module Projects =
    let cosmoStore = {
        Name = "CosmoStore"
        Url = "https://github.com/Dzoukr/CosmoStore"
        Description = "F# Event store for Azure Cosmos DB, Table Storage, Postgres, LiteDB & ServiceStack"
        Logo = Some "https://raw.githubusercontent.com/Dzoukr/CosmoStore/master/logo.png"
        Tags = ["event store";"cosmos db"]
    }

    let dapperFSharp = {
        Name = "Dapper.FSharp"
        Url = "https://github.com/Dzoukr/Dapper.FSharp"
        Description = "Lightweight F# extension for StackOverflow Dapper with support for MSSQL, MySQL and PostgreSQL"
        Logo = Some "https://raw.githubusercontent.com/Dzoukr/Dapper.FSharp/master/logo.png"
        Tags = ["mssql";"mysql";"postgres"]
    }

    let felizBulma = {
        Name = "Feliz.Bulma"
        Url = "https://github.com/Dzoukr/Feliz.Bulma"
        Description = "Bulma UI wrapper for amazing Feliz DSL"
        Logo = None
        Tags = ["react";"fable";"css"]
    }

    let tablesFSharp = {
        Name = "Tables.FSharp"
        Url = "https://github.com/Dzoukr/Tables.FSharp"
        Description = "Lightweight F# extension for the latest Azure.Data.Tables SDK"
        Logo = Some "https://github.com/Dzoukr/Azure.Data.Tables.FSharp/raw/master/logo.png"
        Tags = ["azure";"tables";"storage account"]
    }

    let yobo = {
        Name = "Yobo"
        Url = "https://github.com/Dzoukr/Yobo"
        Description = "F# Yoga Class Booking System"
        Logo = None
        Tags = ["fable";"booking";"yoga"]
    }

    let safer = {
        Name = "SAFEr.Template"
        Url = "https://github.com/Dzoukr/SAFEr.Template"
        Description = "Strongly opinionated modification of amazing SAFE Stack Template for full-stack development in F#"
        Logo = None
        Tags = ["dotnet";"fullstack";"template"]
    }


let projectInfo (p:Project) =
    Bulma.card [
        Bulma.cardContent [
            Bulma.block [
                if p.Logo.IsSome then
                    Html.img [
                        prop.style [ style.width 32; style.verticalAlign.bottom ]
                        prop.src p.Logo.Value
                    ]
                Bulma.title.h4 [
                    Html.a [
                        prop.href p.Url
                        prop.text p.Name
                    ]
                ]
            ]
            Bulma.block [
                Bulma.tags [
                for t in p.Tags do
                    Bulma.tag [
                        prop.text $"#{t}"
                    ]
                ]
            ]
            Bulma.block [
                Html.text p.Description
            ]
        ]
    ]
    |> List.singleton
    |> Html.divClassed "project"

let fsharpingInfo =
    Bulma.card [
        Bulma.cardContent [
            Bulma.block [
                Html.img [
                    prop.style [ style.width 128; style.verticalAlign.middle ]
                    prop.src "https://secure.meetupstatic.com/photos/event/c/0/0/b/600_459109163.jpeg"
                ]
                Bulma.title.h4 [
                    Html.a [
                        prop.href "https://fsharping.com"
                        prop.text "FSharping"
                    ]
                ]
            ]
            Bulma.block [
                Bulma.tags [
                    yield!
                        ["community"; "meetup"]
                        |> List.map (fun x -> Bulma.tag [
                                prop.text $"#{x}"
                            ]
                        )
                ]
            ]
            Bulma.block [
                Html.text "FSharping is a community around F# programming language for functional programming enthusiasts. "
                Html.text "We want to learn together and share new knowledge with zero entry-level barriers. "
                Html.text "As an affiliated user group of "
                Html.a [
                    prop.text "fsharp.org"
                    prop.href "https://fsharp.org"
                ]
                Html.text ", the F# Software Foundation, we strive to build a great community of passionate people who care deeply about F#, and want to make our community a safe and welcoming place for everyone. We expect everyone attending our events to be respectful, open, and considerate, and to follow the F# Software Foundation Code of Conduct."
            ]
            Bulma.block [
                Html.div [
                    Html.faIconTextLink "fab fa-twitter" "https://twitter.com/fsharping" "twitter.com/fsharping" |> Html.div
                ]
            ]
        ]
    ]

let fsharpConf =
    Bulma.card [
        Bulma.cardContent [
            Html.div [
                Bulma.block [
                    Html.img [
                        prop.style [ style.height 128; style.verticalAlign.middle ]
                        prop.src "img/fsharpconf.png"
                    ]
                    Bulma.title.h4 [
                        Html.a [
                            prop.href "http://fsharpconf.com/"
                            prop.text "FSharpConf 2020"
                        ]
                    ]
                ]
                Bulma.block [
                    Bulma.tags [
                        yield!
                            ["community"; "online"; "conference"]
                            |> List.map (fun x -> Bulma.tag [
                                    prop.text $"#{x}"
                                ]
                            )
                    ]
                ]

            ]
            Bulma.block [
                Html.text "The fsharpConf conference is organized by the F# community. "
                Html.text "The event is the third edition, following a successful fsharpConf in 2018 and 2016. "
                Html.text "It was originally inspired by dotNetConf and we are excited to be continuing this great tradition of virtual events!"
            ]
            Bulma.block [
                Html.div [
                    Html.faIconTextLink "fab fa-twitter" "https://twitter.com/fsharponline" "twitter.com/fsharponline" |> Html.div
                    Html.faIconTextLink "fab fa-youtube" "https://www.youtube.com/watch?v=ybkYHYKYeNw" "https://www.youtube.com/watch?v=ybkYHYKYeNw" |> Html.div
                ]
            ]
        ]
    ]


[<ReactComponent>]
let ProjectsView () =
    let col c = Bulma.column [ column.is4Widescreen; column.is6Tablet; prop.children [ c ] ]
    Html.divClassed "projects" [
        Bulma.section [
            Bulma.block [
                Bulma.title.h1 "Community Projects"
                Bulma.columns [
                    columns.isMultiline
                    prop.children [
                        Bulma.column [
                            column.isHalf
                            prop.children [ fsharpingInfo ]
                        ]
                        Bulma.column [
                            column.isHalf
                            prop.children [ fsharpConf ]
                        ]
                    ]
                ]
                |> List.singleton
                |> Html.divClassed "project-community"
            ]
        ]
        Bulma.section [
            Bulma.block [
                Bulma.title.h1 "F# Projects"
                Bulma.columns [
                    columns.isMultiline
                    prop.children [
                        col (projectInfo Projects.cosmoStore)
                        col (projectInfo Projects.dapperFSharp)
                        col (projectInfo Projects.tablesFSharp)
                        col (projectInfo Projects.felizBulma)
                        col (projectInfo Projects.yobo)
                        col (projectInfo Projects.safer)
                    ]
                ]
            ]

        ]
    ]