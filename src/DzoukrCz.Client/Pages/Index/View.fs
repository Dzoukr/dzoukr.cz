module DzoukrCz.Client.Pages.Index.View

open System
open Feliz
open Feliz.Bulma
open DzoukrCz.Client.SharedView
open DzoukrCz.Client.Pages.Layout

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
    Html.divClassed "projects" [
        Bulma.block [
            Bulma.title.h2 "F# Projects"
            Bulma.columns [
                Bulma.column (projectInfo Projects.cosmoStore)
                Bulma.column (projectInfo Projects.dapperFSharp)
                Bulma.column (projectInfo Projects.tablesFSharp)
            ]
            Bulma.columns [
                Bulma.column (projectInfo Projects.felizBulma)
                Bulma.column (projectInfo Projects.yobo)
                Bulma.column (projectInfo Projects.safer)
            ]
        ]
    ]

type TalkLang = CZ | EN

type Talk = {
    Date : DateTime
    Event : string
    Title : string
    Location : string
    Link : string option
    Lang : TalkLang
}

module Talks =
    let private prg = "Prague, CZ"
    let private brn = "Brno, CZ"
    let private bra = "Bratislava, SK"
    let private lon = "London, UK"
    let private ber = "Berlin, GER"
    let private olo = "Olomouc, CZ"
    let private zli = "Zlín, CZ"
    let private mei = "Meinz, GER"
    let private vil = "Vilnius, LAT"

    let all = [
        { Date = DateTime(2018,3,29); Event = "SYMA 2018"; Title = "SW Quality Discussion Panel"; Location = prg; Link = Some "https://www.tpconsulting.cz/konference-syma-2018-csj-n24695.htm"; Lang = CZ }
        { Date = DateTime(2018,4,27); Event = "CN Group Partnership Conference"; Title = "Trends in FP"; Location = bra; Link = None; Lang = EN }
        { Date = DateTime(2018,8,28); Event = "FSharping #18"; Title = "Event Sourcing with F# and Azure Cosmos DB"; Location = prg; Link = None; Lang = EN }
        { Date = DateTime(2018,6,4); Event = "F#unctional Londoners Meetup Group"; Title = "Event Sourcing with F# and Azure Cosmos DB"; Location = lon; Link = Some "https://skillsmatter.com/skillscasts/11783-event-sourcing-with-f-sharp-and-azure-cosmos-db"; Lang = EN }
        { Date = DateTime(2018,10,26); Event = "FableConf 2018"; Title = "Event Sourcing with F# and Azure Cosmos DB"; Location = ber; Link = Some "https://www.youtube.com/watch?v=E2oVmA3QKpA"; Lang = EN }
        { Date = DateTime(2018,10,31); Event = "WUG Olomouc"; Title = "Intro into F#"; Location = olo; Link = Some "https://wug.cz/olomouc/akce/1112-Prakticky-uvod-do-jazyka-F"; Lang = CZ }
        { Date = DateTime(2019,3,13); Event = "WUG Zlín"; Title = "Intro into F#"; Location = zli; Link = Some "https://wug.cz/zlin/akce/1128-Prakticky-uvod-do-jazyka-F"; Lang = CZ }
        { Date = DateTime(2019,4,4); Event = "FSharp eXchange 2019"; Title = "Sneaking F# into your organization"; Location = lon; Link = Some "https://skillsmatter.com/skillscasts/13410-lightning-talk-sneaking-f-sharp-into-your-organization"; Lang = EN }
        { Date = DateTime(2019,9,3); Event = ".NETCZ Podcast"; Title = "Talking about F#"; Location = prg; Link = Some "https://www.dotnetpodcast.cz/episodes/ep51/"; Lang = CZ }
        { Date = DateTime(2019,9,14); Event = "WUG Days 2019"; Title = "The state of F# in 2019"; Location = brn; Link = Some "https://wug.cz/brno/akce/1228-Co-je-F-a-jak-se-mu-dari-v-roce-2019"; Lang = CZ }
        { Date = DateTime(2019,9,25); Event = "Basta Conference 2019"; Title = "Functional .NET for Inevitable Success"; Location = mei; Link = None; Lang = EN }
        { Date = DateTime(2019,11,2); Event = "Dotnet Days 2019"; Title = "SAFE Stack: Fullstack Development in F#"; Location = prg; Link = Some "https://www.youtube.com/watch?v=QdNClItMtOM&feature=youtu.be&ab_channel=DotnetDaysCZ"; Lang = CZ }
        { Date = DateTime(2019,11,6); Event = "CN Group Fu**up Night"; Title = "Fuckups in F# team"; Location = zli; Link = None; Lang = CZ }
        { Date = DateTime(2020,10,22); Event = "WUG Days 2020"; Title = "Continuous Deployment on Azure"; Location = prg; Link = Some "https://wug.cz/zaznamy/674-WUG-Days-2020-Automaticky-deployment-na-Azure-chytre-a-bezpecne"; Lang = CZ }
        { Date = DateTime(2020,10,22); Event = "WUG Days 2020"; Title = "The state of F# in 2020"; Location = prg; Link = Some "https://wug.cz/zaznamy/686-WUG-Days-2020-F-v-roce-2020"; Lang = CZ }
        { Date = DateTime(2020,11,3); Event = "TopMonks Caffè"; Title = "Continuous Deployment on Azure"; Location = prg; Link = Some "https://www.youtube.com/watch?v=1_VKebxX-vU"; Lang = CZ }
        { Date = DateTime(2021,6,8); Event = "DevDays Europe 2021"; Title = "Functional .NET for Inevitable Success"; Location = vil; Link = Some "https://devdays.lt/roman-provaznik/"; Lang = EN }
    ]

[<ReactComponent>]
let Talks () =
    let upcoming,past = Talks.all |> List.sortByDescending (fun x -> x.Date) |> List.partition (fun x -> x.Date > DateTime.UtcNow)
    let active,setActive = React.useState(1)
    let data = if active = 1 then upcoming else past

    Html.divClassed "talks" [
        Bulma.block [
            Bulma.title.h2 "Talks"
            Bulma.tabs [
                tabs.isMedium
                tabs.isToggle
                prop.children [
                    Html.ul [
                        Bulma.tab [
                            if active = 1 then tab.isActive
                            prop.children [
                                Html.a [
                                    prop.text "Upcoming"
                                    prop.href "#"
                                    prop.onClick (fun e -> e.preventDefault(); 1 |> setActive)
                                ]
                            ]
                        ]
                        Bulma.tab [
                            if active = 2 then tab.isActive
                            prop.children [
                                Html.a [
                                    prop.text "Past"
                                    prop.href "#"
                                    prop.onClick (fun e -> e.preventDefault(); 2 |> setActive)
                                ]
                            ]
                        ]
                    ]
                ]
            ]

            if data.Length > 0 then
                Bulma.table [
                    table.isFullWidth
                    table.isStriped
                    table.isNarrow
                    prop.children [
                        Html.thead [
                            Html.th "Date"
                            Html.th "Location"
                            Html.th "Event"
                            Html.th "Title"
                            Html.th "Language"
                        ]
                        Html.tbody [
                            for d in data do
                                Html.tr [
                                    Html.td [ prop.text (d.Date.ToString("dd. MM. yyyy")) ]
                                    Html.td [ prop.text d.Location ]
                                    Html.td [ prop.text d.Event ]
                                    Html.td [
                                        match d.Link with
                                        | Some l -> Html.a [ prop.href l; prop.text d.Title ]
                                        | None -> Html.text d.Title
                                    ]
                                    Html.td [ prop.text (string d.Lang) ]
                                ]
                        ]
                    ]
                ]
            else
                Bulma.block [
                    Html.text "No talk available now, but stay tuned :)"
                ]
        ]
    ]

[<ReactComponent>]
let IndexView () =
    [
        projects
        Talks ()
    ]
    |> React.fragment
    |> Bulma.box
    |> basic