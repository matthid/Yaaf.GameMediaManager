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
    using System.Globalization;

    using Yaaf.WirePlugin.WinFormGui.Database;

    public partial class ManageMatchPlayers : Form
    {
        private readonly Logging.LoggingInterfaces.ITracer logger;

        private readonly LocalDatabaseWrapper context;

        private readonly MatchSession session;

        private List<MatchSessions_Player> old;
        
        public ManageMatchPlayers(Logging.LoggingInterfaces.ITracer logger, LocalDatabaseWrapper context, MatchSession session)
        {
            this.logger = logger;
            this.context = context;
            this.session = session;
            InitializeComponent();
        }

        private void ManageMatchPlayers_Load(object sender, EventArgs e)
        {
            Logging.setupLogging(logger); 
            try
            {
                session.MatchSessions_Player.Load();
                old = new List<MatchSessions_Player>(session.MatchSessions_Player);
                matchSessionsPlayerBindingSource.DataSource = new List<MatchSessions_Player>(old);
                
                DataTable teamSubdt = new DataTable();
                teamSubdt.Columns.Add("value", typeof(byte));
                teamSubdt.Columns.Add("name");
                int i;
                for (i = 1; i < 11; i++)
                {
                    teamSubdt.Rows.Add((byte?)i, i.ToString());
                }
                teamDataGridViewTextBoxColumn.ValueType = typeof(byte);
                teamDataGridViewTextBoxColumn.ValueMember = "value";
                teamDataGridViewTextBoxColumn.DisplayMember = "name";
                teamDataGridViewTextBoxColumn.DataSource = teamSubdt;

                DataTable skillSubdt = new DataTable();
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
                    row.Cells["EslPlayerId"].Value =
                        player.Player.EslPlayerId == null
                            ? ""
                            : player.Player.EslPlayerId.ToString();
                    row.Cells["PlayerName"].Value = player.Player.Name;
                    row.Cells["PlayerId"].Value = player.Player.Id.ToString();
                    player.Player.Player_Tag.Load();
                }
                //matchPlayersDataGridView.DataError
            }
            catch (Exception ex)
            {
                logger.LogError("{0}", "Can't load player matchdata: " + ex);
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.Close();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                int i = -1;
                var toAdd = new List<MatchSessions_Player>();
                var toRemoveFromOld = new List<MatchSessions_Player>();
                foreach (var item in matchSessionsPlayerBindingSource.Cast<MatchSessions_Player>())
                {
                    i++;
                    var row = matchPlayersDataGridView.Rows[i];
                    var eslIdT = row.Cells["EslPlayerId"].Value.ToString();
                    var eslId = string.IsNullOrEmpty(eslIdT) ? null : (int?)int.Parse(eslIdT);
                    var name = row.Cells["PlayerName"].Value.ToString();
                    var playerIdString = row.Cells["PlayerId"].Value.ToString();
                    var playerId = string.IsNullOrEmpty(playerIdString) ? -1 : int.Parse(playerIdString);

                    if (playerId > -1)
                    { // Update player
                        if (item.PlayerId != playerId)
                        { // Find given player
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
                    { // Add new Player
                        var player = new Database.Player();
                        player.Name = name;
                        player.EslPlayerId = eslId;
                        item.Player = player;
                    }

                    var tags = row.Cells["Tags"].Value.ToString().Split(',');
                    item.Player.Player_Tag.Load();
                    foreach (var tag in tags)
                    {
                        if (!(from assoc in item.Player.Player_Tag
                              where assoc.Tag.Name == tag
                              select assoc).Any())
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
                    session.MatchSessions_Player.Remove(matchSessionsPlayer);
                }

                this.Close();
            }
            catch (Exception ex)
            {
                logger.LogError("{0}", "Can't save player matchdata: " + ex);
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void matchPlayersDataGridView_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            logger.LogWarning("{0}", "DataError: " + e.Exception);
            //e.ThrowException = false;
        }

    }
}
