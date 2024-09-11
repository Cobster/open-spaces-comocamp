module OpenSpaces.Serialization

open Thoth.Json.Net 

let createSerializer (encoder :Encoder<'t>) = encoder >> Encode.toString 0
let createDeserializer (decoder :string -> Decoder<'t>) = 
    let deserialize = decoder >> Decode.fromString
    fun kind json ->
        match deserialize kind json with 
        | Ok e -> e 
        | Error reason -> failwith reason 