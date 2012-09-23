// ----------------------------------------------------------------------------
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.
// ----------------------------------------------------------------------------
namespace Yaaf.Utils

/// A generic Upgrading helper
module Upgrading = 
    open System.Net
    open Microsoft.FSharp.Control.WebExtensions
    let createClient () = new System.Net.WebClient()
    
    let downloadString uri = async {
        let client = createClient()
        let! data = client.AsyncDownloadString (System.Uri(uri))
        return data }
        
    let downloadFile progressChanged uri localFile = async {
        let client = createClient()
        client.DownloadProgressChanged
            |> Event.add (fun t ->  progressChanged(t))

        let start = 
            oneTime (fun () ->
                client.DownloadFileAsync(System.Uri(uri), localFile)) 

        let! completed = 
            client.DownloadFileCompleted
                |> Event.guard start
                |> Event.await client.CancelAsync
        if completed.Error <> null then
            raise (System.Reflection.TargetInvocationException("error while downloading file", completed.Error)) }

    open System.Xml
    let loadXml data = 
        let doc = new XmlDocument()
        doc.LoadXml data
        doc

    open System
    [<Flags>]
    type Sources = 
        | Github = 1
        | Esl = 2
        | Mirror = 4

    type FileUrl = {
        Url : string
        Source : Sources }

    type VersionData = {
        Version : Version
        CanBeSkipped : bool
        FileUrls : FileUrl seq
        Message : string }
    
    /// Parses the given Xml Document to a VersionData sequence
    let parseUpgradeFile (xml:XmlDocument) = 
        /// Parses the given Version Element
        let parseUpgradeFileItem (item:XmlElement) =
            let getBool defValue s = 
                match bool.TryParse s with
                | true, v -> v
                | false, _ -> defValue
            let getSource defValue s = 
                match Enums<Sources,_>.TryParse s with
                | true, s -> s
                | false, _ -> defValue
            /// Parses the given FileUrl Element
            let parseFileUrl (item:XmlElement) = 
                {
                    Url = item.InnerText
                    Source = getSource Sources.Mirror (item.GetAttribute "Source")
                }
            {
                Version = Version(item.GetAttribute "VersionNumber")
                CanBeSkipped = getBool true (item.GetAttribute "CanBeSkipped")
                FileUrls = 
                    item.ChildNodes
                        |> Seq.cast
                        |> Seq.map parseFileUrl
                Message = item.GetAttribute "Message"
            }
            
        xml.DocumentElement.ChildNodes
            |> Seq.cast
            |> Seq.map parseUpgradeFileItem
    
    let parseUpgradeFileString s = 
        let xml = loadXml s
        parseUpgradeFile xml

    /// Filter the version to only include the given Sources
    let filterVersions (filterFlags:Sources) versions = 
        let filterFile f = 
            Enums.GetFlags(filterFlags)
                |> Seq.exists (fun includeFlag -> Enums.HasFlag(f.Source, includeFlag))

        versions
            |> Seq.map (fun d -> { d with FileUrls = d.FileUrls |> Seq.filter filterFile })
            |> Seq.filter (fun d -> d.FileUrls |> Seq.isEmpty |> not)
    
    /// Finds the next valid version where startVersion if the Version to start looking from (the current Version)
    let findNextVersion startVersion versions = 
        let newVersions = 
            versions
                |> Seq.sortBy (fun t -> t.Version)
                |> Seq.skipWhile (fun d -> d.Version <= startVersion)
                |> Seq.cache
        let newVersionCount = newVersions |> Seq.length

        newVersions
            |> Seq.mapi (fun i d -> i,d)    
            |> Seq.skipWhile (fun (i,d) -> d.CanBeSkipped && i < newVersionCount - 1)
            |> Seq.map (fun (i,d) -> d)
            |> Seq.tryHead

    
