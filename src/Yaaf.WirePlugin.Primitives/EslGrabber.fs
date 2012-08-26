// ----------------------------------------------------------------------------
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.
// ----------------------------------------------------------------------------
namespace Yaaf.WirePlugin
module EslGrabber = 
    open System
    open System.IO
    open System.Net
    open System.Text.RegularExpressions
    open HtmlAgilityPack
    open Microsoft.FSharp.Control.WebExtensions

    let idPattern = @"(?<=player/).*(?=/)"
    
    // They are thread safe and can be shared see http://msdn.microsoft.com/en-us/library/6h453d2h.aspx
    let idRegex = new Regex(idPattern)

    type Player = {
        Id : int;
        EslLink : string;
        Nick : string;
        Team: int }

    let htmlWeb = new HtmlWeb()
 
    let loadHtmlDocument (link:string) = async {
        let req = HttpWebRequest.Create(link)
        let! resp = req.AsyncGetResponse()
        let stream = resp.GetResponseStream()
        use reader = new StreamReader(stream)
        let! source = reader.AsyncReadToEnd()
        let matchDocument = HtmlDocument()
        matchDocument.LoadHtml(source)
        return matchDocument }

    let eslUrl = "http://www.esl.eu"
    let asyncUnit = async { return () }
    let getPlayerFromLink team link nick = async {
        match System.Int32.TryParse(idRegex.Match(link).ToString()) with
        | (true, id) ->
            return {
                Id = id;
                Nick = nick;
                EslLink = link
                Team = team}
        | _ ->
        let! playerDocument = loadHtmlDocument link
        let playerNodes = playerDocument.GetElementbyId "player_logo"
        let playerLinkNode= playerNodes.SelectSingleNode "a"
        let playerLinkWithId = playerLinkNode.Attributes.["href"].Value
                        
        let playerId = 
            idRegex.Match(playerLinkWithId).ToString()
            |> System.Int32.Parse
        return {
            Id = playerId;
            Nick = nick;
            EslLink = link
            Team = team} }

    let getMatchMembers link = async {
        let! matchDocument = loadHtmlDocument link
        let playerVotes = matchDocument.GetElementbyId "votes_table"
        if (playerVotes = null) then invalidOp "Vote Table could not be found (invalid Matchlink?)"
        
        let allLinks = 
            playerVotes.Descendants "a"
        let linkCount = allLinks |> Seq.length
        return!
            allLinks 
                |> Seq.map (fun htmlLink -> eslUrl + htmlLink.Attributes.["href"].Value, htmlLink.InnerText.Trim())
                |> Seq.mapi 
                    (fun i (link, nick) -> getPlayerFromLink (if i < linkCount / 2 then 1 else 2) link nick)
                |> Async.Parallel }
