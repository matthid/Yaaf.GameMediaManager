namespace Yaaf.WirePlugin.WinFormGui
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Linq;
    using System.Linq;
    using System.Windows.Forms;

    using Yaaf.WirePlugin.WinFormGui.Database;

    public class ManagePlayersHelper
    {
        private readonly EntitySet<MatchSessions_Player> matchSessionsPlayer;

        private readonly LocalDatabaseWrapper context;

        private readonly MatchSession session;

        private readonly DataGridView matchPlayersDataGridView;

        private readonly BindingSource bindingSource;

        private readonly DataGridViewComboBoxColumn teamDataGridViewTextBoxColumn;

        private readonly DataGridViewComboBoxColumn skillDataGridViewTextBoxColumn;

        private List<MatchSessions_Player> old;

        public ManagePlayersHelper(LocalDatabaseWrapper context, MatchSession session, DataGridView matchPlayersDataGridView, BindingSource bindingSource, DataGridViewComboBoxColumn teamDataGridViewTextBoxColumn, DataGridViewComboBoxColumn skillDataGridViewTextBoxColumn)
        {
            this.matchSessionsPlayer = session.MatchSessions_Player;
            this.context = context;
            this.session = session;
            this.matchPlayersDataGridView = matchPlayersDataGridView;
            this.bindingSource = bindingSource;
            this.teamDataGridViewTextBoxColumn = teamDataGridViewTextBoxColumn;
            this.skillDataGridViewTextBoxColumn = skillDataGridViewTextBoxColumn;
        }

        public void Load()
        {
            matchSessionsPlayer.Load();
            old = new List<MatchSessions_Player>(matchSessionsPlayer);
            bindingSource.DataSource = new List<MatchSessions_Player>(old);

            var teamSubdt = new DataTable();
            teamSubdt.Columns.Add("value", typeof(byte));
            teamSubdt.Columns.Add("name");
            int i;
            for (i = 1; i < 12; i++)
            {
                teamSubdt.Rows.Add((byte?)i, i.ToString());
            }
            teamDataGridViewTextBoxColumn.ValueType = typeof(byte);
            teamDataGridViewTextBoxColumn.ValueMember = "value";
            teamDataGridViewTextBoxColumn.DisplayMember = "name";
            teamDataGridViewTextBoxColumn.DataSource = teamSubdt;

            var skillSubdt = new DataTable();
            skillSubdt.Columns.Add("value", typeof(byte));
            skillSubdt.Columns.Add("name");
            for (i = 0; i < 11; i++)
            {
                skillSubdt.Rows.Add((byte)(i * 10), (i * 10).ToString());
            }
            skillDataGridViewTextBoxColumn.ValueType = typeof(byte);
            skillDataGridViewTextBoxColumn.ValueMember = "value";
            skillDataGridViewTextBoxColumn.DisplayMember = "name";
            skillDataGridViewTextBoxColumn.DataSource = skillSubdt;
            i = -1;
            foreach (var player in old)
            {
                i++;
                var row = matchPlayersDataGridView.Rows[i];

                row.Cells["Tags"].Value = String.Join(
                    ",", (from assoc in player.Player.Player_Tag select assoc.Tag.Name).ToArray());
                row.Cells["EslPlayerId"].Value = player.Player.EslPlayerId == null
                                                     ? ""
                                                     : player.Player.EslPlayerId.ToString();
                row.Cells["PlayerName"].Value = player.Player.Name;
                row.Cells["PlayerId"].Value = player.Player.Id.ToString();
                player.Player.Player_Tag.Load();
            }
            //matchPlayersDataGridView.DataError
            
        }

        public void Save()
        {
            var i = -1;
            var toAdd = new List<MatchSessions_Player>();
            var toRemoveFromOld = new List<MatchSessions_Player>();
            foreach (var item in bindingSource.Cast<MatchSessions_Player>())
            {
                i++;
                var row = matchPlayersDataGridView.Rows[i];
                var eslIdT = row.Cells["EslPlayerId"].Value.ToString();
                var eslId = string.IsNullOrEmpty(eslIdT) ? null : (int?)int.Parse(eslIdT);
                var name = row.Cells["PlayerName"].Value.ToString();
                var playerIdString = row.Cells["PlayerId"].Value.ToString();
                var playerId = string.IsNullOrEmpty(playerIdString) ? -1 : int.Parse(playerIdString);

                if (playerId > -1)
                {
                    // Update player
                    if (item.PlayerId != playerId)
                    {
                        // Find given player
                        var p =
                            (from player in context.Context.Players where player.Id == playerId select player).
                                SingleOrDefault();
                        if (p == null)
                        {
                            throw new InvalidOperationException("PlayerId " + playerId + " was not found!");
                        }
                        item.Player = p;
                    }

                    item.Player.EslPlayerId = eslId;
                    item.Player.Name = name;
                }
                else
                {
                    // Add new Player
                    var player = new Player();
                    player.Name = name;
                    player.EslPlayerId = eslId;
                    item.Player = player;
                }

                var tags = row.Cells["Tags"].Value.ToString().Split(',');
                item.Player.Player_Tag.Load();
                foreach (var tag in tags)
                {
                    if (!(from assoc in item.Player.Player_Tag where assoc.Tag.Name == tag select assoc).Any())
                    {
                        var association = new Player_Tag();
                        association.Player = item.Player;
                        association.Tag = context.GetTag(tag);
                        context.Context.Player_Tags.InsertOnSubmit(association);
                        item.Player.Player_Tag.Add(association);
                    }
                }

                if (!old.Contains(item))
                {
                    toAdd.Add(item);
                }
                else
                {
                    // will be updated (and not removed)
                    toRemoveFromOld.Add(item);
                }
            }

            session.MatchSessions_Player.AddRange(toAdd);
            foreach (var rem in toRemoveFromOld)
            {
                old.Remove(rem);
            }

            foreach (var matchSessionsPlayer in old)
            {
                matchSessionsPlayer.Player.MatchSessions_Player.Remove(matchSessionsPlayer);
                session.MatchSessions_Player.Remove(matchSessionsPlayer);
            }

        }
    }
}