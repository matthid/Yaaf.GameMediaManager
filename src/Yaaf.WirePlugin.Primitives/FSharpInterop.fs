// ----------------------------------------------------------------------------
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.
// ----------------------------------------------------------------------------
namespace Yaaf.WirePlugin.Primitives

open System
open System.Xml

type Task<'T>(a:Async<'T>) =
    let a = a
    let errorEvent = new Event<exn>()
    let finishedEvent = new Event<'T>()
    let mutable result = None
    let mutable error = None
    let start () =
        async {
            try
                let! item = a
                result <- Some item
                finishedEvent.Trigger item
            with exn ->
                error <- Some exn
                errorEvent.Trigger exn
        }
        
    member x.Async with get() = a
    member x.Start () = start() |> Async.Start
    member x.Result = result
    member x.ErrorObj = error
    member x.WaitForFinished() = 
        start() |> Async.RunSynchronously

    [<CLIEvent>]
    member x.Error = errorEvent.Publish
    
    [<CLIEvent>]
    member x.Finished = finishedEvent.Publish

type FSharpInterop() = 

    static member Test() = 
        ""