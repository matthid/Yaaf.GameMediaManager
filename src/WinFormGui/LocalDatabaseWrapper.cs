namespace Yaaf.WirePlugin.WinFormGui
{
    using System;
    using System.Collections.Generic;
    using System.Data.Linq;
    using System.Data.SqlClient;
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

        public ActionObject GetMoveToMatchmediaActionObject()
        {
            bool wasNotSet = false;
            if (Properties.Settings.Default.DefaultEslWarAction == -1)
            {
                wasNotSet = true;
                var contextCopy = this.Copy();
                var noneFilter = contextCopy.GetActionAndFilter("None");
                var copyAction = contextCopy.GetActionAndFilter("CopyToEslMatchmedia");

                var obj = new ActionObject();
                obj.Action = copyAction;
                obj.Filter = noneFilter;
                obj.Name = "DefaultActionObject_CopyToEslMatchMedia";

                contextCopy.Context.ActionObjects.InsertOnSubmit(obj);
                contextCopy.Context.SubmitChanges();
                Properties.Settings.Default.DefaultEslWarAction = obj.Id;
                Properties.Settings.Default.Save();
                //var paramCopy = new ObjectParameter();
                //paramCopy.ActionObject = obj;
                //paramCopy.ParamNum = 1;
                //paramCopy.Type = 2;
                //var s = System.IO.Path.DirectorySeparatorChar;
                //paramCopy.Parameter = "{11}" + s + "{6}_"
            }

            var id = Properties.Settings.Default.DefaultEslWarAction;
            var t = (from tag in this.context.ActionObjects where tag.Id == id select tag);
            try 
	        {	        
		        return t.Single();
	        }
	        catch (InvalidOperationException e)
	        {
                if (!wasNotSet)
                {
                    logger.LogError("Could not find DefaultActionObject_CopyToEslMatchMedia object trying to reset (Error: {0}", e);
                    Properties.Settings.Default.DefaultEslWarAction = -1;
                    return this.GetMoveToMatchmediaActionObject();
                }
                else
                {
                    throw;
                }
	        } 
        }

        private ActionAndFilter GetActionAndFilter(string name)
        {
            return (from tag in this.context.ActionAndFilters where tag.Name == name select tag).Single();
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