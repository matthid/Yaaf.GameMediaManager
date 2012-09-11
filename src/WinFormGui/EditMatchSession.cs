using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Yaaf.WirePlugin.WinFormGui
{
    using Yaaf.WirePlugin.WinFormGui.Database;

    public partial class EditMatchSession : Form
    {
        private readonly Logging.LoggingInterfaces.ITracer logger;

        private readonly MatchSession session;

        private ManagePlayersHelper helper;

        public EditMatchSession(Logging.LoggingInterfaces.ITracer logger, LocalDatabaseWrapper context, MatchSession session)
        {
            this.logger = logger;
            this.session = session;
            InitializeComponent();
            session.Matchmedia.Load();
            helper =
                new ManagePlayersHelper(
                    context,
                    session,
                    matchPlayersDataGridView,
                    matchSessionsPlayerBindingSource,
                    teamDataGridViewTextBoxColumn,
                    skillDataGridViewTextBoxColumn);
        }

        private void matchPlayersDataGridView_SelectionChanged(object sender, EventArgs e)
        {
            var selectedItems = 
                matchPlayersDataGridView
                    .SelectedRows
                    .Cast<DataGridViewRow>()
                    .Select <DataGridViewRow, MatchSessions_Player>(
                        f => (MatchSessions_Player)f.DataBoundItem);

            var matchSessionsPlayers = selectedItems as List<MatchSessions_Player> ?? selectedItems.ToList();
            if (!matchSessionsPlayers.Any())
            {
                var item = matchPlayersDataGridView.CurrentRow == null
                               ? null
                               : (MatchSessions_Player)matchPlayersDataGridView.CurrentRow.DataBoundItem;
                if (item == null)
                {
                    item = (MatchSessions_Player)playerBindingSource.Current;
                }

                if (item != null)
                {
                    matchSessionsPlayers = new List<MatchSessions_Player>() { item };

                }
            }

            var table = new DataTable("MatchmediaTable");
            table.Columns.Add("Id", typeof(int));
            table.Columns.Add("Created", typeof(DateTime));
            table.Columns.Add("Map", typeof(string));
            table.Columns.Add("Name", typeof(string));
            table.Columns.Add("Path", typeof(string));
            table.Columns.Add("Type", typeof(string));
            foreach (var player in matchSessionsPlayers)
            {
                MatchSessions_Player player1 = player;
                if (player1 == null) continue;
                foreach (var matchmedia in from f in session.Matchmedia
                                           where f.Player == player1.Player
                                           select f)
                {
                    var row = table.NewRow();
                    row["Id"] = matchmedia.Id;
                    row["Created"] = matchmedia.Created;
                    row["Map"] = matchmedia.Map;
                    row["Name"] = matchmedia.Name;
                    row["Path"] = matchmedia.Path;
                    row["Type"] = matchmedia.Type;
                    table.Rows.Add(row);
                }
            }
            matchmediaBindingSource.DataSource = table;
        }

        private void matchPlayersDataGridView_MultiSelectChanged(object sender, EventArgs e)
        {
            var selectedItems =
                matchPlayersDataGridView
                    .SelectedRows
                    .Cast<DataGridViewRow>()
                    .Select<DataGridViewRow, MatchSessions_Player>(
                        f => (MatchSessions_Player)f.DataBoundItem);
            var table = new DataTable("MatchmediaTable");
            table.Columns.Add("Id", typeof(int));
            table.Columns.Add("Created", typeof(DateTime));
            table.Columns.Add("Map", typeof(string));
            table.Columns.Add("Name", typeof(string));
            table.Columns.Add("Path", typeof(string));
            table.Columns.Add("Type", typeof(string));
            foreach (var player in selectedItems)
            {
                MatchSessions_Player player1 = player;
                foreach (var matchmedia in from f in session.Matchmedia
                                           where f.Player == player1.Player
                                           select f)
                {
                    var row = table.NewRow();
                    row["Id"] = matchmedia.Id;
                    row["Created"] = matchmedia.Created;
                    row["Map"] = matchmedia.Map;
                    row["Name"] = matchmedia.Name;
                    row["Path"] = matchmedia.Path;
                    row["Type"] = matchmedia.Type;
                    table.Rows.Add(row);
                }
            }
            matchmediaBindingSource.DataSource = table;
        }

        private void EditMatchSession_Load(object sender, EventArgs e)
        {
            try
            {
                helper.Load();
            }
            catch (Exception ex)
            {
                ex.ShowError(logger, "Can't load EditMatchSession-Form");
                Close();
            }
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            try
            {
                helper.Save();
                Close();
            }
            catch (Exception ex)
            {
                ex.ShowError(logger, "Can't save MatchSession");
            }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void dataGridView2_DragDrop(object sender, DragEventArgs e)
        {

        }

        private void showInExplorerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var matchmedia = (Matchmedia)matchmediaBindingSource.Current;
            if (matchmedia == null)
            {
                MessageBox.Show("Please select a File first!");
                return;
            }
        }

        private void copyFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var matchmedia = (Matchmedia)matchmediaBindingSource.Current;
            if (matchmedia == null)
            {
                MessageBox.Show("Please select a File first!");
                return;
            }
        }

        private void matchPlayersDataGridView_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            logger.LogWarning("{0}", "DataError: " + e.Exception);
        }


        
    }
}
