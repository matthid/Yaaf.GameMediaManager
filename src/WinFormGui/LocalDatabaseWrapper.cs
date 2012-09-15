// ----------------------------------------------------------------------------
// This file (LocalDatabaseWrapper.cs) is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package (Yaaf.WirePlugin).
// Last Modified: 2012/09/10 14:08
// Created: 2012/08/26 20:57
// ----------------------------------------------------------------------------

namespace Yaaf.WirePlugin.WinFormGui
{
    using System;
    using System.Collections.Generic;
    using System.Data.Linq;
    using System.Data.Linq.Mapping;
    using System.Data.SqlClient;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;

    using Yaaf.WirePlugin.Primitives;
    using Yaaf.WirePlugin.WinFormGui.Database;
    using Yaaf.WirePlugin.WinFormGui.Properties;

    using Action = Yaaf.WirePlugin.WinFormGui.Database.Action;

    public class LocalDatabaseWrapper
    {
        private static readonly TraceSource loggerSource;

        private static readonly Logging.LoggingInterfaces.ITracer logger;

        private readonly LocalDataContext context;

        static LocalDatabaseWrapper()
        {
            loggerSource = Logging.Source("Yaaf.WirePlugin.WinFormGui.LocalDatabaseWrapper", "");
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

                var matchSessionsPlayers = allchanges.Select(o => o as MatchSessions_Player).Where(o => o != null);

                foreach (var matchSessionsPlayer in matchSessionsPlayers)
                {
                    if (matchSessionsPlayer.Player.MyId != 0)
                    {
                        var newPlayer = FSharpInterop.Interop.Database.GetPlayerById(
                            this, matchSessionsPlayer.Player.MyId);
                        var oldPlayer = matchSessionsPlayer.Player;
                        if (newPlayer != null)
                        {
                            UpdateMatchSessionPlayer(oldPlayer, newPlayer, matchSessionsPlayer);
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
                            UpdateMatchSessionPlayer(oldPlayer, newPlayer, matchSessionsPlayer);
                        }
                    }
                }
                var matchmedias = allchanges.Select(o => o as Matchmedia).Where(o => o != null).Cache();

                foreach (var matchmedia in matchmedias)
                {
                    if (matchmedia.MyMatchSessionsPlayer != null)
                    {
                        matchmedia.MatchSessions_Player = matchmedia.MyMatchSessionsPlayer;
                    }
                } 
                
                var watchFolders = allchanges.Select(o => o as WatchFolder).Where(o => o != null).Cache();

                foreach (var watchFolder in watchFolders)
                {
                    if (watchFolder.MyGame != null)
                    {
                        watchFolder.Game = watchFolder.MyGame;
                    }
                }

                var deletedSessions = changes.Deletes.Select(o => o as MatchSession).Where(o => o != null);
                foreach (var deletedSession in deletedSessions)
                {
                    FSharpInterop.Interop.Database.DeleteMatchSession(this, true, deletedSession);
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
                logger.LogError("Exception: {0}", e);
                throw;
            }
        }

        private void UpdateMatchSessionPlayer(Player oldPlayer, Player newPlayer, MatchSessions_Player matchSessionsPlayer)
        {
            matchSessionsPlayer.Player = newPlayer;
            newPlayer.EslPlayerId = oldPlayer.EslPlayerId;
            newPlayer.MyTags = oldPlayer.MyTags;
            newPlayer.Name = oldPlayer.Name;
            if (oldPlayer.Id == 0)
            {
                Context.Players.DeleteOnSubmit(oldPlayer);
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