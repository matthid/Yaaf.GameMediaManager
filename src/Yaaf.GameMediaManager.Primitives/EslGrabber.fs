// ----------------------------------------------------------------------------
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.
// ----------------------------------------------------------------------------
namespace Yaaf.GameMediaManager
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
    let getPlayerFromLink team nick link = async {
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

    let getLinkAndNickFromNode (htmlLink:HtmlNode) = 
        eslUrl + htmlLink.Attributes.["href"].Value, htmlLink.InnerText.Trim()
        
    let getVersusMatchMembers link = async {
        let! matchDocument = loadHtmlDocument link
        let playerVotes = matchDocument.GetElementbyId "votes_table"
        if (playerVotes = null) then invalidOp "Vote Table could not be found (invalid Matchlink?)"
        
        let allLinks = 
            playerVotes.Descendants "a"
        let linkCount = allLinks |> Seq.length
        let! players =
            allLinks 
                |> Seq.map getLinkAndNickFromNode
                |> Seq.mapi 
                    (fun i (link, nick) -> getPlayerFromLink (if i < linkCount / 2 then 1 else 2) nick link)
                |> Async.Parallel 
        return players :> Player seq}
    
    let getTeamMembers team teamLink = async {
        let! matchDocument = loadHtmlDocument teamLink
        let playerNodes = matchDocument.GetElementbyId "main_content"
        return!
            (playerNodes.SelectNodes "./div[4]/div[5]/table/tr/td[2]/div")
            .Descendants "a"
            |> Seq.map getLinkAndNickFromNode
            |> Seq.filter (fun (link, nick) -> not <| link.Contains "playercard")
            //|> Seq.map (fun (link, node) -> link, )
            |> Seq.map (fun (link, nick) -> getPlayerFromLink team nick link)
            |> Async.Parallel
        }
        
    let getRegularMatchMembers link = async {
        let! matchDocument = loadHtmlDocument link
        let teamLinkNodes = matchDocument.GetElementbyId "main_content"
        let getTeamLink team nodeString = 
            let teamLink = teamLinkNodes.SelectSingleNode nodeString
            let teamOrPlayerlink,nick = getLinkAndNickFromNode teamLink
            if link.Contains "/1on1/" then
                async {
                    let! player = getPlayerFromLink team nick teamOrPlayerlink
                    return [| player |]
                }
            else
                getTeamMembers team teamOrPlayerlink
        
        let! teamplayers = 
            [ getTeamLink 1 "./table/tr[3]/td[1]/table/tr[2]/td[1]/a[1]";
              getTeamLink 2 "./table/tr[3]/td[1]/table/tr[2]/td[3]/a[1]" ]
            |> Async.Parallel
        return
            teamplayers
            |> Seq.collect (fun t -> t)
        }

    let getMatchMembers (link:string) = 
        let fetchFun = 
            if link.Contains "versus" then
                getVersusMatchMembers
            else getRegularMatchMembers
        fetchFun link
    