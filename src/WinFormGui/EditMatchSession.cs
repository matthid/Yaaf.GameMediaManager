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
    using System.Collections.Specialized;
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

        private MatchSessions_Player primaryPlayer;

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
            primaryPlayer = matchSessionsPlayers.Item2;
            primaryPlayerLabel.Text = primaryPlayer == null ? "None" : string.Format("{0} ({1})", primaryPlayer.Player.Name, primaryPlayer.PlayerId);

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
            return matchPlayersDataGridView.GetSelection<MatchSessions_Player>(playerBindingSource);
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
            try
            {
                if (primaryPlayer == null)
                {
                    MessageBox.Show("Please select a Player first!");
                    return;
                }

                var files = (string[])e.Data.GetData(DataFormats.FileDrop);
                foreach (var file in files)
                {
                    AddMatchmedia(file);
                }
            }
            catch (Exception ex)
            {
                ex.ShowError(logger, "Drag&Drop error!");
            }
        }
        private void AddMatchmedia (string file)
        {
            var row = (System.Data.DataRowView)matchmediaBindingSource.AddNew();
            if (row == null)
            {
                throw new InvalidOperationException("Couldn't add new Matchmedia (was null)");
            } 
            var media = oldWrapper.GetCopyItem(row.Row);
            media.AddDataFromFile(file);
            oldWrapper.UpdateItem(media);
        }
        private void showInExplorerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try {
                var row = (DataRowView)matchmediaBindingSource.Current;
                if (row == null)
                {
                    MessageBox.Show("Please select a File first!");
                    return;
                }
                var matchmedia = oldWrapper.GetCopyItem(row.Row);
                if (!System.IO.File.Exists(matchmedia.Path))
                {
                    MessageBox.Show("Matchmedia was deleted!");
                    return;
                }
                System.Diagnostics.Process.Start(System.IO.Path.GetDirectoryName(matchmedia.Path));
            }
            catch (Exception ex)
            {
                ex.ShowError(logger, "Show in explorer error!");
            }
        }

        private void copyFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                var selection = playersDataGridView.GetSelection<DataRowView>(playerBindingSource);
                var paths = new StringCollection();
                paths.AddRange(selection.Item1.Select(r => oldWrapper.GetCopyItem(r.Row)).Select(m => m.Path).ToArray());
                Clipboard.SetFileDropList(paths);
            }
            catch (Exception ex)
            {
                ex.ShowError(logger, "Copy media to clipboard error!");
            }
        }

        private void matchPlayersDataGridView_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            logger.LogWarning("{0}", "DataError: " + e.Exception);
        }

        private void dataGridView2_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Copy;
        }


        
    }
}
