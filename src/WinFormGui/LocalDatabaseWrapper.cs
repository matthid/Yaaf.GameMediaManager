namespace Yaaf.WirePlugin.WinFormGui
{
    using System;
    using System.Collections.Generic;
    using System.Data.Linq;
    using System.Diagnostics;
    using System.Linq;

    using Yaaf.WirePlugin.WinFormGui.Database;

    public class LocalDatabaseWrapper
    {
        private readonly LocalDatabaseDataContext context;

        private static TraceSource loggerSource;

        private static Logging.LoggingInterfaces.ITracer logger;

        public LocalDatabaseWrapper(Database.LocalDatabaseDataContext context)
        {
            this.context = context;
        }
        static LocalDatabaseWrapper()
        {

            loggerSource = Logging.Source("Yaaf.WirePlugin.WinFormGui.LocalDatabaseWrapper", "");
            logger = Logging.DefaultTracer(loggerSource, "Initialization");
        }

        public LocalDatabaseDataContext Context
        {
            get
            {
                return this.context;
            }
        }

        public Tag GetTag (string tagString)
        {
            try
            {
                var t = (from tag in context.Tags
                         where tag.Name == tagString
                         select tag).SingleOrDefault();
                if (t != null)
                {
                    return t;
                }

                var newContext = new LocalDatabaseDataContext(context.Connection);
                t = new Tag();
                t.Name = tagString;
                newContext.Tags.InsertOnSubmit(t);
                newContext.SubmitChanges();
                return (from tag in this.context.Tags where tag.Id == t.Id select tag).Single();
            }
            catch (Exception ex)
            {
                logger.LogError("Could not create tag: {0}", ex);

                // Maybe added by another thread
                return (from tag in this.context.Tags where tag.Name == tagString select tag).Single();
            }
        }

        public void UpdateDatabase<T>(Table<T> table, IEnumerable<T> changedItems, IList<T> originalItems) where T : class
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
    }
}