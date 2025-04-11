module DzoukrCz.WebServer.Templates.Header

open Feliz.ViewEngine

let private stylesTags =
#if DEBUG
        [
            Html.script [ prop.type' "module"; prop.src "http://localhost:8080/@vite/client" ]
            Html.script [ prop.type' "module"; prop.src "http://localhost:8080/scripts.js" ]
            Html.link [ prop.rel "stylesheet"; prop.href "http://localhost:8080/styles.css" ]
        ]
#else
        [
            Html.link [ prop.rel "stylesheet"; prop.href "/styles.css" ]
            Html.script [ prop.type' "module"; prop.src "/scripts.js" ]
        ]
#endif

let head =
    Html.head [
        Html.title "Džoukr.cz"
        Html.meta [ prop.charset "utf-8" ]
        Html.meta [ prop.name "viewport"; prop.content "width=device-width, initial-scale=1, shrink-to-fit=no" ]
        yield! stylesTags
    ]