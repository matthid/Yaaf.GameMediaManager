// ----------------------------------------------------------------------------
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.
// ----------------------------------------------------------------------------
namespace Yaaf.WirePlugin.Primitives

open System
open System.Xml

[<System.Runtime.CompilerServices.Extension>]
module Interop = 
    [<System.Runtime.CompilerServices.Extension>]
    let IsNone o = 
        match o with
        | None -> true
        | _ -> false
    
    [<System.Runtime.CompilerServices.Extension>]
    let IsSome o = 
        not <| IsNone o

type ITask<'T> = 
    abstract member Start : unit -> unit
    abstract member WaitForFinished : unit -> unit
    abstract member Result : 'T option with get
    abstract member ErrorObj : exn option with get
    
    [<CLIEvent>]
    abstract member Error : IEvent<exn>
    [<CLIEvent>]
    abstract member Update : IEvent<string>
    [<CLIEvent>]
    abstract member Finished : IEvent<'T>
    
[<System.Runtime.CompilerServices.Extension>]
module TaskExtensions = 
    type ITask<'T> with
        member x.MapTask mapFun = 
            { new ITask<_> with
                member i.Start () = x.Start()
                member i.WaitForFinished () = x.WaitForFinished()
                member i.Result 
                    with get() = 
                        match x.Result with
                        | None -> None
                        | Some t -> Some (mapFun t)
                member i.ErrorObj 
                    with get() = x.ErrorObj
            
                [<CLIEvent>]
                member i.Error = x.Error

                [<CLIEvent>]
                member i.Update = x.Update
    
                [<CLIEvent>]
                member i.Finished = x.Finished |> Event.map mapFun }
    
    [<System.Runtime.CompilerServices.Extension>]
    let MapTask (task:ITask<'T>) (mapFun:Func<_,_>) = task.MapTask mapFun.Invoke 
        

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
    
    interface ITask<'T> with
        member x.Start () = start() |> Async.Start
        member x.Result with get () = result
        member x.ErrorObj with get () = error
        member x.WaitForFinished() = 
            start() |> Async.RunSynchronously

        [<CLIEvent>]
        member x.Error = errorEvent.Publish

        [<CLIEvent>]
        member x.Update = updateEvent
    
        [<CLIEvent>]
        member x.Finished = finishedEvent.Publish

    member x.Async with get() = a
    static member FromDelegate (func:Func<_>)  = 
        let asyncTask = async { return func.Invoke() }
        Task(asyncTask) :> ITask<_>

