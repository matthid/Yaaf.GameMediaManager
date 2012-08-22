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
    let db = 
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
    let wrapper = 
        new LocalDatabaseWrapper(db)

    let getIdMaybe s = 
        let result = 
            s |> Query.query |> Seq.tryTake 1
        if result |> Seq.isEmpty then
            None
        else Some (result |> Seq.head)
        
    /// Finds a game in database
    let getGame id = 
        getIdMaybe
            <@ seq {
                for a in db.Games do
                    if a.Id = id then
                        yield a } @>
    
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

    let getActivatedMatchFormActions isWar (game : Database.Game) = 
        <@ seq {
            for a in game.MatchFormAction do
                if isWar && a.WarActivated || not isWar && a.PublicActivated then
                    yield a } @> |> Query.query

    let matchmediaContainsTag (m:Database.Matchmedia) tag = 
        <@ seq {
            for a in m.Matchmedia_Tag do
                if a.Tag.Name = tag then
                    yield tag } @> |> Query.query |> Seq.isEmpty |> not

    let rec getFilter (action:Database.ActionObject) = 
        action.ObjectParameter.Load()
        let parameter = action.ObjectParameter |> Seq.toArray
        if (parameter.Length <> int action.Filter.Parameters) then
            invalidOp "Database is broken invalid parameter count for filter in actionobject %d" action.Id
        match action.Filter.Name with
        | "Merge" ->
            (fun (m:Database.Matchmedia) ->
                getFilter parameter.[0].ActionObject m &&
                getFilter parameter.[1].ActionObject m)
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

    let rec getAction (action:Database.ActionObject) = 
        action.ObjectParameter.Load()
        let parameter = action.ObjectParameter |> Seq.toArray
        if (parameter.Length <> int action.Action.Parameters) then
            invalidOp "Database is broken invalid parameter count for action in actionobject %d" action.Id
        match action.Action.Name with
        | "Merge" ->
            (fun (m:Database.Matchmedia) ->
                getAction parameter.[0].ActionObject m 
                getAction parameter.[1].ActionObject m)
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
                        (if m.MatchSession.EslMatchId.HasValue then m.MatchSession.EslMatchId.Value else 0),
                        (if m.MatchSession.EslMatchId.HasValue then
                            let info = Wire.InterfaceFactory.gameInterface().matchInfo(m.MatchSession.EslMatchId.Value)
                            info.["name"] :?> string
                         else "unknown"))
                File.Copy(m.Path, targetPath))
        | "CopyToEslMatchmedia" ->
            (fun (m:Database.Matchmedia) ->
                if m.MatchSession.EslMatchId.HasValue then 
                    let gameInterface = Wire.InterfaceFactory.gameInterface()
                    gameInterface.copyToMatchMedia(m.Path, m.MatchSession.EslMatchId.Value)
                        |> ignore
                else invalidOp "Can't copy to matchmedia on public match")
        | "DeleteMedia" ->
            (fun (m:Database.Matchmedia) ->
                File.Delete(m.Path)
                m.Path <- null)
        | _ as e -> invalidOp (sprintf "Unknown Action: %s" e)
        