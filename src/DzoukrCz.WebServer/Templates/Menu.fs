module DzoukrCz.WebServer.Templates.Menu

open Feliz.ViewEngine
open DzoukrCz.WebServer
open DzoukrCz.WebServer.Site

let private btn (activeUrl:string) (name:string) (link:string)  =
    let isActive = activeUrl = link
    let css = "text-2xl text-warning py-2 px-4"
    Html.a [
        prop.text name
        if isActive then prop.className (css + " text-neutral bg-neutral outline rounded-sm")
        else prop.className css
        prop.href link
    ]

let menu (activeUrl:string) =
    let btn = btn activeUrl

    Html.divClassed "pb-4 border-b-1 border-neutral-700" [
        Html.divClassed "navbar" [
            Html.divClassed "navbar-start" [
                Html.a [
                    prop.className "text-2xl lg:text-3xl font-bold not-a"
                    prop.children [
                        Html.span "<dzoukr"
                        Html.span [ prop.className " text-warning"; prop.text ".cz/>" ]
                    ]
                    prop.href "/"
                ]
            ]
            Html.divClassed "navbar-center" []
            Html.divClassed "navbar-end" [
                Html.divClassed "flex gap-2 hidden sm:flex" [
                    btn "home" SiteUrls.IndexUrl
                    btn "speaking" SiteUrls.SpeakingUrl
                    btn "blog" SiteUrls.BlogUrl
                    //btn "projects" "" false
                ]

                Html.div [
                    prop.className "flex sm:hidden"
                    prop.children [
                        Html.faIcon "fa-solid fa-bars p-2 text-warning bg-neutral outline rounded-sm"
                    ]
                    prop.custom("onclick", """document.getElementById('myElement').style.display = document.getElementById('myElement').style.display === 'none' ? 'block' : 'none';""")
                ]
            ]
        ]
        Html.div [
            prop.className "flex sm:hidden"
            prop.children [
                Html.ul [
                    prop.id "myElement"
                    prop.style [ style.display.none ]
                    prop.className "menu"
                    prop.tabIndex 0
                    prop.children [
                        Html.li "AAAA"
                    ]
                ]
            ]
        ]
    ]
