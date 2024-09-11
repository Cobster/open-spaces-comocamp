namespace OpenSpaces

type Decider<'s,'c,'e, 'x> =
    {
        decide: 'c -> 's -> Result<'e list,'x>
        evolve: 's -> 'e -> 's
        initialState: 's 
    }
    member inline this.fold (events :#seq<'e>) = Seq.fold this.evolve this.initialState events
