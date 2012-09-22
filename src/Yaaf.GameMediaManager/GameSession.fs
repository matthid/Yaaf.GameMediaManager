// ----------------------------------------------------------------------------
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.
// ----------------------------------------------------------------------------
namespace Yaaf.GameMediaManager

open Wire
open System
open System.Windows.Forms
open System.IO
open System.Drawing
open System.Diagnostics
open Yaaf.Logging
open Yaaf.GameMediaManager.WinFormGui
open Yaaf.GameMediaManager.Primitives
open Yaaf.GameMediaManager.WinFormGui.Properties
open Yaaf.GameMediaManager.Grabbing

/// Data of Esl Matches
type EslMatchData = {
    EslId : int
    MatchUrl : string }

/// Data from the current Game
type GameData = {
        Game : Database.Game
        Watcher : MatchmediaWatcher
        DefaultPlayer : Database.MatchSessions_Player
        MatchSession : Database.MatchSession
    }

/// Data from the current Session
type Session = {
        Grabber : Grabber
        GameData : GameData option
        StartTime : DateTime
        EslMatch : EslMatchData option
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

[<AutoOpen>]
module SessionOperations = 
    let matchStarted logger matchId =
        let localContext = Database.getContext()
        {
            Grabber = new Grabber(logger)
            Context = localContext
            GameData = None
            StartTime = DateTime.Now
            EslMatch = matchId }

    let gameStarted (logger:ITracer) gameId gamePath (session:Session) =
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
                    | Some (warData) ->
                        // Check if this EslMatch already exisits and use the old session
                        let eslMatchLink = warData.MatchUrl
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
                                MyMatchSession = matchSession,
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
                    session.Grabber.StartGrabAction session.Context logger matchSession matchSession.EslMatchLink

                Some { Watcher = w; Game = game; MatchSession = matchSession; DefaultPlayer = playerAssoc }
            | None -> None

        { session with GameData = gameData }

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
            let finishTask = session.Grabber.WaitForFinish() 
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
                        if File.Exists m.Path then
                            File.Move (m.Path, dbMediaPath)
                            m.Path <- dbMediaPath
                        else 
                            logger.logErr "Could not find Matchmedia (%s)!" m.Path 
                    with :? IOException as e ->
                        logger.logErr "Could not move Matchmedia from %s to Database! (Error: %O)" m.Path e   
        
                logger.logInfo "Change to MediaPath in Database"  
                session.Context.MySubmitChanges() 
                true
            with exn ->
                logger.logCrit "Error while saving in Database: %O" exn
                //session.Context.DumpData(logger)
                System.Windows.Forms.MessageBox.Show(
                    "You triggered a critical bug in Yaaf.GameMediaManager.\n"+
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
            let executeAction (context:Database.LocalDataContext) (actionObj:Database.ActionObject) (session:Database.MatchSession) = 
                let action = Database.getAction context actionObj
                try
                    action (session:>obj)
                finally
                    context.SubmitChanges()
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
    type Session with
        static member MatchStarted logger matchId = matchStarted logger matchId
        member x.GameStarted (logger:ITracer) gameId gamePath = x |> gameStarted logger gameId gamePath
        member x.SessionEnd (logger:ITracer) = x |> sessionEnd logger

