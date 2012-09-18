// ----------------------------------------------------------------------------
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.
// ----------------------------------------------------------------------------

#r @"D:\Projects\Aktuell\WireYaafCssPlugin\Yaaf\src\Yaaf.GameMediaManager\bin\Debug\Yaaf.GameMediaManager.Primitives.CSharp.dll"
#r @"D:\Projects\Aktuell\WireYaafCssPlugin\Yaaf\src\Yaaf.GameMediaManager\bin\Debug\Yaaf.GameMediaManager.Primitives.dll"
#r @"D:\Projects\Aktuell\WireYaafCssPlugin\Yaaf\src\Yaaf.GameMediaManager\bin\Debug\Yaaf.GameMediaManager.WinFormGui.dll"
#r @"D:\Projects\Aktuell\WireYaafCssPlugin\Yaaf\src\Yaaf.GameMediaManager\bin\Debug\Yaaf.GameMediaManager.dll"

#r "System.Data.Linq.dll"

open System
open System.IO

open Yaaf.GameMediaManager
open Yaaf.GameMediaManager.Primitives
open Yaaf.GameMediaManager.WinFormGui.Database

let context = Database.getContext()
context.Context.MatchSessions_Players.InsertOnSubmit(new MatchSessions_Player())
try
    context.Context.SubmitChanges()
with exn ->
    printfn "type %s" (exn.GetType().FullName)
    printfn "msg %s" exn.Message
    printfn "stack %s" exn.StackTrace
