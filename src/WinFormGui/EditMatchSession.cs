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
    using System.Reflection;

    using Yaaf.WirePlugin.Primitives;
    using Yaaf.WirePlugin.WinFormGui.Database;

    public partial class EditMatchSession : Form
    {
        private readonly Logging.LoggingInterfaces.ITracer logger;

        private readonly LocalDatabaseWrapper context;

        private readonly WrapperDataTable.WrapperTable<Matchmedia> matchmediaData;

        private readonly MatchSession session;

        private ManagePlayersHelper helper;

        private DataTableWatcher watcher;

        private WrapperDataTable.WrapperTable<Matchmedia> oldWrapper;

        private WrapperDataTable.WrapperTable<Matchmedia> matchmediaDataCopy;

        public EditMatchSession(Logging.LoggingInterfaces.ITracer logger, LocalDatabaseWrapper context, WrapperDataTable.WrapperTable<Matchmedia> matchmediaData, MatchSession session)
        {
            this.logger = logger;
            this.context = context;
            this.matchmediaData = matchmediaData;
            this.matchmediaDataCopy = matchmediaData.Clone();
            this.session = session;
            InitializeComponent();
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
            var matchSessionsPlayers = GetSelectedPlayers();
            var primaryPlayer = matchSessionsPlayers.Item2;
            IEnumerable<Matchmedia> media = new Matchmedia[0];
            media =
                matchSessionsPlayers.Item1
                    .Where(player1 => player1 != null)
                    .Aggregate(
                        media,
                        (current, player1) => current.Union(from f in matchmediaDataCopy.CopyLinqData where f.PlayerId == player1.PlayerId select f));

            var value = matchmediaDataCopy.GetCopiedTable(media);
            value.SetInitializer(mediaM =>
                    {
                        mediaM.Created = DateTime.Now; mediaM.MatchSession = session;
                        mediaM.Player = primaryPlayer.Player;
                    });
           
            SetNewWrapper(value);
        }

        private void SetNewWrapper(WrapperDataTable.WrapperTable<Matchmedia> value)
        {
            if (oldWrapper != null)
            {
                matchmediaDataCopy.ImportChanges(oldWrapper);
            }

            oldWrapper = value;
            matchmediaBindingSource.DataSource = value.SourceTable;
        }

        private Tuple<List<MatchSessions_Player>, MatchSessions_Player> GetSelectedPlayers()
        {
            var selectedItems =
                matchPlayersDataGridView.SelectedRows.Cast<DataGridViewRow>().Select<DataGridViewRow, MatchSessions_Player>(
                    f => (MatchSessions_Player)f.DataBoundItem);

            var matchSessionsPlayers = selectedItems as List<MatchSessions_Player> ?? selectedItems.ToList();

            var primaryPlayer = matchPlayersDataGridView.CurrentRow == null
                               ? null
                               : (MatchSessions_Player)matchPlayersDataGridView.CurrentRow.DataBoundItem;
            if (primaryPlayer == null)
            {
                primaryPlayer = (MatchSessions_Player)playerBindingSource.Current;
            }

            
            if (!matchSessionsPlayers.Any())
            {
                if (primaryPlayer != null)
                {
                    matchSessionsPlayers = new List<MatchSessions_Player>() { primaryPlayer };
                }
            }
            return Tuple.Create(matchSessionsPlayers, primaryPlayer);
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
                matchmediaDataCopy.ImportChanges(oldWrapper);
                matchmediaData.ImportChanges(matchmediaDataCopy);

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
