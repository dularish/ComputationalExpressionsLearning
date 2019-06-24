module MaybeBuilderWithLazinessModule

//type Maybe<'a> = Maybe of (unit -> 'a option)//With same signature as a delay function
type Maybe<'a> = Maybe of Lazy<'a option> //Changed from above to introduce True laziness such that a function would be executed only once


type MaybeBuilderWithLaziness() =
    member this.Bind (x, f) =
        Option.bind f x

     member this.Return(x) =
        Some x

    member this.ReturnFrom(Maybe x) =
        //x()
        x.Force()//This is because, after introducing true laziness, Maybe no more wraps a function

    member this.Zero() =
        None
    //Combine on a delayed implementation has a signature of ('c option * (unit -> 'c option) -> 'c option)
    member this.Combine(a,b) =
        match a with
        | Some _ -> a
        | None -> b()//The second parameter of a combine in a delayed implementation is a delayed function

    member this.Delay(funcToDelay) =
        let delayed = fun () ->
            let delayedResult = funcToDelay()//Remember that this line is not being executed, this will be executed only when the generated function is called
            delayedResult
        delayed        

    member this.Run(f) =
        //f()
        Maybe (lazy f())

    


let maybeWithLazinessInst = MaybeBuilderWithLaziness()

let run (Maybe f) = f.Force()

let examplesForMaybeBuilderWithLaziness =
    maybeWithLazinessInst
        {
            printfn "Part 1 : before return statement"
            return 1
            printfn "Part 2 : after return statement"
        } |> run |> printfn "Maybe with laziness result of example 1 : %A"

    maybeWithLazinessInst
        {
            printfn "Part 1 : about to return None"
            return! Maybe (lazy ((fun () -> None)()))//Notice this statement how on changing the ReturnFrom signature, we are not able to naturally return something like "return! None"
            printfn "Part 2 : after none returning"
        }  |> run  |> printfn "Maybe with laziness result of example 2 : %A"

    let childWorkflow =
        maybeWithLazinessInst
            {
                printfn "Child workflow"
            }

    //This example doesn't run the child workflow after introducing the Maybe type wrapper for computation expression builder delay
    maybeWithLazinessInst
        {
            printfn "Part 1 : about to enter 1 and then return childworkflow"
            return 1
            return! childWorkflow
        }    |> run |> printfn "Result with child workflow : %A"    

    //Child workflow twice, but should be executed only once
    //Learning: This is why functional programming concepts say that a function shouldn't cause any side effect, the effect of a function must be such that if it's executed once or a million times, it must be same
    maybeWithLazinessInst
        {
            return! maybeWithLazinessInst {printfn "About to call child workflow twice"}
            return! childWorkflow
            return! childWorkflow
        }    |> run |> printfn "Result with child workflow twice : %A"

