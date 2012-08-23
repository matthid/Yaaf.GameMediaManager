// ----------------------------------------------------------------------------
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.
// ----------------------------------------------------------------------------
namespace Yaaf

/// Provides some logging capabilities
module Logging = 
    open System
    open System.IO
    open System.Diagnostics

    [<AutoOpen>]
    module LoggingInterfaces =
        /// Tracer Interface for easy code tracing
        type ITracer = 
            inherit IDisposable
            /// The tracesource to log into
            abstract member TraceSource : TraceSource
            /// The activity id of the current tracer
            abstract member ActivityId : Guid

    type ITracer with 
        /// executes the given function on the Activity-Id of the current tracer (Sets the CorrelationManager)
        member x.doInId f = 
            let oldId = Trace.CorrelationManager.ActivityId
            try
                Trace.CorrelationManager.ActivityId <- x.ActivityId
                f()
            finally
                Trace.CorrelationManager.ActivityId <- oldId
        /// Helper function for advanced printing
        member x.logHelper ty (s : string) =  
            x.doInId 
                (fun () ->
                    x.TraceSource.TraceEvent(ty, 0, s)
                    x.TraceSource.Flush())
        /// Generic logging function
        member x.log ty fmt = Printf.kprintf (x.logHelper ty) fmt  
        member x.logVerb fmt = x.log System.Diagnostics.TraceEventType.Verbose fmt
        member x.logWarn fmt = x.log System.Diagnostics.TraceEventType.Warning fmt
        member x.logCrit fmt = x.log System.Diagnostics.TraceEventType.Critical fmt
        member x.logErr fmt =  x.log System.Diagnostics.TraceEventType.Error fmt
        member x.logInfo fmt = x.log System.Diagnostics.TraceEventType.Information fmt

    let CSharpInteropLog (tracer:ITracer) ty s = tracer.log ty "%s" s
    /// Simple state tracer
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

    /// The Logging Path
    let logPath = 
        let path = [
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            "Yaaf"; "WirePlugin" ; "log" ]
                |> pathCombine
        if not <| Directory.Exists path then
            Directory.CreateDirectory path |> ignore
        path
    /// Creates a logfile-path with the given name
    let logFile name = 
        Path.Combine(logPath, name)
    /// Creates A Tracesource with the given name and entry
    let Source entryName traceEntry  = 
        let source = new TraceSource(entryName)
        let traceEntry = if String.IsNullOrEmpty(traceEntry) then "" else "." + traceEntry
        let listener = new XmlWriterTraceListener(logFile (entryName + traceEntry + ".svclog"))
        listener.Filter <- new EventTypeFilter(SourceLevels.All)
        source.Listeners.Add(listener) |> ignore
        source.Switch.Level <- SourceLevels.All
        source
    /// Creates a defaulttracer
    let DefaultTracer traceSource id = 
        new DefaultStateTracer(traceSource, id) :> ITracer
    
    type ITracer with 
        /// creates a child tracer with the new activity
        member x.childTracer newActivity = 
            let baseTracer = x.TraceSource
            let tracer = new DefaultStateTracer(baseTracer, newActivity) :> ITracer
            x.doInId 
                (fun () -> 
                    x.TraceSource.TraceTransfer(0, "Switching to " + newActivity, tracer.ActivityId))
            tracer