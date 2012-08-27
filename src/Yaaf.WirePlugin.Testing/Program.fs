// Weitere Informationen zu F# unter "http://fsharp.net".

printf "%O"
    (Yaaf.WirePlugin.EslGrabber.getMatchMembers "http://www.esl.eu/eu/wire/anti-cheat/css/anticheat_test/match/26077222/"
    |> Async.RunSynchronously)