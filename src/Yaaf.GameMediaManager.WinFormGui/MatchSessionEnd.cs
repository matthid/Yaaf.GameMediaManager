// ----------------------------------------------------------------------------
// This file (MatchSessionEnd.cs) is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package (Yaaf.GameMediaManager).
// Last Modified: 2012/09/13 19:20
// Created: 2012/08/26 20:57
// ----------------------------------------------------------------------------

namespace Yaaf.GameMediaManager.WinFormGui
{
    using System;
    using System.Data;
    using System.Linq;
    using System.Windows.Forms;

    using Microsoft.FSharp.Control;
    using Microsoft.FSharp.Core;

    using Yaaf.GameMediaManager.WinFormGui.Database;

    using WrapperMatchmediaTable = Primitives.WrapperDataTable.WrapperTable<Database.Matchmedia>;
    using WrapperPlayerTable = Primitives.WrapperDataTable.WrapperTable<Database.MatchSessions_Player>;
    using SessionData =
        System.Tuple
            <Primitives.WrapperDataTable.WrapperTable<Database.Matchmedia>,
                Primitives.WrapperDataTable.WrapperTable<Database.MatchSessions_Player>>;


    public partial class MatchSessionEnd : Form
    {
        private readonly LocalDatabaseWrapper context;

        private readonly Logging.LoggingInterfaces.ITracer logger;

        private readonly WrapperMatchmediaTable matchmediaTableCopy;

        private readonly WrapperPlayerTable playerTableCopy;

        private readonly MatchSession session;

        private readonly SessionData sessionData;

        private MatchSessions_Player primaryPlayer;

        public MatchSessionEnd(
            Logging.LoggingInterfaces.ITracer logger,
            LocalDatabaseWrapper context,
            SessionData sessionData,
            MatchSession session)
        {
            this.logger = logger;
            this.context = context;
            this.sessionData = sessionData;
            matchmediaTableCopy = sessionData.Item1.Clone();

            playerTableCopy = sessionData.Item2.Clone();
            this.session = session;
            InitializeComponent();
        }

        public bool? DeleteMatchmedia { get; private set; }

        private void MatchSessionEnd_Load(object sender, EventArgs e)
        {
            this.SetupForm(logger);
            try
            {
                var me = FSharpInterop.Interop.Database.GetIdentityPlayer(context);
                primaryPlayer = session.MatchSessions_Player.FirstOrDefault(p => p.Player == me);
                if (primaryPlayer == null)
                {
                    primaryPlayer = session.MatchSessions_Player.FirstOrDefault();
                    if (primaryPlayer == null)
                    {
                        primaryPlayer = new MatchSessions_Player(){Player = me, MyMatchSession = session};
                        playerTableCopy.Add(primaryPlayer);
                    }
                }
                tagTextBox.Text = session.MyTags;
                matchNameTextBox.Text = session.Name;
                linkLabel.Text = session.EslMatchLink;
                playerTableCopy.UserAddedRow += playerTableCopy_UserAddedRow;
                matchmediaBindingSource.DataSource = matchmediaTableCopy.SourceTable;
            }
            catch (Exception ex)
            {
                ex.ShowError(logger, "Could not load SessionEnd-View");
                Close();
            }
        }

        void playerTableCopy_UserAddedRow(object sender, MatchSessions_Player args)
        { // can't happen actually.
            args.MyMatchSession = session;
        }

        private void saveMatchmediaButton_Click(object sender, EventArgs e)
        {
            try
            {
                SaveData();

                SetupRemember(true);
                DeleteMatchmedia = false;
                Close();
            }
            catch (Exception ex)
            {
                ex.ShowError(logger, "Could not save Matchmedia");
            }
        }

        private void SaveData()
        {
            session.SetOriginal();
            session.MyTags = tagTextBox.Text;
            session.EslMatchLink = linkLabel.Text;
            if (matchNameTextBox.Text.Length > 100)
            {
                throw new InvalidOperationException("Matchname can't exceed 100 characters!");
            }
            session.Name = matchNameTextBox.Text;
            sessionData.Item1.ImportChanges(matchmediaTableCopy);
            sessionData.Item2.ImportChanges(playerTableCopy);
        }

        private void deleteMatchmediaButton_Click(object sender, EventArgs e)
        {
            try
            {
                SaveData();
                SetupRemember(false);
                DeleteMatchmedia = true;
            }
            catch (Exception ex)
            {
                ex.ShowError(logger, "Could not delete Matchmedia");
            }
        }

        private void SetupRemember(bool saveData)
        {
            if (!rememberCheckBox.Checked || !rememberCheckBox.Enabled)
            {
                return;
            }

            var game = session.Game;
            if (string.IsNullOrEmpty(session.EslMatchLink))
            {
                // Public Mode
                game.EnableMatchForm = false;
                game.PublicMatchFormSaveFiles = saveData;
            }
            else
            {
                game.EnableWarMatchForm = false;
                game.WarMatchFormSaveFiles = saveData;
            }
        }

        private void AddMatchmedia(string safeFileName)
        {
            if (matchmediaTableCopy == null)
            {
                throw new InvalidOperationException("Couldn't add new Matchmedia (was null)");
            }
            var media = Helpers.MediaFromFile(safeFileName, primaryPlayer);
            matchmediaTableCopy.Add(media);
        }

        private void switchToAdvancedViewButton_Click(object sender, EventArgs e)
        {
            try
            {
                SaveData();

                var managePlayer = new EditMatchSession(logger, context, sessionData, session, true);
                Visible = false;
                managePlayer.ShowDialog();
                DeleteMatchmedia = managePlayer.DeleteMatchmedia;
                Close();
            }
            catch (Exception ex)
            {
                ex.ShowError(logger, "Could not open EditMatchSession");
            }
        }

        private void fetchMatchDatabutton_Click(object sender, EventArgs e)
        {
            try
            {
                var players = Helpers.ShowLoadMatchDataDialog(logger, session.EslMatchLink);
                FSharpInterop.Interop.FillWrapperTable(session, players.Item1, playerTableCopy, matchmediaTableCopy);
                linkLabel.Text = players.Item2;
                rememberCheckBox.Enabled = false;
            }
            catch (Exception ex)
            {
                ex.ShowError(logger, "Could not load data");
            }
        }

        private void matchmediaDataGridView_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                var files = (string[])e.Data.GetData(DataFormats.FileDrop);
                foreach (var file in files)
                {
                    AddMatchmedia(file);
                }
                rememberCheckBox.Enabled = false;
            }
            catch (Exception ex)
            {
                ex.ShowError(logger, "Drag&Drop error!");
            }
        }

        private void matchmediaDataGridView_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }
    }
}