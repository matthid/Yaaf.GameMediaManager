// ----------------------------------------------------------------------------
// This file (EditMatchSession.cs) is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package (Yaaf.WirePlugin).
// Last Modified: 2012/09/13 19:23
// Created: 2012/09/13 08:49
// ----------------------------------------------------------------------------

namespace Yaaf.WirePlugin.WinFormGui
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Data;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Windows.Forms;

    using Yaaf.WirePlugin.Primitives;
    using Yaaf.WirePlugin.WinFormGui.Database;

    using WrapperMatchmediaTable = Primitives.WrapperDataTable.WrapperTable<Database.Matchmedia>;
    using WrapperPlayerTable = Primitives.WrapperDataTable.WrapperTable<Database.MatchSessions_Player>;
    using SessionData =
        System.Tuple
            <Primitives.WrapperDataTable.WrapperTable<Database.Matchmedia>,
                Primitives.WrapperDataTable.WrapperTable<Database.MatchSessions_Player>>;

    public partial class EditMatchSession : Form
    {
        private readonly LocalDatabaseWrapper context;

        private readonly Logging.LoggingInterfaces.ITracer logger;

        private readonly bool matchEndMode;

        private readonly WrapperMatchmediaTable matchmediaData;

        private readonly WrapperMatchmediaTable matchmediaDataCopy;

        private readonly WrapperPlayerTable playersData;

        private readonly WrapperPlayerTable playersDataCopy;

        private readonly MatchSession session;

        private WrapperMatchmediaTable oldWrapper;

        private MatchSessions_Player primaryPlayer;

        public EditMatchSession(
            Logging.LoggingInterfaces.ITracer logger,
            LocalDatabaseWrapper context,
            SessionData sessionData,
            MatchSession session,
            bool matchEndMode)
        {
            this.logger = logger;
            this.context = context;
            matchmediaData = sessionData.Item1;
            matchmediaDataCopy = matchmediaData.Clone();
            playersData = sessionData.Item2;
            playersDataCopy = playersData.Clone();
            this.session = session;
            this.matchEndMode = matchEndMode;
            InitializeComponent();
        }

        public bool? DeleteMatchmedia { get; private set; }

        private void matchPlayersDataGridView_SelectionChanged(object sender, EventArgs e)
        {
            RefreshMatchmediaView();
        }

        private void RefreshMatchmediaView()
        {
            var matchSessionsPlayers = GetSelectedPlayers();
            primaryPlayer = matchSessionsPlayers.Item2;
            primaryPlayerLabel.Text = primaryPlayer == null
                                          ? "None"
                                          : string.Format("{0} ({1})", primaryPlayer.MyName, primaryPlayer.MyPlayerId);

            IEnumerable<Matchmedia> media = new Matchmedia[0];
            media = matchSessionsPlayers.Item1.Where(player1 => player1 != null).Aggregate(
                media,
                (current, player1) =>
                current.Union(from f in matchmediaDataCopy.CopyLinqData where f.PlayerId == player1.MyPlayerId select f));

            var value = matchmediaDataCopy.GetCopiedTable(media);
            value.SetInitializer(
                mediaM =>
                    {
                        mediaM.Created = DateTime.Now;
                        mediaM.MatchSession = session;
                        mediaM.PlayerId = primaryPlayer.MyPlayerId;
                    });

            SetNewWrapper(value);
        }

        private void SetNewWrapper(WrapperMatchmediaTable value)
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
            return
                matchPlayersDataGridView.GetSelection<DataRowView>(matchSessionsPlayerBindingSource).MapSelection(
                    v => playersDataCopy.GetCopyItem(v.Row)).FilterSelection(v => v.IsSome()).MapSelection(v => v.Value);
        }

        private void EditMatchSession_Load(object sender, EventArgs e)
        {
            try
            {
                if (matchEndMode)
                {
                    saveButton.Text = "Save Matchmedia";
                    cancelButton.Text = "Delete Matchmedia And Session";
                }

                Team.ValueType = typeof(PlayerTeam);
                Team.DataSource = Helpers.GetEnumTable(typeof(PlayerTeam));
                Team.ValueMember = "value";
                Team.DisplayMember = "name";
                Skill.ValueType = typeof(PlayerSkill);
                Skill.DataSource = Helpers.GetEnumTable(typeof(PlayerSkill));
                Skill.ValueMember = "value";
                Skill.DisplayMember = "name";

                matchSessionsPlayerBindingSource.DataSource = playersDataCopy.SourceTable;
                playersDataCopy.SetInitializer(
                    player =>
                        {
                            player.MyTeam = PlayerTeam.Team1;
                            player.MySkill = PlayerSkill.Mid;
                            player.MyMatchSessionId = session.Id;
                        });
                matchTagsTextBox.Text = session.MyTags;
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
                matchmediaDataCopy.ImportChanges(oldWrapper);
                matchmediaData.ImportChanges(matchmediaDataCopy);
                playersData.ImportChanges(playersDataCopy);

                session.MyTags = matchTagsTextBox.Text;
                DeleteMatchmedia = false;
                Close();
            }
            catch (Exception ex)
            {
                ex.ShowError(logger, "Can't save MatchSession");
            }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            DeleteMatchmedia = true;
            Close();
        }

        private void matchmediaDataGridView_DragDrop(object sender, DragEventArgs e)
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

        private void matchmediaDataGridView_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        private void AddMatchmedia(string file)
        {
            if (oldWrapper == null)
            {
                throw new InvalidOperationException("Couldn't add new Matchmedia (was null)");
            }
            var media = new Matchmedia();
            media.AddDataFromFile(file);
            media.MatchSession = session;
            media.PlayerId = primaryPlayer.MyPlayerId;
            oldWrapper.Add(media);
        }

        private void showInExplorerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                var row = (DataRowView)matchmediaBindingSource.Current;
                if (row == null)
                {
                    MessageBox.Show("Please select a File first!");
                    return;
                }
                var matchmedia = oldWrapper.GetCopyItem(row.Row);
                if (matchmedia.IsNone())
                {
                    MessageBox.Show("This row is not fully added!");
                    return;
                }
                var path = matchmedia.Value.Path;
                if (!File.Exists(path))
                {
                    MessageBox.Show("Matchmedia was deleted!");
                    return;
                }
                Process.Start(Path.GetDirectoryName(path));
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
                var selection = matchmediaDataGridView.GetSelection<DataRowView>(matchmediaBindingSource);
                var paths = new StringCollection();
                paths.AddRange(
                    selection.Item1.Select(r => oldWrapper.GetCopyItem(r.Row)).Where(r => r.IsSome()).Select(
                        r => r.Value).Select(m => m.Path).ToArray());
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

        private void loadMatchDataButton_Click(object sender, EventArgs e)
        {
            try
            {
                var players = Helpers.ShowLoadMatchDataDialog(logger, session.EslMatchLink);
                matchmediaDataCopy.ImportChanges(oldWrapper);
                oldWrapper = null;
                FSharpInterop.Interop.FillWrapperTable(players, playersDataCopy, matchmediaDataCopy);
                RefreshMatchmediaView();
            }
            catch (Exception ex)
            {
                ex.ShowError(logger, "Could not load data");
            }
        }
    }
}