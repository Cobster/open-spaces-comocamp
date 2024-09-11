module OpenSpaces.Tests

open System
open Xunit
open OpenSpaces.Domain

let testEvents =
    [
        OpenSpaceNamed 
            { 
                SpaceName = "EM Open spaces"
                Id = Guid.Parse("fceee960-2f9f-47b0-ad19-fed15d4f82cb") 
                Timestamp = DateTimeOffset.Parse("2024-05-21T00:00:00.000Z") 
            } 

        OpenSpaceNamed
            { 
                SpaceName = "Event Modeling Space"
                Id = Guid.Parse("2ceee960-2f9f-47b0-ad19-fed15d4f82cb") 
                Timestamp = DateTimeOffset.Parse("2024-05-22T00:00:00.000Z") 
            }

        OpenSpaceNamed
            { 
                SpaceName = "Event Modeling Open Spaces"
                Id = Guid.Parse("3ceee960-2f9f-47b0-ad19-fed15d4f82cb") 
                Timestamp = DateTimeOffset.Parse("2024-05-23T00:00:00.000Z") 
            }
    ]

let shouldEqual (expected :'a) (actual :'a) :unit = Assert.Equal<'a>(expected,actual)

[<Fact>]
let ``NameOpenSpace can't have a blank name`` () =
    decider.fold testEvents 
    |> decider.decide (NameOpenSpace { SpaceName = "" ; Id = Guid.NewGuid(); Timestamp = DateTimeOffset.UtcNow })
    |> shouldEqual (Error SpaceNameRequired)

[<Fact>]
let ``NameOpenSpace should be valid with no prior events`` () =
    let command :NameOpenSpaceCommand = { SpaceName = "EM Open spaces"; Id = Guid.NewGuid(); Timestamp = DateTimeOffset.Now }
    let expected :OpenSpaceNamedEvent = { SpaceName = command.SpaceName; Id = command.Id; Timestamp = command.Timestamp }

    decider.fold [] 
    |> decider.decide (NameOpenSpace command) 
    |> shouldEqual (Ok [OpenSpaceNamed expected])

[<Fact>]
let ``NameOpenSpace should not match the last name set event even if extra whitespace is present`` () =
    decider.fold testEvents
    |> decider.decide (NameOpenSpace { SpaceName = "Event Modeling Open Spaces "; Id = Guid.NewGuid(); Timestamp = DateTimeOffset.Now })
    |> shouldEqual (Error SpaceNameAlreadyExists)