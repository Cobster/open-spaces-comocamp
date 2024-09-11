module OpenSpaces.Routes

open System
open Microsoft.AspNetCore.Http
open Giraffe
open Giraffe.EndpointRouting

let getRoot = redirectTo true "/create_space"

let getCreateSpace = htmlView (Views.createSpace {| SpaceName = ""; Id = Guid.NewGuid() |})

let postCreateSpace = 
    fun (next :HttpFunc) (ctx :HttpContext) ->
        // todo: handle the command
        redirectTo true "/space_created_confirmation" next ctx 

let getSpaceCreatedConfirmation =
    fun (next :HttpFunc) (ctx :HttpContext) ->
        Views.spaceCreatedConfirmation (Error "test failure") 
        |> htmlView <|| (next,ctx)

let endpoints = 
    [
        GET [ 
            route "/" getRoot
            route "/create_space" getCreateSpace
            route "/space_created_confirmation" getSpaceCreatedConfirmation 
        ]
        POST [
            route "/create_space" postCreateSpace
        ] 
    ]