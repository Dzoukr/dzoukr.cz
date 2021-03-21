module DzoukrCz.Client.Pages.AboutMe

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
            prop.children [
                Html.divClassed "project" [
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
        ]
    ]

let projects =
    let col c = Bulma.column [ column.is4Widescreen; column.is6Tablet; prop.children [ c ] ]
    Html.divClassed "projects" [
        Bulma.block [
            Bulma.title.h2 "F# Projects"
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




let leftCol =
    Html.divClassed "left-col" [
        Bulma.block [
            Bulma.image [
                prop.children [
                    Html.img [
                        prop.src "img/profile.png"
                        image.isRounded
                    ]
                ]
            ]
        ]
        Bulma.block [
            Html.div [
                Html.faIconTextLink "fab fa-github" "https://github.com/dzoukr" "github.com/dzoukr" |> Html.div
                Html.faIconTextLink "fab fa-twitter" "https://twitter.com/dzoukr" "twitter.com/dzoukr" |> Html.div
                Html.faIconTextLink "fab fa-linkedin" "https://www.linkedin.com/in/dzoukr/" "linkedin.com/in/dzoukr" |> Html.div
                Html.faIconTextLink "far fa-envelope" "mailto:dzoukr@dzoukr.cz" "dzoukr@dzoukr.cz" |> Html.div
            ]
        ]
        Bulma.block [
            Divider.divider [ divider.text "Home Team" ]
            Html.divClassed "home-team" [
                Html.a [
                    prop.children [
                        Html.img [ prop.src "https://www.cngroup.dk/static/media/CN-logo-2019.d720839f.svg" ]
                    ]
                    prop.href "https://www.cngroup.dk"
                ]
            ]
        ]
    ]


let rightCol =
    Html.divClassed "right-col" [
        Bulma.block [
            Bulma.title.h1 [
                prop.children [
                    Html.text "Hi, I'm "
                    Html.strong "Roman Provazník"
                ]
            ]
        ]
        Bulma.block [
            Html.p [
                Html.text "I am F# Team Leader / Architect at "
                Html.a [ prop.text "CN Group CZ"; prop.href "https://www.cngroup.dk" ]
                Html.text ", melomaniac, "
                Html.a ("speaker", Page.Talks)
                Html.text " and a terrible drummer."
            ]
        ]
        Bulma.block [
            Html.p [
                Html.text "I have more than 20 years of experience with software development using languages like Pascal, Delphi, Prolog, PHP, C#, or F#, "
                Html.text "but I successfully managed to erase most of these languages out of my brain."
            ]
        ]
        Bulma.block [
            Html.p [
                Html.text "As a big fan of functional-first .NET language "
                Html.a [ prop.text "F#"; prop.href "https://www.fsharp.org" ]
                Html.text ", I founded the Czech F# community called "
                Html.a [ prop.text "FSharping"; prop.href "https://fsharping.com" ]
                Html.text " which I love to maintain and grow. I also somehow sneaked in F# Software Foundation Board of Trustees for one year."
            ]
        ]
        Bulma.block [
            Html.p [
                Html.text "If you were listening to Kiss Radio between 2002 and 2005, I'm the guy who annoyed you during the morning show."
            ]
        ]
    ]

[<ReactComponent>]
let AboutMeView () =
    Html.divClassed "about-me" [
        Bulma.section [
            Bulma.columns [
                Bulma.column [
                    column.isOneThird
                    prop.children [
                        leftCol
                    ]
                ]
                Bulma.column [
                    column.isTwoThirds
                    prop.children [
                        rightCol
                    ]
                ]
            ]
        ]
    ]