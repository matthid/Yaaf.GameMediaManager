# Yaaf.GameMediaManager 

GameMediaManager makes uploading Matchmedia in ESL-Matches very simple. 
Whenever you record anything in any (needs wire support) game or make a screenshot GameMediaManager will manage these files for you.
The idea is basically to play a game and after playing already have the "Matchmedia upload Window" perfectly filled with all your data.
You can tag matches, players and files. 
You can also add some info to your enemies or mates, for example set their skill level, flag as cheater or save a description how they played in this game.

All this data can be searched (not fully implemented) and exported (media files).

GameMediaManager goes even one step further and allows the same file/player/media management for any gaming you do besides ESL-Matches.

Features:
- Manages your matchmedia
- Automatically detects new matchmedia from games (after setup)
- tag players/matches/media
- save a short description to players
- save additional player info to matches (skill/cheater/team)
- link to your esl matches
- fills your enemies for Esl Matches (even for Versus if you provide the Versus match-link)
- fills the Esl matchmedia upload window
- integrated in Wire so no additional program has to be opened (besides Wire)
- automatic actions based on tags/names/games/... (not implemented)


## Dependencies

- http://www.microsoft.com/de-de/download/details.aspx?id=5783 (Microsoft SQL Server Compact 3.5 Service Pack 2 f�r Windows Desktop)
- https://github.com/fsharp/fsharp FSharp libraries required to use GameMediaManager and fsc required to build the Project
  * install: http://www.microsoft.com/de-de/download/details.aspx?id=13450
- http://fsharppowerpack.codeplex.com/ FSharp Powerpack
- CLI Runtime (one of those)
  * .NET 3.5 (Included in > Windows Vista) http://www.microsoft.com/de-de/download/details.aspx?id=21
- Wire http://www.esl.eu/de/wire/

- F# Project System Extender (Visual Studio Extension)


Code Dependencies (no need to install, shipped)
- http://htmlagilitypack.codeplex.com/ Html Agility Pack

## Using

Read the Wiki:
https://github.com/matthid/Yaaf.GameMediaManager/wiki

Download the binaries and double click or build yourself and then copy the files to C:\Users\_\AppData\Local\ESL Wire Game Client\plugins\Yaaf.GameMediaManager.
Restart Wire. Make sure to install all dependencies (see above).

## Contributing

Read the wishlist in the releasenotes (https://github.com/matthid/Yaaf.GameMediaManager/blob/master/Releasenotes.txt) if you want to get some ideas where to start.
If you start something you can send me a mail to coordinate progress/tasks if you want (matthi.d@googlemail.com).

### There are 3 ways to contribute to the project.

- If you plan to send multiple patches in the future the best would be to sign a Contributor-Agreement (https://github.com/matthid/Yaaf.GameMediaManager/blob/master/ContributorAgreement.md) and send a scanned copy to matthi.d@googlemail.com.

- If you only want to send a single patch (or very few in general) you can state in the comment note and the pull request, that you share your changes under the MIT-License. For example: "This contribution is Licensed unter the http://www.opensource.org/licenses/mit-license.html"

- Report an issue https://github.com/matthid/Yaaf.GameMediaManager/issues. See "Report a Bug"

### Report a Bug

The log folder is in C:\Users\_\AppData\Local\Yaaf\GameMediaManager\log
Do the following:

- move the contents of the "logs" folder, do not remove/move the logs folder itself.
- start wire and produce the bug/problem.
- attach the contents of the logs folder to your issue request.

If you can't reproduce the bug attach the moved contents instead.
If it is not a bug you do not have to attach any logs of course. This is true for feature requests, api change requests...

Sometimes even with logs the bug can't be figured out easily. In this case I will request additional info from you.

### Why so "complicated"?

I do not consider these above steps as a complication of the contribution process. 
That's what you get for living in a state of law.
I really think free software licenses are the way to go. But the GPL is very restrictive in some ways. Consider these things:

- GPL is only recommend when there is no proprietary �quivalent. This can change in the future and with a pure GPL licensing you can not change.

- I started this project with a lot of effort put into it. Even when the project evolves I would like to have the possibility to use it in other software projects.

- You do not have to be afraid of changing the License of your contribution at will, and you can use your contribution wherever you want (see ContributorAgreement.md). 
  * "Any contribution we make available under any license will also be made available under a Free Culture (as defined by http://freedomdefined.org)  or Free Software/Open Source licence (as defined and approved by the Free Software Foundation or the Open Source Initiative)"
  * "Except as set out above, you keep all right, title, and interest in your contribution."

- If you feal like you can't contribute because of this, please send me a mail or open a issue.

## Licensing

This project is subject to the terms and conditions defined in file 'LICENSE.txt', which is part of this source code package. 
https://github.com/matthid/Yaaf.GameMediaManager/blob/master/LICENSE.txt is a GPL License in version 3.

If you require another licensing please write to matthi.d@googlemail.com. (I will always consider helping open source projects).
Also remember: If you massively contribute to the project I have the option to give you any license you may require.
