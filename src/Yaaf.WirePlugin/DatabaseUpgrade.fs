// ----------------------------------------------------------------------------
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.
// ----------------------------------------------------------------------------
namespace Yaaf.WirePlugin
module DatabaseUpgrade = 
    open System
    
    open Yaaf.WirePlugin.WinFormGui
    
    let empty = async { return () }
    let combine asy1 asy2 = 
        if obj.ReferenceEquals(asy1, empty) && obj.ReferenceEquals(asy2, empty)
        then empty 
        else
            async {
                do! asy1
                do! asy2 }
    let createTask t = 
        if obj.ReferenceEquals(t, empty)
        then None
        else Some t
        
    open Microsoft.FSharp.Linq
    open Yaaf.WirePlugin.WinFormGui.Database.OldSchemas
    let upgrade1_0_0_0to1_1_0_0 update = async {
        // db can not be used actually, but this will work
        use db = Database.getContext().Context
        use old = new v1_0_0_0.LocalDataContext(db.Connection)

        // now db is actually a valid reference to our new database
        use db = 
            new v1_1_0_0.LocalDataContext
                ("convertto1_1_0_0" |> Database.dbFile |> Database.connectString)
        
        // Create the new database
        if db.DatabaseExists() then db.DeleteDatabase()
        db.CreateDatabase()

        // convert tags
        db.Tags.InsertAllOnSubmit
            (Query.query <@ seq { for a in old.Tags do yield a } @>
                |> Seq.map (fun a -> v1_1_0_0.Tag(Name = a.Name)))

        // convert games
        db.Games.InsertAllOnSubmit
            (Query.query <@ seq { for a in old.Games do yield a } @>
                |> Seq.map 
                    (fun a -> 
                        v1_1_0_0.Game(
                            Name = a.Name, Shortname = a.Shortname, EnableNotification = a.EnableNotification,
                            EnableMatchForm = a.EnableMatchForm, EnablePublicNotification = a.EnablePublicNotification,
                            EnableWarMatchForm = a.EnableWarMatchForm, WarMatchFormSaveFiles = a.WarMatchFormSaveFiles,
                            PublicMatchFormSaveFiles = a.PublicMatchFormSaveFiles)))

        // convert Actions
        db.Actions.InsertAllOnSubmit
            (Query.query <@ seq { for a in old.Actions do yield a } @>
                |> Seq.map (fun a -> v1_1_0_0.Actions(Name = a.Name, Parameters = a.Parameters)))

        db.SubmitChanges()


//        let me = Database.getIdentityPlayer db
//        let query = sprintf "ALTER TABLE Matchmedia
//	ADD PlayerId integer 
//		DEFAULT %d
//		NOT NULL REFERENCES Players (Id)" me.Id
//        db.ExecuteQuery(query, [||]) |> ignore
//        return ()
        }
        
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
        empty
            |> combine
                (if (currentSchemaVersion = Version(1,0,0,0)) then
                    upgrade1_0_0_0to1_1_0_0 logger
                 else empty)
            |> createTask
