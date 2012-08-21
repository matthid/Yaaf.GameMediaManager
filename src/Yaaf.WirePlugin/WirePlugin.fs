// ----------------------------------------------------------------------------
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.
// ----------------------------------------------------------------------------
namespace Yaaf.WirePlugin

open Wire
open System.Windows.Forms
open System.IO
open System.Drawing
open System.Diagnostics
open Yaaf.Logging
open Yaaf.WirePlugin.WinFormGui
open Yaaf.WirePlugin.WinFormGui.Properties

/// The Wire Plugin implementation
type ReplayWirePlugin() as x = 
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
                    using (new OptionsForm(fun tr s -> logger.log tr "%s" s)) (fun o ->
                        o.ShowDialog() |> ignore))
            item Resources.ReportBug Resources.mail
                (fun () -> 
                    try 
                        Process.Start("https://github.com/matthid/Yaaf.WirePlugin/issues") |> ignore
                    with exn -> logger.logErr "Error: %O" exn)
            item Resources.Info Resources.i
                (fun () ->
                    using (new InfoForm(fun tr s -> logger.log tr "%s" s)) (fun o ->
                        o.ShowDialog() |> ignore))
            new ToolStripSeparator() :> ToolStripItem
            item Resources.CloseMenu Resources.cancel id
        ]
            |> List.iter (fun t -> cm.Items.Add(t) |> ignore)
        cm
    let mutable matchData = None
    let mutable watcher = None
    let mutable startTime = System.DateTime.Now
    
    let mutable gameInterface = None 

    /// Starts the mediawatcher for the given game
    let gameStarted (logger:ITracer) gameId gamePath = 
        startTime <- System.DateTime.Now
        let game = Database.getGame gameId
        watcher <- 
            match game with
            | None ->
                logger.logInfo "Ignoring unknown game: %d, %s" gameId gamePath
                None
            | Some (game) ->
                // Load data and create watcher
                game.WatchFolder.Load()
                let watchFolder = 
                    game.WatchFolder 
                    |> Seq.map (fun w -> w.Folder, w.Filter, if w.NotifyOnInativity.HasValue then Some w.NotifyOnInativity.Value else None)
                    |> Seq.toList
                new GenericMatchmediaWatcher(
                    logger,
                    watchFolder) |> Some  
                
        match watcher with
        | Some (w) -> 
            w.StartGame()
        | None -> ()

    let escapeInvalidChars escChar (path:string)  = 
        let invalid = 
            Path.GetInvalidFileNameChars() 
            |> Seq.append (Path.GetInvalidPathChars())   
            |> Seq.filter (fun c -> c <> Path.DirectorySeparatorChar)

        (
        path 
            |> Seq.fold 
                (fun (builder:System.Text.StringBuilder) char -> 
                    builder.Append(
                        if invalid |> Seq.exists (fun i -> i = char) 
                        then escChar
                        else char))
                (new System.Text.StringBuilder(path.Length))
        ).ToString()

    let backupFile file = 
        if (File.Exists file) then
            let rename = 
                Path.Combine(
                    (Path.GetDirectoryName file),
                    sprintf "%s-%s%s" (Path.GetFileNameWithoutExtension file) (System.Guid.NewGuid().ToString()) (Path.GetExtension file))
            File.Move(file, rename)

    /// Stops the watcher and renames the files properly
    let gameStopped (logger:ITracer) gameId = 
        let game = Database.getGame gameId
        match watcher, game with
        | Some(w), Some(game) ->
            w.EndGame()
            let game = 
                match gameId with
                | 43 -> "hl2" // HL2
                | 59 -> "cscz" // CS:CZ
                | 60 -> "css" // CSS
                | 61 -> "cs16"// CS 1.6
                | 64 -> "dod" // DoD
                | 112 -> "dodsource" // DOD:Source
                | 126 -> "tf2" // TF2
                | 182 -> "hl2dm" // HL2:DM
                | 687 -> "sc2"
                | 5484 -> "cspromod" // CS Promod
                | 6220 -> "csgo" // CS:GO
                | _ -> "unknown_game"

            match matchData with
            | Some (warId, matchMediaPath) ->
                let info = x.GameInterface.matchInfo(warId)
                let enemy = info.["name"] :?> string
                let enemy =
                    if enemy.StartsWith("vs. ") then
                        enemy.Substring(4)
                    else enemy

                ///let gameTitle = info.["gameTitle"]
                //let gameTime = info.["time"]
                let warFormat = Settings.Default.WarFileFormat
                if Settings.Default.WarSaveInWire then
                    w.FoundMedia
                    |> Seq.iteri 
                        (fun i (lNum, mediaDate, m) ->
                        let info = MediaAnalyser.analyseMedia m
                        let oldParent = Path.GetDirectoryName(m)
                        let newName = 
                            System.String.Format(warFormat, startTime, mediaDate, info.Map, game, lNum, i, warId, enemy) + Path.GetExtension(m)
                        let newName = newName |> escapeInvalidChars '_' 
                        let oldRenamed = Path.Combine(oldParent, newName)
                        
                        backupFile(oldRenamed)
                        File.Move(m, oldRenamed)
                              
                        x.GameInterface.moveToMatchMedia(
                            oldRenamed, 
                            warId) |> ignore)

            | None ->
                let publicFormat = Settings.Default.PublicFileFormat
                if Settings.Default.PublicSaveInWire then
                    let publicWireFolderFormat = Settings.Default.PublicFolderFormat

                    let newPath = Path.Combine(publicWireFolderFormat, publicFormat)

                    w.FoundMedia
                    |> Seq.iteri 
                        (fun i (lNum, mediaDate, m) ->
                            let info = MediaAnalyser.analyseMedia m
                            let gameInfo = x.GameInterface.gameInfo(gameId)
                            let newFileName = 
                                System.String.Format(newPath, startTime, mediaDate, info.Map, game, lNum, i) + Path.GetExtension(m)
                            let newFileName = newFileName |> escapeInvalidChars '_' 
                            let target = Path.Combine(Settings.Default.MatchMediaPath, newFileName)
                            let parent = Path.GetDirectoryName(target)
                            if not <| Directory.Exists parent then
                                Directory.CreateDirectory parent |> ignore
                                
                            backupFile(target)
                            File.Move(m, target))

        | _ ->
            logger.logInfo "unknown game or game without watcher closed: %d" gameId

    do  logger.logVerb "Starting up Yaaf wire plugin"
    static do 
        Application.EnableVisualStyles()
        
        if Settings.Default.upgradeSettings then
            Settings.Default.Upgrade()
            Settings.Default.upgradeSettings <- false
            Settings.Default.Save()
        
        if System.String.IsNullOrEmpty Settings.Default.MatchMediaPath then
            Settings.Default.MatchMediaPath <-
                Path.Combine(
                    System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments),
                    "ESL Match Media")
            Settings.Default.Save()
    member x.GameInterface 
        with get() : GameInterface = 
            match gameInterface with
            | Some (i) -> i
            | None -> invalidOp "not initialized"
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
        try 
            logger.logVerb "Setup Icon"
            x.setIcon(Resources.bluedragon)
        
            logger.logVerb "Setup Events"
            gameInterface <- Some (InterfaceFactory.gameInterface())
            // Match started (before Game started and only with wire)
            x.GameInterface.add_MatchStarted
                (fun matchId matchMediaPath -> 
                    let logger = logger.childTracer ("MatchStart" + string matchId)
                    try
                        Settings.Default.MatchMediaPath <- matchMediaPath
                        Settings.Default.Save()
                        matchData <- Some(matchId, matchMediaPath)
                    with exn ->
                        logger.logErr "Error: %O" exn)

            // Game started (with or without wire)
            x.GameInterface.add_GameStarted
                (fun gameId gamePath -> 
                    let logger = logger.childTracer ("GameStart" + string gameId)
                    try
                        gameStarted logger gameId gamePath
                    with exn ->
                        logger.logErr "Error: %O" exn)
            // Game stopped (before Match ended and always)
            // On real matches we have time until Anticheat has finished (parallel) for copying our matchmedia
            x.GameInterface.add_GameStopped
                (fun gameId -> 
                    let logger = logger.childTracer  ("GameEnd" + string gameId)
                    try
                        gameStopped logger gameId
                    with exn ->
                        logger.logErr "Error: %O" exn)

            // Match ended (the Matchmedia dialog is already opened)
            x.GameInterface.add_MatchEnded
                (fun matchId -> 
                    let logger = logger.childTracer  ("MatchEnd" + string matchId)
                    try
                        matchData <- None
                    with exn ->
                        logger.logErr "Error: %O" exn)
        
        with exn ->
            logger.logErr "Error: %O" exn

