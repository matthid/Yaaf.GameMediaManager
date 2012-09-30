module EslGrabberTests

open TestRunner

let testLinkSimple link = Yaaf.GameMediaManager.EslGrabber.getMatchMembers link

let testLink link playerCount = async {
    let! players = Yaaf.GameMediaManager.EslGrabber.getMatchMembers link
    assert (players |> Seq.length = playerCount * 2)
    return ()
    }

let tests = [
    yield!
        [ 
        "TestMatch", "http://www.esl.eu/eu/wire/anti-cheat/css/anticheat_test/match/26077222/"
        ] |> Seq.map (fun (name, workingLink) -> simpleTest (sprintf "TestEslMatches_%s" name) (fun o -> testLinkSimple workingLink |> Async.Ignore))

    // Some working links (links that should work)
    yield! 
        [
        "MatchwithCheater", "http://www.esl.eu/de/csgo/ui/versus/match/3035028",5
        "DeletedAccount", "http://www.esl.eu/de/css/ui/versus/match/2852106" ,5
        "CS1.6", "http://www.esl.eu/de/cs/ui/versus/match/2997440" ,5
        "2on2Versus", "http://www.esl.eu/de/css/ui/versus/match/3012767" ,2
        "SC2cup1on1", "http://www.esl.eu/eu/sc2/go4sc2/cup230/match/26964055/",1
        "CSGO2on2Cup", "http://www.esl.eu/de/csgo/cups/2on2/season_08/match/26854846/",2
        "CSSAwpCup", "http://www.esl.eu/eu/css/cups/2on2/awp_cup_11/match/26811005/",2
        ] |> Seq.map (fun (name, workingLink,players) -> simpleTest (sprintf "TestEslMatches_%s" name) (fun o -> testLink workingLink players))
    ]
