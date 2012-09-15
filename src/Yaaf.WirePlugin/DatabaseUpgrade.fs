// ----------------------------------------------------------------------------
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.
// ----------------------------------------------------------------------------
namespace Yaaf.WirePlugin
module DatabaseUpgrade = 
    open System
    
    open Yaaf.WirePlugin.WinFormGui
    type UpgradeTask = Async<unit> * IEvent<string>
    let empty() = 
        async { 
            return () }, (new Event<string>()).Publish
    let attach task2 task1 = 
        let asy1, e1 = task1
        let asy2, e2 = task2            
        async {
            do! asy1
            do! asy2
            return () }, Event.merge e1 e2
     

    open Microsoft.FSharp.Linq
    let copyTableOnSubmit (newTable:Data.Linq.Table<_>) (oldTable:Data.Linq.Table<_>) creator = 
        let dict = new System.Collections.Generic.Dictionary<_,_>()
        let newData = 
            Query.query <@ seq { for a in oldTable do yield a } @>
                |> Seq.map 
                    (fun a -> 
                        let newItem = creator a
                        match newItem with
                        | Some t ->
                            dict.Add(a, t)
                        | None -> ()
                        newItem)
                |> Seq.filter (fun a -> match a with | None -> false | _ -> true)
                |> Seq.map (fun a -> match a with Some t -> t | _ -> failwith "should always be some")
                |> Seq.toList
        newTable.InsertAllOnSubmit(newData)
        (fun old ->
            dict.Item old)  

    open Yaaf.WirePlugin.WinFormGui.Database.OldSchemas
    open System.IO
    module Upgrade1_0_0_0to1_1_0_0 = 
        let OldVersion = "1.0.0.0"
        let NewVersion = "1.1.0.0"

        type OldContext                 = v1_0_0_0.LocalDataContext
        type OldActionObject            = v1_0_0_0.ActionObject
        type OldAction                  = v1_0_0_0.Actions
        type OldGame                    = v1_0_0_0.Game
        type OldMatchFormAction         = v1_0_0_0.MatchFormAction
        type OldMatchSession            = v1_0_0_0.MatchSession
        type OldMatchSessions_Player    = v1_0_0_0.MatchSessions_Player
        type OldMatchSessions_Tag       = v1_0_0_0.MatchSessions_Tag
        type OldMatchmedia              = v1_0_0_0.Matchmedia
        type OldMatchmedia_Tag          = v1_0_0_0.Matchmedia_Tag
        type OldObjectParameter         = v1_0_0_0.ObjectParameter
        type OldPlayer                  = v1_0_0_0.Player
        type OldPlayer_Tag              = v1_0_0_0.Player_Tag
        type OldTag                     = v1_0_0_0.Tag
        type OldWatchFolder             = v1_0_0_0.WatchFolder
        
        type NewContext                 = v1_1_0_0.LocalDataContext
        type NewActionObject            = v1_1_0_0.ActionObject
        type NewAction                  = v1_1_0_0.Action
        type NewGame                    = v1_1_0_0.Game
        type NewMatchFormAction         = v1_1_0_0.MatchFormAction
        type NewMatchSession            = v1_1_0_0.MatchSession
        type NewMatchSessions_Player    = v1_1_0_0.MatchSessions_Player
        type NewMatchSessions_Tag       = v1_1_0_0.MatchSessions_Tag
        type NewMatchmedia              = v1_1_0_0.Matchmedia
        type NewMatchmedia_Tag          = v1_1_0_0.Matchmedia_Tag
        type NewObjectParameter         = v1_1_0_0.ObjectParameter
        type NewPlayer                  = v1_1_0_0.Player
        type NewPlayer_Tag              = v1_1_0_0.Player_Tag
        type NewTag                     = v1_1_0_0.Tag
        type NewWatchFolder             = v1_1_0_0.WatchFolder
        
        let Old_Version = OldVersion.Replace(".","_")
        let New_Version = NewVersion.Replace(".","_")
        let fileAdd = sprintf "convertfrom%sto%s" Old_Version New_Version
        let message = sprintf "%s -> %s" Old_Version New_Version
        let backup = sprintf "%sbackup" Old_Version

        let Upgrade update = async {
            let statusUpdate s = update (sprintf "%s: %s" message s)
            statusUpdate "initialization"
            // db can not be used actually, but this will work
            let db = Database.getContext().Context
            use old = new OldContext(db.Connection)
            
            // now db is actually a valid reference to our new database
            let upgradeFile = fileAdd |> Database.dbFile
            let databaseFile = "" |> Database.dbFile
            let backupFile = backup |> Database.dbFile
            use db = 
                new NewContext
                    (upgradeFile |> Database.connectString)
            
            // Create the new database
            if db.DatabaseExists() then db.DeleteDatabase()
            db.CreateDatabase()
            
            // Simple items (no depenencies), 1st Generation
            statusUpdate "tags"
            let getTag = 
                copyTableOnSubmit db.Tags old.Tags (fun a -> NewTag(Name = a.Name)|> Some)
            let getTagId (oldTag:OldTag) = (getTag oldTag).Id

            statusUpdate "games"
            let getGame =
                copyTableOnSubmit db.Games old.Games 
                    (fun a -> 
                        NewGame(
                            Id = a.Id,
                            Name = a.Name, Shortname = a.Shortname, EnableNotification = a.EnableNotification,
                            EnableMatchForm = a.EnableMatchForm, EnablePublicNotification = a.EnablePublicNotification,
                            EnableWarMatchForm = a.EnableWarMatchForm, WarMatchFormSaveFiles = a.WarMatchFormSaveFiles,
                            PublicMatchFormSaveFiles = a.PublicMatchFormSaveFiles)
                        |> Some)

            let getGameId (oldGame:OldGame) = oldGame.Id
            
            statusUpdate "actions"
            let getAction =
                copyTableOnSubmit db.Actions old.Actions (fun a -> NewAction(Name = a.Name, Parameters = a.Parameters)|>Some)
            let getActionId (oldAction:OldAction) = (getAction oldAction).Id

            statusUpdate "players"
            let getPlayer =
                copyTableOnSubmit db.Players old.Players
                    (fun a -> 
                        NewPlayer(Name = a.Name, EslPlayerId = a.EslPlayerId)
                        |> Some)
            let getPlayerId (oldPlayer:OldPlayer)=  
                let preventCheckPlayerId = oldPlayer.EslPlayerId = Nullable()
                (getPlayer oldPlayer).Id

            let getPlayerByEslId (db:NewContext) id nick = 
                let mid = System.Nullable(id)
                let myQuery = 
                    <@ seq {
                        for m in db.Players do
                            if m.EslPlayerId = mid then
                                yield m } @>
                Database.getOrCreateItem myQuery (fun _ ->
                    use dbCopy = new NewContext(db.Connection)
                    let newIdentity = 
                        new NewPlayer(
                            EslPlayerId = System.Nullable(id),
                            Name = nick)
                    dbCopy.Players.InsertOnSubmit(newIdentity)
                    dbCopy.SubmitChanges()
                    Database.getSingle myQuery)

            statusUpdate "submit 1st gen"
            db.SubmitChanges()

            // 2nd Generation
            statusUpdate "watchfolders"
            let getWatchFolder =
                copyTableOnSubmit db.WatchFolders old.WatchFolders 
                    (fun a -> 
                        NewWatchFolder(
                            GameId = getGameId a.Game, Folder = a.Folder,
                            Filter = a.Filter, NotifyOnInactivity = a.NotifyOnInactivity)
                        |> Some)
            
            let rec tryGetActionObjectId (oldActionObject:OldActionObject) = 
                if oldActionObject = null 
                then Nullable()
                else
                    let nextId = tryGetActionObjectId oldActionObject.NextActionObject
                    let actionId = getActionId oldActionObject.Action
                    Database.getSingle
                        <@ seq { for b in db.ActionObjects do
                                    if b.Name = oldActionObject.Name 
                                    && b.NextActionObjectId = nextId
                                    && b.ActionId = actionId
                                    then
                                        yield Nullable(b.Id) } @>

            statusUpdate "actionobjects"
            let getActionObject = 
                copyTableOnSubmit db.ActionObjects old.ActionObjects
                    (fun a ->                             
                        NewActionObject(Action = getAction a.Action, NextActionObjectId = tryGetActionObjectId a.NextActionObject,
                                        Name = a.Name)
                        |> Some)
            let getAcionObjectId (old:OldActionObject) = 
                (getActionObject old).Id
            
            statusUpdate "matchsessions"
            let getMatchSession =
                copyTableOnSubmit db.MatchSessions old.MatchSessions 
                    (fun a -> 
                        NewMatchSession(GameId = getGameId a.Game, Startdate = a.Startdate,
                                        Duration = a.Duration, EslMatchLink = a.EslMatchLink)
                        |> Some)
            let getMatchSessionId (old:OldMatchSession) = (getMatchSession old).Id

            statusUpdate "player tags"
            let getPlayerTag = 
                copyTableOnSubmit db.Player_Tags old.Player_Tags 
                    (fun a -> 
                        let playerId = getPlayerId a.Player                                
                        let tagId = getTagId a.Tag
                        NewPlayer_Tag(PlayerId = playerId, TagId = tagId)
                        |> Some)
            
            
            statusUpdate "submit 2nd gen"
            db.SubmitChanges()

            // 3rd Generation
            
            statusUpdate "matchsession tags"
            let getMatchSessionTag = 
                copyTableOnSubmit db.MatchSessions_Tags old.MatchSessions_Tags 
                    (fun a -> 
                        let matchSessionId = getMatchSessionId a.MatchSession
                        let tagId = getTagId a.Tag
                        NewMatchSessions_Tag(MatchSessionId = matchSessionId, TagId = tagId)
                        |> Some)
                 
            let nick, eslId = Database.getIdentityPlayerInfo()           
            let identityPlayer = getPlayerByEslId db eslId nick
            
            
                                    
            statusUpdate "matchsession players"
            let getMatchSessionPlayer = 
                copyTableOnSubmit db.MatchSessions_Players old.MatchSessions_Players
                    (fun a -> 
                        if (a.Player = null || a.MatchSession = null ) then None
                        else
                            let playerId = getPlayerId a.Player
                            let matchSessionId = getMatchSessionId a.MatchSession
                            NewMatchSessions_Player(
                                PlayerId = playerId, MatchSessionId = matchSessionId, Team = a.Team,
                                Skill = a.Skill, Description = a.Description, Cheating = a.Cheating)
                                |> Some)
            
            statusUpdate "submit matchsession players"
            db.SubmitChanges()

            statusUpdate "matchmedias"
            let getMatchmedia = 
                copyTableOnSubmit db.Matchmedias old.Matchmedias
                    (fun a -> 
                        let matchSession = getMatchSession a.MatchSession
                        let attachedPlayers = 
                            Query.query 
                                <@ seq { for n in db.MatchSessions_Players do
                                             if n.MatchSessionId = matchSession.Id && n.PlayerId = identityPlayer.Id then
                                                 yield n } @>
                        let loaded = attachedPlayers |> Seq.toArray
                        if attachedPlayers
                            |> Seq.isEmpty then
                            try
                                db.MatchSessions_Players.InsertOnSubmit(
                                    NewMatchSessions_Player(
                                        PlayerId = identityPlayer.Id, MatchSessionId = matchSession.Id, Team = 11uy,
                                        Skill = Nullable(50uy), Description = "", Cheating = false))
                                db.SubmitChanges()
                            with exn -> 
                                ()

                        NewMatchmedia(MatchSessionId = matchSession.Id, Name = a.Name, Type = a.Type,
                                        Map = a.Map, Path = a.Path, Created = a.Created, PlayerId = identityPlayer.Id)
                        |> Some)
            
            let getMatchmediaId (oldMatchMedia:OldMatchmedia) = (getMatchmedia oldMatchMedia).Id

            statusUpdate "actionobject parameters"
            let getObjectParamter = 
                copyTableOnSubmit db.ObjectParameters old.ObjectParameters 
                    (fun a -> 
                        let objectId = (tryGetActionObjectId a.ActionObject).Value
                        NewObjectParameter(
                            ObjectId = objectId, ParamNum = a.ParamNum, Parameter = a.Parameter)
                        |>Some)
            
            statusUpdate "matchformactions"
            let getMatchFormAction = 
                copyTableOnSubmit db.MatchFormActions old.MatchFormActions
                    (fun a -> 
                        let actionObjectId = (tryGetActionObjectId a.ActionObject).Value
                        let gameId = getGameId a.Game
                        NewMatchFormAction(
                            ActionObjectId = actionObjectId, GameId = gameId, WarActivated = a.WarActivated,
                            PublicActivated = a.PublicActivated)
                            |> Some)
            
            
            statusUpdate "submit 3rd gen"
            db.SubmitChanges()
            
            
            statusUpdate "matchmedia tags"
            let getMatchmediaTag = 
                copyTableOnSubmit db.Matchmedia_Tags old.Matchmedia_Tags 
                    (fun a -> 
                        let matchmediaId = getMatchmediaId a.Matchmedia
                        let tagId = getTagId a.Tag
                        NewMatchmedia_Tag(MatchmediaId = matchmediaId, TagId = tagId)
                        |> Some)

            
            statusUpdate "submit 4th gen"
            db.SubmitChanges()
            db.Connection.Close()

            statusUpdate "finishing"
            if File.Exists backupFile then File.Delete backupFile
            File.Move(databaseFile, backupFile)
            File.Move(upgradeFile, databaseFile)
            Properties.Settings.Default.DatabaseSchemaVersion <- NewVersion
            Properties.Settings.Default.Save()
            }

    module Upgrade1_1_0_0to1_1_1_0 = 
        let OldVersion = "1.1.0.0"
        let NewVersion = "1.1.1.0"

        type OldContext                 = v1_1_0_0.LocalDataContext
        type OldActionObject            = v1_1_0_0.ActionObject
        type OldAction                  = v1_1_0_0.Action
        type OldGame                    = v1_1_0_0.Game
        type OldMatchFormAction         = v1_1_0_0.MatchFormAction
        type OldMatchSession            = v1_1_0_0.MatchSession
        type OldMatchSessions_Player    = v1_1_0_0.MatchSessions_Player
        type OldMatchSessions_Tag       = v1_1_0_0.MatchSessions_Tag
        type OldMatchmedia              = v1_1_0_0.Matchmedia
        type OldMatchmedia_Tag          = v1_1_0_0.Matchmedia_Tag
        type OldObjectParameter         = v1_1_0_0.ObjectParameter
        type OldPlayer                  = v1_1_0_0.Player
        type OldPlayer_Tag              = v1_1_0_0.Player_Tag
        type OldTag                     = v1_1_0_0.Tag
        type OldWatchFolder             = v1_1_0_0.WatchFolder
        
        type NewContext                 = v1_1_1_0.LocalDataContext
        type NewActionObject            = v1_1_1_0.ActionObject
        type NewAction                  = v1_1_1_0.Action
        type NewGame                    = v1_1_1_0.Game
        type NewMatchFormAction         = v1_1_1_0.MatchFormAction
        type NewMatchSession            = v1_1_1_0.MatchSession
        type NewMatchSessions_Player    = v1_1_1_0.MatchSessions_Player
        type NewMatchSessions_Tag       = v1_1_1_0.MatchSessions_Tag
        type NewMatchmedia              = v1_1_1_0.Matchmedia
        type NewMatchmedia_Tag          = v1_1_1_0.Matchmedia_Tag
        type NewObjectParameter         = v1_1_1_0.ObjectParameter
        type NewPlayer                  = v1_1_1_0.Player
        type NewPlayer_Tag              = v1_1_1_0.Player_Tag
        type NewTag                     = v1_1_1_0.Tag
        type NewWatchFolder             = v1_1_1_0.WatchFolder
        
        let Old_Version = OldVersion.Replace(".","_")
        let New_Version = NewVersion.Replace(".","_")
        let fileAdd = sprintf "convertfrom%sto%s" Old_Version New_Version
        let message = sprintf "%s -> %s" Old_Version New_Version
        let backup = sprintf "%sbackup" Old_Version

        let Upgrade update = async {
            let statusUpdate s = update (sprintf "%s: %s" message s)
            statusUpdate "initialization"
            // db can not be used actually, but this will work
            let db = Database.getContext().Context
            use old = new OldContext(db.Connection)
            
            // now db is actually a valid reference to our new database
            let upgradeFile = fileAdd |> Database.dbFile
            let databaseFile = "" |> Database.dbFile
            let backupFile = backup |> Database.dbFile
            use db = 
                new NewContext
                    (upgradeFile |> Database.connectString)
            
            // Create the new database
            if db.DatabaseExists() then db.DeleteDatabase()
            db.CreateDatabase()
            
            // Simple items (no depenencies), 1st Generation
            statusUpdate "tags"
            let getTag = 
                copyTableOnSubmit db.Tags old.Tags (fun a -> NewTag(Name = a.Name)|> Some)
            let getTagId (oldTag:OldTag) = (getTag oldTag).Id

            statusUpdate "games"
            let getGame =
                copyTableOnSubmit db.Games old.Games 
                    (fun a -> 
                        NewGame(
                            Id = a.Id,
                            Name = a.Name, Shortname = a.Shortname, EnableNotification = a.EnableNotification,
                            EnableMatchForm = a.EnableMatchForm, EnablePublicNotification = a.EnablePublicNotification,
                            EnableWarMatchForm = a.EnableWarMatchForm, WarMatchFormSaveFiles = a.WarMatchFormSaveFiles,
                            PublicMatchFormSaveFiles = a.PublicMatchFormSaveFiles)
                        |> Some)

            let getGameId (oldGame:OldGame) = oldGame.Id
            
            statusUpdate "actions"
            let getAction =
                copyTableOnSubmit db.Actions old.Actions (fun a -> NewAction(Name = a.Name, Parameters = a.Parameters)|>Some)
            let getActionId (oldAction:OldAction) = (getAction oldAction).Id

            statusUpdate "players"
            let getPlayer =
                copyTableOnSubmit db.Players old.Players
                    (fun a -> 
                        NewPlayer(Name = a.Name, EslPlayerId = a.EslPlayerId)
                        |> Some)
            let getPlayerId (oldPlayer:OldPlayer)=  
                let preventCheckPlayerId = oldPlayer.EslPlayerId = Nullable()
                (getPlayer oldPlayer).Id

            statusUpdate "submit 1st gen"
            db.SubmitChanges()

            // 2nd Generation
            statusUpdate "watchfolders"
            let getWatchFolder =
                copyTableOnSubmit db.WatchFolders old.WatchFolders 
                    (fun a -> 
                        NewWatchFolder(
                            GameId = getGameId a.Game, Folder = a.Folder,
                            Filter = a.Filter, NotifyOnInactivity = a.NotifyOnInactivity)
                        |> Some)
            
            let rec tryGetActionObjectId (oldActionObject:OldActionObject) = 
                if oldActionObject = null 
                then Nullable()
                else
                    let nextId = tryGetActionObjectId oldActionObject.NextActionObject
                    let actionId = getActionId oldActionObject.Action
                    Database.getSingle
                        <@ seq { for b in db.ActionObjects do
                                    if b.Name = oldActionObject.Name 
                                    && b.NextActionObjectId = nextId
                                    && b.ActionId = actionId
                                    then
                                        yield Nullable(b.Id) } @>

            statusUpdate "actionobjects"
            let getActionObject = 
                copyTableOnSubmit db.ActionObjects old.ActionObjects
                    (fun a ->                             
                        NewActionObject(Action = getAction a.Action, NextActionObjectId = tryGetActionObjectId a.NextActionObject,
                                        Name = a.Name)
                        |> Some)
            let getAcionObjectId (old:OldActionObject) = 
                (getActionObject old).Id
            
            statusUpdate "matchsessions"
            let getMatchSession =
                copyTableOnSubmit db.MatchSessions old.MatchSessions 
                    (fun a -> 
                        NewMatchSession(GameId = getGameId a.Game, Startdate = a.Startdate,
                                        Duration = a.Duration, EslMatchLink = a.EslMatchLink)
                        |> Some)
            let getMatchSessionId (old:OldMatchSession) = (getMatchSession old).Id

            statusUpdate "player tags"
            let getPlayerTag = 
                copyTableOnSubmit db.Player_Tags old.Player_Tags 
                    (fun a -> 
                        let playerId = getPlayerId a.Player                                
                        let tagId = getTagId a.Tag
                        NewPlayer_Tag(PlayerId = playerId, TagId = tagId)
                        |> Some)
            
            
            statusUpdate "submit 2nd gen"
            db.SubmitChanges()

            // 3rd Generation
            
            statusUpdate "matchsession tags"
            let getMatchSessionTag = 
                copyTableOnSubmit db.MatchSessions_Tags old.MatchSessions_Tags 
                    (fun a -> 
                        let matchSessionId = getMatchSessionId a.MatchSession
                        let tagId = getTagId a.Tag
                        NewMatchSessions_Tag(MatchSessionId = matchSessionId, TagId = tagId)
                        |> Some)
                                    
            statusUpdate "matchsession players"
            let getMatchSessionPlayer = 
                copyTableOnSubmit db.MatchSessions_Players old.MatchSessions_Players
                    (fun a -> 
                        if (a.Player = null || a.MatchSession = null ) then None
                        else
                            let playerId = getPlayerId a.Player
                            let matchSessionId = getMatchSessionId a.MatchSession
                            NewMatchSessions_Player(
                                PlayerId = playerId, MatchSessionId = matchSessionId, Team = a.Team,
                                Skill = a.Skill, Description = a.Description, Cheating = a.Cheating)
                                |> Some)
            
            statusUpdate "submit matchsession players"
            db.SubmitChanges()

            statusUpdate "matchmedias"
            let getMatchmedia = 
                copyTableOnSubmit db.Matchmedias old.Matchmedias
                    (fun a -> 
                        let matchSession = getMatchSession a.MatchSession
                        let player = getPlayer a.Player
                        let attachedPlayers = 
                            Query.query 
                                <@ seq { for n in db.MatchSessions_Players do
                                             if n.MatchSessionId = matchSession.Id && n.PlayerId = player.Id then
                                                 yield n } @>
                        let loaded = attachedPlayers |> Seq.toArray
                        if attachedPlayers
                            |> Seq.isEmpty then
                            try
                                db.MatchSessions_Players.InsertOnSubmit(
                                    NewMatchSessions_Player(
                                        PlayerId = player.Id, MatchSessionId = matchSession.Id, Team = 11uy,
                                        Skill = Nullable(50uy), Description = "", Cheating = false))
                                db.SubmitChanges()
                            with exn -> 
                                ()

                        NewMatchmedia(MatchSessionId = matchSession.Id, Name = a.Name, Type = a.Type,
                                        Map = a.Map, Path = a.Path, Created = a.Created, PlayerId = player.Id)
                        |> Some)
            
            let getMatchmediaId (oldMatchMedia:OldMatchmedia) = (getMatchmedia oldMatchMedia).Id

            statusUpdate "actionobject parameters"
            let getObjectParamter = 
                copyTableOnSubmit db.ObjectParameters old.ObjectParameters 
                    (fun a -> 
                        let objectId = (tryGetActionObjectId a.ActionObject).Value
                        NewObjectParameter(
                            ObjectId = objectId, ParamNum = a.ParamNum, Parameter = a.Parameter)
                        |>Some)
            
            statusUpdate "matchformactions"
            let getMatchFormAction = 
                copyTableOnSubmit db.MatchFormActions old.MatchFormActions
                    (fun a -> 
                        let actionObjectId = (tryGetActionObjectId a.ActionObject).Value
                        let gameId = getGameId a.Game
                        NewMatchFormAction(
                            ActionObjectId = actionObjectId, GameId = gameId, WarActivated = a.WarActivated,
                            PublicActivated = a.PublicActivated)
                            |> Some)
            
            
            statusUpdate "submit 3rd gen"
            db.SubmitChanges()
            
            
            statusUpdate "matchmedia tags"
            let getMatchmediaTag = 
                copyTableOnSubmit db.Matchmedia_Tags old.Matchmedia_Tags 
                    (fun a -> 
                        let matchmediaId = getMatchmediaId a.Matchmedia
                        let tagId = getTagId a.Tag
                        NewMatchmedia_Tag(MatchmediaId = matchmediaId, TagId = tagId)
                        |> Some)

            
            statusUpdate "submit 4th gen"
            db.SubmitChanges()
            db.Connection.Close()

            statusUpdate "finishing"
            if File.Exists backupFile then File.Delete backupFile
            File.Move(databaseFile, backupFile)
            File.Move(upgradeFile, databaseFile)
            Properties.Settings.Default.DatabaseSchemaVersion <- NewVersion
            Properties.Settings.Default.Save()
            }

    open Yaaf.Logging
    let fromUpgradeFunc (logger:ITracer) upgradeFun = 
        let e = new Event<_>()
        let updateFun s = 
            e.Trigger s
        upgradeFun updateFun, e.Publish

    /// Tries to return an upgrade Task or None if no upgrade is required
    let getUpgradeDatabaseTask logger = 
        let currentSchemaVersion = Version(Properties.Settings.Default.DatabaseSchemaVersion)
        
        let newSchemaVersion = ProjectConstants.DatabaseSchemaVersion

        if currentSchemaVersion < Version(1,0,0,0) then
            failwith 
                (sprintf "Upgrade from this Version (%O) is not supported, try upgrading with an older version first!" currentSchemaVersion)
        
        if currentSchemaVersion > newSchemaVersion then
            failwith 
                (sprintf 
                    "The current Schmema-Version (%O) is newer than the required one (%O). Are you trying to downgrade your version? Downgrading is not supported!"
                    currentSchemaVersion
                    newSchemaVersion)
        if (currentSchemaVersion = newSchemaVersion) then None
        else
        empty()
            |> attach
                (if (currentSchemaVersion = Version(1,0,0,0)) then
                    fromUpgradeFunc logger Upgrade1_0_0_0to1_1_0_0.Upgrade 
                 else empty())
            |> attach 
                (if currentSchemaVersion <= Version(1,1,0,0) then
                    fromUpgradeFunc logger Upgrade1_1_0_0to1_1_1_0.Upgrade
                 else empty())
            |> Some
