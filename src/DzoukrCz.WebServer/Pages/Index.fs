module DzoukrCz.WebServer.Pages.Index

open Feliz.ViewEngine
open DzoukrCz.WebServer
open DzoukrCz.WebServer.Templates
open DzoukrCz.WebServer.Site

let private body =
    Html.divClassed "formatted-text" [
        Html.img [ prop.src "/img/profile.png"; prop.className "not-prose mx-auto lg:float-left mb-6 lg:my-0 lg:mr-8 w-[60%] lg:w-[30%] object-cover rounded-xl" ]
        Html.h1 "👋 Hi, I'm Roman"
        Html.p [
            Html.text "I am a Head of Product Engineering at "
            Html.a [
                prop.text "Ciklum Czech Republic & Slovakia"
                prop.href "https://www.ciklum.com/we"
            ]
            Html.text ", melomaniac, "
            Html.a [
                prop.text "speaker"
                prop.href SiteUrls.SpeakingUrl
            ]
            Html.text ", "
            Html.a [ prop.text "podcaster"; prop.href "https://www.podvocasem.cz" ]
            Html.text ", and a terrible drummer."
        ]
        Html.p "I have more than 20 years of experience with software development using languages like Pascal, Delphi, Prolog, PHP, C#, or F#, but I successfully managed to erase most of these languages from my brain."
        Html.p [
            Html.text "As a big fan of functional-first .NET language "
            Html.a [ prop.text "F#"; prop.href "https://fsharp.org" ]
            Html.text ", I founded the Czech F# community called FSharping which I love to maintain and grow. I also somehow sneaked into F# Software Foundation Board of Trustees for one year."
        ]
        Html.p "If you were listening to Kiss Radio between 2002 and 2005, I'm the guy who annoyed you during the morning show."
        Html.p [
            Html.text "Since October 2021, I am co-hosting a Czech IT podcast called "
            Html.a [ prop.text "PodVocasem"; prop.href "https://www.podvocasem.cz" ]
            Html.text "."
        ]
    ]

let page = Document.html SiteUrls.IndexUrl body
