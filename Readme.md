# Yaaf.WirePlugin 


WirePlugin makes uploading Matchmedia in ESL-Matches very simple. 
Whenever you record anything in any supported game or make a screenshot WirePlugin will rename and move this files however you like.
The idea is basically play the game and after playing already have the "Matchmedia upload Window" perfectly filled with all your data.

WirePlugin goes even one step further and allows the same file management for any gaming you do on any public server.

## Dependencies

- https://github.com/fsharp/fsharp FSharp libraries required to use SyncLib and fsc required to build the Project
  * install: http://www.microsoft.com/de-de/download/details.aspx?id=13450
- CLI Runtime (one of those)
  * .NET 3.5 (Included in > Windows Vista) http://www.microsoft.com/de-de/download/details.aspx?id=21
- Wire http://www.esl.eu/de/wire/

## Using

Download the binaries or build yourself and then copy the files to C:\Users\_\AppData\Local\ESL Wire Game Client\plugins\Yaaf.WirePlugin.
Restart Wire.

## Contributing

### There are 3 ways to contribute to the project.

- If you plan to send multiple patches in the future the best would be to sign a Contributor-Agreement (https://github.com/matthid/Yaaf.WirePlugin/blob/master/ContributorAgreement.md) and send a scanned copy to matthi.d@googlemail.com.

- If you only want to send a single patch (or very few in general) you can state in the comment note and the pull request, that you share your changes under the MIT-License. For example: "This contribution is Licensed unter the http://www.opensource.org/licenses/mit-license.html"

- Report an issue https://github.com/matthid/Yaaf.WirePlugin/issues. See "Report a Bug"

### Report a Bug

The log folder is in C:\Users\_\AppData\Local\Yaaf\WirePlugin\log
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

- GPL is only recommend when there is no proprietary äquivalent. This can change in the future and with a pure GPL licensing you can not change.

- I started this project with a lot of effort put into it. Even when the project evolves I would like to have the possibility to use it in other software projects.

- You do not have to be afraid of changing the License of your contribution at will, and you can use your contribution wherever you want (see ContributorAgreement.md). 
  * "Any contribution we make available under any license will also be made available under a Free Culture (as defined by http://freedomdefined.org)  or Free Software/Open Source licence (as defined and approved by the Free Software Foundation or the Open Source Initiative)"
  * "Except as set out above, you keep all right, title, and interest in your contribution."

- If you feal like you can't contribute because of this, please send me a mail or open a issue.

## Licensing

This project is subject to the terms and conditions defined in file 'LICENSE.txt', which is part of this source code package. 
https://github.com/matthid/Yaaf.WirePlugin/blob/master/License.txt is a GPL License in version 3.

If you require another licensing please write to matthi.d@googlemail.com. (I will always consider helping open source projects).
Also remember: If you massively contribute to the project I have the option to give you any license you may require.
