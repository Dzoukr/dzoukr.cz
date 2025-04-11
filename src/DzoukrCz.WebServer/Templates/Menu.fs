module DzoukrCz.WebServer.Templates.Menu

open Feliz.ViewEngine
open DzoukrCz.WebServer
open DzoukrCz.WebServer.Site

let private btn (activeUrl:string) (name:string) (link:string)  =
    let isActive = activeUrl = link
    let css = "text-2xl text-warning py-2 px-4 not-a"
    Html.a [
        prop.text name
        if isActive then prop.className (css + " text-neutral bg-neutral outline rounded-sm")
        else prop.className css
        prop.href link
    ]

let private btnMobile (activeUrl:string) (name:string) (link:string)  =
    let isActive = activeUrl = link
    let css = "text-warning py-2 text-xl not-a"
    Html.li [
        Html.a [
            prop.text name
            if isActive then prop.className (css + " text-neutral bg-neutral outline rounded-sm")
            else prop.className css
            prop.href link
        ]
    ]

let menu (activeUrl:string) =
    let btn = btn activeUrl
    let btnMobile = btnMobile activeUrl

    Html.div [
        Html.divClassed "navbar" [
            Html.divClassed "navbar-start" [
                Html.a [
                    prop.className "text-2xl lg:text-3xl font-bold not-a"
                    prop.children [
                        Html.span "<džoukr"
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
                    //btn "blog" SiteUrls.BlogUrl
                    //btn "projects" "" false
                ]

                Html.div [
                    prop.id "mobileMenuBtn"
                    prop.className "flex sm:hidden"
                    prop.children [
                        Html.faIcon "fa-solid fa-bars p-2 text-warning bg-neutral outline rounded-sm"
                    ]
                ]
            ]
        ]
        Html.div [
            prop.id "mobileMenu"
            prop.className "flex sm:hidden hidden"
            prop.children [
                Html.ul [
                    prop.className "menu w-full"
                    prop.children [
                        btnMobile "home" SiteUrls.IndexUrl
                        btnMobile "speaking" SiteUrls.SpeakingUrl
                        //btnMobile "blog" SiteUrls.BlogUrl
                    ]
                ]
            ]
        ]
    ]
