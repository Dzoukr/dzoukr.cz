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
        { Date = DateTime(2021,6,8); Logo = devDaysLogo; Event = "DevDays Europe 2021"; Title = "Functional .NET for Inevitable Success"; Location = vil; Link = Some "https://devdays.lt/roman-provaznik/"; Lang = EN }
        { Date = DateTime(2021,2,23); Logo = cnLogo; Event = "CN Group Webinar"; Title = "Speed up time to market delivery with the Functional Approach"; Location = onl; Link = Some "https://cngroup.clickmeeting.com/566151195/register"; Lang = EN }
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
            |> inLink
            |> Bulma.cardContent
        ]
    ]

let contactMeTalk =
    Bulma.card [
        Bulma.cardContent [
            Html.a [
                prop.href "mailto:dzoukr@dzoukr.cz"
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