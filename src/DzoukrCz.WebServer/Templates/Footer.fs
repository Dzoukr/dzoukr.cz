module DzoukrCz.WebServer.Templates.Footer

open Feliz.ViewEngine
open DzoukrCz.WebServer

let footer =
    let iconLink (icon:string) (text:string) (href:string) =
        Html.divClassed "flex gap-1 items-center" [
            Html.faIcon icon
            Html.a [
                prop.href href
                prop.text text
            ]
        ]

    Html.divClassed "footer footer-vertical lg:footer-horizontal max-w-4xl mx-auto py-5 px-5 lg:px-0" [
        Html.nav [
            Html.p "Roman \"Džoukr\" Provazník"
            Html.p [ prop.text "Building software with ❤️ + 🧠"; prop.className "text-xs opacity-90" ]
        ]
        Html.nav [
            Html.divClassed "footer-title" [ Html.text "Projects" ]
            iconLink "fa-solid fa-podcast" "PodVocasem" "https://www.podvocasem.cz"
            // iconLink "fa-brands fa-twitter" "FSharping" "https://twitter.com/fsharping"
            iconLink "fa-solid fa-code" "fsharpConf" "https://fsharpconf.com"
            iconLink "fa-solid fa-hockey-puck" "Hlášky" "https://hlasky.dzoukr.cz"
        ]
        Html.nav [
            Html.divClassed "footer-title" [ Html.text "Contact" ]
            iconLink "fa-regular fa-envelope" "dzoukr@dzoukr.cz" "mailto:dzoukr@dzoukr.cz"
            iconLink "fa-brands fa-bluesky" "@dzoukr.cz" "https://bsky.app/profile/dzoukr.cz"
            iconLink "fa-brands fa-github" "dzoukr" "https://github.com/dzoukr"
            iconLink "fa-brands fa-linkedin" "Roman Provazník" "https://www.linkedin.com/in/dzoukr/"
        ]
    ]

