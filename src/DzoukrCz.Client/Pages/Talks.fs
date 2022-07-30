module DzoukrCz.Client.Pages.Talks

open System
open Feliz
open Feliz.Bulma
open DzoukrCz.Client.SharedView

type TalkLang = CZ | EN

type Talk = {
    Date : DateTime
    Event : string
    Title : string
    Location : string
    Link : string option
    Lang : TalkLang
    Logo : string option
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
    let private onl = "Online"
    let private oslo = "Oslo, NOR"

    let private wugLogo = Some "https://www.wug.cz/App_Themes/Default/Img/wug-logo-twitter-square.png"
    let private symaLogo = Some "https://casopisczechindustry.cz/_files/system_preview_200001795-89bca8abb3/csj.JPG"
    let private cnLogo = Some "https://res.cloudinary.com/dzoukr/image/upload/c_scale,q_100,w_200/v1611912090/dzoukr.cz/CN_Group_icon.png"
    let private tmLogo = Some "https://media-exp1.licdn.com/dms/image/C4D0BAQFtnoJx62rHjw/company-logo_200_200/0/1531926392509?e=2159024400&v=beta&t=nikuxR_Xzq38VfI1ZCI5mhLb2voU70jNTjIot6BI2qA"
    let private fsLogo = Some "https://pbs.twimg.com/profile_images/662191384762388480/DUCx3K5T_400x400.png"
    let private fableLogo = Some "https://www.softwaretalks.io/images/conferences/fable-conf.jpg"
    let private londonLogo = Some "https://secure.meetupstatic.com/photos/event/b/4/c/4/global_12946276.jpeg"
    let private fsxLogo = Some "https://res.cloudinary.com/skillsmatter/image/upload/c_crop,g_custom/c_fit,w_127,h_127/v1548240171/d4tb0nrzseddydmvbay8.png"
    let private czPodcastLogo = Some "https://i1.sndcdn.com/avatars-000336810693-s58ehz-t200x200.jpg"
    let private bastaLogo = Some "https://res.cloudinary.com/dzoukr/image/upload/v1611908339/dzoukr.cz/BASTA_2019_Logo_4c_52294_v1_zbh52v.png"
    let private ddaysLogo = Some "https://res.cloudinary.com/dzoukr/image/upload/c_scale,q_100,w_200/v1611909106/dzoukr.cz/dotnet-days-logo.png"
    let private devDaysLogo = Some "https://pbs.twimg.com/profile_images/982141167687974912/kaBQ0Odd.jpg"
    let private twitchLogo = Some "https://res.cloudinary.com/dzoukr/image/upload/c_scale,q_100,w_200/v1612163006/dzoukr.cz/twitch.png"
    let private fsConfLogo = Some "https://res.cloudinary.com/dzoukr/image/upload/c_scale,q_100,w_300/v1611912585/dzoukr.cz/fsharpconf2020.png"
    let private ndcLogo = Some "data:image/png;base64,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"

    let all = [
        { Date = DateTime(2018,3,29); Logo = symaLogo; Event = "SYMA 2018"; Title = "SW Quality Discussion Panel"; Location = prg; Link = Some "https://www.tpconsulting.cz/konference-syma-2018-csj-n24695.htm"; Lang = CZ }
        { Date = DateTime(2018,4,27); Logo = cnLogo; Event = "CN Group Partnership Conference"; Title = "Trends in FP"; Location = bra; Link = None; Lang = EN }
        { Date = DateTime(2018,8,28); Logo = fsLogo; Event = "FSharping #18"; Title = "Event Sourcing with F# and Azure Cosmos DB"; Location = prg; Link = None; Lang = EN }
        { Date = DateTime(2018,6,4); Logo = londonLogo; Event = "F#unctional Londoners Meetup Group"; Title = "Event Sourcing with F# and Azure Cosmos DB"; Location = lon; Link = Some "https://skillsmatter.com/skillscasts/11783-event-sourcing-with-f-sharp-and-azure-cosmos-db"; Lang = EN }
        { Date = DateTime(2018,10,26); Logo = fableLogo; Event = "FableConf 2018"; Title = "Event Sourcing with F# and Azure Cosmos DB"; Location = ber; Link = Some "https://www.youtube.com/watch?v=E2oVmA3QKpA"; Lang = EN }
        { Date = DateTime(2018,10,31); Logo = wugLogo; Event = "WUG Olomouc"; Title = "Intro into F#"; Location = olo; Link = Some "https://wug.cz/olomouc/akce/1112-Prakticky-uvod-do-jazyka-F"; Lang = CZ }
        { Date = DateTime(2019,3,13); Logo = wugLogo; Event = "WUG Zlín"; Title = "Intro into F#"; Location = zli; Link = Some "https://wug.cz/zlin/akce/1128-Prakticky-uvod-do-jazyka-F"; Lang = CZ }
        { Date = DateTime(2019,4,4); Logo = fsxLogo; Event = "FSharp eXchange 2019"; Title = "Sneaking F# into your organization"; Location = lon; Link = Some "https://skillsmatter.com/skillscasts/13410-lightning-talk-sneaking-f-sharp-into-your-organization"; Lang = EN }
        { Date = DateTime(2019,9,3); Logo = czPodcastLogo; Event = ".NETCZ Podcast"; Title = "Talking about F#"; Location = prg; Link = Some "https://www.dotnetpodcast.cz/episodes/ep51/"; Lang = CZ }
        { Date = DateTime(2019,9,14); Logo = wugLogo; Event = "WUG Days 2019"; Title = "The state of F# in 2019"; Location = brn; Link = Some "https://wug.cz/brno/akce/1228-Co-je-F-a-jak-se-mu-dari-v-roce-2019"; Lang = CZ }
        { Date = DateTime(2019,9,25); Logo = bastaLogo; Event = "Basta Conference 2019"; Title = "Functional .NET for Inevitable Success"; Location = mei; Link = None; Lang = EN }
        { Date = DateTime(2019,11,2); Logo = ddaysLogo; Event = "Dotnet Days 2019"; Title = "SAFE Stack: Fullstack Development in F#"; Location = prg; Link = Some "https://www.youtube.com/watch?v=QdNClItMtOM&feature=youtu.be&ab_channel=DotnetDaysCZ"; Lang = CZ }
        { Date = DateTime(2019,11,6); Logo = cnLogo; Event = "CN Group Fu**up Night"; Title = "Fuckups in F# team"; Location = zli; Link = None; Lang = CZ }
        { Date = DateTime(2020,06,5); Logo = fsConfLogo; Event = "FSharpConf 2020"; Title = "FSharpConf 2020"; Location = onl; Link = Some "https://www.youtube.com/watch?v=ybkYHYKYeNw"; Lang = EN }
        { Date = DateTime(2020,10,22); Logo = wugLogo; Event = "WUG Days 2020"; Title = "Continuous Deployment on Azure"; Location = prg; Link = Some "https://wug.cz/zaznamy/674-WUG-Days-2020-Automaticky-deployment-na-Azure-chytre-a-bezpecne"; Lang = CZ }
        { Date = DateTime(2020,10,22); Logo = wugLogo; Event = "WUG Days 2020"; Title = "The state of F# in 2020"; Location = prg; Link = Some "https://wug.cz/zaznamy/686-WUG-Days-2020-F-v-roce-2020"; Lang = CZ }
        { Date = DateTime(2020,11,3); Logo = tmLogo; Event = "TopMonks Caffè"; Title = "Continuous Deployment on Azure"; Location = prg; Link = Some "https://www.youtube.com/watch?v=1_VKebxX-vU"; Lang = CZ }
        { Date = DateTime(2021,01,28); Logo = twitchLogo; Event = "Twitch Stream"; Title = "Fable Introduction with Zaid Ajaj"; Location = onl; Link = Some "https://www.twitch.tv/videos/891580510"; Lang = EN }
        { Date = DateTime(2021,2,23); Logo = cnLogo; Event = "CN Group Webinar"; Title = "Speed up time to market delivery with the Functional Approach"; Location = onl; Link = Some "https://cngroup.clickmeeting.com/566151195/register"; Lang = EN }
        { Date = DateTime(2021,6,9); Logo = devDaysLogo; Event = "DevDays Europe 2021"; Title = "Functional .NET for Inevitable Success"; Location = vil; Link = Some "https://devdays.lt/roman-provaznik/"; Lang = EN }
        { Date = DateTime(2021,8,30); Logo = cnLogo; Event = "Functional Programming Evening"; Title = "F# vs Elm"; Location = zli; Link = None; Lang = CZ }
        { Date = DateTime(2021,10,20); Logo = fsxLogo; Event = "FSharp eXchange 2021"; Title = "In defense of Exceptions: Throw (away) your Result"; Location = onl; Link = Some "https://skillsmatter.com/skillscasts/17243-in-defense-of-exceptions-throw-away-your-result"; Lang = EN }
        { Date = DateTime(2022,2,3); Logo = wugLogo; Event = ".NET 6 Extravaganza"; Title = "Novinky v F#6"; Location = onl; Link = Some "https://wug.cz/online/akce/1410--NET-6-extravaganza-Novinky-v-F-6"; Lang = CZ }
        { Date = DateTime(2022,9,28); Logo = ndcLogo; Event = "NDC Oslo"; Title = "Reducing the environmental footprint in nautical transport with F# & Serverless"; Location = oslo; Link = Some "https://ndcoslo.com/agenda/reducing-the-environmental-footprint-in-nautical-transport-with-f-and-serverless-0dwq/0sorc47549t"; Lang = EN }
    ]

let talkInfo (t:Talk) =
    let date = t.Date.ToString("dd. MM. yyyy")
    let now = DateTime.UtcNow
    let isPast = t.Date.Date < now.Date
    let daysLeft = (t.Date.Date - now).TotalDays
    let startsInText =
        if daysLeft > 1. then $"Starts in {int daysLeft} days"
        else if daysLeft > 0. && daysLeft < 1. then "Starts tomorrow"
        else "Starts today!"

    let inLink (elm:ReactElement) =
        if t.Link.IsSome then
            Html.a [
                prop.href t.Link.Value
                prop.children [ elm ]
            ]
        else elm

    Bulma.card [
        if isPast then prop.className "is-past"
        prop.children [
            [
                if not isPast then
                    Html.divClassed "talk-teaser" [
                        Bulma.tag [
                            tag.isMedium
                            color.isSuccess
                            prop.text startsInText
                        ]
                    ]
                Bulma.block [
                    if t.Logo.IsSome then
                        Html.img [
                            prop.src t.Logo.Value
                        ]
                    Bulma.title.h5 t.Title
                ]
                Html.div [
                    Html.divClassed "talk-event" [ Html.text t.Event ]
                    Html.divClassed "talk-date" [
                        Html.faIcon "far fa-calendar-alt"
                        Html.text date
                    ]
                ]
            ]
            |> React.fragment
            |> Bulma.cardContent
            |> inLink
        ]
    ]

let contactMeTalk =
    Bulma.card [
        Html.a [
            prop.href "mailto:dzoukr@dzoukr.cz"
            prop.children [
                Bulma.cardContent [
                    prop.children [
                        Html.divClassed "talk-teaser" [
                            Bulma.tag [
                                tag.isMedium
                                color.isInfo
                                prop.text "Invite me to talk"
                            ]
                        ]
                        Bulma.block [
                            Html.divClassed "talk-icon" [
                                Html.i [
                                    prop.className "fas fa-plus-circle"
                                ]
                            ]
                            Bulma.title.h5 [
                                Html.text "Next awesome talk you gonna love"
                            ]
                        ]
                        Html.div [
                            Html.divClassed "talk-event" [ Html.text "Your amazing conference" ]
                            Html.divClassed "talk-date" [
                                Html.faIcon "far fa-calendar-alt"
                                Html.text "Bright future"
                            ]
                        ]
                    ]
                ]
            ]
        ]
    ]

[<ReactComponent>]
let TalksView () =
    let talks = Talks.all |> List.sortByDescending (fun x -> x.Date)
    let inCol elm = Bulma.column [ column.is3Widescreen; column.is6Tablet; prop.children [ elm ] ]
    let future,past = talks |> List.partition (fun x -> x.Date > DateTime.UtcNow)

    Html.divClassed "talks" [
        Bulma.section [
            Bulma.block [
                Bulma.title.h1 "Talks & Events"
                Bulma.columns [
                    columns.isMultiline
                    prop.children [
                        yield! future |> List.map (talkInfo >> inCol)
                        contactMeTalk |> inCol
                        yield! past |> List.map (talkInfo >> inCol)
                    ]
                ]
            ]
        ]
    ]