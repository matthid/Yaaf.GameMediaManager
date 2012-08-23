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
    using System.Data.Linq;
    using System.Diagnostics;

    using Yaaf.WirePlugin.WinFormGui.Database;

    public partial class EditGames : Form
    {
        private readonly Logging.LoggingInterfaces.ITracer logger;

        private readonly LocalDatabaseDataContext context;

        private List<Game> old;

        private LocalDatabaseWrapper wrapper;

        private bool saveData = false;

        public EditGames(Logging.LoggingInterfaces.ITracer logger, LocalDatabaseWrapper context)
        {
            this.logger = logger;
            
            // this is a copy.. this way we can discard everything at the end, if we need to
            this.wrapper = context; 
            this.context = wrapper.Context;
            InitializeComponent();
        }


        private void EditGames_Load(object sender, EventArgs e)
        {
            Logging.setupLogging(logger); 
            var games = from g in this.context.Games select g;
            old = new System.Collections.Generic.List<Database.Game>(games);
            gameBindingSource.DataSource = new System.Collections.Generic.List<Database.Game>(old);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.saveData = true;
            this.Close();
        }

        private void SaveData()
        {
            try
            {
                // Add all new, delete all deleted and update all changed games.
                this.wrapper.UpdateDatabase(this.context.Games, this.gameBindingSource.Cast<Game>(), this.old);
                
                // TODO: Check for invalid WatchFolder Entries (well they are not critical)

                this.wrapper.MySubmitChanges();
            }
            catch (Exception ex)
            {
                logger.LogError("{0}", "Can't change game changes: " + ex);
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.saveData = false;
            this.Close();
        }

        private void OpenGameForm(Func<Game, Form> createForm)
        {
            var current = (Game)gameBindingSource.Current;
            if (current == null)
            {
                MessageBox.Show("Bitte ein Game auswählen");
                return;
            }
            try
            {
                //if (!old.Contains(current))
                //{
                    //context.Games.InsertOnSubmit(current);
                    //context.SubmitChanges();
                    //old.Add(current);
                //}

                var form = createForm(current);
                if (form != null)
                    form.ShowDialog();
            }
            catch (Exception ex)
            {
                logger.LogError("{0}", "Can't open Game form: " + ex);
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.OpenGameForm((current) => new ManageWatchFolder(logger, wrapper, current));
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.OpenGameForm((current) =>
                {
                    MessageBox.Show("Not implemented");
                    return null;
                });
        }

        private void EditGames_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.saveData)
            {
                this.SaveData();
            }
        }

        private void dataGridView1_UserAddedRow(object sender, DataGridViewRowEventArgs e)
        {
            try
            {
                var item = (Game)e.Row.DataBoundItem;
                if (item == null) item = (Game)gameBindingSource.Current;
                // Setup default action for this game (add to matchmedia path)
                var copyAction = wrapper.GetMoveToMatchmediaActionObject();

                var assoc = new MatchFormAction();
                assoc.ActionObject = copyAction;
                assoc.Game = item;
                assoc.PublicActivated = false;
                assoc.WarActivated = true;
                item.MatchFormAction.Add(assoc);
                // context.MatchFormActions.InsertOnSubmit(assoc);
            }
            catch (Exception ex)
            {
                logger.LogError("{0}", "Can't add defaults for new game: " + ex);
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void dataGridView1_UserDeletedRow(object sender, DataGridViewRowEventArgs e)
        {
            // TODO: Should we really delete all related Actions?
            var item = (Game)e.Row.DataBoundItem;
            item.MatchFormAction.Load();
            context.MatchFormActions.DeleteAllOnSubmit(item.MatchFormAction);
        }
    }
}
