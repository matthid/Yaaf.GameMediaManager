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

    public partial class ManageMatchPlayers : Form
    {
        private readonly Action<TraceEventType, string> logger;

        private readonly LocalDatabaseWrapper context;

        private readonly MatchSession session;

        private List<MatchSessions_Player> old;
        
        public ManageMatchPlayers(Action<TraceEventType, string> logger, LocalDatabaseWrapper context, MatchSession session)
        {
            this.logger = logger;
            this.context = context;
            this.session = session;
            InitializeComponent();
        }

        private void ManageMatchPlayers_Load(object sender, EventArgs e)
        {
            session.MatchSessions_Player.Load();
            old = new List<MatchSessions_Player>(session.MatchSessions_Player);
            matchSessionsPlayerBindingSource.DataSource = new List<MatchSessions_Player>(old);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                int i = -1;
                var toAdd = new List<MatchSessions_Player>();
                var toRemove = new List<MatchSessions_Player>();
                foreach (var item in matchSessionsPlayerBindingSource.Cast<MatchSessions_Player>())
                {
                    i++;
                    if (!old.Contains(item))
                    {
                        var eslIdT = matchPlayersDataGridView.Rows[0].Cells["EslPlayerId"].Value.ToString();
                        var eslId = string.IsNullOrEmpty(eslIdT) ? -1 : int.Parse(eslIdT);
                        var name = matchPlayersDataGridView.Rows[0].Cells["Name"].Value.ToString();
                        if (eslId == -1 || eslId == 0)
                        {
                            var player = new Database.Player();
                            player.Name = name;
                            // context.Context.Players.InsertOnSubmit(player);
                            item.Player = player;
                        }
                        else
                        {
                            var t =
                                (from p in context.Context.Players where p.EslPlayerId == eslId select p).
                                    SingleOrDefault();
                            if (t == null)
                            {
                                var player = new Database.Player();
                                player.Name = name;
                                // context.Context.Players.InsertOnSubmit(player);
                                item.Player = player;
                            }
                            else
                            {
                                item.Player = t;
                            }
                        }

                        toAdd.Add(item);
                    }
                    else
                    {
                        toRemove.Add(item);
                    }
                }

                session.MatchSessions_Player.AddRange(toAdd);
                foreach (var rem in toRemove)
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
                logger(TraceEventType.Error, "Can't save player matchdata: " + ex);
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
