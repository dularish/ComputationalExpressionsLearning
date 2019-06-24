module IdentifyWorkflowImp
//here in this case of builder both the wrapper type and the wrapped type are same, and it's fine
type IdentityBuilder() =
    member this.Bind (x, f) =
        f x

    member this.Return (x) = x

    member this.ReturnFrom (x) = x

let identityWorkflowInst = IdentityBuilder()

let exampleidentityWorkflow =
    identityWorkflowInst
        {
            let! x = 1
            let! y = 2
            return x + y
        }