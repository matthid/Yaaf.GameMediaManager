// ----------------------------------------------------------------------------
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.
// ----------------------------------------------------------------------------
namespace Yaaf.WirePlugin
module DatabaseUpgrade = 
    open System
    
    open Yaaf.WirePlugin.WinFormGui

    /// Tries to return an upgrade Task or None if no upgrade is required
    let getUpgradeDatabaseTask logger = 
        let currentSchemaVersion = Version(Properties.Settings.Default.DatabaseSchemaVersion)
        let defaults = Properties.Settings()
        defaults.Reset()
        let newSchemaVersion = Version(defaults.DatabaseSchemaVersion)
        if currentSchemaVersion < Version(1,0,0,0) then
            failwith 
                (sprintf "Upgrade from this Version (%O) is not supported, try upgrading with an older version first!" currentSchemaVersion)
        
        if currentSchemaVersion > newSchemaVersion then
            failwith 
                (sprintf 
                    "The current Schmema-Version (%O) is newer than the required one (%O). Are you trying to downgrade your version? Downgrading is not supported!"
                    currentSchemaVersion
                    newSchemaVersion)

        None
