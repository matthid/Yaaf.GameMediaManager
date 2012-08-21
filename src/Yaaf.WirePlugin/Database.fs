// ----------------------------------------------------------------------------
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.
// ----------------------------------------------------------------------------
namespace Yaaf.WirePlugin

module Database = 
    open System.IO
    open Microsoft.FSharp.Linq

    let db = 
        let dbFileName = "LocalDatabase.sdf"
        let dbFile =
            [ System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData);
                "Yaaf"; "WirePlugin"; dbFileName ] |> pathCombine
        
        if not <| File.Exists dbFile then
            if File.Exists dbFileName then
                File.Copy(dbFileName, dbFile)
            else
                invalidOp "Source DB-File could not be found"

        let connectString = sprintf "Data Source=%s" dbFile
        new Yaaf.WirePlugin.WinFormGui.Database.LocalDatabaseDataContext(connectString)
    //let wrapper = 
        //new LocalDatabaseWrapper(db)

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
    



        