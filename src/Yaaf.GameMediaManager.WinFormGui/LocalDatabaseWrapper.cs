// ----------------------------------------------------------------------------
// This file (LocalDatabaseWrapper.cs) is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package (Yaaf.GameMediaManager).
// Last Modified: 2012/09/10 14:08
// Created: 2012/08/26 20:57
// ----------------------------------------------------------------------------

namespace Yaaf.GameMediaManager.WinFormGui
{
    using System;
    using System.Collections.Generic;
    using System.Data.Linq;
    using System.Data.Linq.Mapping;
    using System.Data.SqlClient;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;

    using Yaaf.GameMediaManager.Primitives;
    using Yaaf.GameMediaManager.WinFormGui.Database;
    using Yaaf.GameMediaManager.WinFormGui.Properties;

    using Action = Yaaf.GameMediaManager.WinFormGui.Database.Action;

    public class LocalDatabaseWrapper
    {
        private static readonly TraceSource loggerSource;

        private static readonly Logging.LoggingInterfaces.ITracer logger;

        private readonly LocalDataContext context;

        static LocalDatabaseWrapper()
        {
            Console.WriteLine("init logger");
            loggerSource = Logging.Source("Yaaf.GameMediaManager.WinFormGui.LocalDatabaseWrapper", "");
            logger = Logging.DefaultTracer(loggerSource, "Initialization");
        }

        public LocalDatabaseWrapper(LocalDataContext context)
        {
            this.context = context;
        }

        public LocalDataContext Context
        {
            get
            {
                return context;
            }
        }

        public Tag GetTag(string tagString)
        {
            try
            {
                var t = (from tag in context.Tags where tag.Name == tagString select tag).SingleOrDefault();
                if (t != null)
                {
                    return t;
                }

                var newContext = new LocalDataContext(context.Connection);
                t = new Tag();
                t.Name = tagString;
                newContext.Tags.InsertOnSubmit(t);
                newContext.SubmitChanges();
                return (from tag in context.Tags where tag.Id == t.Id select tag).Single();
            }
            catch (Exception ex)
            {
                logger.LogError("Could not create tag: {0}", ex);

                // Maybe added by another thread
                return (from tag in context.Tags where tag.Name == tagString select tag).Single();
            }
        }

        public void UpdateDatabase<T>(Table<T> table, IEnumerable<T> changedItems, IList<T> originalItems)
            where T : class
        {
            foreach (var item in changedItems)
            {
                if (!originalItems.Contains(item))
                {
                    table.InsertOnSubmit(item);
                }
                else
                {
                    originalItems.Remove(item);
                }
            }

            table.DeleteAllOnSubmit(originalItems);
        }

        public LocalDatabaseWrapper Copy()
        {
            return new LocalDatabaseWrapper(new LocalDataContext(context.Connection));
        }

        public ActionObject GetMoveToMatchmediaActionObject()
        {
            var defActionObjectName = "DefaultActionObject_CopyToEslMatchMedia";
            var def = 
                (from actionObject in context.ActionObjects 
                 where actionObject.Name == defActionObjectName
                 select actionObject).SingleOrDefault();
            if (def == null)
            {
                var contextCopy = Copy();
                var copyAction = contextCopy.GetAction("CopyToEslMatchmedia");

                var obj = new ActionObject();
                obj.Action = copyAction;
                obj.Name = defActionObjectName;

                contextCopy.Context.ActionObjects.InsertOnSubmit(obj);
                contextCopy.Context.SubmitChanges();
                def = (from actionObject in context.ActionObjects
                       where actionObject.Name == defActionObjectName
                       select actionObject).Single();
            }
            return def;
        }

        private Action GetAction(string name)
        {
            return (from tag in context.Actions where tag.Name == name select tag).Single();
        }

        public void MySubmitChanges()
        {
            try
            {
                var changes = Context.GetChangeSet();
                var allchanges = changes.Inserts.Union(changes.Updates).ToList();

                // Update MatchSessions_Player
                var matchSessionsPlayers = allchanges.Select(o => o as MatchSessions_Player).Where(o => o != null);
                foreach (var matchSessionsPlayer in matchSessionsPlayers)
                {
                    matchSessionsPlayer.MatchSession = matchSessionsPlayer.MyMatchSession;
                    if (matchSessionsPlayer.Player.MyId != 0)
                    {
                        var newPlayer = FSharpInterop.Interop.Database.GetPlayerById(
                            this, matchSessionsPlayer.Player.MyId);
                        var oldPlayer = matchSessionsPlayer.Player;
                        if (newPlayer != null)
                        {
                            UpdateMatchSessionPlayer(matchSessionsPlayer, oldPlayer, newPlayer);
                        }
                        else
                        {
                            matchSessionsPlayer.PlayerId = 0;
                        }
                    }

                    if (matchSessionsPlayer.Player.EslPlayerId.HasValue && matchSessionsPlayer.PlayerId == 0)
                    {
                        var newPlayer = FSharpInterop.Interop.Database.GetPlayerByEslId(
                            this, matchSessionsPlayer.Player.EslPlayerId.Value);
                        var oldPlayer = matchSessionsPlayer.Player;
                        if (newPlayer != null)
                        {
                            UpdateMatchSessionPlayer(matchSessionsPlayer, oldPlayer, newPlayer);
                        }
                    }
                }

                // Update Matchmedias
                var matchmedias = allchanges.Select(o => o as Matchmedia).Where(o => o != null).Cache();
                foreach (var matchmedia in matchmedias)
                {
                    if (matchmedia.MyMatchSessionsPlayer != null)
                    {
                        matchmedia.MatchSessions_Player = matchmedia.MyMatchSessionsPlayer;
                    }
                } 

                // Update WatchFolders
                var watchFolders = allchanges.Select(o => o as WatchFolder).Where(o => o != null).Cache();
                foreach (var watchFolder in watchFolders)
                {
                    if (watchFolder.MyGame != null)
                    {
                        watchFolder.Game = watchFolder.MyGame;
                    }
                }

                // Update MatchSessions
                var deletedSessions = changes.Deletes.Select(o => o as MatchSession).Where(o => o != null);
                foreach (var deletedSession in deletedSessions)
                {
                    FSharpInterop.Interop.Database.DeleteMatchSession(this, true, deletedSession);
                } 
                var addedSessions = changes.Inserts.Select(o => o as MatchSession).Where(o => o != null);
                foreach (var addedSession in addedSessions)
                {
                    if (addedSession.MyGame != null)
                    {
                        addedSession.Game = addedSession.MyGame;
                    }
                }

                try
                {
                    Context.SubmitChanges(ConflictMode.ContinueOnConflict);
                }
                catch (ChangeConflictException e)
                {
                    logger.LogError("Recognized a ChangeConflict! Error: {0}", e);
                    foreach (ObjectChangeConflict occ in Context.ChangeConflicts)
                    {
                        MetaTable metatable = Context.Mapping.GetTable(occ.Object.GetType());
                        var entityInConflict = occ.Object;
                        logger.LogInformation("Table name: {0}", metatable.TableName);
                        logger.LogInformation("Object: {0}", entityInConflict);
                        foreach (MemberChangeConflict mcc in occ.MemberConflicts)
                        {
                            object currVal = mcc.CurrentValue;
                            object origVal = mcc.OriginalValue;
                            object databaseVal = mcc.DatabaseValue;
                            MemberInfo mi = mcc.Member;
                            logger.LogInformation("Member: {0}", mi.Name);
                            logger.LogInformation("current value: {0}", currVal);
                            logger.LogInformation("original value: {0}", origVal);
                            logger.LogInformation("database value: {0}", databaseVal);
                        }
                        occ.Resolve(RefreshMode.KeepChanges);
                    }
                }

                Context.SubmitChanges(ConflictMode.FailOnFirstConflict);
            }
            catch (SqlException e)
            {
                logger.LogError("SqlException: {0}", e);
                throw;
            }
            catch (Exception e)
            {
                logger.LogError("Exception: Type: {0}, ToString: {1}", e.GetType(), e.ToString());
                if (e.GetType().FullName == "System.Data.SqlServerCe.SqlCeException")
                {
                    TryDumpData();
                }
                throw;
            }
        }

        private void TryDumpData()
        {
            try
            {
                var changes = Context.GetChangeSet();
                foreach (var delete in changes.Deletes)
                {
                    logger.LogInformation("Dump data: Deletion({0})", delete.ToString());
                } 

                foreach (var insert in changes.Inserts)
                {
                    logger.LogInformation("Dump data: Insertion({0})", insert.ToString());
                }

                foreach (var update in changes.Updates)
                {
                    var table = Context.GetTable(update.GetType());
                    var original = table.GetOriginalEntityState(update);
                    logger.LogInformation("Dump data: Update({0}, {1})", original.ToString(), update.ToString());
                }
            }
            catch (Exception e)
            {
                logger.LogError("Exception while trying to dump data: {0}", e.ToString());
            }
        }

        private void UpdateMatchSessionPlayer(MatchSessions_Player matchSessionsPlayer, Player copyPlayer, Player databasePlayer)
        {
            matchSessionsPlayer.Player = databasePlayer;
            databasePlayer.EslPlayerId = copyPlayer.EslPlayerId;
            databasePlayer.MyTags = copyPlayer.MyTags;
            databasePlayer.Name = copyPlayer.Name;
            if (copyPlayer.Id == 0)
            {
                if (Context.Players.GetOriginalEntityState(copyPlayer) != null)
                { // should not happen
                    Context.Players.DeleteOnSubmit(copyPlayer);
                }
            }
        }

        public void UpdateMatchSessionPlayerTable(WrapperDataTable.WrapperTable<MatchSessions_Player> playerTable)
        {
            var localDataContext = Context;
            playerTable
                .UpdateTable(localDataContext.MatchSessions_Players);
            // TODO: check if we have to delete "player" references
        }
    }
}