// ----------------------------------------------------------------------------
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.
// ----------------------------------------------------------------------------
namespace Yaaf.WirePlugin

module Database = 
    open System.IO
    open Microsoft.FSharp.Linq
    open Yaaf.WirePlugin.WinFormGui
    let pluginFolder =
            [ System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData);
                "Yaaf"; "WirePlugin" ] |> pathCombine
    let private db = 
        let dbFileName = "LocalDatabase.sdf"
        let dbFile =
            [ pluginFolder; dbFileName ] |> pathCombine
        
        if not <| File.Exists dbFile then
            if File.Exists dbFileName then
                File.Copy(dbFileName, dbFile)
            else
                invalidOp "Source DB-File could not be found"

        let connectString = sprintf "Data Source=%s" dbFile
        new Database.LocalDatabaseDataContext(connectString)
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
    let getGame (db:Database.LocalDatabaseDataContext) id = 
        tryGetSingle
            <@ seq {
                for a in db.Games do
                    if a.Id = id then
                        yield a } @>
    
    let findEslMatch (db:Database.LocalDatabaseDataContext) link = 
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

    let getActivatedMatchFormActions (db:Database.LocalDatabaseDataContext) isWar (game : Database.Game) = 
        <@ seq {
            for a in db.MatchFormActions do
                if a.GameId = game.Id && (isWar && a.WarActivated || not isWar && a.PublicActivated) then
                    yield a } @> |> Query.query

    let matchmediaContainsTag (m:Database.Matchmedia) tag = 
        <@ seq {
            for a in m.Matchmedia_Tag do
                if a.Tag.Name = tag then
                    yield tag } @> |> Query.query |> Seq.isEmpty |> not

    let getParameterForAction actionType (action:Database.ActionObject)  = 
        action.ObjectParameter.Load()
        let parameter = 
            action.ObjectParameter
                |> Seq.filter (fun o -> o.Type = actionType)
                |> Seq.sortBy (fun o -> o.ParamNum)
                |> Seq.toArray
        parameter
            |> Seq.iteri 
                (fun i o -> 
                    if int o.ParamNum <> i then 
                        invalidOp "Database is broken: Invalid parameter number for type %d in actionobject %d" actionType action.Id)
        if (parameter.Length <> int action.Action.Parameters) then
            invalidOp "Database is broken: Invalid parameter count for type %d in actionobject %d" actionType action.Id
        parameter 

    let rec getFilter (action:Database.ActionObject) = 
        let parameter = getParameterForAction 1uy action
        match action.Filter.Name with
        | "Merge" ->
            (fun (m:Database.Matchmedia) ->
                getFilter parameter.[0].ActionObject m &&
                getFilter parameter.[1].ActionObject m) 
        | "None" ->
            (fun (m:Database.Matchmedia) -> true)
        | "OrFilter" ->
            (fun (m:Database.Matchmedia) ->
                getFilter parameter.[0].ActionObject m ||
                getFilter parameter.[1].ActionObject m)
        | "TypeFilter" ->
            (fun (m:Database.Matchmedia) ->
                m.Type = parameter.[0].Parameter)
        | "RegexPathFilter" ->
            (fun (m:Database.Matchmedia) ->
                let regex = parameter.[0].Parameter
                let r = new System.Text.RegularExpressions.Regex(regex)
                r.IsMatch(m.Path))
        | "RegexNameFilter" ->
            (fun (m:Database.Matchmedia) ->
                let regex = parameter.[0].Parameter
                let r = new System.Text.RegularExpressions.Regex(regex)
                r.IsMatch(m.Name))
        | "MapNameFilter" ->
            (fun (m:Database.Matchmedia) ->
                m.Map = parameter.[0].Parameter)                
        | "TagFilter" ->
            (fun (m:Database.Matchmedia) -> 
                let tag = parameter.[0].Parameter
                matchmediaContainsTag m tag)
        | _ as e -> invalidOp (sprintf "Unknown Filter: %s" e)

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

    let rec getAction (action:Database.ActionObject) = 
        let parameter = getParameterForAction 2uy action
        match action.Action.Name with
        | "Merge" ->
            (fun (m:Database.Matchmedia) ->
                getAction parameter.[0].ActionObject m 
                getAction parameter.[1].ActionObject m)
        | "None" ->
            (fun (m:Database.Matchmedia) -> ())
        | "CopyMedia" ->
            (fun (m:Database.Matchmedia) ->
                let rawTargetPath = parameter.[0].Parameter
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

        | "CopyToEslMatchmedia" ->
            (fun (m:Database.Matchmedia) ->
                if m.MatchSession.EslMatchLink <> null then 
                    let gameInterface = Wire.InterfaceFactory.gameInterface()
                    let eslId = getIdFromLink m.MatchSession.EslMatchLink
                    gameInterface.copyToMatchMedia(m.Path, eslId)
                        |> ignore
                else invalidOp "Can't copy to matchmedia on public match")
        | "DeleteMedia" ->
            (fun (m:Database.Matchmedia) ->
                File.Delete(m.Path)
                m.Path <- null)
        | _ as e -> invalidOp (sprintf "Unknown Action: %s" e)
    
    let getIdentityPlayer (db:Database.LocalDatabaseDataContext) = 
        let id = Properties.Settings.Default.MyIdentity
        let myQuery = 
            <@ seq {
                for a in db.Players do
                    if a.Id = id then
                        yield a } @>
        let player =
            tryGetSingle myQuery
        match player with
        | None ->
            let dbCopy = wrapper.Copy().Context
            let newIdentity = 
                new Database.Player(
                    Name = "Me")
            dbCopy.Players.InsertOnSubmit(newIdentity)
            dbCopy.SubmitChanges()
            Properties.Settings.Default.MyIdentity <- newIdentity.Id
            Properties.Settings.Default.Save()
            getSingle myQuery
        | Some (p) -> p