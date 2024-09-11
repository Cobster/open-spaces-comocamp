module OpenSpaces.Views 

open System 
open Giraffe.ViewEngine

let layout pageTitle pageContent = 
    html [] [
        head [] [
            meta [ _charset "utf-8" ]
            meta [ _name "viewport"; _content "width=device-width, initial-scale=1.0" ]
            title [] [ str pageTitle ]
        ]
        body [] pageContent
    ]

let createSpace (model : {| SpaceName: string; Id: Guid |}) = layout "Create Space" [
    h1 [] [ str "Please name the open space" ]
    form [ _action "/create_space"; _method "POST" ] [
        label [ _for "spaceName" ] [ str "Space Name:" ]
        input [ _type "text"; _id "spaceName"; _name "spaceName"; _required; _value model.SpaceName ]
        input [ _type "hidden"; _id "id"; _name "id"; _value (model.Id.ToString()) ]
        button [ _type "submit" ] [ str "Set Name" ]
    ]
]

let spaceCreatedConfirmation (model :Result<string,string>) = 
    match model with 
    | Ok spaceName ->
        [
            h1 [] [str "Space Created Successfully!"]
            p [] [ str $"The space named {spaceName} has been successfully created." ]
        ]
    | Error message ->
        [
            h1 [] [str "Space Created Failed"]
            p [_class "error" ] [ str $"Error: {message}" ]
        ]
    |> layout "Space Creation Status"