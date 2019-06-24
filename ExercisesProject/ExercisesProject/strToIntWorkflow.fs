module StrToIntWorkflowImp
open System

let strToInt (str:string) = 
    match (Int32.TryParse (str)) with
    | true,num -> Some(num)
    | false, _ -> None

type StrToIntWorkflow() =
    //Bind typically accepts a wrapped value and returns a wrapped value
    member this.Bind (x, f) =
        Option.bind f x
    //Return typically accepts an unwrapped value and returns a wrapped value
    member this.Return(x) =
        Some(x)    

    //Return typically accepts a wrapped value and returns a wrapped value
    member this.ReturnFrom(x) =
        x    

let strToIntInstance = StrToIntWorkflow()

let stringAddWorkflow x y z =
    strToIntInstance
        {
            let! a = strToInt x//Learning : Right side expression is of Wrapper<'T> but left side expression is of 'T
            let! b = strToInt y
            let! c = strToInt z
            return a + b + c//Learning : return converts an expression of 'T to Wrapper<'T>
        }

//Additional bind functions
// let strAdd str i =
//     match (Int32.TryParse (str)) with
//     | true, num -> 
//         match i with 
//         | Some integer -> Some(num + integer)
//         | None -> Some num
//     | false, _ -> i

let strAdd str i =
    match (Int32.TryParse (str)) with
    | true, num -> Some(num + i)
    | false, _ -> Some(i)

let (>>=) m f = 
    Option.bind f m

let stringAddWorkflowNeglectingErrors x y z =
    strToIntInstance
        {
            let! a = strToInt x
            let! b = strAdd y a
            return! strAdd z b//Learning: Notice that expression which returns Wrapper<'T> can be returned directly using return!
        }

let stringAddWorkflowJustUnwrappingTest x =
    strToIntInstance
        {
            let! a = Some x//Learning: this is quite amazing, how wrapped is directly converted without specifying a function
            return a
        }