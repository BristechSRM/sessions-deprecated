module Patch

open System
open System.Net
open Models

let isNullOrWhiteSpaceOption str = 
    if String.IsNullOrWhiteSpace str then 
        None 
    else 
        Some str

let parseOp op = 
    op
    |> isNullOrWhiteSpaceOption
    |> Option.map (fun str -> str.ToLower())

let parsePath path = 
    let p = 
        path
        |> isNullOrWhiteSpaceOption
        |> Option.filter (fun str -> str.StartsWith "/")
        |> Option.map (fun str -> (str.ToLower().Split [|'/'|]) |> Array.skip 1 )
    match p with
    | Some path -> Success path
    | None -> Failure <| sprintf "'path' field: %s could not be correctly parsed" path

let validValue value = 
    if String.IsNullOrWhiteSpace value then 
        Failure <| sprintf "'value' field: %s was null, whitespace or missing" value
    else 
        Success value

let createFailMessage fieldMessage (rawOp: RawPatchOperation) = 
    sprintf "%s. Operation was: %A" fieldMessage rawOp

let parseOperation (rawOp: RawPatchOperation) =
    match parseOp rawOp.Op with 
    | Some "replace" -> 
        match parsePath rawOp.Path with
        | Success path -> 
            match validValue rawOp.Value with
            | Success value -> Success <| Replace(path, value)
            | Failure message -> Failure message
        | Failure message -> Failure message
    | _ -> Failure <| sprintf "'op' field: %s did not match an accepted patch operation" rawOp.Op

let parseOperations (rawOps: RawPatchOperation list option) = 
    match rawOps with 
    | Some ops -> 
        let parsedOps = ops |> List.map (parseOperation) 
        let failedOps = parsedOps |> List.choose(function | Failure message -> Some message | _ -> None)
        match failedOps with
        | [] ->  parsedOps |> List.choose(function | Success op -> Some op | _ -> None) |> Success
        | failedOps -> 
            let messages = "Parsing of input operations list failed with the following errors:"::failedOps
            Failure { HttpStatus = HttpStatusCode.BadRequest; Message = messages |> String.concat "\n"}
    | None -> Failure { HttpStatus = HttpStatusCode.BadRequest; Message = "Input operations list was null "}

