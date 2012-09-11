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
    open Yaaf.WirePlugin.WinFormGui.Database.OldSchemas
    module Upgrade1_0_0_0to1_1_0_0 = 
        let fileAdd = "convertfrom1_0_0_0to1_1_0_0"
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

        let Upgrade update = async {
            let statusUpdate s = update (sprintf "%s: %s" fileAdd s)
            statusUpdate "initialization"
            // db can not be used actually, but this will work
            let db = Database.getContext().Context
            use old = new OldContext(db.Connection)

            // now db is actually a valid reference to our new database
            use db = 
                new NewContext
                    (fileAdd |> Database.dbFile |> Database.connectString)
            
            // Create the new database
            if db.DatabaseExists() then db.DeleteDatabase()
            db.CreateDatabase()

            let headFromQuery query = 
                Query.query query
                    |> Seq.head

            // Simple items (no depenencies), 1st Generation
            statusUpdate "tags"
            db.Tags.InsertAllOnSubmit
                (Query.query <@ seq { for a in old.Tags do yield a } @>
                    |> Seq.map (fun a -> NewTag(Name = a.Name)))

            let getTagId (oldTag:OldTag) = 
                headFromQuery
                    <@ seq { for b in db.Tags do
                                if b.Name = oldTag.Name then
                                    yield b.Id } @>

            statusUpdate "games"
            db.Games.InsertAllOnSubmit
                (Query.query <@ seq { for a in old.Games do yield a } @>
                    |> Seq.map 
                        (fun a -> 
                            NewGame(
                                Id = a.Id,
                                Name = a.Name, Shortname = a.Shortname, EnableNotification = a.EnableNotification,
                                EnableMatchForm = a.EnableMatchForm, EnablePublicNotification = a.EnablePublicNotification,
                                EnableWarMatchForm = a.EnableWarMatchForm, WarMatchFormSaveFiles = a.WarMatchFormSaveFiles,
                                PublicMatchFormSaveFiles = a.PublicMatchFormSaveFiles)))

            let getGameId (oldGame:OldGame) = oldGame.Id
            
            statusUpdate "actions"
            db.Actions.InsertAllOnSubmit
                (Query.query <@ seq { for a in old.Actions do yield a } @>
                    |> Seq.map (fun a -> NewAction(Name = a.Name, Parameters = a.Parameters)))
            
            
            let getActionId (oldAction:OldAction) = 
                headFromQuery
                    <@ seq { for b in db.Actions do
                                if b.Name = oldAction.Name && b.Parameters = oldAction.Parameters then
                                    yield b.Id } @>

            statusUpdate "players"
            db.Players.InsertAllOnSubmit
                (Query.query <@ seq { for a in old.Players do yield a } @>
                    |> Seq.map (fun a -> NewPlayer(Name = a.Name, EslPlayerId = a.EslPlayerId)))

            let getPlayerId (oldPlayer:OldPlayer)=  
                headFromQuery
                    <@ seq { for b in db.Players do
                                if (b.EslPlayerId = Nullable() && oldPlayer.EslPlayerId = Nullable() || b.EslPlayerId <> Nullable() && b.EslPlayerId = oldPlayer.EslPlayerId) 
                                    && b.Name = oldPlayer.Name then
                                    yield b.Id } @>

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
            db.WatchFolders.InsertAllOnSubmit
                (Query.query <@ seq { for a in old.WatchFolders do yield a } @>
                    |> Seq.map (fun a -> NewWatchFolder(GameId = getGameId a.Game, Folder = a.Folder,
                                            Filter = a.Filter, NotifyOnInactivity = a.NotifyOnInactivity)))
            
            let rec tryGetActionObjectId (oldActionObject:OldActionObject) = 
                if oldActionObject = null 
                then Nullable()
                else
                    let nextId = tryGetActionObjectId oldActionObject.NextActionObject
                    let actionId = getActionId oldActionObject.Action
                    headFromQuery
                        <@ seq { for b in db.ActionObjects do
                                    if b.Name = oldActionObject.Name 
                                    && b.NextActionObjectId = nextId
                                    && b.ActionId = actionId
                                    then
                                        yield Nullable(b.Id) } @>

            statusUpdate "actionobjects"
            db.ActionObjects.InsertAllOnSubmit
                (Query.query <@ seq { for a in old.ActionObjects do yield a } @>
                    |> Seq.map 
                        (fun a -> 
                            let action = 
                                Query.query 
                                    <@ seq { for b in db.Actions do
                                                if b.Name = a.Action.Name then
                                                    yield b } @>
                                    |> Seq.head
                            
                            NewActionObject(Action = action, NextActionObjectId = tryGetActionObjectId a.NextActionObject,
                                            Name = a.Name)))
            
            
            statusUpdate "matchsessions"
            db.MatchSessions.InsertAllOnSubmit
                (Query.query <@ seq { for a in old.MatchSessions do yield a } @>
                    |> Seq.map 
                        (fun a -> 
                            NewMatchSession(GameId = getGameId a.Game, Startdate = a.Startdate,
                                            Duration = a.Duration, EslMatchLink = a.EslMatchLink)))

            let getMatchSessionId (oldMatchSession:OldMatchSession) = 
                headFromQuery
                    <@ seq { for b in db.MatchSessions do
                                if (b.EslMatchLink = null && oldMatchSession.EslMatchLink = null || b.EslMatchLink <> null && b.EslMatchLink = oldMatchSession.EslMatchLink) 
                                    && b.GameId = oldMatchSession.GameId
                                    && b.Startdate = oldMatchSession.Startdate
                                    && b.Duration = oldMatchSession.Duration
                                then
                                    yield b.Id } @>

            statusUpdate "player tags"
            db.Player_Tags.InsertAllOnSubmit
                (Query.query <@ seq { for a in old.Player_Tags do yield a } @>
                    |> Seq.map 
                        (fun a -> 
                            let playerId = getPlayerId a.Player                                
                            let tagId = getTagId a.Tag
                            NewPlayer_Tag(PlayerId = playerId, TagId = tagId)))
            
            
            statusUpdate "submit 2nd gen"
            db.SubmitChanges()

            // 3rd Generation
            
            statusUpdate "matchsession tags"
            db.MatchSessions_Tags.InsertAllOnSubmit
                (Query.query <@ seq { for a in old.MatchSessions_Tags do yield a } @>
                    |> Seq.map 
                        (fun a -> 
                            let matchSessionId = getMatchSessionId a.MatchSession
                            let tagId = getTagId a.Tag
                            NewMatchSessions_Tag(MatchSessionId = matchSessionId, TagId = tagId)))
                 
            let nick, eslId = Database.getIdentityPlayerInfo()           
            let identityPlayer = getPlayerByEslId db eslId nick
            
            statusUpdate "matchmedias"
            db.Matchmedias.InsertAllOnSubmit
                (Query.query <@ seq { for a in old.Matchmedias do yield a } @>
                    |> Seq.map 
                        (fun a -> 
                            let matchSessionId = getMatchSessionId a.MatchSession
                            
                            NewMatchmedia(MatchSessionId = matchSessionId, Name = a.Name, Type = a.Type,
                                          Map = a.Map, Path = a.Path, Created = a.Created, PlayerId = identityPlayer.Id)))
            
            let getMatchmediaId (oldMatchMedia:OldMatchmedia) = 
                let matchSessionId = getMatchSessionId oldMatchMedia.MatchSession
                headFromQuery
                    <@ seq { for b in db.Matchmedias do
                                if b.Created = oldMatchMedia.Created 
                                    && b.Map = oldMatchMedia.Map
                                    && b.MatchSessionId = matchSessionId
                                    && b.Name = oldMatchMedia.Name
                                    && b.Type = oldMatchMedia.Type
                                    && b.Path = oldMatchMedia.Path
                                    && b.Created = oldMatchMedia.Created
                                    && b.PlayerId = identityPlayer.Id
                                then
                                    yield b.Id } @>
                                    
            statusUpdate "matchsession players"
            db.MatchSessions_Players.InsertAllOnSubmit
                (Query.query <@ seq { for a in old.MatchSessions_Players do yield a } @>
                    |> Seq.map 
                        (fun a -> 
                            let playerId = getPlayerId a.Player
                            let matchSessionId = getMatchSessionId a.MatchSession
                            NewMatchSessions_Player(
                                PlayerId = playerId, MatchSessionId = matchSessionId, Team = a.Team,
                                Skill = a.Skill, Description = a.Description, Cheating = a.Cheating)))

            statusUpdate "actionobject parameters"
            db.ObjectParameters.InsertAllOnSubmit
                (Query.query <@ seq { for a in old.ObjectParameters do yield a } @>
                    |> Seq.map 
                        (fun a -> 
                            let objectId = (tryGetActionObjectId a.ActionObject).Value
                            NewObjectParameter(
                                ObjectId = objectId, ParamNum = a.ParamNum, Parameter = a.Parameter)))
            
            statusUpdate "matchformactions"
            db.MatchFormActions.InsertAllOnSubmit
                (Query.query <@ seq { for a in old.MatchFormActions do yield a } @>
                    |> Seq.map 
                        (fun a -> 
                            let actionObjectId = (tryGetActionObjectId a.ActionObject).Value
                            let gameId = getGameId a.Game
                            NewMatchFormAction(
                                ActionObjectId = actionObjectId, GameId = gameId, WarActivated = a.WarActivated,
                                PublicActivated = a.PublicActivated)))
            
            
            statusUpdate "submit 3rd gen"
            db.SubmitChanges()
            
            
            statusUpdate "matchmedia tags"
            db.Matchmedia_Tags.InsertAllOnSubmit
                (Query.query <@ seq { for a in old.Matchmedia_Tags do yield a } @>
                    |> Seq.map 
                        (fun a -> 
                            let matchmediaId = getMatchmediaId a.Matchmedia
                            let tagId = getTagId a.Tag
                            NewMatchmedia_Tag(MatchmediaId = matchmediaId, TagId = tagId)))

            
            statusUpdate "submit 4th gen"
            db.SubmitChanges()

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
            |> Some
