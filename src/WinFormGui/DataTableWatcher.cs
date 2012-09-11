namespace Yaaf.WirePlugin.WinFormGui
{
    using System.Data;

    public class DataTableWatcher
    {
        private readonly Logging.LoggingInterfaces.ITracer logger;

        private DataTableWatcher(Logging.LoggingInterfaces.ITracer logger, DataTable table)
        {
            this.logger = logger;
            table.RowChanged += table_RowChanged;
            table.RowChanging += table_RowChanging;
            table.RowDeleted += table_RowDeleted;
            table.RowDeleting += table_RowDeleting;
            table.TableNewRow += table_TableNewRow;
        }

        void table_TableNewRow(object sender, DataTableNewRowEventArgs e)
        {
            logger.LogVerbose("table_TableNewRow");
        }

        void table_RowDeleting(object sender, DataRowChangeEventArgs e)
        {
            logger.LogVerbose("table_RowDeleting");
        }

        void table_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            logger.LogVerbose("table_RowDeleted");
        }

        void table_RowChanging(object sender, DataRowChangeEventArgs e)
        {
            logger.LogVerbose("table_RowChanging");
        }

        void table_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            logger.LogVerbose("RowChanged");
        }

        public static DataTableWatcher WatchTable(Logging.LoggingInterfaces.ITracer logger, DataTable table)
        {
            return new DataTableWatcher(logger, table);
        }
    }
}