module ListWorkflowImp

type ListWorkFlowBuilder() =
    member this.Bind(x, f) =
        x |> List.collect f

    member this.Return(x) =
        printfn "Returning unwrapped %A as list" x
        [x]    

    member this.Yield(x) =
        printfn "Yielding unwrapped %A as list" x
        [x]
    member this.YieldFrom(x) =
        printfn "Yielding wrapped %A as list" x
        x

    member this.For(x, f) =
        printfn "For %A" x
        this.Bind(x, f)

    member this.Combine(a,b) =
        printfn "Combining %A and %A" a b
        List.concat [a;b] 

    member this.Delay (f) = 
        f()

let listWorkflow = ListWorkFlowBuilder()

let unwrappingList x=
    listWorkflow
        {
            let! unwrapped = x
            return unwrapped   
        }

let addingLists x y =
    listWorkflow
        {
            let! firstListUnwrapped = x
            let! secondListUnwrapped = y
            return firstListUnwrapped + secondListUnwrapped
        }    

let genericFnLists x y fn =
    listWorkflow
        {
            let! firstListUnwrapped = x
            let! secondListUnwrapped = y
            return (fn firstListUnwrapped secondListUnwrapped)                        
        }

let exampleforlistworkflow =
    listWorkflow
        {
            for x in [1..3] do
            for y in [10;20;30] do
            return x + y
        }   
    |> printfn "Result of using for loops : %A"

    listWorkflow
        {
            yield 1
            yield 2
            yield 3
        } |> printfn "result of yielding multiple items : %A"

    listWorkflow
        {
            for i in ["red";"blue"] do
                yield i
                for j in ["hat";"tie"] do
                    yield! [i + " " + j;"-"]            
        }    |> printfn " Result of nested for : %A"

    listWorkflow
        {
            for i in [1..5] do yield i + 2
            yield 42
        }    |> printfn "Result : %A"