module DzoukrCz.Client.Pages.Index

open Feliz
open Feliz.DaisyUI
open Elmish
open Feliz.UseElmish
open DzoukrCz.Client.Server

type private State = {
    Message : string
}

type private Msg =
    | AskForMessage of bool

let private init () = { Message = "Click on one of the buttons!" }, Cmd.none

let private update (msg:Msg) (model:State) : State * Cmd<Msg> =
    match msg with
    | AskForMessage success -> model, Cmd.none// Cmd.OfAsync.eitherAsResult (fun _ -> service.GetMessage success) MessageReceived

[<ReactComponent>]
let IndexView () =
    let state, dispatch = React.useElmish(init, update, [| |])

    React.fragment [
        Html.div state.Message
        Daisy.join [
            Daisy.button.button [
                join.item
                button.primary
                prop.text "Click me for success"
                prop.onClick (fun _ -> true |> AskForMessage |> dispatch)
            ]
            Daisy.button.button [
                join.item
                button.secondary
                prop.text "Click me for error"
                prop.onClick (fun _ -> false |> AskForMessage |> dispatch)
            ]
        ]
    ]

