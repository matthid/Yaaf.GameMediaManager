// ----------------------------------------------------------------------------
// This file (ManageWatchFolder.cs) is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package (Yaaf.WirePlugin).
// Last Modified: 2012/09/10 14:08
// Created: 2012/08/26 20:57
// ----------------------------------------------------------------------------

namespace Yaaf.WirePlugin.WinFormGui
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Forms;

    using Yaaf.WirePlugin.WinFormGui.Database;

    public partial class ManageWatchFolder : Form
    {
        private readonly LocalDatabaseWrapper context;

        private readonly Game game;

        private readonly Logging.LoggingInterfaces.ITracer logger;

        private List<WatchFolder> old;

        public ManageWatchFolder(Logging.LoggingInterfaces.ITracer logger, LocalDatabaseWrapper context, Game game)
        {
            this.logger = logger;
            this.context = context;
            this.game = game;
            InitializeComponent();
        }

        private void ManageWatchFolder_Load(object sender, EventArgs e)
        {
            Logging.setupLogging(logger);
            try
            {
                var folders =
                    (from g in context.Context.WatchFolders where g.Game == game select g).AsEnumerable().Union(
                        context.Context.GetChangeSet().Inserts.Where(d => d is WatchFolder).Select(d => (WatchFolder)d))
                        .Except(
                            context.Context.GetChangeSet().Deletes.Where(d => d is WatchFolder).Select(
                                d => (WatchFolder)d));

                old = new List<WatchFolder>(folders);
                watchFolderBindingSource.DataSource = new List<WatchFolder>(old);
            }
            catch (Exception ex)
            {
                logger.LogError("{0}", "Can't load watchfolder data: " + ex);
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Close();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                // Add all new, delete all deleted and update all changed games.
                foreach (var w in watchFolderBindingSource.Cast<WatchFolder>())
                {
                    w.Game = game;
                }

                context.UpdateDatabase(context.Context.WatchFolders, watchFolderBindingSource.Cast<WatchFolder>(), old);
                Close();
            }
            catch (Exception ex)
            {
                logger.LogError("{0}", "Can't change watchfolder changes: " + ex);
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}