// Weitere Informationen zu F# unter "http://fsharp.net".

namespace Yaaf.WirePlugin

open Wire
open System.Windows.Forms
open System.IO
open System.Drawing
open System.Diagnostics
open Yaaf.Logging
open Yaaf.WirePlugin.WinFormGui
open Yaaf.WirePlugin.WinFormGui.Properties

type ReplayWirePlugin() = 
    inherit Wire.Plugin()
    
    let logger = 
        let trace = Source "Yaaf.WirePlugin.ReplayWirePlugin" ""
        DefaultTracer trace "Initialization"
    
    let contextMenu = 
        let cm = 
            new ContextMenuStrip(
                RenderMode = ToolStripRenderMode.ManagerRenderMode,
                Font = new Font("Tahoma", 8.0f),
                ShowCheckMargin = false,
                ShowItemToolTips = false,
                AutoClose = true)
        let item s pic f = 
             new ToolStripMenuItem(s, pic, (fun sender e -> f())) 
             :> ToolStripItem
        [
            item Resources.Settings Resources.settingsPic
                (fun () ->
                    using (new OptionsForm()) (fun o ->
                        o.ShowDialog() |> ignore))
            item Resources.ReportBug Resources.mail
                (fun () -> 
                    try 
                        Process.Start("always-funny@gmx.net") |> ignore
                    with exn -> logger.logErr "Error: %O" exn)
            new ToolStripSeparator() :> ToolStripItem
            item Resources.CloseMenu Resources.cancel id
        ]
            |> List.iter (fun t -> cm.Items.Add(t) |> ignore)
        cm
    let mutable matchData = None
    let mutable watcher = None
    do  logger.logVerb "Starting up Yaaf wire plugin"
    static do 
        Application.EnableVisualStyles()
        
        if Settings.Default.upgradeSettings then
            Settings.Default.Upgrade()
            Settings.Default.upgradeSettings <- false
            Settings.Default.Save()
        
    override x.Author with get() = "Matthias Dittrich"
    override x.Title with get() = "Yaaf WirePlugin"
    override x.Version with get() = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString()
    override x.iconClicked (xCord, yCord, button) = 
        match button with
        | Plugin.MouseButton.RightButton ->
            contextMenu.Show(xCord, yCord)
        | Plugin.MouseButton.LeftButton ->
            ()
        | _ ->
            ()

        base.iconClicked(xCord, yCord, button)
    
    override x.init () =
        
        logger.logVerb "Setup Icon"
        x.setIcon(Resources.bluedragon)
        
        logger.logVerb "Setup Events"
        let gameInterface = InterfaceFactory.gameInterface()
        
        // Match started (before Game started and only with wire)
        gameInterface.add_MatchStarted
            (fun matchId matchMediaPath -> 
                let logger = logger.childTracer ("MatchStart" + string matchId)
                matchData <- Some(matchId, matchMediaPath)
                ())
        // Game started (with or without wire)
        gameInterface.add_GameStarted
            (fun gameId gamePath -> 
                let logger = logger.childTracer ("GameStart_" + string gameId)
                let parentDir = Path.GetDirectoryName(gamePath)
                let modDir, source =
                    match gameId with
                    | 43 -> // HL2
                        "hl2", Some true
                    | 60 -> // CSS
                        "cstrike" , Some true
                    | 112 -> // DOD:Source
                        "dod", Some true
                    | 126 -> // TF2
                        "tf", Some true
                    | 182 -> // HL2:DM
                        "hl2", Some true
                    | 5484 -> // CS Promod
                        "cstrike", Some true
                    | 59 -> // CS:CZ
                        "czero", Some false
                    | 61 -> // CS 1.6
                        "cstrike", Some false
                    | 64 -> // DoD
                        "dod", Some false
                    | _ ->
                        "", None
                match source with
                | Some(sourceGame) ->
                    watcher <- 
                        new SourceMatchmediaWatcher(logger, Path.Combine(parentDir, modDir), sourceGame)
                        |> Some
                | None ->
                    logger.logInfo "Ignoring unknown game: %d, %s" gameId gamePath
                )
        // Game stopped (before Match ended and always)
        // On real matches we have time until Anticheat has finished (parallel) for copying our matchmedia
        gameInterface.add_GameStopped
            (fun gameId -> 
                let logger = logger.childTracer  ("GameEnd" + string gameId)
                match watcher with
                | Some (w) ->
                    w.EndGame()
                    for m in w.FoundMedia do
                        
                ())

        // Match ended (the Matchmedia dialog is already opened)
        gameInterface.add_MatchEnded
            (fun matchId -> 
                let logger = logger.childTracer  ("MatchEnd" + string matchId)
                matchData <- None
                ())
        

