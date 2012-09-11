// ----------------------------------------------------------------------------
// This file (ManageMatchPlayers.cs) is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package (Yaaf.WirePlugin).
// Last Modified: 2012/09/10 14:08
// Created: 2012/08/26 20:57
// ----------------------------------------------------------------------------

namespace Yaaf.WirePlugin.WinFormGui
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Windows.Forms;

    using Yaaf.WirePlugin.WinFormGui.Database;

    public partial class ManageMatchPlayers : Form
    {
        private readonly Logging.LoggingInterfaces.ITracer logger;

        private readonly ManagePlayersHelper helper;

        public ManageMatchPlayers(
            Logging.LoggingInterfaces.ITracer logger, LocalDatabaseWrapper context, MatchSession session)
        {
            InitializeComponent();
            this.logger = logger;
            this.helper = 
                new ManagePlayersHelper(
                    context,
                    session,
                    matchPlayersDataGridView, 
                    matchSessionsPlayerBindingSource, 
                    teamDataGridViewTextBoxColumn, 
                    skillDataGridViewTextBoxColumn);
        }

        private void ManageMatchPlayers_Load(object sender, EventArgs e)
        {
            Logging.setupLogging(logger);
            try
            {
                helper.Load();
            }
            catch (Exception ex)
            {
                ex.ShowError(logger, "Can't load player matchdata");
                Close();
            }
        }

        private void save_Click(object sender, EventArgs e)
        {
            try
            {
                helper.Save();
                Close();
            }
            catch (Exception ex)
            {
                ex.ShowError(logger, "Can't save player matchdata");
            }
        }

        private void cancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void matchPlayersDataGridView_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            logger.LogWarning("{0}", "DataError: " + e.Exception);
        }
    }
}