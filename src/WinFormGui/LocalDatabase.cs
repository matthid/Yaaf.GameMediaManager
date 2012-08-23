namespace Yaaf.WirePlugin.WinFormGui.Database
{
    partial class Game
    {
        partial void OnCreated()
        {
            this.EnableMatchForm = true;
            this.EnableNotification = true;
            this.EnablePublicNotification = true;
            this.EnableWarMatchForm = true;
            this.WarMatchFormSaveFiles = true;
            this.PublicMatchFormSaveFiles = true;
        }
    }

    partial class MatchSessions_Player
    {
        private string dataGridHelperName;

        public string DataGridHelper_Name
        {
            get
            {
                return this.dataGridHelperName;
            }
            set
            {
                this.SendPropertyChanging();
                this.dataGridHelperName = value;
                this.SendPropertyChanged("DataGridHelper_Name");
            }
        }

        private int dataGridHelperEslPlayerId;

        public int DataGridHelper_EslPlayerId
        {
            get
            {
                return this.dataGridHelperEslPlayerId;
            }
            set
            {
                this.SendPropertyChanging();
                this.dataGridHelperEslPlayerId = value;
                this.SendPropertyChanged("DataGridHelper_EslPlayerId");
            }
        }
    }

    partial class WatchFolder
    {
    }

    partial class LocalDatabaseDataContext
    {
    }
}
