// Weitere Informationen zu F# unter "http://fsharp.net".

namespace Yaaf.WirePlugin

open Wire
open System.Diagnostics

type ReplayWirePlugin() = 
    inherit Wire.Plugin()
    
    let logHelper s = 
        Trace.WriteLine(System.String.Format("{0}{1}", "YAAF: ", s))

    let log s = 
        Printf.kprintf logHelper
    let logInfo = 
        log TraceEventType.Information
    let logErr = 
        log TraceEventType.Error

    override x.Author with get() = "Matthias Dittrich"
        
    override x.Title with get() = "Yaaf WirePlugin"
    
    override x.Version with get() = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString()
    
    override x.init () =
        let gameInterface = InterfaceFactory.gameInterface()
        
        // Game started (with or without wire)
        gameInterface.add_GameStarted
            (fun gameId gamePath -> 
                logInfo "Game Started"
                ())
        // Match started (before Game started and only with wire)
        gameInterface.add_MatchStarted
            (fun matchId matchMediaPath -> 
                logInfo "Match Started"
                ())
        // Game stopped (before Match ended and always)
        // On real matches we have time until Anticheat has finished (parallel) for copying our matchmedia
        gameInterface.add_GameStopped
            (fun gameId -> 
                logInfo "Game Stopped"
                ())

        // Match ended (the Matchmedia dialog is already opened)
        gameInterface.add_MatchEnded
            (fun matchId -> 
                logInfo "Match Ended"
                ())
        ()

