module MaybeBuilderModule

let divideBy bottom top = 
    if bottom = 0
    then None
    else Some(top/bottom)

type MaybeBuilder() = 

    member this.Bind(x, f) =
        Option.bind f x

    member this.Return(x) =
        Some x


let maybe = MaybeBuilder()

let (>>=) m f = Option.bind f m

let divideByThreeTimes init x y z = 
    maybe
        {
        let! a = init |> divideBy x
        let! b = a |> divideBy y
        let! c = b |> divideBy z
        return c
        }

