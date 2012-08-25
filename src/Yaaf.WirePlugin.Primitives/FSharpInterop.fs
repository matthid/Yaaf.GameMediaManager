namespace Yaaf.WirePlugin.Primitives

open System
open System.Xml

type Task<'T>(a:Async<'T>) =
    let a = a
    let errorEvent = new Event<exn>()
    let finishedEvent = new Event<'T>()

    let start () =
        async {
            try
                let! item = a
                finishedEvent.Trigger item
            with exn ->
                errorEvent.Trigger exn
        }
        
    member x.Async with get() = a
    member x.Start () = start() |> Async.Start

    member x.WaitForFinished() = 
        start() |> Async.RunSynchronously

    [<CLIEvent>]
    member x.Error = errorEvent.Publish
    
    [<CLIEvent>]
    member x.Finished = finishedEvent.Publish

type FSharpInterop() = 

    static member Test() = 
        ""