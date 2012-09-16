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
open Yaaf.WirePlugin.Primitives
open Yaaf.WirePlugin.WinFormGui.Properties
type GameData = {
        Game : Database.Game
        Watcher : MatchmediaWatcher
        DefaultPlayer : Database.MatchSessions_Player
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

type GrabberProcMsg =
    | StartGrabbing
    | StoppedGrabbing
    | WaitForFinish of AsyncReplyChannel<unit>
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
             new ToolStripMenuItem(s, pic, 
                (fun sender e -> 
                    let t = 
                        new System.Threading.Thread(fun () ->
                            try
                                f()
                            with exn ->
                                logger.logErr "Error: %O" exn)
                    t.SetApartmentState(System.Threading.ApartmentState.STA)
                    t.Start())) 
             :> ToolStripItem
        let seperator () = new ToolStripSeparator() :> ToolStripItem
        let showForm (f:System.Windows.Forms.Form) = 
            using f (fun form ->
                form.ShowDialog() |> ignore)
        [
            item Resources.ReportBug Resources.mail
                (fun () -> 
                    try 
                        Process.Start("https://github.com/matthid/Yaaf.WirePlugin/issues") |> ignore
                    with exn -> logger.logErr "Error: %O" exn)
            item Resources.Info Resources.i
                (fun () -> new InfoForm(logger) |> showForm)
            item Resources.EditGames Resources.add
                (fun () -> new EditGames(logger)|> showForm)
            item Resources.MatchSessions Resources.favs
                (fun () ->
                    new ViewMatchSessions(logger)|> showForm)
            item Resources.EditPlayers Resources.edit
                (fun () ->
                    new ManagePlayers(logger)|> showForm)
            seperator()
            item Resources.CloseMenu Resources.cancel id
        ]
            |> List.iter (fun t -> cm.Items.Add(t) |> ignore)
        cm

    let mutable gameInterface = None 
    let gameSessions = new System.Collections.Generic.Dictionary<int, Session option>()
    let getGameSession id = 
        match gameSessions.TryGetValue id with
        | true, v -> v
        | false, _ -> None
    let setGameSession id game = gameSessions.[id] <- game

    let sessionMatchStarted logger matchId = 
        let localContext = Database.getContext()
        {
            Context = localContext
            GameData = None
            StartTime = DateTime.Now
            EslMatch = matchId }
    
    let grabberProcessor = MailboxProcessor.Start(fun inbox ->
        let rec loop grabbingNum (waitingList:AsyncReplyChannel<unit> list)= async {
            let! msg = inbox.Receive()
            match msg with
            | GrabberProcMsg.StartGrabbing ->
                logger.logInfo "StartGrabbing..."
                return! loop (grabbingNum + 1) waitingList
            | GrabberProcMsg.StoppedGrabbing ->
                logger.logInfo "StoppedGrabbing..."
                let newWaitingList = 
                    if grabbingNum = 1 then
                        waitingList
                            |> List.iter (fun w -> w.Reply())
                        []
                    else waitingList
                return! loop (grabbingNum - 1) newWaitingList
            | GrabberProcMsg.WaitForFinish(replyChannel) ->
                logger.logInfo "WaitForFinish..."
                let newWaitingList =
                    if grabbingNum = 0 then
                        waitingList
                            |> List.iter (fun w -> w.Reply())
                        logger.logInfo "Send Reply..."
                        replyChannel.Reply()
                        []
                    else replyChannel :: waitingList
                return! loop grabbingNum newWaitingList
            }
        loop 0 [])
        
    let getGrabAction db (logger:ITracer) matchSession link = 
        async  {
            try
                try
                    logger.logInfo "Starting grabbing..."
                    grabberProcessor.Post GrabberProcMsg.StartGrabbing
                    let! players = EslGrabber.getMatchMembers link
                    Database.fillPlayers db matchSession players
                with exn ->
                    logger.logErr "Could not grab esl-infos: %O" exn    
            finally
                logger.logInfo "Finished grabbing..."
                grabberProcessor.Post GrabberProcMsg.StoppedGrabbing    
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
                            Name = "",
                            Game = game,
                            Startdate = session.StartTime,
                            Duration = elapsedTime,
                            EslMatchLink = eslMatchLink), false       
                    | None ->
                        new Database.MatchSession(
                            Name = "",
                            Game = game,
                            Startdate = session.StartTime,
                            Duration = elapsedTime), false
                
                let playerAssoc = 
                    if not sessionAdded then
                        // Add me to the session...
                        let me = Database.getIdentityPlayer db
                        let newPlayerAssociation = 
                            new Database.MatchSessions_Player(
                                MatchSession = matchSession,
                                Cheating = false,
                                Player = me,
                                Skill = System.Nullable(100uy),
                                Team = 11uy)
                        matchSession.MatchSessions_Player.Add(newPlayerAssociation)
                        db.MatchSessions.InsertOnSubmit matchSession
                        newPlayerAssociation
                    else
                       matchSession.MatchSessions_Player |> Seq.head
                
                if (session.IsEslMatch) then
                    getGrabAction session.Context logger matchSession matchSession.EslMatchLink
                    |> Async.Start

                Some { Watcher = w; Game = game; MatchSession = matchSession; DefaultPlayer = playerAssoc }
            | None -> None

        { session with GameData = gameData }

    let executeAction (context:Database.LocalDataContext) (actionObj:Database.ActionObject) (session:Database.MatchSession) = 
        let action = Database.getAction context actionObj
        try
            action (session:>obj)
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
        let me = Database.getIdentityPlayer db
        watcher.FoundMedia
            |> Seq.mapi  
                (fun i (lNum, mediaDate, m) -> 
                        new Database.Matchmedia(
                            MatchSessions_Player = data.DefaultPlayer,
                            Player = me,
                            Created = mediaDate, 
                            MatchSession = matchSession,
                            Map = (MediaAnalyser.analyseMedia m).Map,
                            Name = Path.GetFileNameWithoutExtension m,
                            Type = Path.GetExtension m,
                            Path = m)
                    )
            |> Seq.iter (fun s -> matchSession.Matchmedia.Add(s))
        logger.logInfo "Waiting for running grabbing tasks"
        let waitGrabbing () = 
            let finishTask = 
                grabberProcessor.PostAndAsyncReply(fun channel -> GrabberProcMsg.WaitForFinish(channel))
            let task = Primitives.Task<_> finishTask
            WaitingForm.StartTask(logger, task, "Waiting for Grabbing Players...")
        
            logger.logInfo "grabbing tasks fininshed"

        waitGrabbing()
        let deleteData =
            if (session.IsEslMatch && game.EnableWarMatchForm) || (not session.IsEslMatch && game.EnableMatchForm) then
                logger.logInfo "opening MatchSessionEnd form"
                let mediaWrapper = WinFormGui.Helpers.GetWrapper(matchSession.Matchmedia)
                let playerWrapper = WinFormGui.Helpers.GetWrapper(matchSession.MatchSessions_Player)
                let form = new MatchSessionEnd(logger, session.Context,(mediaWrapper,playerWrapper), matchSession)
                form.ShowDialog() |> ignore
                if form.DeleteMatchmedia.HasValue then
                    let getOriginal d = Seq.map (fun itemData -> itemData.Original) d
                    for original in mediaWrapper.Deletions |> getOriginal do
                        original.MatchSessions_Player <- null
                        original.Player <- null
                        original.MatchSession <- null
                    for original in mediaWrapper.Inserts |> getOriginal do
                        matchSession.Matchmedia.Add original
                    mediaWrapper.UpdateTable session.Context.Context.Matchmedias |> ignore

                    for original in playerWrapper.Deletions |> getOriginal do
                        original.Player <- null
                        original.MatchSession <- null
                    for original in playerWrapper.Inserts |> getOriginal do
                        matchSession.MatchSessions_Player.Add original
                    session.Context.UpdateMatchSessionPlayerTable playerWrapper
                if not <| form.DeleteMatchmedia.HasValue then
                    false
                else
                    form.DeleteMatchmedia.Value
            else
                // Could be changed in the meantime
                logger.logInfo "jumping over the MatchSessionEnd form"
                (session.IsEslMatch && not game.WarMatchFormSaveFiles) || (not session.IsEslMatch && not game.PublicMatchFormSaveFiles)

        let doCustomAction = 
            try
                if (deleteData) then
                    // Removing the session and the files from the database
                    Database.removeSession db true matchSession
                    session.Context.MySubmitChanges()
                    false
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
                true
            with exn ->
                logger.logCrit "Error while saving in Database: %O" exn
                System.Windows.Forms.MessageBox.Show(
                    "You triggered a critical bug in Yaaf.Wireplugin.\n"+
                    " Please let me know how you did it and report it on the website (rightclick -> Report Bug or Request Feature).\n"+
                    " Dont forget to send your logfiles (%LOCALAPPDATA%\Yaaf\WirePlugin\log\*.svclog)" + 
                    " Dont panik! Your matchmedia is either safe on his original place or in the database folder (or deleted if you clicked the delete matchmedia button)!" + 
                    " Error Message: " + exn.Message,
                    "Gratulation!!!", 
                    MessageBoxButtons.OK, 
                    MessageBoxIcon.Exclamation) 
                    |> ignore
                false

        if doCustomAction then
            // Execute automatic actions for this game
            for action in Database.getActivatedMatchFormActions db session.IsEslMatch game do
                try
                    logger.logInfo "Executing Action %s on game %s" action.ActionObject.Name game.Name
                    let data = executeAction db action.ActionObject matchSession
                    logger.logInfo "Action completed with object: %O" data
                with exn ->
                    logger.logErr "Action %s on game %s failed! Error %O" action.ActionObject.Name game.Name exn
                    System.Windows.Forms.MessageBox.Show(
                        "At least one of your automatic actions failed :(\n"+
                        " Error Message: " + exn.Message + "\n" +
                        " If you feel like this is a bug, report it: (rightclick -> Report Bug or Request Feature).\n"+
                        " Dont forget to send your logfiles (%LOCALAPPDATA%\Yaaf\WirePlugin\log\*.svclog)", 
                        "Ups!", 
                        MessageBoxButtons.OK, 
                        MessageBoxIcon.Warning) 
                        |> ignore
                
        
    let interop = 
        let dataBaseInterop = 
            { new IFSharpDatabase with
                member x.GetIdentityPlayer db = Database.getIdentityPlayer db.Context
                member x.GetPlayerByEslId (db, eslId) = 
                    match Database.tryGetPlayerByEslId db.Context eslId with
                    | Some s -> s
                    | None -> null
                member x.GetPlayerById (db, id) = 
                    match Database.tryGetPlayerById db.Context id with
                    | Some s -> s
                    | None -> null 
                member x.DeleteMatchSession (db, delFiles,session) = 
                    Database.removeSession db.Context delFiles session }
        { new IFSharpInterop with
            member x.GetMatchmediaPath media = 
                Database.mediaPath media
            member x.Database with get() = dataBaseInterop
            member x.GetNewContext () = Database.getContext()     
            member x.GetFetchPlayerTask link =
                let asyncTask = async { return! EslGrabber.getMatchMembers link } 
                Primitives.Task(asyncTask) :> Primitives.ITask<_> 
            member x.FillWrapperTable(players, wrapperTable, mediaTable) = 
                Database.fillWrapperTable players wrapperTable mediaTable }    

    do  logger.logVerb "Starting up Yaaf.WirePlugin (%s)" ProjectConstants.VersionString
    static do 
        Application.EnableVisualStyles()

        Settings.Default.SetProjectDefaults()        
        if Version(Settings.Default.SettingsVersion) < ProjectConstants.ProjectVersion then
            Settings.Default.Upgrade()
            Settings.Default.SettingsVersion <- ProjectConstants.VersionString
        
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
            logger.logVerb "Init Plugin" 

            FSharpInterop.Interop <- interop

            x.setIcon(Resources.bluedragon)
            gameInterface <- Some (InterfaceFactory.gameInterface())
        
            let task = DatabaseUpgrade.getUpgradeDatabaseTask logger
            match task with
            | Some (t, update) ->     
                let t = Primitives.Task<_>(t, update) :> Primitives.ITask<_>
                WaitingForm.StartTask(logger, t, "Upgrading database...")
                match t.ErrorObj with
                | Some error ->
                    MessageBox.Show(
                        "Failed to upgrade your database!\n"+
                        "Message: " + error.Message, 
                        "Error")
                        |> ignore
                    raise (InvalidOperationException("Unable to startup with possible corrupt database", error))
                | None -> ()
            | None -> ()

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

            let formatGameData (data:System.Collections.Generic.Dictionary<_,_>) =
                let stringJoin sep strings =
                    System.String.Join(sep, strings |> Seq.toArray)
                let s = 
                    data
                    |> Seq.map (fun k -> k.Key, k.Value)
                    |> Seq.map (fun (k,v) -> sprintf "\t[%s, %O]" k v)
                    |> stringJoin "; \n"
                sprintf "[ \n%s ]" s

            // Match started (before Game started and only with wire)
            x.GameInterface.add_MatchStarted
                (fun matchId matchMediaPath -> 
                    logEvent ("MatchStart" + string matchId) (fun logger ->
                        Settings.Default.MatchMediaPath <- matchMediaPath
                        Settings.Default.Save()
                        let data = x.GameInterface.matchInfo matchId
                        logger.logInfo 
                            "Starting Match with data: %s" (data |> formatGameData)
                        let gameId = data.["gameId"] :?> int
                        match getGameSession gameId with
                        | Some (s) -> 
                            failwith "a previous session was not closed properly!"
                        | None ->
                            let session = Some (sessionMatchStarted logger (Some matchId))
                            setGameSession gameId session))

            // Game started (with or without wire)
            x.GameInterface.add_GameStarted
                (fun gameId gamePath -> 
                    logEvent ("GameStart" + string gameId) (fun logger ->
                        let currS = 
                            match getGameSession gameId with
                            | Some (s) -> s // Use this session as it was created in MatchStart
                            | None -> sessionMatchStarted logger None
                        
                        sessionGameStarted logger currS gameId gamePath 
                        |> Some
                        |> setGameSession gameId))

            // Game stopped (before Match ended and always)
            // On real matches we have time until Anticheat has finished (parallel) for copying our matchmedia
            x.GameInterface.add_GameStopped
                (fun gameId -> 
                    logEvent ("GameEnd" + string gameId) (fun logger ->
                        match getGameSession gameId with
                        | Some (s) -> 
                            // Use this session as it was created in MatchStart
                            setGameSession gameId None
                            sessionEnd logger s
                        | None -> 
                            failwith "No session found on GameEnd!"))

            // Match ended (the Matchmedia dialog is already opened)
            // This event brings no additional information...
            x.GameInterface.add_MatchEnded
                (fun matchId -> 
                    logEvent ("MatchEnd" + string matchId) (fun logger ->
                        let data = x.GameInterface.matchInfo matchId
                        logger.logInfo 
                            "Ending match session with data: %s" (data |> formatGameData)
                             
                        let gameId = data.["gameId"] :?> int
                        match getGameSession gameId with
                        | Some (s) -> 
                            setGameSession gameId None
                            sessionEnd logger s
                            failwith "Session not already closed on MatchEnd!"
                        | None -> ()))
        with exn ->
            logger.logErr "Error: %O" exn

