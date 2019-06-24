module OrElseBuilderModule

type OrElseBuilder() =
    
    member this.ReturnFrom(x) = x

    member this.Combine(a,b) =
        match a with
        | Some _ -> a
        | None -> b

    member this.Delay(f) = f()    

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