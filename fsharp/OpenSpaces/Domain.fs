module OpenSpaces.Domain

open System 

type SessionType = | Suggestion

type Event =
    | OpenSpaceNamed of OpenSpaceNamedEvent 
    | DateRangeSet of DateRangeSetEvent
    | TopicSubmitted of TopicSubmittedEvent
    | TimeSlotAdded of TimeSlotAddedEvent
    | RequestedConfId of RequestedConfIdEvent

and OpenSpaceNamedEvent = 
    {
        Id: Guid
        Timestamp: DateTimeOffset
        SpaceName: string
    }

and DateRangeSetEvent = 
    {
        Id: Guid
        Timestamp: DateTimeOffset 
        StartDate: DateOnly
        EndDate: DateOnly
    }

and TimeSlotAddedEvent =
    {
        Id: Guid
        Timestamp: DateTimeOffset
        Name: string 
        StartTime: TimeOnly 
        EndTime: TimeOnly
    }
   
and TopicSubmittedEvent =
    {
        Id: Guid
        Timestamp: DateTimeOffset
        Topic: string 
        SessionType: SessionType 
    }

and RequestedConfIdEvent =
    {
        Id: Guid 
        Timestamp: DateTimeOffset
    }

module OpenSpaceNamed =
    open Thoth.Json.Net 
    let decoder :Decoder<Event> =
        Decode.object (fun get -> 
            OpenSpaceNamed
                {
                    Id = get.Required.Field "id" Decode.guid 
                    Timestamp = get.Required.Field "timestamp" Decode.datetimeOffset
                    SpaceName = get.Required.Field "spaceName" Decode.string 
                }
        )
    let encoder (data :OpenSpaceNamedEvent) =
        Encode.object
            [
                "type", "OpenSpaceNamedEvent" |> Encode.string 
                "id", data.Id |> Encode.guid 
                "timestamp", data.Timestamp |> Encode.datetimeOffset
            ]

module RequestedConfId =
    open Thoth.Json.Net 
    let encoder (data :RequestedConfIdEvent) =
        Encode.object 
            [
                "type", "RequestedConfIdEvent" |> Encode.string 
                "id", data.Id |> Encode.guid 
                "timestamp", data.Timestamp |> Encode.datetimeOffset
            ]

[<AutoOpen>]
module Event =
    open Thoth.Json.Net
    let decoder (eventType :string) :Decoder<Event> =
        match eventType with 
        | "OpenSpaceNamedEvent" -> OpenSpaceNamed.decoder
        | invalid -> Decode.fail $"Failed to deserialize `{invalid}` it is an invalid event type" 
    let encoder (event :Event) =
        match event with 
        | OpenSpaceNamed data -> OpenSpaceNamed.encoder data 
        | _ -> NotImplementedException() |> raise 
        

type Command = 
    | NameOpenSpace of NameOpenSpaceCommand
    | AddTimeSlot of AddTimeSlotCommand 
    | RequestConfId of RequestConfIdCommand

and NameOpenSpaceCommand =
    {
        Id: Guid
        Timestamp: DateTimeOffset
        SpaceName: string 
    }

and AddTimeSlotCommand = 
    {
        Id: Guid
        Timestamp: DateTimeOffset
        Name: string 
        StartTime: TimeOnly
        EndTime: TimeOnly
    }

and RequestConfIdCommand =
    {
        Id: Guid
        Timestamp: DateTimeOffset
    }

type Error =
    | SpaceNameRequired
    | SpaceNameAlreadyExists

type State = 
    {
        SpaceName: string option
    }

let private initialState = 
    {
        SpaceName = None 
    }

let private evolve (state :State) (event :Event) =
    match event with 
    | OpenSpaceNamed e -> { state with SpaceName = Some e.SpaceName }
    | _ -> state 


let private handleNameOpenSpace (command :NameOpenSpaceCommand) (state :State) =
    if String.IsNullOrWhiteSpace(command.SpaceName) then 
        Error SpaceNameRequired
    elif state.SpaceName |> Option.contains (command.SpaceName.Trim()) then 
        Error SpaceNameAlreadyExists
    else 
        Ok [ OpenSpaceNamed { Id = command.Id; Timestamp = command.Timestamp; SpaceName = command.SpaceName }]


let private decide (command :Command) (state :State) =
    match command with 
    | NameOpenSpace c -> handleNameOpenSpace c state 
    | _ -> Ok []

let decider =
    {
        initialState = initialState
        decide = decide
        evolve = evolve 
    }
