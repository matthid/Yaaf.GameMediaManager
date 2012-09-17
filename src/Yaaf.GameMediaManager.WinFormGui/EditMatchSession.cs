// ----------------------------------------------------------------------------
// This file (EditMatchSession.cs) is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package (Yaaf.GameMediaManager).
// Last Modified: 2012/09/14 15:53
// Created: 2012/09/13 08:49
// ----------------------------------------------------------------------------

namespace Yaaf.GameMediaManager.WinFormGui
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Data;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Windows.Forms;

    using Microsoft.FSharp.Core;

    using Yaaf.GameMediaManager.Primitives;
    using Yaaf.GameMediaManager.WinFormGui.Database;

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
            playersDataCopy.ItemChanged += playersDataCopy_ItemChanged;
            playersDataCopy.DeletedRow += playersDataCopy_DeletedRow;
            this.session = session;
            this.matchEndMode = matchEndMode;
            InitializeComponent();
        }

        void playersDataCopy_DeletedRow(object sender, MatchSessions_Player args)
        {
            var originalPlayer = playersDataCopy.get_CopyItemToOriginal(args);
            var count = (from matchmediaCopy in matchmediaDataCopy.CopyLinqData
                         let original = matchmediaDataCopy.get_CopyItemToOriginal(matchmediaCopy)
                         where original.MyMatchSessionsPlayer == originalPlayer
                         select original).Count();

            if (count > 0)
            {
                MessageBox.Show("Please delete all matchmedias for this player!");
                throw new InvalidOperationException("Can't delete row with matchmedia!");
            }
        }

        public bool? DeleteMatchmedia { get; private set; }

        private void playersDataCopy_ItemChanged(
            object sender, Tuple<MatchSessions_Player, MatchSessions_Player, FSharpRef<bool>> tuple)
        {
            var copyItem = tuple.Item2;

            var changedCopyItem = false;
            if (copyItem.Player.MyId != 0)
            {
                var player = FSharpInterop.Interop.Database.GetPlayerById(context, copyItem.Player.MyId);
                if (player != null)
                {
                    if (!copyItem.Player.EslPlayerId.HasValue || copyItem.Player.EslPlayerId.Value != player.EslPlayerId)
                    {
                        copyItem.Player.EslPlayerId = player.EslPlayerId;
                        changedCopyItem = true;
                    }

                    if (ChangeCopyItem(player, copyItem))
                    {
                        changedCopyItem = true;
                    }
                }
                else
                {
                    copyItem.Player.MyId = 0;
                    changedCopyItem = true;
                }
            }

            if (copyItem.Player.MyId == 0 && copyItem.Player.EslPlayerId.HasValue)
            {
                var player = FSharpInterop.Interop.Database.GetPlayerByEslId(context, copyItem.Player.EslPlayerId.Value);
                if (player != null)
                {
                    if (copyItem.Player.MyId != player.Id)
                    {
                        copyItem.Player.MyId = player.Id;
                        changedCopyItem = true;
                    }
                    if (ChangeCopyItem(player, copyItem))
                    {
                        changedCopyItem = true;
                    }
                }
            }

            if (changedCopyItem)
            {
                tuple.Item3.Value = true;
            }
        }

        private bool ChangeCopyItem(Player player, MatchSessions_Player copyItem)
        {
            var changedCopyItem = false;
            if (string.IsNullOrEmpty(copyItem.Player.Name) || copyItem.Player.Name == "unknown")
            {
                copyItem.Player.Name = player.Name;
                changedCopyItem = true;
            }

            if (string.IsNullOrEmpty(copyItem.Player.MyTags))
            {
                copyItem.Player.MyTags = player.MyTags;
                changedCopyItem = true;
            }
            return changedCopyItem;
        }

        private void matchPlayersDataGridView_SelectionChanged(object sender, EventArgs e)
        {
            if (!matchmediaDataCopy.IsInvalidated)
            {
                RefreshMatchmediaView();
            }
        }

        private void RefreshMatchmediaView()
        {
            var matchSessionsPlayers = GetSelectedPlayers();
            primaryPlayer = matchSessionsPlayers.Item2 != null ? matchSessionsPlayers.Item2.Original : null;
            var changedPrimaryPlayer = matchSessionsPlayers.Item2 != null ? matchSessionsPlayers.Item2.Copy : null;
            primaryPlayerLabel.Text = changedPrimaryPlayer == null
                                          ? "None"
                                          : string.Format("{0} ({1})", changedPrimaryPlayer.Player.Name, changedPrimaryPlayer.Player.MyId);

            IEnumerable<Matchmedia> media = new Matchmedia[0];
            media = 
                matchSessionsPlayers.Item1
                    .Aggregate(media, (current, selectedPlayer) =>
                        current.Union(from copy in matchmediaDataCopy.CopyLinqData
                                      let row = matchmediaDataCopy.GetRowFromCopy(copy)
                                      let orig = matchmediaDataCopy.GetItem(row)
                                      where orig.MatchSessions_Player == primaryPlayer
                                      select copy));

            var value = matchmediaDataCopy.GetCopiedTableFromCopyData(media);
            value.InitRow += value_InitRow;
            value.UserAddedRow += value_UserAddedRow;

            SetNewWrapper(value);
        }

        private void value_UserAddedRow(object sender, Matchmedia media)
        {
            media.PlayerId = primaryPlayer.PlayerId;
        }

        private void value_InitRow(object sender, Matchmedia media)
        {
            media.Created = DateTime.Now;
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

        private Tuple<List<WrapperDataTable.ItemData<MatchSessions_Player>>, WrapperDataTable.ItemData<MatchSessions_Player>> GetSelectedPlayers()
        {
            return
                matchPlayersDataGridView
                    .GetSelection<DataRowView>(matchSessionsPlayerBindingSource)
                    .MapSelection(v => playersDataCopy.GetRowData(v.Row))
                    .FilterSelection(v => v.IsSome())
                    .MapSelection(v => v.Value);
        }

        private void EditMatchSession_Load(object sender, EventArgs e)
        {
            this.SetupForm(logger);
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

                matchnameTextBox.Text = session.Name;
                matchSessionsPlayerBindingSource.DataSource = playersDataCopy.SourceTable;
                linkLabel.Text = session.EslMatchLink;
                playersDataCopy.InitRow += playersDataCopy_InitRow;
                matchTagsTextBox.Text = session.MyTags;
            }
            catch (Exception ex)
            {
                ex.ShowError(logger, "Can't load EditMatchSession-Form");
                Close();
            }
        }
        
        private void playersDataCopy_InitRow(object sender, MatchSessions_Player player)
        {
            player.MyTeam = PlayerTeam.Team1;
            player.MySkill = PlayerSkill.Mid;
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            try
            {
                SaveData();
                DeleteMatchmedia = false;
                Close();
            }
            catch (Exception ex)
            {
                ex.ShowError(logger, "Can't save MatchSession");
            }
        }

        private void SaveData()
        {
            matchmediaDataCopy.ImportChanges(oldWrapper);
            matchmediaData.ImportChanges(matchmediaDataCopy);
            var playerData = from copyData in playersDataCopy.CopyLinqData
                             let row = playersDataCopy.get_CopyItemToRow(copyData)
                             let original = playersDataCopy.GetItem(row)
                             select original;
            foreach (var p in playerData)
            {
                p.MatchSession = session;
            }

            if (matchnameTextBox.Text.Length > 100)
            {
                throw new InvalidOperationException("Matchname can't exceed 100 characters!");
            }

            session.Name = matchnameTextBox.Text;
            playersData.ImportChanges(playersDataCopy);

            session.EslMatchLink = linkLabel.Text;
            session.MyTags = matchTagsTextBox.Text;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            if (matchEndMode)
            {
                SaveData();
            }
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
            var media = Helpers.MediaFromFile(file, primaryPlayer);
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
                FSharpInterop.Interop.FillWrapperTable(players.Item1, playersDataCopy, matchmediaDataCopy);
                linkLabel.Text = players.Item2;
                RefreshMatchmediaView();
            }
            catch (Exception ex)
            {
                ex.ShowError(logger, "Could not load data");
            }
        }
    }
}