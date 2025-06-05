module DzoukrCz.WebServer.Templates.Document

open Feliz.ViewEngine
open DzoukrCz.WebServer

let html (header:ReactElement) (activeUrl:string) (bodyContent:ReactElement) =
    let padding = "py-3 px-3 lg:py-5 lg:px-0"
    Html.html [
        prop.lang "en"
        prop.children [
            header
            Html.body [
                prop.custom("data-theme","dark")
                prop.className "h-screen"
                prop.children [
                    Html.divClassed "grow max-w-4xl mx-auto" [
                        Html.divClassed padding [ Menu.menu activeUrl ]
                        Html.hr [ prop.className "text-gray-700" ]
                        Html.divClassed (padding + " mt-2") [ bodyContent ]
                        Html.hr [ prop.className "text-gray-700" ]
                        Footer.footer
                    ]
                ]
            ]
        ]
    ]

