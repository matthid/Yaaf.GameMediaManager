// ----------------------------------------------------------------------------
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.
// ----------------------------------------------------------------------------
namespace Yaaf.WirePlugin

module Database = 
    open System.IO
    open Microsoft.FSharp.Linq
    open Yaaf.WirePlugin.Primitives
    open Yaaf.WirePlugin.WinFormGui
    type Context = Database.LocalDataContext
    let pluginFolder =
            [ System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData);
                "Yaaf"; "WirePlugin" ] |> pathCombine
    
    let dbFile addendum =
        let dbFileName = sprintf "LocalDatabase%s.sdf" (if addendum |> System.String.IsNullOrEmpty then "" else sprintf "_%s" addendum)
        [ pluginFolder; dbFileName ] |> pathCombine
    let connectString dbFile = 
        sprintf "Data Source=%s" dbFile
    let private initDatabase (db:Database.LocalDataContext) = 
        let initActions = 
            [ "CopyToEslMatchmedia", 0      
              "TypeFilter", 1               
              "RegexPathFilter", 1          
              "RegexNameFilter", 1          
              "MapNameFilter", 1            
              "TagFilter", 1                
              "CopyMedia", 1                
              "DeleteMedia", 0              
              "DeleteSession", 0            
              "ExtractMatchmedia", 0        
              "ExternalToolEachMedia", 1    
              "CopyToTempFolder",0          
              "ExternalTool",1 ]            
            |> Seq.map (fun (name, parameters) ->
                Database.Action(Name=name,Parameters=byte parameters))
        db.Actions.InsertAllOnSubmit initActions
        db.SubmitChanges()

    let private db = 
        let dbFile = (dbFile "")
        let db = new Database.LocalDataContext(connectString dbFile)
        if not <| db.DatabaseExists() then
            db.CreateDatabase()
            initDatabase db
        db
         
        
    let private wrapper = 
        new LocalDatabaseWrapper(db)

    let getContext () = wrapper.Copy()

    let tryGetSingle s = 
        let result = 
            s |> Query.query |> Seq.tryTake 1
        if result |> Seq.isEmpty then
            None
        else Some (result |> Seq.head)

    let getSingle s = 
        s |> Query.query |> Seq.head
   
    /// Finds a game in database
    let getGame (db:Context) id = 
        tryGetSingle
            <@ seq {
                for a in db.Games do
                    if a.Id = id then
                        yield a } @>
    
    let findEslMatch (db:Context) link = 
        tryGetSingle
            <@ seq {
                for m in db.MatchSessions do
                    if m.EslMatchLink = link then
                        yield m } @>


    let mediaPath (m:Database.Matchmedia) = 
        let innerPath = 
            System.String.Format(
                "{1}{0}{2}{0}{3} ({4}){5}",
                Path.DirectorySeparatorChar,
                m.MatchSession.Game.Shortname,
                m.MatchSession.Id,
                m.Name,
                m.Id,
                m.Type)

        let mediaPath = [ pluginFolder; "media"; innerPath ] |> pathCombine
        let dir = Path.GetDirectoryName mediaPath
        if not <| Directory.Exists dir then
            Directory.CreateDirectory dir |> ignore
        mediaPath

    let getActivatedMatchFormActions (db:Context) isWar (game : Database.Game) = 
        <@ seq {
            for a in db.MatchFormActions do
                if a.GameId = game.Id && (isWar && a.WarActivated || not isWar && a.PublicActivated) then
                    yield a } @> |> Query.query

    let matchmediaContainsTag (m:Database.Matchmedia) tag = 
        <@ seq {
            for a in m.Matchmedia_Tag do
                if a.Tag.Name = tag then
                    yield tag } @> |> Query.query |> Seq.isEmpty |> not

    let getParameterForAction (action:Database.ActionObject)  = 
        action.ObjectParameter.Load()
        let parameter = 
            action.ObjectParameter
                |> Seq.sortBy (fun o -> o.ParamNum)
                |> Seq.toArray
        parameter
            |> Seq.iteri 
                (fun i o -> 
                    if int o.ParamNum <> i then 
                        invalidOp "Database is broken: Invalid parameter number in actionobject %d" action.Id)
        if (parameter.Length <> int action.Action.Parameters) then
            invalidOp "Database is broken: Invalid parameter count in actionobject %d" action.Id
        parameter 

    let getIdFromLink link = 
        let uri = new System.Uri(link)
        (uri.Segments
            |> Seq.nth (uri.Segments.Length - 1))
            .Trim([|'/'|])
            |> System.Int32.Parse

    let backupFile file = 
        if (File.Exists file) then
            let rename = 
                Path.Combine(
                    (Path.GetDirectoryName file),
                    sprintf "%s-%s%s" (Path.GetFileNameWithoutExtension file) (System.Guid.NewGuid().ToString()) (Path.GetExtension file))
            File.Move(file, rename)
    
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

    let fromGenericFun<'T1, 'T2> (f:'T1 -> 'T2) = 
        (fun (m:obj) ->
            match m with
            | :? 'T1 as typedObj ->
                typedObj
                    |> f
                    :> obj
            | _ -> invalidOp (sprintf "invalid typed input value, got %O expected %O" (m.GetType()) typeof<'T1>))
    
    let getAction (db:Context) (action:Database.ActionObject) =
        let rec getActionRec prevFun (action:Database.ActionObject) = 
            let parameter = getParameterForAction action
        
            let fromFilterFun f = fromGenericFun (Seq.filter f)
            let fromActionFun f = fromGenericFun (Seq.map f)
            let actionFun = 
                match action.Action.Name with
                | "CopyToEslMatchmedia" ->
                    fromGenericFun (fun (matchSession:Database.MatchSession) ->
                        if matchSession.EslMatchLink <> null then 
                            let gameInterface = Wire.InterfaceFactory.gameInterface()
                            let eslId = getIdFromLink matchSession.EslMatchLink
                            matchSession.Matchmedia
                                |> Seq.iter (fun m ->
                                    gameInterface.copyToMatchMedia(m.Path, eslId)
                                        |> ignore)
                            () :> obj
                        else invalidOp "Can't copy to matchmedia on public match!")
                
                | "TypeFilter" ->
                    let fileType = parameter.[0].Parameter
                    fromFilterFun (fun (m:Database.Matchmedia) ->
                        m.Type = fileType)
                | "RegexPathFilter" ->
                    let regex = parameter.[0].Parameter
                    fromFilterFun (fun (m:Database.Matchmedia) ->
                        let r = new System.Text.RegularExpressions.Regex(regex)
                        r.IsMatch(m.Path))
                | "RegexNameFilter" ->
                    let regex = parameter.[0].Parameter
                    fromFilterFun (fun (m:Database.Matchmedia) ->
                        let r = new System.Text.RegularExpressions.Regex(regex)
                        r.IsMatch(m.Name))
                | "MapNameFilter" ->
                    let map = parameter.[0].Parameter
                    fromFilterFun (fun (m:Database.Matchmedia) ->
                        m.Map = map)                
                | "TagFilter" ->
                    let tag = parameter.[0].Parameter
                    fromFilterFun (fun (m:Database.Matchmedia) -> 
                        matchmediaContainsTag m tag)
                | "CopyMedia" ->
                    let rawTargetPath = parameter.[0].Parameter
                    fromActionFun (fun (m:Database.Matchmedia) ->
                        let targetPath = 
                            System.String.Format(
                                rawTargetPath,
                                m.MatchSession.Startdate, 
                                m.Created,
                                m.Map,
                                m.MatchSession.Game.Shortname,
                                m.Type,
                                m.Name,
                                (if m.MatchSession.EslMatchLink <> null then m.MatchSession.EslMatchLink else ""),
                                (if m.MatchSession.EslMatchLink <> null then
                                    let eslId = getIdFromLink m.MatchSession.EslMatchLink
                                    let info = Wire.InterfaceFactory.gameInterface().matchInfo(eslId)
                                    info.["name"] :?> string
                                    else "unknown"),
                                System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData),
                                System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData),
                                System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments),
                                Properties.Settings.Default.MatchMediaPath)
                            |> escapeInvalidChars '_'
                        File.Copy(m.Path, targetPath))

                | "DeleteMedia" ->
                    fromActionFun (fun (m:Database.Matchmedia) ->
                        File.Delete(m.Path)
                        m.Path <- null)
                | "DeleteSession" ->
                    fromGenericFun (fun (session:Database.MatchSession) ->
                        db.MatchSessions.DeleteOnSubmit(session))
                | "ExtractMatchmedia" ->
                    fromGenericFun (fun (session:Database.MatchSession) ->
                        session.Matchmedia :> Database.Matchmedia seq)
                | _ as e -> invalidOp (sprintf "Unknown Action: %s" e)
            let actionFun = 
                (fun (m:obj) ->
                    actionFun(prevFun(m)))
            if action.NextActionObjectId.HasValue then
                getActionRec actionFun action.NextActionObject
            else
                actionFun
        getActionRec id action

    let getOrCreateItem query f = 
        let item =
            tryGetSingle query
        match item with
        | None -> f()
        | Some (s) -> s

    let tryGetPlayerById (db:Context) id =
        tryGetSingle
            <@ seq {
                for a in db.Players do
                    if a.Id = id then
                        yield a } @>

    let tryGetPlayerByEslId (db:Context) eslId =
        tryGetSingle
            <@ seq {
                for a in db.Players do
                    if a.EslPlayerId = System.Nullable(eslId) then
                        yield a } @>

    let getPlayerByEslId (db:Context) id nick = 
        let mid = System.Nullable(id)
        let myQuery = 
            <@ seq {
                for m in db.Players do
                    if m.EslPlayerId = mid then
                        yield m } @>
        getOrCreateItem myQuery (fun _ ->
            let dbCopy = wrapper.Copy().Context
            let newIdentity = 
                new Database.Player(
                    EslPlayerId = System.Nullable(id),
                    Name = nick)
            dbCopy.Players.InsertOnSubmit(newIdentity)
            dbCopy.SubmitChanges()
            getSingle myQuery)

    open Wire
    let getIdentityPlayerInfo() = 
        let session = InterfaceFactory.sessionInterface()
        let userInfo = session.user()
        let nick = userInfo.["nickname"] :?> string
        let eslId = userInfo.["id"] :?> int
        nick, eslId

    let getIdentityPlayer (db:Context) = 
        let nick, eslId = getIdentityPlayerInfo()
        getPlayerByEslId db eslId nick
    let fillWrapperTable
        (players:EslGrabber.Player seq) 
        (wrapperTable:Primitives.WrapperDataTable.WrapperTable<Database.MatchSessions_Player>)
        (mediaTable:Primitives.WrapperDataTable.WrapperTable<Database.Matchmedia>)
            = 
        let data = wrapperTable.CopyLinqData

        let copyData = new System.Collections.Generic.HashSet<_>( data )
        let eslDict = new System.Collections.Generic.Dictionary<int,_>()
        for key, value in
            data |> Seq.filter (fun da -> da.Player.EslPlayerId.HasValue)
                 |> Seq.map (fun da -> da.Player.EslPlayerId.Value, da) do
            eslDict.Add(key, value)
        players
            |> Seq.iter 
                (fun p -> 
                    match eslDict.TryGetValue p.Id with
                    | true, data -> 
                        copyData.Remove data |> ignore
                        data.Player.Name <- p.Nick
                        data.Team <- byte p.Team
                        wrapperTable.UpdateItem data
                    | false, _ ->
                        let newPlayer = 
                            Database.MatchSessions_Player(
                                Team = byte p.Team, Player = Database.Player(Name = p.Nick, EslPlayerId = System.Nullable(p.Id)))
                        wrapperTable.Add newPlayer)
        let medias =
            mediaTable.CopyLinqData
        for item in copyData do
            if medias |> Seq.exists (fun m -> m.PlayerId = item.PlayerId) |> not then
                wrapperTable.DeleteCopyItem item
            else
                item.Team <- 11uy
                wrapperTable.UpdateItem item

    let fillPlayers (db:LocalDatabaseWrapper) (matchSession:Database.MatchSession) (players:EslGrabber.Player seq) =   
        let wrapper = WinFormGui.Helpers.GetWrapper(matchSession.MatchSessions_Player)
        let medias = WinFormGui.Helpers.GetWrapper(matchSession.Matchmedia)
        fillWrapperTable players wrapper medias
        db.UpdateMatchSessionPlayerTable(wrapper)


    let removeSession (db:Context) deleteFiles (matchSession:Database.MatchSession) = 
        for matchmedia in System.Collections.Generic.List<_> matchSession.Matchmedia do
            if deleteFiles && File.Exists matchmedia.Path then
                File.Delete matchmedia.Path
            matchSession.Matchmedia.Remove matchmedia |> ignore
            matchmedia.MatchSessions_Player.Player.Matchmedia.Remove matchmedia |> ignore
            
            if db.Matchmedias.GetOriginalEntityState(matchmedia) <> null then
                db.Matchmedias.DeleteOnSubmit matchmedia

            for tag in System.Collections.Generic.List<_> matchmedia.Matchmedia_Tag do
                matchmedia.Matchmedia_Tag.Remove tag |> ignore
                tag.Tag.Matchmedia_Tag.Remove tag |> ignore
                
                if db.Matchmedia_Tags.GetOriginalEntityState(tag) <> null then
                    db.Matchmedia_Tags.DeleteOnSubmit tag
                

        for player in System.Collections.Generic.List<_> matchSession.MatchSessions_Player do
            matchSession.MatchSessions_Player.Remove player |> ignore
            player.Player.MatchSessions_Player.Remove player |> ignore
            
            if db.MatchSessions_Players.GetOriginalEntityState(player) <> null then
                db.MatchSessions_Players.DeleteOnSubmit player

        for tag in System.Collections.Generic.List<_> matchSession.MatchSessions_Tag do
            matchSession.MatchSessions_Tag.Remove tag |> ignore
            tag.Tag.MatchSessions_Tag.Remove tag |> ignore
            
            if db.MatchSessions_Tags.GetOriginalEntityState(tag) <> null then
                db.MatchSessions_Tags.DeleteOnSubmit tag

        db.MatchSessions.DeleteOnSubmit(matchSession)