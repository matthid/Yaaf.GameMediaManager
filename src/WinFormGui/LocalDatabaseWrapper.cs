namespace Yaaf.WirePlugin.WinFormGui
{
    using System.Collections.Generic;
    using System.Data.Linq;

    using Yaaf.WirePlugin.WinFormGui.Database;

    public class LocalDatabaseWrapper
    {
        private readonly LocalDatabaseDataContext context;


        public LocalDatabaseWrapper(Database.LocalDatabaseDataContext context)
        {
            this.context = context;
        }


        public LocalDatabaseDataContext Context
        {
            get
            {
                return this.context;
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
            this.Context.SubmitChanges();
        }

    }
}