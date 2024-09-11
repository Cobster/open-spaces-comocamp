module OpenSpaces.EventStore

module InMemory =

    let run (decider :Decider<'s,'c,'e,'x>) =

        let mutable store :Map<string,'e list> = Map.empty

        let readEvents (stream :string) :'e list = 
            store 
            |> Map.tryFind stream 
            |> Option.defaultValue []

        let appendEvents (stream :string) (events :'e list) :Result<'e list,'x> =  
            store <-
                store 
                |> Map.change stream (function 
                                    | Some e -> Some (e @ events)
                                    | None -> Some events) 
            Ok events

        let loadState = readEvents >> decider.fold 

        fun stream command ->
            stream 
            |> loadState 
            |> decider.decide command
            |> Result.bind (fun events -> appendEvents stream events)

module FileSystem =

    open System.IO
    open System.Text

    let run (path :string) (decider :Decider<'s,'c,'e,'x>) (serdes :'e -> string * string -> string -> 'e) =
        let serialize, deserialize = serdes

        let readEvents () = 
            Directory.GetFiles(path)
            |> Array.filter (fun file -> file.EndsWith(".json"))
            |> Array.map (fun file -> File.ReadAllText(file, Encoding.UTF8))
    
