// Diese Datei ist ein Skript, das mit F# interaktiv ausgeführt werden kann.  
// Es kann zur Erkundung und zum Testen des Bibliotheksprojekts verwendet werden.
// Skriptdateien gehören nicht zum Projektbuild.

#r "D:\Projects\Aktuell\WireYaafCssPlugin\Yaaf\lib\HtmlAgilityPack\HtmlAgilityPack.dll"
#r "FSharp.PowerPack.dll";;

benc 
    [ "http://www.esl.eu/de/css/ui/versus/match/2778059"; 
      "http://www.esl.eu/de/css/ui/versus/match/2777955"; 
      "http://www.esl.eu/de/ui/versus/match/2758882"; 
      "http://www.esl.eu/de/css/ui/versus/match/2758197"; 
      "http://www.esl.eu/de/css/ui/versus/match/2754367"] getMatchMembers;;