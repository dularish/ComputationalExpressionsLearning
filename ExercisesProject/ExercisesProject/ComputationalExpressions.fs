module SomeModule

printfn "Hello world"

type Person = {First:string; Last:string}
let alice = {First="Alice"; Last="Doe"}
let {First=first} = alice //Notice how to assign a prop of object to another value
printfn "%s" first

