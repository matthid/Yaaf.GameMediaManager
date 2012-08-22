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
    using System.Diagnostics;

    using Yaaf.WirePlugin.WinFormGui.Database;

    public partial class ManageWatchFolder : Form
    {
        private readonly Action<TraceEventType, string> logger;

        private readonly LocalDatabaseWrapper context;

        private readonly Game game;

        private List<WatchFolder> old;


        public ManageWatchFolder(Action<TraceEventType, string> logger, LocalDatabaseWrapper context, Game game)
        {
            this.logger = logger;
            this.context = context;
            this.game = game;
            InitializeComponent();
        }

        private void ManageWatchFolder_Load(object sender, EventArgs e)
        {
            try
            {
                var folders = from g in this.context.Context.WatchFolders
                              where g.Game == this.game
                              select g;
                old = new System.Collections.Generic.List<Database.WatchFolder>(folders);
                watchFolderBindingSource.DataSource = new System.Collections.Generic.List<Database.WatchFolder>(old);
            }
            catch (Exception ex)
            {
                logger(TraceEventType.Error, "Can't load watchfolder data: " + ex);
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.Close();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                // Add all new, delete all deleted and update all changed games.
                foreach (var w in watchFolderBindingSource.Cast<WatchFolder>())
                {
                    w.Game = this.game;
                }

                context.UpdateDatabase(context.Context.WatchFolders, watchFolderBindingSource.Cast<WatchFolder>(), old);
                this.Close();
            }
            catch (Exception ex)
            {
                logger(TraceEventType.Error, "Can't change watchfolder changes: " + ex);
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
