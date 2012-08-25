
namespace Yaaf.WirePlugin.WinFormGui
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Forms;

    using Yaaf.WirePlugin.WinFormGui.Database;
    using Yaaf.WirePlugin.WinFormGui.Properties;

    public partial class ManagePlayers : Form
    {
        private Logging.LoggingInterfaces.ITracer logger;

        private LocalDatabaseWrapper wrapper;

        private Player me;

        private List<Player> old;

        public ManagePlayers(Logging.LoggingInterfaces.ITracer logger, LocalDatabaseWrapper context, Player me)
        {
            this.logger = logger;

            // this is a copy.. this way we can discard everything at the end, if we need to
            this.wrapper = context;
            this.me = me;
            InitializeComponent();
        }

        private void ManagePlayers_Load(object sender, EventArgs e)
        {
            var players = from p in this.wrapper.Context.Players select p;
            old = new List<Player>(players);
            playerBindingSource.DataSource = new List<Player>(old);
            this.SetMe();
        }

        private void setAsMeButton_Click(object sender, EventArgs e)
        {
            var player = (Player)playerBindingSource.Current;
            if (player == null)
            {
                MessageBox.Show(Resources.ManagePlayers_setAsMeButton_Click_Please_select_a_player_); 
                return;
            }

            me = player;
            SetMe();
        }

        private void SetMe()
        {
            meLabel.Text = 
                string.Format(
                    "{0}, ID: {1}, EslID: {2}", 
                    me.Name, 
                    me.Id == 0 ? "Not Set" : me.Id.ToString(),
                    me.EslPlayerId.HasValue ? "Not Set" : me.EslPlayerId.Value.ToString());
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            try
            {
                // Add all new, delete all deleted and update all changed games.
                this.wrapper.UpdateDatabase(this.wrapper.Context.Players, this.playerBindingSource.Cast<Player>(), this.old);
                
                this.wrapper.MySubmitChanges();
                Properties.Settings.Default.MyIdentity = me.Id;
                Properties.Settings.Default.Save();
                this.Close();
            }
            catch (Exception ex)
            {
                logger.LogError("{0}", "Can't change player changes: " + ex);
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
