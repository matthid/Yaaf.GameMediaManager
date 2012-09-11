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

        private ManagePlayersHelper helper;

        public EditMatchSession(Logging.LoggingInterfaces.ITracer logger, LocalDatabaseWrapper context, MatchSession session)
        {
            this.logger = logger;
            InitializeComponent();

            helper =
                new ManagePlayersHelper(
                    context,
                    session,
                    matchPlayersDataGridView,
                    matchSessionsPlayerBindingSource,
                    teamDataGridViewTextBoxColumn,
                    skillDataGridViewTextBoxColumn);
            matchSessionsPlayerBindingSource.CurrentItemChanged += new EventHandler(matchSessionsPlayerBindingSource_CurrentItemChanged);
        }

        void matchSessionsPlayerBindingSource_CurrentItemChanged(object sender, EventArgs e)
        {

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
