module DzoukrCz.Client.Pages.Index.View

open System
open Feliz
open Feliz.Bulma
open State
open Feliz.UseElmish
open DzoukrCz.Client.SharedView
open DzoukrCz.Client.Pages.Layout

[<ReactComponent>]
let IndexView () =
    let model, dispatch = React.useElmish(State.init, State.update, [| |])
    Bulma.box [
        Html.h1 """Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vestibulum fermentum eleifend volutpat. Aliquam nec nisl eu lacus ullamcorper varius vitae a felis. Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia curae; Vivamus bibendum auctor scelerisque. Quisque pulvinar mattis augue. Maecenas vel massa mi. Cras tristique id nisi sit amet mollis. Aenean lectus urna, vehicula nec varius quis, rhoncus ac libero. Sed volutpat ante id scelerisque aliquet. Vivamus sollicitudin massa sed ligula commodo, id convallis eros rhoncus. Nam mollis blandit feugiat. Suspendisse neque orci, tempor a rhoncus ut, maximus ac elit. Ut in tristique libero, vitae posuere dolor. Vestibulum id ex et tellus fringilla facilisis sit amet vel tellus.

Maecenas nec suscipit nibh. Ut aliquet magna at neque condimentum, sit amet pulvinar tellus malesuada. Nulla facilisi. Sed nec tristique quam. Sed placerat felis non viverra posuere. Quisque condimentum pellentesque facilisis. Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nam a accumsan quam. In sit amet enim magna. Mauris maximus tincidunt luctus. Etiam dictum nec libero luctus rhoncus. Ut luctus mauris rhoncus justo sagittis ultricies. Quisque sit amet felis congue ante venenatis volutpat sit amet in magna. Sed quis vestibulum erat. Donec aliquam pellentesque tempus.

Etiam sed ligula eget tortor vehicula tempor. Proin eleifend ligula purus, eu scelerisque odio maximus id. Donec laoreet dui et nisl pharetra dapibus. Morbi porttitor diam in pretium convallis. Mauris quis neque nec lorem consequat varius eget ut sapien. Proin pharetra laoreet volutpat. Nulla et risus ex. Suspendisse ornare ultrices nisl, id varius orci facilisis quis. Fusce ac felis lorem. Interdum et malesuada fames ac ante ipsum primis in faucibus. Phasellus laoreet porta tortor sit amet lacinia. Sed vel nulla vitae lacus consectetur ultrices non nec ligula. Sed semper, tellus vitae imperdiet gravida, eros urna mattis dolor, vitae facilisis erat ligula vitae orci. Quisque aliquam nisi elit, vel fermentum orci ornare ac. In nisl est, posuere sit amet magna ullamcorper, molestie sollicitudin urna.

Sed quis volutpat mauris, id molestie urna. Nam quis lobortis risus, vitae mattis tellus. Aliquam pharetra diam eu metus mattis, eu suscipit leo venenatis. Maecenas sed nulla vitae nunc rutrum semper. Integer porttitor tellus a leo pharetra vulputate. Vestibulum finibus neque vitae pellentesque scelerisque. Integer dui nisi, sollicitudin et pellentesque eu, ultrices sit amet enim. Curabitur mattis varius quam eget elementum. Sed vel metus euismod, ornare orci gravida, varius leo. Etiam bibendum quam in aliquet sodales. Vestibulum id dolor lectus. Praesent porta faucibus tempus. Aliquam vel scelerisque enim, nec viverra sem. Curabitur ornare lectus vel libero efficitur lobortis.

Proin mattis, felis nec faucibus convallis, quam velit euismod magna, quis ultrices mi nisi eu magna. Aliquam sem magna, maximus vitae aliquam eget, egestas nec nisi. Cras eleifend suscipit lectus, at posuere ligula sollicitudin luctus. Mauris vitae eros tincidunt, cursus ligula nec, tempus lorem. Mauris vestibulum erat luctus nisi ornare aliquam. Ut ut feugiat libero, nec tristique nunc. Sed porttitor sem at justo semper, sed vehicula eros facilisis. Integer ut urna et augue tincidunt sollicitudin ut sit amet turpis. Donec imperdiet nulla ut purus maximus tempus vitae nec tellus. Fusce leo velit, vulputate et mi quis, accumsan mattis elit. Nulla sed ante et ex tempor bibendum at sit amet nisl."""
    ]
    |> basic