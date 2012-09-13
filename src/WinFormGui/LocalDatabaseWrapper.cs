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
    using System.Data.SqlClient;
    using System.Diagnostics;
    using System.Linq;

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
            var newContext = new LocalDatabaseWrapper(new LocalDataContext(context.Connection));
            //var newChanges = context.GetChangeSet();
            //foreach (var d in newChanges.Deletes)
            //{
            //    var table = newContext.Context.GetTable(d.GetType());
            //    table.DeleteOnSubmit(d);
            //} 
            //foreach (var i in newChanges.Inserts)
            //{
            //    var table = newContext.Context.GetTable(i.GetType());
            //    table.InsertOnSubmit(i);
            //} 
            //foreach (var u in newChanges.Updates)
            //{
            //    var table = newContext.Context.GetTable(u.GetType());
            //    var changes =
            //        table.GetModifiedMembers(u);

            //}
            return newContext;
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
                Context.SubmitChanges();
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

        public void UpdateMatchSessionPlayerTable(WrapperDataTable.WrapperTable<MatchSessions_Player> playerTable)
        {
            var localDataContext = Context;
            var removeTags =
                playerTable.UpdateTable(localDataContext.MatchSessions_Players).Where(p => p.RemoveTags.Count > 0).Select(
                    p => (IEnumerable<Player_Tag>)p.RemoveTags).Union(new[] { new Player_Tag[0] }).Aggregate(
                        (left, right) => left.Union(right));
            localDataContext.Player_Tags.DeleteAllOnSubmit(removeTags);
        }
    }
}