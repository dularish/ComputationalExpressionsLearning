module ExercisesProject

open MaybeBuilderModule
open StrToIntWorkflowImp
open ListWorkflowImp
open IdentifyWorkflowImp
open TracebuilderImp
open BuilderWithCombineSeqImp
open MaybeBuilderWithLazinessModule
open IntegerAdderBuilderModule
open BuilderWithAdditionalMethodsModule

[<EntryPoint>]
let main argv =
    printfn "List workflow example"
    printfn "%A" (exampleforlistworkflow |> ignore)
    printfn "Hello Wordld %A" argv
    printfn "%A" (divideByThreeTimes 10 2 2 2)
    printfn "Printing StringToIntWorkflow results"

    printfn "%A" (stringAddWorkflow "a" "12" "12")
    printfn "Additional exercises for bind"
    let good = (stringAddWorkflowNeglectingErrors "1" "2" "4")
    let bad = (stringAddWorkflowNeglectingErrors "xy" "3" "3")
    printfn "Good output1 : %A" good
    printfn "Bad output 1: %A" bad
    printfn "Something unexpected : %A" (Some(None))
    printfn "Unwrapping Test "
    printfn "%A" (stringAddWorkflowJustUnwrappingTest 10)

    printfn "Unwrapping List"
    printfn "%A" (unwrappingList [10;20;30])

    printfn "Adding lists fun test"
    printfn "%A" (addingLists [1;2;3] [10;20;30])

    printfn "Generic lists function - multiplication"
    printfn "%A" (genericFnLists [1;2;3] [10;20;30] (( * )))

    printfn "Identity Workflow example"
    printfn "%A" (exampleidentityWorkflow)

    printfn "Trace builder example"
    printfn "%A" (someexamplesfromtracebuilder |> ignore)

    printfn "OrElse examples"
    printfn "%A" (OrElseBuilderModule.orElseExamples)

    printfn "%A" (combineSeqBuilderExample)

    printfn "%A" (examplesForMaybeBuilderWithLaziness)

    printfn "%A" (exampleForIntegerAdderBuilder)

    printfn "%A" (exampleWithAdvMethods)

    System.Console.ReadKey() |> ignore
    0 // return an integer exit code
