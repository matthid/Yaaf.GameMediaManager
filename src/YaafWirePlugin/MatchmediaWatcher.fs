namespace Yaaf.WirePlugin

open Yaaf.WirePlugin.WinFormGui.Properties
open Yaaf.Logging
open System.IO

[<AbstractClass>]
type MatchmediaWatcher(logger : ITracer) =
    let settings = new Settings()
    let foundMedia = new System.Collections.Generic.List<System.String>()
    let watcher = new System.Collections.Generic.List<FileSystemWatcher>()
    let mutable started = false
    let defaultFilters = 
        NotifyFilters.LastWrite ||| NotifyFilters.Size ||| NotifyFilters.CreationTime
    do
        settings.Reload()


    member x.Settings with get() = settings
    member x.FoundMedia with get() = new System.Collections.Generic.List<System.String>(foundMedia)
    member x.BasicWatchFolder path filter = 
        let w = new FileSystemWatcher(path, filter)
        w.Error
            |> Event.add (fun e -> logger.logErr "Error in Watcher: %O" (e.GetException()))
        w.Created
            |> Event.add 
                (fun e -> 
                    logger.logVerb "Found Matchmedia: %s" e.FullPath
                    foundMedia.Add(e.FullPath)
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

type SourceMatchmediaWatcher (logger : ITracer, modPath:string, sourceGame:bool) = 
    inherit MatchmediaWatcher(logger)

    override x.FileChanged path = 
        ()

    override x.StartGameAbstract() = 
        x.BasicWatchFolder modPath ".dem"
        x.BasicWatchFolder modPath (if sourceGame then ".jpeg" else ".bmp")
        ()

    override x.EndGameAbstract() = 
        ()

    


