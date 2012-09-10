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

    using Yaaf.WirePlugin.WinFormGui.Database;
    using Yaaf.WirePlugin.WinFormGui.Properties;

    public class LocalDatabaseWrapper
    {
        private static readonly TraceSource loggerSource;

        private static readonly Logging.LoggingInterfaces.ITracer logger;

        private readonly LocalDatabaseDataContext context;

        static LocalDatabaseWrapper()
        {
            loggerSource = Logging.Source("Yaaf.WirePlugin.WinFormGui.LocalDatabaseWrapper", "");
            logger = Logging.DefaultTracer(loggerSource, "Initialization");
        }

        public LocalDatabaseWrapper(LocalDatabaseDataContext context)
        {
            this.context = context;
        }

        public LocalDatabaseDataContext Context
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

                var newContext = new LocalDatabaseDataContext(context.Connection);
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
            return new LocalDatabaseWrapper(new LocalDatabaseDataContext(context.Connection));
        }

        public ActionObject GetMoveToMatchmediaActionObject()
        {
            var wasNotSet = false;
            if (Settings.Default.DefaultEslWarAction == -1)
            {
                wasNotSet = true;
                var contextCopy = Copy();
                var copyAction = contextCopy.GetAction("CopyToEslMatchmedia");

                var obj = new ActionObject();
                obj.Action = copyAction;
                obj.Name = "DefaultActionObject_CopyToEslMatchMedia";

                contextCopy.Context.ActionObjects.InsertOnSubmit(obj);
                contextCopy.Context.SubmitChanges();
                Settings.Default.DefaultEslWarAction = obj.Id;
                Settings.Default.Save();
            }

            var id = Settings.Default.DefaultEslWarAction;
            var t = (from tag in context.ActionObjects where tag.Id == id select tag);
            try
            {
                return t.Single();
            }
            catch (InvalidOperationException e)
            {
                if (!wasNotSet)
                {
                    logger.LogError(
                        "Could not find DefaultActionObject_CopyToEslMatchMedia object trying to reset (Error: {0}", e);
                    Settings.Default.DefaultEslWarAction = -1;
                    return GetMoveToMatchmediaActionObject();
                }

                throw;
            }
        }

        private Actions GetAction(string name)
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
    }
}