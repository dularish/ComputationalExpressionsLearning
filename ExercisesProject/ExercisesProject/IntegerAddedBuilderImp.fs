module IntegerAdderBuilderModule

type IntegerAddedBuilder() =
    member this.Bind(m, f)=
        Option.bind f m

    member this.Return(x) =
        Some x

    member this.ReturnFrom(m) =
        m

    //Learning : Notice how the second parameter is a function in a delayed implementation
    //Also see what if the second parameter is required in the form of wrapper type, it is being executed
    member this.Combine(m,y) =
        let n = y()

        match m, n with
        | Some u, Some v ->
            Some (u + v)
        | Some u, None ->
            Some (u)        
        | None , Some v ->
            Some (v)        
        | None, None ->
            None

    //The actual signature is (unit -> int option) -> (unit -> int option)
    member this.Delay(funcToDelay) =
        funcToDelay

    member this.Run(funcToRun) =
        funcToRun()


let integerAdderInst = IntegerAddedBuilder()

let exampleForIntegerAdderBuilder = 
    integerAdderInst
        {
            return 1
            return 2
        } |> printfn "IntegerAdderBuilder example 1 result : %A"