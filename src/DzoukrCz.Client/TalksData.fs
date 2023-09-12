module DzoukrCz.Client.TalksData

open System

type TalkLang = CZ | EN

type Talk = {
    Date : DateTime
    Event : string
    Title : string
    Location : string
    Link : string option
    Lang : TalkLang
    Logo : string
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

    module Logos =
        let syma = "img/syma_logo.png"
        let cn = "img/cn_logo.png"
        let fsLondon = "img/fslondon_logo.jpg"
        let fsharping = "img/fsharping_logo.png"
        let fable = "img/fable_logo.png"
        let wug = "img/wug_logo.png"
        let fsx2019 = "img/fsx_2019_logo.webp"
        let ddays = "img/dotnet_days_logo.png"
        let dotnetPodcast = "img/dotnet_podcast_logo.jpg"
        let basta = "img/basta_logo.png"
        let fsConf = "img/fsharpconf2020_logo.png"
        let tm = "img/tm_logo.jpg"
        let twitch = "img/twitch_logo.png"
        let devDays = "img/devdays_logo.png"
        let fsx2021 = "img/fsx_2021_logo.png"
        let ndc = "img/ndc_logo.png"
        let jbDays2022 = "img/jb_days_2022.jpg"

    let all = [
        { Date = DateTime(2018,3,29); Logo = Logos.syma; Event = "SYMA 2018"; Title = "SW Quality Discussion Panel"; Location = prg; Link = Some "https://www.tpconsulting.cz/konference-syma-2018-csj-n24695.htm"; Lang = CZ }
        { Date = DateTime(2018,4,27); Logo = Logos.cn; Event = "CN Group Partnership Conference"; Title = "Trends in FP"; Location = bra; Link = None; Lang = EN }
        { Date = DateTime(2018,8,28); Logo = Logos.fsharping; Event = "FSharping #18"; Title = "Event Sourcing with F# and Azure Cosmos DB"; Location = prg; Link = None; Lang = EN }
        { Date = DateTime(2018,6,4); Logo = Logos.fsLondon; Event = "F#unctional Londoners Meetup Group"; Title = "Event Sourcing with F# and Azure Cosmos DB"; Location = lon; Link = Some "https://skillsmatter.com/skillscasts/11783-event-sourcing-with-f-sharp-and-azure-cosmos-db"; Lang = EN }
        { Date = DateTime(2018,10,26); Logo = Logos.fable; Event = "FableConf 2018"; Title = "Event Sourcing with F# and Azure Cosmos DB"; Location = ber; Link = Some "https://www.youtube.com/watch?v=E2oVmA3QKpA"; Lang = EN }
        { Date = DateTime(2018,10,31); Logo = Logos.wug; Event = "WUG Olomouc"; Title = "Intro into F#"; Location = olo; Link = Some "https://wug.cz/olomouc/akce/1112-Prakticky-uvod-do-jazyka-F"; Lang = CZ }
        { Date = DateTime(2019,3,13); Logo = Logos.wug; Event = "WUG Zlín"; Title = "Intro into F#"; Location = zli; Link = Some "https://wug.cz/zlin/akce/1128-Prakticky-uvod-do-jazyka-F"; Lang = CZ }
        { Date = DateTime(2019,4,4); Logo = Logos.fsx2019; Event = "FSharp eXchange 2019"; Title = "Sneaking F# into your organization"; Location = lon; Link = Some "https://skillsmatter.com/skillscasts/13410-lightning-talk-sneaking-f-sharp-into-your-organization"; Lang = EN }
        { Date = DateTime(2019,9,3); Logo = Logos.dotnetPodcast; Event = ".NETCZ Podcast"; Title = "Talking about F#"; Location = prg; Link = Some "https://www.dotnetpodcast.cz/episodes/ep51/"; Lang = CZ }
        { Date = DateTime(2019,9,14); Logo = Logos.wug; Event = "WUG Days 2019"; Title = "The state of F# in 2019"; Location = brn; Link = Some "https://wug.cz/brno/akce/1228-Co-je-F-a-jak-se-mu-dari-v-roce-2019"; Lang = CZ }
        { Date = DateTime(2019,9,25); Logo = Logos.basta; Event = "Basta Conference 2019"; Title = "Functional .NET for Inevitable Success"; Location = mei; Link = None; Lang = EN }
        { Date = DateTime(2019,11,2); Logo = Logos.ddays; Event = "Dotnet Days 2019"; Title = "SAFE Stack: Fullstack Development in F#"; Location = prg; Link = Some "https://www.youtube.com/watch?v=QdNClItMtOM&feature=youtu.be&ab_channel=DotnetDaysCZ"; Lang = CZ }
        { Date = DateTime(2019,11,6); Logo = Logos.cn; Event = "CN Group Fu**up Night"; Title = "Fuckups in F# team"; Location = zli; Link = None; Lang = CZ }
        { Date = DateTime(2020,06,5); Logo = Logos.fsConf; Event = "FSharpConf 2020"; Title = "FSharpConf 2020"; Location = onl; Link = Some "https://www.youtube.com/watch?v=ybkYHYKYeNw"; Lang = EN }
        { Date = DateTime(2020,10,22); Logo = Logos.wug; Event = "WUG Days 2020"; Title = "Continuous Deployment on Azure"; Location = prg; Link = Some "https://wug.cz/zaznamy/674-WUG-Days-2020-Automaticky-deployment-na-Azure-chytre-a-bezpecne"; Lang = CZ }
        { Date = DateTime(2020,10,22); Logo = Logos.wug; Event = "WUG Days 2020"; Title = "The state of F# in 2020"; Location = prg; Link = Some "https://wug.cz/zaznamy/686-WUG-Days-2020-F-v-roce-2020"; Lang = CZ }
        { Date = DateTime(2020,11,3); Logo = Logos.tm; Event = "TopMonks Caffè"; Title = "Continuous Deployment on Azure"; Location = prg; Link = Some "https://www.youtube.com/watch?v=1_VKebxX-vU"; Lang = CZ }
        { Date = DateTime(2021,01,28); Logo = Logos.twitch; Event = "Twitch Stream"; Title = "Fable Introduction with Zaid Ajaj"; Location = onl; Link = Some "https://www.twitch.tv/videos/891580510"; Lang = EN }
        { Date = DateTime(2021,2,23); Logo = Logos.cn; Event = "CN Group Webinar"; Title = "Speed up time to market delivery with the Functional Approach"; Location = onl; Link = Some "https://cngroup.clickmeeting.com/566151195/register"; Lang = EN }
        { Date = DateTime(2021,6,9); Logo = Logos.devDays; Event = "DevDays Europe 2021"; Title = "Functional .NET for Inevitable Success"; Location = onl; Link = Some "https://devdays.lt/roman-provaznik/"; Lang = EN }
        { Date = DateTime(2021,8,30); Logo = Logos.cn; Event = "Functional Programming Evening"; Title = "F# vs Elm"; Location = zli; Link = None; Lang = CZ }
        { Date = DateTime(2021,10,20); Logo = Logos.fsx2021; Event = "FSharp eXchange 2021"; Title = "In defense of Exceptions: Throw (away) your Result"; Location = onl; Link = Some "https://skillsmatter.com/skillscasts/17243-in-defense-of-exceptions-throw-away-your-result"; Lang = EN }
        { Date = DateTime(2022,2,3); Logo = Logos.wug; Event = ".NET 6 Extravaganza"; Title = "Novinky v F#6"; Location = onl; Link = Some "https://wug.cz/online/akce/1410--NET-6-extravaganza-Novinky-v-F-6"; Lang = CZ }
        { Date = DateTime(2022,9,28); Logo = Logos.ndc; Event = "NDC Oslo"; Title = "Reducing the environmental footprint in nautical transport with F# & Serverless"; Location = oslo; Link = Some "https://ndcoslo.com/agenda/reducing-the-environmental-footprint-in-nautical-transport-with-f-and-serverless-0dwq/0sorc47549t"; Lang = EN }
        { Date = DateTime(2022,10,26); Logo = Logos.jbDays2022; Event = "JetBrains .NET Days 2022"; Title = "Reducing the environmental footprint in nautical transport with F# & Serverless"; Location = onl; Link = Some "https://www.youtube.com/watch?v=2xLnUhnv_wY&ab_channel=JetBrains"; Lang = EN }
        { Date = DateTime(2023,09,11); Logo = Logos.wug; Event = "WUG Days 2023"; Title = "Burn all the servers!"; Location = onl; Link = Some "https://www.wug.cz/brno/akce/1593-Spalte-servery"; Lang = CZ }
    ]