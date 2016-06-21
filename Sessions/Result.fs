[<RequireQualifiedAccess>]
module Result

open Models

let map func input = 
    match input with 
    | Failure failure -> Failure failure
    | Success value -> Success (func value)

let bind func input : Result<'b,'c>= 
    match input with
    | Failure failure -> Failure failure
    | Success value -> func value

let filter func failureValue input = 
    match input with
    | Failure failure -> Failure failure
    | Success value -> if func value then Success value else Failure failureValue