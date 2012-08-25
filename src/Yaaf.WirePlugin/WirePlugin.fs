// ----------------------------------------------------------------------------
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.
// ----------------------------------------------------------------------------
namespace Yaaf.WirePlugin

open Wire
open System
open System.Windows.Forms
open System.IO
open System.Drawing
open System.Diagnostics
open Yaaf.Logging
open Yaaf.WirePlugin.WinFormGui
open Yaaf.WirePlugin.WinFormGui.Properties
type GameData = {
        Game : Database.Game
        Watcher : MatchmediaWatcher
        MatchSession : Database.MatchSession
    }
type Session = {
        GameData : GameData option
        StartTime : DateTime
        EslMatch : int option
        Context : LocalDatabaseWrapper
    } with
        member x.IsEslMatch =
            match x.EslMatch with
            | Some(_) -> true
            | None -> false
        member x.EslMatchId 
            with get () = 
                match x.EslMatch with
                | Some(id) -> id
                | None -> invalidOp "this is no esl match"
        member x.DB = 
            x.Context.Context

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
        let seperator () = new ToolStripSeparator() :> ToolStripItem
        [
            item Resources.ReportBug Resources.mail
                (fun () -> 
                    try 
                        Process.Start("https://github.com/matthid/Yaaf.WirePlugin/issues") |> ignore
                    with exn -> logger.logErr "Error: %O" exn)
            item Resources.Info Resources.i
                (fun () ->
                    using (new InfoForm(logger)) (fun o ->
                        o.ShowDialog() |> ignore))
            item Resources.EditGames Resources.add
                (fun () ->
                    using (new EditGames(logger, Database.getContext())) (fun o ->
                        o.ShowDialog() |> ignore))
            item Resources.EditPlayers Resources.edit
                (fun () ->
                    let dbContext = Database.getContext()
                    using (new ManagePlayers(logger, dbContext, Database.getIdentityPlayer dbContext.Context)) (fun o ->
                        o.ShowDialog() |> ignore))
            seperator()
            item Resources.CloseMenu Resources.cancel id
        ]
            |> List.iter (fun t -> cm.Items.Add(t) |> ignore)
        cm

    let mutable gameInterface = None 

    let mutable session = None : Session option

    let sessionMatchStarted logger matchId = 
        let localContext = Database.getContext()
        {
            Context = localContext
            GameData = None
            StartTime = DateTime.Now
            EslMatch = matchId }
    
    let getGrabAction db matchSession link = 
        async  {
            try
                let! players = EslGrabber.getMatchMembers link
                Database.fillPlayers db matchSession players
            with exn ->
                logger.logErr "Could not grab esl-infos: %O" exn        
            return ()
        }

    /// Starts the mediawatcher for the given game
    let sessionGameStarted (logger:ITracer) (session:Session) gameId gamePath = 
        let db = session.DB
        let game = Database.getGame db gameId
        let watcher = 
            match game with
            | None ->
                logger.logInfo "Ignoring unknown game: %d, %s" gameId gamePath
                None
            | Some (game) ->
                if (session.IsEslMatch && not game.EnableWarMatchForm && not game.WarMatchFormSaveFiles) ||
                   (not session.IsEslMatch && not game.EnableMatchForm && not game.PublicMatchFormSaveFiles) then
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
                        watchFolder) :> MatchmediaWatcher |> Some  
        let gameData = 
            match watcher with
            | Some (w) -> 
                w.StartGameWatching()
                let game = match game with | Some (g) -> g | None -> failwith "game should not be None when watcher is not!"
                let matchSession, sessionAdded = 
                    let elapsedTime = int (System.DateTime.Now - session.StartTime).TotalSeconds
                    match session.EslMatch with
                    | Some (warId) ->
                        // Check if this EslMatch already exisits and use the old session
                        let info = x.GameInterface.matchInfo(warId)
                        let eslMatchLink = info.["uri"] :?> string
                        match Database.findEslMatch db eslMatchLink with
                        | Some (availableMatch) -> availableMatch, true
                        | None ->
                        new Database.MatchSession(
                            Game = game,
                            Startdate = session.StartTime,
                            Duration = elapsedTime,
                            EslMatchLink = eslMatchLink), false       
                    | None ->
                        new Database.MatchSession(
                            Game = game,
                            Startdate = session.StartTime,
                            Duration = elapsedTime), false
                
                matchSession.MatchSessions_Player.Load()
                matchSession.Matchmedia.Load()
                if not sessionAdded then
                    // Add me to the session...
                    let me = Database.getIdentityPlayer db
                    let newPlayerAssociation = 
                        new Database.MatchSessions_Player(
                            Cheating = false,
                            Player = me,
                            Skill = System.Nullable(100uy),
                            Team = 1uy)
                    matchSession.MatchSessions_Player.Add(newPlayerAssociation)
                    db.MatchSessions.InsertOnSubmit matchSession
                
                if (session.IsEslMatch) then
                    getGrabAction db matchSession matchSession.EslMatchLink
                    |> Async.Start

                Some { Watcher = w; Game = game; MatchSession = matchSession }
            | None -> None

        { session with GameData = gameData }

    let executeAction (context:Database.LocalDatabaseDataContext) (action:Database.ActionObject) (media:Database.Matchmedia seq) = 
        let filter = Database.getFilter action
        let action = Database.getAction action
        try
            media 
                |> Seq.filter filter
                |> Seq.iter action
        finally
            context.SubmitChanges()
            
    /// Stops the watcher saves the matchmedia and starts the given actions
    let sessionEnd (logger:ITracer) (session:Session) = 
        let db = session.DB

        
        if (match session.GameData with
            | Some(data) ->
                false
            | _ ->
                true) then
            logger.logInfo "unknown game or game without watcher closed!"
        else
        let data = session.GameData.Value
        let watcher, game = data.Watcher, data.Game
        watcher.EndGameWatching()
        let matchSession = data.MatchSession 
           
        watcher.FoundMedia
            |> Seq.mapi  
                (fun i (lNum, mediaDate, m) -> 
                    new Database.Matchmedia(
                        Created = mediaDate, 
                        MatchSession = matchSession,
                        Map = (MediaAnalyser.analyseMedia m).Map,
                        Name = Path.GetFileNameWithoutExtension m,
                        Type = Path.GetExtension m,
                        Path = m))
            |> Seq.iter (fun s -> matchSession.Matchmedia.Add(s))

        let deleteData =
            if (session.IsEslMatch && game.EnableWarMatchForm) || (not session.IsEslMatch && game.EnableMatchForm) then
                let formSession = 
                    { new IMatchSession with
                        member x.LoadEslPlayers link = 
                            getGrabAction db matchSession link
                        member x.Session 
                            with get() = matchSession
                        }
                let form = new MatchSessionEnd(logger, session.Context, formSession)
                form.ShowDialog() |> ignore
                form.DeleteMatchmedia
            else
                // Could be changed in the meantime
                (session.IsEslMatch && not game.WarMatchFormSaveFiles) || (not session.IsEslMatch && not game.PublicMatchFormSaveFiles)

        if (deleteData) then
            for matchmedia in matchSession.Matchmedia do
                if (File.Exists(matchmedia.Path)) then
                    File.Delete(matchmedia.Path)

            db.MatchSessions.DeleteOnSubmit(matchSession)
            session.Context.MySubmitChanges()
        else

        logger.logInfo "Add Match to Database"   
        session.Context.MySubmitChanges()
        
        logger.logInfo "Move Media to MediaPath"   
        for m in matchSession.Matchmedia do
            let dbMediaPath = Database.mediaPath m
            try
                File.Move (m.Path, dbMediaPath)
                m.Path <- dbMediaPath
            with :? IOException as e ->
                logger.logErr "Could not move Matchmedia from %s to Database! (Error: %O)" m.Path e   
        
        logger.logInfo "Change to MediaPath in Database"  
        session.Context.MySubmitChanges() 

        // Execute automatic actions for this game
        for action in Database.getActivatedMatchFormActions db session.IsEslMatch game do
            try
                logger.logInfo "Executing Action %s on game %s" action.ActionObject.Name game.Name
                executeAction db action.ActionObject matchSession.Matchmedia
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
                        match session with
                        | Some (s) -> 
                            failwith "a previous session was not closed properly!"
                        | None ->
                            session <-
                                Some (sessionMatchStarted logger (Some matchId))))

            // Game started (with or without wire)
            x.GameInterface.add_GameStarted
                (fun gameId gamePath -> 
                    logEvent ("GameStart" + string gameId) (fun logger ->
                        let currS = 
                            match session with
                            | Some (s) -> s // Use this session as it was created in MatchStart
                            | None -> sessionMatchStarted logger None
                        session <-
                            sessionGameStarted logger currS gameId gamePath |> Some))

            // Game stopped (before Match ended and always)
            // On real matches we have time until Anticheat has finished (parallel) for copying our matchmedia
            x.GameInterface.add_GameStopped
                (fun gameId -> 
                    logEvent ("GameEnd" + string gameId) (fun logger ->
                        match session with
                        | Some (s) -> 
                            // Use this session as it was created in MatchStart
                            sessionEnd logger s
                            session <- None
                        | None -> 
                            failwith "No session found on GameEnd!"))

            // Match ended (the Matchmedia dialog is already opened)
            // This event brings no additional information...
            x.GameInterface.add_MatchEnded
                (fun matchId -> 
                    logEvent ("MatchEnd" + string matchId) (fun logger ->
                        match session with
                        | Some (s) -> 
                            sessionEnd logger s
                            session <- None
                            failwith "Session not already closed on MatchEnd!"
                        | None -> ()))
        with exn ->
            logger.logErr "Error: %O" exn

