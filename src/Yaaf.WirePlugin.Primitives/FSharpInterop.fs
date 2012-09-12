// ----------------------------------------------------------------------------
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.
// ----------------------------------------------------------------------------
namespace Yaaf.WirePlugin.Primitives

open System
open System.Xml

module Interop = 
    let isNone o = 
        match o with
        | None -> true
        | _ -> false
    let isSome o = 
        not <| isNone o

type Task<'T>(a:Async<'T>, update:IEvent<string>) =
    let a = a
    let updateEvent = update
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
        
    new a = Task (a, (new Event<string>()).Publish)
    member x.Async with get() = a
    member x.Start () = start() |> Async.Start
    member x.Result = result
    member x.ErrorObj = error
    member x.WaitForFinished() = 
        start() |> Async.RunSynchronously
        
    [<CLIEvent>]
    member x.Error = errorEvent.Publish

    [<CLIEvent>]
    member x.Update = updateEvent
    
    [<CLIEvent>]
    member x.Finished = finishedEvent.Publish

type FSharpInterop() = 

    static member Test() = 
        ""