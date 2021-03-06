﻿// ----------------------------------------------------------------------------
// This file (ManagePlayers.cs) is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package (Yaaf.GameMediaManager).
// Last Modified: 2012/09/10 14:08
// Created: 2012/08/26 20:57
// ----------------------------------------------------------------------------

namespace Yaaf.GameMediaManager.WinFormGui
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows.Forms;

    using Yaaf.GameMediaManager.WinFormGui.Database;
    using Yaaf.GameMediaManager.WinFormGui.Properties;

    public partial class ManagePlayers : Form
    {
        private readonly Logging.LoggingInterfaces.ITracer logger;

        private readonly LocalDatabaseWrapper wrapper;

        private Player me;

        public ManagePlayers(Logging.LoggingInterfaces.ITracer logger)
        {
            this.logger = logger;

            // this is a copy.. this way we can discard everything at the end, if we need to
            wrapper = FSharpInterop.Interop.GetNewContext();
            InitializeComponent();
        }

        private void CurrentPlayer_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            SetMe((Player)sender);
        }

        private void ManagePlayers_Load(object sender, EventArgs e)
        {
            this.SetupForm(logger);
            try
            {
                var task = Primitives.Task.FromDelegate(
                () =>
                {
                    foreach (var player in wrapper.Context.Players)
                    {
                    }
                    return wrapper.Context.Players;
                });
                WaitingForm.StartTask(logger, task, "loading players...");
                playerBindingSource.DataSource = task.Result.Value;
                SetMe(FSharpInterop.Interop.Database.GetIdentityPlayer(wrapper));
            }
            catch (Exception ex)
            {
                ex.ShowError(logger, "Could not load ManagePlayers");
                this.Close();
            }
            
        }
        
        private void SetMe(Player player)
        {
            var oldPlayer = me;
            if (oldPlayer != null)
            {
                oldPlayer.PropertyChanged -= CurrentPlayer_PropertyChanged;
            }

            me = player;

            if (player != null)
            {
                player.PropertyChanged += CurrentPlayer_PropertyChanged;
                meLabel.Text = string.Format(
                    "{0}, ID: {1}, EslID: {2}",
                    player.Name,
                    player.Id != 0 ? me.Id.ToString() : "Not Set",
                    player.EslPlayerId.HasValue ? player.EslPlayerId.Value.ToString() : "Not Set");
            }
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            try
            {
                // Add all new, delete all deleted and update all changed games.
                //wrapper.MySubmitChanges();
                Close();
            }
            catch (Exception ex)
            {
                logger.LogError("{0}", "Can't change player changes: " + ex);
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}