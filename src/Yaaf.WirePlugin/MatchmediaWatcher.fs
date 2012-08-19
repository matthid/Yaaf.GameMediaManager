// ----------------------------------------------------------------------------
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.
// ----------------------------------------------------------------------------
namespace Yaaf.WirePlugin

open Yaaf.WirePlugin.WinFormGui.Properties
open Yaaf.Logging
open System.IO

/// This class provides observation functionality which can used in subclasses
[<AbstractClass>]
type MatchmediaWatcher(logger : ITracer) =
    let settings = new Settings()
    let notifyEvent = new Event<unit>()
    let foundMedia = new System.Collections.Generic.List<int * System.String>()
    let watcher = new System.Collections.Generic.List<FileSystemWatcher>()
    let mutable started = false
    let defaultFilters = 
        NotifyFilters.LastWrite ||| 
        NotifyFilters.Size ||| 
        NotifyFilters.CreationTime ||| 
        NotifyFilters.Attributes |||
        NotifyFilters.FileName |||
        NotifyFilters.Security |||
        NotifyFilters.LastAccess |||
        NotifyFilters.DirectoryName
    do
        settings.Reload()

    [<CLIEventAttribute>]
    member x.NotifyRecord = notifyEvent.Publish

    member x.NotifyRecordMissing () = notifyEvent.Trigger()

    member x.Settings with get() = settings
    member x.FoundMedia with get() = new System.Collections.Generic.List<int * System.String>(foundMedia)
    member x.BasicWatchFolder path filter = 
        let localNr = ref 0
        let w = new FileSystemWatcher(path, filter)
        w.Error
            |> Event.add (fun e -> logger.logErr "Error in Watcher: %O" (e.GetException()))
        w.Created
            |> Event.add 
                (fun e -> 
                    logger.logVerb "Found Matchmedia: %s" e.FullPath
                    foundMedia.Add(!localNr, e.FullPath)
                    localNr := !localNr + 1
                    x.FileChanged(e.FullPath))
        w.Changed
            |> Event.add (fun e -> x.FileChanged(e.FullPath))
        w.EnableRaisingEvents <- true
        w.IncludeSubdirectories <- true
        w.NotifyFilter <- defaultFilters
        watcher.Add(w)

    member x.StartGame () = 
        if started then
            invalidOp "Can't start twice"
        started <- true
        x.StartGameAbstract()
    abstract member StartGameAbstract : unit -> unit
    abstract member FileChanged : string -> unit
    member x.EndGame() = 
        for w in watcher do
            w.Dispose()
        watcher.Clear()
        x.EndGameAbstract()
    abstract member EndGameAbstract : unit -> unit

/// Observator for a source powered game (and old hl mods)
type SourceMatchmediaWatcher (logger : ITracer, modPath:string, sourceGame:bool) = 
    inherit MatchmediaWatcher(logger)

    override x.FileChanged path = logger.logVerb "Media %s changed" path

    override x.StartGameAbstract() = 
        logger.logVerb "Starting watching in %s" modPath
        x.BasicWatchFolder modPath "*.dem"
        x.BasicWatchFolder modPath (if sourceGame then "*.jpg" else "*.bmp")

    override x.EndGameAbstract() = ()

    


