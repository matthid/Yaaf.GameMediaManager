
namespace Yaaf

module Logging = 
    open System
    open System.IO
    open System.Diagnostics
    [<AutoOpen>]
    module LoggingInterfaces =
        type ITracer = 
            inherit IDisposable
            abstract member TraceSource : TraceSource
            abstract member ActivityId : Guid

    type ITracer with 
        member x.doInId f = 
            let oldId = Trace.CorrelationManager.ActivityId
            try
                Trace.CorrelationManager.ActivityId <- x.ActivityId
                f()
            finally
                Trace.CorrelationManager.ActivityId <- oldId
        member x.logHelper ty (s : string) =  
            x.doInId 
                (fun () ->
                    x.TraceSource.TraceEvent(ty, 0, s)
                    x.TraceSource.Flush())
        member x.log ty fmt = Printf.kprintf (x.logHelper ty) fmt  
        member x.logVerb fmt = x.log System.Diagnostics.TraceEventType.Verbose fmt
        member x.logWarn fmt = x.log System.Diagnostics.TraceEventType.Warning fmt
        member x.logCrit fmt = x.log System.Diagnostics.TraceEventType.Critical fmt
        member x.logErr fmt =  x.log System.Diagnostics.TraceEventType.Error fmt
        member x.logInfo fmt = x.log System.Diagnostics.TraceEventType.Information fmt

    type DefaultStateTracer(traceSource:TraceSource, activityName:string) as x = 
        let trace = traceSource
        let activity = Guid.NewGuid()
        do 
            x.doInId (fun () -> trace.TraceEvent(TraceEventType.Start, 0, activityName))
        
        interface IDisposable with
            member x.Dispose() = 
                x.doInId (fun () -> trace.TraceEvent(TraceEventType.Stop, 0, activityName))
        
        interface ITracer with 
            member x.TraceSource = trace
            member x.ActivityId = activity

    let curry f a b = f (a,b)
    let uncurry f (a,b) = f a b
    let logPath = 
        let path = [
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            "Yaaf"; "WirePlugin" ; "log" ]
                |> List.fold (curry Path.Combine) ""
        if not <| Directory.Exists path then
            Directory.CreateDirectory path |> ignore
        path

    let logFile name = 
        Path.Combine(logPath, name)

    let Source entryName traceEntry  = 
        let source = new TraceSource(entryName)
        let traceEntry = if String.IsNullOrEmpty(traceEntry) then "" else "." + traceEntry
        let listener = new XmlWriterTraceListener(logFile (entryName + traceEntry + ".svclog"))
        listener.Filter <- new EventTypeFilter(SourceLevels.All)
        source.Listeners.Add(listener) |> ignore
        source.Switch.Level <- SourceLevels.All
        source
    
    let DefaultTracer traceSource id = 
        new DefaultStateTracer(traceSource, id) :> ITracer
        
    type ITracer with 
        member x.childTracer newActivity = 
            let baseTracer = x.TraceSource
            let tracer = new DefaultStateTracer(baseTracer, newActivity) :> ITracer
            x.doInId 
                (fun () -> 
                    x.TraceSource.TraceTransfer(0, "Switching to " + newActivity, tracer.ActivityId))
            tracer