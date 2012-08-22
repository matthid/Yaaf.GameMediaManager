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
            item Resources.ReportBug Resources.mail
                (fun () -> 
                    try 
                        Process.Start("https://github.com/matthid/Yaaf.WirePlugin/issues") |> ignore
                    with exn -> logger.logErr "Error: %O" exn)
            item Resources.Info Resources.i
                (fun () ->
                    using (new InfoForm(fun tr s -> logger.log tr "%s" s)) (fun o ->
                        o.ShowDialog() |> ignore))
            item Resources.EditGames Resources.add
                (fun () ->
                    using (new EditGames((fun tr s -> logger.log tr "%s" s), Database.wrapper)) (fun o ->
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
    let sessionStarted (logger:ITracer) gameId gamePath = 
        startTime <- System.DateTime.Now
        let game = Database.getGame gameId
        watcher <- 
            match game with
            | None ->
                logger.logInfo "Ignoring unknown game: %d, %s" gameId gamePath
                None
            | Some (game) ->
                
                if (matchData.IsSome && not game.EnableWarMatchForm && not game.WarMatchFormSaveFiles) ||
                   (not matchData.IsSome && not game.EnableMatchForm && not game.PublicMatchFormSaveFiles) then
                    logger.logWarn "Not saving data becaue it is disabled for game: %d" gameId
                    None
                else
                    // Load data and create watcher
                    game.WatchFolder.Load()
                    let watchFolder = 
                        game.WatchFolder 
                        |> Seq.map (fun w -> w.Folder, w.Filter, if w.NotifyOnInactivity.HasValue then Some w.NotifyOnInactivity.Value else None)
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
    
    let executeAction (action:Database.ActionObject) (media:Database.Matchmedia seq) = 
        let filter = Database.getFilter action
        let action = Database.getAction action
        try
            media 
                |> Seq.filter filter
                |> Seq.iter action
        finally
            Database.db.SubmitChanges()
            
    /// Stops the watcher saves the matchmedia and starts the given actions
    let sessionStopped (logger:ITracer) gameId = 
        let game = Database.getGame gameId
        if (match watcher, game with
            | Some(w), Some(game) ->
                false
            | _ ->
                true) then
            logger.logInfo "unknown game or game without watcher closed: %d" gameId
        else
        let watcher, game = watcher.Value, game.Value
        watcher.EndGame()
        let session, sessionAdded = 
            let elapsedTime = int (System.DateTime.Now - startTime).TotalSeconds
            match matchData with
            | Some (warId, matchMediaPath) ->
                // TODO: Check if this EslMatch already exisits and use the old session
                let info = x.GameInterface.matchInfo(warId)
                
                new Database.MatchSession(
                    Game = game,
                    Startdate = startTime,
                    Duration = elapsedTime,
                    EslMatchLink = (info.["uri"] :?> string)), false       
            | None ->
                new Database.MatchSession(
                    Game = game,
                    Startdate = startTime,
                    Duration = elapsedTime), false
        if not sessionAdded then
            // Add me...
            let me = Database.getIdentityPlayer()
            let newPlayerAssociation = 
                new Database.MatchSessions_Player(
                    Cheating = false,
                    Player = me,
                    Skill = System.Nullable(100uy),
                    Team = 1uy)
            session.MatchSessions_Player.Add(newPlayerAssociation)

        let warId = 
            match matchData with
            | Some (warId, matchMediaPath) -> Some warId
            | None -> None

        let showEndSessionWindow media = 
            if (matchData.IsSome && not game.EnableWarMatchForm) || (matchData.IsNone && not game.EnableMatchForm) then
                // Could be changed in the meantime
                if (matchData.IsSome && not game.WarMatchFormSaveFiles) || (matchData.IsNone && not game.PublicMatchFormSaveFiles) then
                    None
                else
                    Some media
            else

            let form = new MatchSessionEnd((fun tr l -> logger.log tr "%s" l), Database.wrapper, session, media)
            form.ShowDialog() |> ignore
            if form.ResultMedia = null then None else Some form.ResultMedia

        let preparedMatchmedia = 
            watcher.FoundMedia
                |> Seq.mapi  
                    (fun i (lNum, mediaDate, m) -> 
                        new Database.Matchmedia(
                            Created = mediaDate, 
                            MatchSession = session,
                            Map = (MediaAnalyser.analyseMedia m).Map,
                            Name = Path.GetFileNameWithoutExtension m,
                            Type = Path.GetExtension m,
                            Path = m))
                |> showEndSessionWindow
        match preparedMatchmedia with
        | None -> ()
        | Some (media) ->

        logger.logInfo "Add Match to Database"   
        Database.db.Matchmedias.InsertAllOnSubmit(media)
        if not sessionAdded then
            Database.db.MatchSessions.InsertOnSubmit(session)

        Database.db.SubmitChanges()
        
        logger.logInfo "Move Media to MediaPath"   
        for m in media do
            let dbMediaPath = Database.mediaPath m
            try
                File.Move (m.Path, dbMediaPath)
                m.Path <- dbMediaPath
            with :? IOException as e ->
                logger.logErr "Could not move Matchmedia from %s to Database! (Error: %O)" m.Path e   
        
        logger.logInfo "Change to MediaPath in Database"   
        Database.db.SubmitChanges()

        // Execute automatic actions for this game
        for action in Database.getActivatedMatchFormActions matchData.IsSome game do
            try
                logger.logInfo "Executing Action %s on game %s" action.ActionObject.Name game.Name
                executeAction action.ActionObject media
            with exn ->
                logger.logErr "Action %s on game %s failed! Error %O" action.ActionObject.Name game.Name exn

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

        #if DEBUG
        let dir = Path.GetDirectoryName (System.Reflection.Assembly.GetExecutingAssembly().Location)
        let winFormGui = System.Reflection.Assembly.LoadFile(Path.Combine(dir, "Yaaf.WirePlugin.WinFormGui.dll"))
        System.AppDomain.CurrentDomain.add_AssemblyResolve
            (System.ResolveEventHandler(fun o e -> if e.Name.StartsWith("Yaaf.WirePlugin.WinFormGui") then winFormGui else null))

        #endif

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
        
            let logEvent event f = 
                let logger = logger.childTracer (event)
                try
                    let t = 
                        new System.Threading.Thread(fun () ->
                            try
                                f logger
                            with exn ->
                                logger.logErr "Error: %O" exn)
                    t.SetApartmentState(System.Threading.ApartmentState.STA)
                    t.Start()
                with exn ->
                    logger.logErr "Error: %O" exn

            // Match started (before Game started and only with wire)
            x.GameInterface.add_MatchStarted
                (fun matchId matchMediaPath -> 
                    logEvent ("MatchStart" + string matchId) (fun logger ->
                        Settings.Default.MatchMediaPath <- matchMediaPath
                        Settings.Default.Save()
                        matchData <- Some(matchId, matchMediaPath)))

            // Game started (with or without wire)
            x.GameInterface.add_GameStarted
                (fun gameId gamePath -> 
                    logEvent ("GameStart" + string gameId) (fun logger ->
                            sessionStarted logger gameId gamePath))

            // Game stopped (before Match ended and always)
            // On real matches we have time until Anticheat has finished (parallel) for copying our matchmedia
            x.GameInterface.add_GameStopped
                (fun gameId -> 
                    logEvent ("GameEnd" + string gameId) (fun logger ->
                            sessionStopped logger gameId))

            // Match ended (the Matchmedia dialog is already opened)
            x.GameInterface.add_MatchEnded
                (fun matchId -> 
                    logEvent ("MatchEnd" + string matchId) (fun logger ->
                            matchData <- None))
        
        with exn ->
            logger.logErr "Error: %O" exn

