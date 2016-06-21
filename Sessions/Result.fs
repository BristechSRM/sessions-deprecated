[<RequireQualifiedAccess>]
module Result

open Models

let map mapFunc input = 
    match input with 
    | Failure failure -> Failure failure
    | Success value -> Success (mapFunc value)

let filter filterFunc failureValue input = 
    match input with
    | Failure failure -> Failure failure
    | Success value -> if filterFunc value then Success value else Failure failureValue