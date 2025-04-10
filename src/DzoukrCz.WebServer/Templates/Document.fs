module DzoukrCz.WebServer.Templates.Document

open Feliz.ViewEngine

let html (header:ReactElement) (bodyContent:ReactElement) =
        Html.html [
            prop.lang "en"
            prop.children [
                header
                Html.body [
                    //prop.custom("data-theme","light")
                    prop.className "h-screen"
                    prop.children [ bodyContent ]
                ]
            ]
        ]