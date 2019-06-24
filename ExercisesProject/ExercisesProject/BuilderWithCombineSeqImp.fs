module BuilderWithCombineSeqImp

type CombineSeqBuilder() =
    member this.Bind (x,f) =
        Option.bind f x

    member this.Return(x)=
        printfn "Returning an unwrapped %A" x
        Some x

    member this.Zero() =
        printfn "Zero"
        this.Return()    
    member this.Combine(a,b) =
        this.Bind(a, fun () -> b)

    member this.Delay(f)= 
        f()


let combineSeqbuilderInst = CombineSeqBuilder()

let combineSeqBuilderExample =
    //The below test doesn't work if Zero() returns None
    combineSeqbuilderInst
        {
            if true then printfn "hello....."
            if false then printfn "....world"
            return 1
        } |> printfn "result of combineSeqbuilderExample1 : %A"