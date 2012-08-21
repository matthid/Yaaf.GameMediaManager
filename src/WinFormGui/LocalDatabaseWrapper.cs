namespace Yaaf.WirePlugin.WinFormGui
{
    using Yaaf.WirePlugin.WinFormGui.Database;

    public class LocalDatabaseWrapper
    {
        private readonly LocalDatabaseDataContext context;


        public LocalDatabaseWrapper(Database.LocalDatabaseDataContext context)
        {
            this.context = context;
        }

        public Game GetGame(int id)
        {
            return null;
        }
    }
}