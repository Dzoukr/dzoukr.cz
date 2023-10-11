module DzoukrCz.Client.Pages.Index

open Feliz
open Feliz.DaisyUI
open Elmish
open Feliz.UseElmish
open DzoukrCz.Client.Server
open DzoukrCz.Client

type private State = {
    Message : string
}

type private Msg =
    | AskForMessage of bool

let private init () = { Message = "Click on one of the buttons!" }, Cmd.none

let private update (msg:Msg) (model:State) : State * Cmd<Msg> =
    match msg with
    | AskForMessage success -> model, Cmd.none// Cmd.OfAsync.eitherAsResult (fun _ -> service.GetMessage success) MessageReceived

let private leftCol =
    let iconLink (icon:string) (text:string) (href:string) =
        Html.divClassed "flex gap-1 items-center" [
            Html.faIcon icon
            Daisy.link [
                prop.href href
                prop.text text
            ]
        ]


    Html.divClassed "flex flex-col gap-8 items-center p-4" [

        Html.divClassed "flex flex-col gap-4 items-center" [
            Html.img [ prop.src "img/profile.png"; prop.className "mask mask-circle w-48 lg:w-64" ]
            Html.divClassed "text-xl" [ Html.text "Building software with ❤️ + 🧠" ]
        ]
        Html.divClassed "flex flex-col gap-1 self-start" [
            Html.divClassed "text-lg font-medium" [ Html.text "Contact" ]
            iconLink "fa-regular fa-envelope" "dzoukr@dzoukr.cz" "mailto:dzoukr@dzoukr.cz"
            iconLink "fa-brands fa-twitter" "@dzoukr" "https://twitter.com/dzoukr"
            iconLink "fa-brands fa-github" "dzoukr" "https://github.com/dzoukr"
            iconLink "fa-brands fa-linkedin" "Roman Provazník" "https://www.linkedin.com/in/dzoukr/"
        ]

        //Daisy.divider [ divider.vertical; prop.text "Home Team" ]
    ]

[<ReactComponent>]
let IndexView () =
    let state, dispatch = React.useElmish(init, update, [| |])

    Html.divClassed "flex flex-col lg:flex-row gap-8 my-8" [
        Html.divClassed "basis-1/3" [ leftCol ]
        Html.divClassed "basis-2/3" [ Html.div "A" ]
    ]


