module DzoukrCz.Client.View

open State
open Feliz
open Feliz.UseElmish
open Router
open SharedView

[<ReactComponent>]
let AppView () =
    let model, dispatch = React.useElmish(State.init, State.update, [| |])
    let render =
        match model.CurrentPage with
        | Page.Index -> Pages.Index.View.IndexView ()
    React.router [
        router.pathMode
        router.onUrlChanged (Page.parseFromUrlSegments >> UrlChanged >> dispatch)
        router.children [ render ]
    ]