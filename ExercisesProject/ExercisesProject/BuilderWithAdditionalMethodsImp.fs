module BuilderWithAdditionalMethodsModule
open System.Collections.Generic
open System.Collections
open System

type BuilderWithAdvMethods() =
    member this.Bind(m, f) =
        Option.bind f m

    member this.Delay funcToDelay =
        funcToDelay

    member this.Return(x) =
        Some x    

    member this.ReturnFrom(m) =
        m

    member this.Zero() =
        Some () //While doesn't work if set to None , because passing None to Bind will cause the worflow to abort early, (Why doesn't the same happens in the other implementations?? - Find out why)

    member this.Run(funcToRun) =
        funcToRun()    

    //Notice the signature, body returns unit option
    member this.While(guard, body) =
        if not (guard())
        then
            this.Zero()
        else
            this.Bind ( body(), fun () -> 
                this.While(guard, body))//rec keyword is not necessary here because it's needed only for standalone functions, not methods

    //Notice the signature
    member this.TryWith(body, errorHandler) =
        try
            this.ReturnFrom(body())//Using returnFrom, as the body should return a wrapped value, and that should be treated as a wrapped value by the builder
        with
            e ->
                errorHandler e        

    member this.TryFinally(body, finallyBlock) =
        try
            this.ReturnFrom(body())
        finally
            finallyBlock()        

    //Why hash is used is that, by doing so, we are not strictly accepting IDisposable and setting a limitation that body must accept an argument of first parameter(disposable) type
        //Otherwise, we will have to pass a body which accepts a IDisposable as input
    member this.Using(disposable:#IDisposable, body) =
        let body' = fun () -> body disposable
        this.TryFinally(body', fun() ->
            match disposable with
            | null -> ()
            | disp -> disp.Dispose())


    //For statement improved
    member this.For(sequence:seq<'a>, body) =
        this.Using(sequence.GetEnumerator(), fun enumerator ->
            this.While((enumerator).MoveNext, 
                this.Delay(fun() -> body enumerator.Current)))       


let builderWithAdvMethodsInst = BuilderWithAdvMethods()

let exampleWithAdvMethods = 

    builderWithAdvMethodsInst
        {
            return 1
        } |> printfn "Result of first example from builder with additional methods module : %A"

    //Trying while loop
    let mutable i = 1
    let test() = i < 5
    let inc() = i <- i + 1
    builderWithAdvMethodsInst
        {
            while test() do
                printfn "i is %i" i
                inc()
        } |> printfn "Result of first example from builder with additional methods module : %A"

    //Testing TryWith

    builderWithAdvMethodsInst
        {
            try 
                failwith "some error thrown"
                printfn "Some random statement that shouldn't ideally be displayed"
            with
            | e -> printfn "The exception message caught is : %A" e.Message                        
        }    |> printfn "Result of TryWith exception catch is : %A"

    //Example of TryFinally
    //But so far couldn't use try..with..finally ( Is it a limitation??)
    builderWithAdvMethodsInst
        {
            try
                printfn "some statement printed" //Couldn't throw any exception here, caused the program to crash, explore why
            finally
                printfn " finally block executed finally"
                       
        }    |> printfn "Result of TryFinally example is : %A"

    //Example for using
    let makeResource name =
        Some {
            new System.IDisposable with
            member this.Dispose() = printfn "Disposing %s" name
        }    

    builderWithAdvMethodsInst
        {
            use! x = makeResource "Resource - 1"
            printfn "After using statement and before returning"
            printfn "Some more printfn statements"
            printfn "Some more printfn statements"
            printfn "Some more printfn statements"
            return 1
        }    |> printfn "Result of using example is : %A"

    //For loop testing
    builderWithAdvMethodsInst
        {
            for i in [1..5] do
                printfn "Printing %i" i
        } |> printfn "Result of for loop : %A"