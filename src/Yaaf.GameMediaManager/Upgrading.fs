// ----------------------------------------------------------------------------
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.
// ----------------------------------------------------------------------------
namespace Yaaf.GameMediaManager

open System
open Yaaf.Logging
open Yaaf.Utils
open Yaaf.Utils.Upgrading
open Yaaf.GameMediaManager.WinFormGui.Properties
open Yaaf.GameMediaManager.Primitives

module Upgrading = 
    let downloadAndInstall show (versionData:VersionData) = async {
        show "Downloading File"
        let file = 
            versionData.FileUrls
                |> Seq.head
        let targetFile = Database.upgradeFile versionData.Version

        do! downloadFile 
                (fun progress -> show (sprintf "Downloading File (%d%%)" progress.ProgressPercentage)) 
                file.Url 
                targetFile
        
        
        show "Installing File"
        System.Diagnostics.Process.Start(targetFile) |> ignore }

    let getUpgradeTask (logger:ITracer) =
        let ev = new Event<string>()
        let show s = ev.Trigger s
        let asy = async {
                show "Checking for new Version"
                let! data = downloadString (ProjectConstants.GetRawLink "Updatefile.xml")
                
                show "Filtering versions"
                let nextVersion = 
                    parseUpgradeFileString data
                    |> filterVersions (Enums<Sources,_>.Parse Settings.Default.AllowedSources)
                    |> findNextVersion ProjectConstants.ProjectVersion
                    
                match nextVersion with
                | Some n ->
                    // Download and start the new Version
                    show "Wait for User Action"
                    let result =
                        System.Windows.Forms.MessageBox.Show(
                            String.Format(Resources.UpgradeQuestion, n.Version, n.Message),
                            Resources.UpgradeCaption,
                            System.Windows.Forms.MessageBoxButtons.YesNo)
                    if result = Windows.Forms.DialogResult.Yes then
                        do! downloadAndInstall show n
                | None -> ()
            }
        Task(asy, ev.Publish) :> ITask<_>
            
        

