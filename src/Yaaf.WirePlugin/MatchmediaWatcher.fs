﻿// ----------------------------------------------------------------------------
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.
// ----------------------------------------------------------------------------
namespace Yaaf.WirePlugin

open Yaaf.WirePlugin.WinFormGui.Properties
open Yaaf.Logging
open System.IO
type RecursiveFileSystemWatcher(path, filter) as x = 
    inherit FileSystemWatcher(path, filter)
    let defaultFilters = 
        NotifyFilters.LastWrite ||| 
        NotifyFilters.Size ||| 
        NotifyFilters.CreationTime ||| 
        NotifyFilters.Attributes |||
        NotifyFilters.FileName |||
        NotifyFilters.Security |||
        NotifyFilters.LastAccess |||
        NotifyFilters.DirectoryName

    let childWatcher = new System.Collections.Generic.List<RecursiveFileSystemWatcher>()
    let addChild (w:RecursiveFileSystemWatcher) = 
        w.Changed |> Event.add (fun e -> x.MyOnChanged(e))
        w.Created |> Event.add (fun e -> x.MyOnCreated(e))
        w.Deleted |> Event.add (fun e -> x.MyOnDeleted(e))
        w.Error |> Event.add (fun e -> x.MyOnError(e))
        w.Renamed |> Event.add (fun e -> x.MyOnRenamed(e))
        x.Disposed 
            |> Event.add (fun e -> w.Dispose())
        childWatcher.Add w
        
    let rec addSymlinks path = 
        let p = new ReparsePoints.ReparsePoint(path)
        if p.Tag = ReparsePoints.ReparsePoint.TagType.SymbolicLink then
            addChild (new RecursiveFileSystemWatcher(p.Target, filter))
        else
            for d in Directory.GetDirectories(path) do
                addSymlinks d

    do
        x.EnableRaisingEvents <- true
        x.IncludeSubdirectories <- true
        x.NotifyFilter <- defaultFilters
        addSymlinks path

    member x.MyOnChanged e = x.OnChanged e
    member x.MyOnCreated e = x.OnCreated e
    member x.MyOnDeleted e = x.OnDeleted e
    member x.MyOnError e = x.OnError e
    member x.MyOnRenamed e = x.OnRenamed e

/// This class provides observation functionality which can used in subclasses
[<AbstractClass>]
type MatchmediaWatcher(logger : ITracer) =
    let settings = new Settings()
    let notifyEvent = new Event<unit>()
    let foundMedia = new System.Collections.Generic.List<int * System.DateTime * System.String>()
    let watcher = new System.Collections.Generic.List<FileSystemWatcher>()
    let mutable started = false
    do
        settings.Reload()

    [<CLIEventAttribute>]
    member x.NotifyRecord = notifyEvent.Publish

    member x.NotifyRecordMissing () = notifyEvent.Trigger()

    member x.Settings with get() = settings
    member x.FoundMedia with get() = new System.Collections.Generic.List<int * System.DateTime * System.String>(foundMedia)
    member x.BasicWatchFolder path filter = 
        let localNr = ref 0
        let w = new RecursiveFileSystemWatcher(path, filter)
        w.Error
            |> Event.add (fun e -> logger.logErr "Error in Watcher: %O" (e.GetException()))
        w.Created
            |> Event.add 
                (fun e -> 
                    logger.logVerb "Found Matchmedia: %s" e.FullPath
                    foundMedia.Add(!localNr, System.DateTime.Now, e.FullPath)
                    localNr := !localNr + 1
                    x.FileChanged(e.FullPath))
        w.Changed
            |> Event.add (fun e -> x.FileChanged(e.FullPath))
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

    
type Starcraft2MediaWatcher(logger: ITracer) = 
    inherit MatchmediaWatcher(logger)

    override x.FileChanged path = logger.logVerb "Media %s changed" path

    override x.StartGameAbstract() = 
        let modPath = 
            Path.Combine(
                System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments),
                "StarCraft II")
        logger.logVerb "Starting watching in %s" modPath
        x.BasicWatchFolder modPath "*.SC2Replay"

    override x.EndGameAbstract() = ()

