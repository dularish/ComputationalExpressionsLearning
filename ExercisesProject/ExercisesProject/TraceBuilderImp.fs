module TracebuilderImp

type TraceBuilder() =
    member this.Bind(x, f) =
        match x with
        | None -> 
            printfn "Binding with None. Exiting"
        | Some a ->
            printfn "Binding with Some(%A). Continuing " a
        Option.bind f x

    member this.Return(x) =
        printfn "Returning an unwrapped %A as an option " x
        Some x

    member this.ReturnFrom(x) = 
        printfn "Returning an option %A directly" x
        x

    //Zero statement gets executed when a statement without any return type is used
    //General guideline : If it's an OrElseBuilder use None as the return, if it's supposed to be sequential , return ()
    member this.Zero() =
        printfn "Executing zero statement"    
        None // This is optional, Zero implementation need not return anything at all

    //Notice the signatures of Return and Yield are similar and also of ReturnFrom and YieldFrom
    //Some builders implemented by .NET developers have either of the two implementations, refer seq example below
    member this.Yield(x) =
        printfn "Yielding an unwrapped %A as an option" x
        Some x    

    member this.YieldFrom(x) =
        printfn "Yielding a wrapped %A" x
        x            

    //Implementing Combine method by just adding numbers
    //Combine on a delayed implementation has a signature of ('c option * (unit -> 'c option) -> 'c option)
    member this.Combine(a,b) =
        // printfn "combining %A with %A" a b
        // match a,b with
        // | Some a', Some b' ->
        //     Some (a' + b')
        // | Some a', None ->
        //     Some (a')        
        // | None, None ->
        //     None
        // | None, Some b' ->
        //     Some b'       
        printfn "Returning early with %A. Ignoring %A" a b    
        a
    //member this.Delay(f) = f()
    //Delay also has to be implemented for Combine to work, as the two parts to be combined are first passed to delay.
    member this.Delay(funcToDelay) =
        printfn "Delay"        
        let delayed = fun() ->
            let delayedResult = funcToDelay()//Remember that this line is not being executed, this will be executed only when the generated function is called
            delayedResult
        delayed        

    //This is for undoing the delays, the above implementation of delay to convert the seq to function, has to be undone so that it can be used normally
    //Run function is called when there is no more combine is possible, after all the delays
    member this.Run(funcToRun) =
        let runResult = funcToRun()
        runResult

let traceInst = TraceBuilder()

let someexamplesfromtracebuilder = 
    traceInst
        {
            return 1
        } |> printfn "Result 1 : %A"

    traceInst
        {
            return! Some 2
        } |> printfn "Result 2 : %A"

    traceInst
        {
            let! x = Some 1
            let! y = Some 2
            return x + y
        } |> printfn "Result 3 : %A"

    traceInst
        {
            let! x = None
            let! y = Some 1
            return x + y            
        }    |> printfn "Result 4 : %A"

    traceInst
        {
            do! Some (printfn "Expression that returns unit")//Learning: do! calls the Bind method and passes unit to it
            //do! (Some 1) //You cannot run this statement. Looks like do! can be called with unit return expressions
            do! Some (printfn "Second expression that returns unit")
            let! x = Some (1) // Verifyting if this statement is executed
            return x
        }    |> printfn "Result from do : %A"


    traceInst
        {
            printfn "Hello. Just a printfn statement"
        }    |> printfn "Printing a simple printfn statement, Result : %A"

    traceInst
        {
            yield 1            
        } |> printfn "yield statement result : %A"

    traceInst
        {
            yield! Some 1            
        } |> printfn "yieldfrom statement result : %A"

    seq
        {
            yield 1
            yield 2
            yield 3
        }    |> printfn "seq example result with multiple yield statements : %A"// This works fine because seq has Combine method defined

    traceInst
        {
            yield 1
            yield 2
        }    |> printfn "Multiple yield statment results : %A"

    //Just syntactic difference in the below case, otherwise same and also notice how combine method is executed
    traceInst
        {
            yield 1
            yield 2
            return 3
            yield 4
        }    |> printfn "Multiple yield statment with return statement results : %A"

    printfn "The start of Delay, Run demonstration"
    //If delay is implemented correctly, the second printfn statement wouldn't be printed
    //Learning : The ultimate essense of Functional programming is here !!!!!!
    //The logic that helps to understand:
        //The last printfn statement(Part 4) is converted into a function in Delay and output of the expression is None as Zero function is called at the end. Note that the function is not executed , but the expression has an evaluation
        //The two printfn statements (Part 2 & 3) along with return 2 statement is now combined, with the Delayed function of (Part 4), and now , without executing, the Part 4 statement is ignored, so only the (Part 2 & 3 along with return 2) is converted to a function, and it has a returning value of 2
        //The first printfn statement(Part 1) along with return 1 is now combined with the delayed function of (Part 2 & 3 along with return 2), the combine implementation simply rejects the (Part 2 & 3 along with return 2) and Delay implementation converts the (part 1) along with return 1 to a function
        //The function is alone, and no more Combine statement can be executed, hence the function is passed to Run function, and this is where the function code is run, but the function code has only that of Part 1 along with return 1, and this is why the rest of the statements are not executed.
    //But how computer processes actually:
        //Run statement is called, Delaying the functions start
        //The (Part 1) along with return 1 has to be combined with the rest of the statements. Here the rest of the statements are in the form of a delayed function
        //But the combine implementation says to ignore the rest
        //So the rest as a function is ignored
        //We end up with only Part 1 with return 1 which is now delayed and run statement is executed.
    traceInst
        {
            printfn "Part 1 : about to return 1"
            return! 1
            printfn "Part 2 : after return 1 has happened"
            printfn "Part 3 : after part 2 has happened"
            return! 2
            printfn "Part 4 : after return 2"
        } |> printfn "Result for part 1 without part 2 : %A"        