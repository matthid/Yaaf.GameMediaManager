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

    public partial class EditGames : Form
    {
        private readonly Action<TraceEventType, string> logger;

        private readonly LocalDatabaseDataContext context;

        private List<Game> old;

        private LocalDatabaseWrapper wrapper;


        public EditGames(Action<System.Diagnostics.TraceEventType, string> logger, LocalDatabaseWrapper context)
        {
            this.logger = logger;
            this.wrapper = context;
            this.context = wrapper.Context;
            InitializeComponent();
        }


        private void EditGames_Load(object sender, EventArgs e)
        {
            var games = from g in this.context.Games select g;
            old = new System.Collections.Generic.List<Database.Game>(games);
            gameBindingSource.DataSource = new System.Collections.Generic.List<Database.Game>(old);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                // Add all new, delete all deleted and update all changed games.
                wrapper.UpdateDatabase(context.Games, gameBindingSource.Cast<Game>(), old);
                this.Close();
            }
            catch (Exception ex)
            {
                logger(TraceEventType.Error, "Can't change game changes: " + ex);
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var current = (Game)gameBindingSource.Current;
            if (current == null)
            {
                MessageBox.Show("Bitte ein Game auswählen");
                return;
            }

            var form = new ManageWatchFolder(logger, wrapper, current);
            form.ShowDialog();
        }
    }
}
