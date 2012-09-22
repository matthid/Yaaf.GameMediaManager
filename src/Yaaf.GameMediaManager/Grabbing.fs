// ----------------------------------------------------------------------------
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.
// ----------------------------------------------------------------------------
namespace Yaaf.GameMediaManager

open Yaaf.Logging

type GrabberProcMsg =
    | StartGrabbing
    | StoppedGrabbing
    | WaitForFinish of AsyncReplyChannel<unit>
type GrabberProc = MailboxProcessor<GrabberProcMsg>

module Grabbing = 
    let createProc (logger:ITracer) =   
        MailboxProcessor.Start(fun inbox ->
            let rec loop grabbingNum (waitingList:AsyncReplyChannel<unit> list)= async {
                let! msg = inbox.Receive()
                match msg with
                | GrabberProcMsg.StartGrabbing ->
                    logger.logInfo "StartGrabbing..."
                    return! loop (grabbingNum + 1) waitingList
                | GrabberProcMsg.StoppedGrabbing ->
                    logger.logInfo "StoppedGrabbing..."
                    let newWaitingList = 
                        if grabbingNum = 1 then
                            waitingList
                                |> List.iter (fun w -> w.Reply())
                            []
                        else waitingList
                    return! loop (grabbingNum - 1) newWaitingList
                | GrabberProcMsg.WaitForFinish(replyChannel) ->
                    logger.logInfo "WaitForFinish..."
                    let newWaitingList =
                        if grabbingNum = 0 then
                            waitingList
                                |> List.iter (fun w -> w.Reply())
                            logger.logInfo "Send Reply..."
                            replyChannel.Reply()
                            []
                        else replyChannel :: waitingList
                    return! loop grabbingNum newWaitingList
                }
            loop 0 [])
    
    let getGrabAction db (logger:ITracer) matchSession link (grabberProcessor:GrabberProc) = 
        async  {
            try
                try
                    logger.logInfo "Starting grabbing..."
                    grabberProcessor.Post GrabberProcMsg.StartGrabbing
                    let! players = EslGrabber.getMatchMembers link
                    Database.fillPlayers db matchSession players
                with exn ->
                    logger.logErr "Could not grab esl-infos: %O" exn    
            finally
                logger.logInfo "Finished grabbing..."
                grabberProcessor.Post GrabberProcMsg.StoppedGrabbing    
            return ()
        }

    let waitForFinish (grabberProcessor:GrabberProc) = 
        grabberProcessor.PostAndAsyncReply(fun channel -> GrabberProcMsg.WaitForFinish(channel))

    type Grabber(logger) =
        let proc = createProc logger
        member x.StartGrabAction db logger matchSession link = 
            proc |> getGrabAction db logger matchSession link |> Async.Start

        member x.WaitForFinish () = proc |> waitForFinish
            