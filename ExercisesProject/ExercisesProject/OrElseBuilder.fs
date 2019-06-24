module OrElseBuilderModule

type OrElseBuilder() =
    member this.Return(x) =
        Some x
    member this.ReturnFrom(x) = x

    member this.Combine(a,b) =
        match a with
        | Some _ -> a
        | None -> b

    member this.Delay(f) = f()    

    member this.Zero() = this.Return()

let map1 = [("1","One"); ("2","Two")] |> Map.ofList
let map2 = [("A", "Apple"); ("B","Ball")] |> Map.ofList
let map3 = [("CA","California"); ("NY", "New York")] |> Map.ofList

let orElse = OrElseBuilder()

let multiLookup key = 
    orElse
        {
            return! map1.TryFind key
            return! map2.TryFind key
            return! map3.TryFind key
        }


printfn "%A" (multiLookup "CA")

type IntOrBool = I of int | B of bool

let parseInt s =
    match System.Int32.TryParse(s) with
    | true, res -> Some (I res)
    | false, _ -> None

let parseBool s =
    match System.Boolean.TryParse(s) with
    | true, res -> Some (B res)
    | false, _ -> None

let orElseExamples = 
    orElse
        {
            return! parseBool "42"
            return! parseInt "42"
            return! parseInt "25"
        } |> printfn "Result of parsing Int or bool : %A"