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
        private readonly Action<TraceEventType, string> logger;

        private readonly LocalDatabaseDataContext context;

        private List<Game> old;

        private LocalDatabaseWrapper wrapper;

        private bool saveData = false;

        public EditGames(Action<System.Diagnostics.TraceEventType, string> logger, LocalDatabaseWrapper context)
        {
            this.logger = logger;
            
            // this is a copy.. this way we can discard everything at the end, if we need to
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

                this.context.SubmitChanges();
            }
            catch (Exception ex)
            {
                this.logger(TraceEventType.Error, "Can't change game changes: " + ex);
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
                logger(TraceEventType.Error, "Can't open Game form: " + ex);
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
            else
            {
                // Revert all WatchFolter changes that are invalid now
                //var changes = this.context.GetChangeSet();
                //Func<object, bool> selector = d =>
                //    {
                //        var w = d as WatchFolder;
                //        if (w == null)
                //        {
                //            return false;
                //        }

                //        // Select those which are not valid (they have to be inserted, but are not as we are canceling)
                //        return !old.Contains(w.Game);
                //    };
                //foreach (var insert in changes.Inserts.Where(selector))
                //{
                //    this.context.GetTable(insert.GetType()).DeleteOnSubmit(insert);
                //}
                //foreach (var deletion in changes.Deletes.Where(selector))
                //{
                //    this.context.GetTable(deletion.GetType()).InsertOnSubmit(deletion);
                //}
                //var updatedTables = new List<ITable>();
                //foreach (var update in changes.Updates.Where(selector))
                //{
                //    var tbl = context.GetTable(update.GetType());
                //    // Make sure not to refresh the same table twice
                //    if (updatedTables.Contains(tbl))
                //    {
                //        continue;
                //    }
                //    else
                //    {
                //        updatedTables.Add(tbl);
                //        context.Refresh(RefreshMode.OverwriteCurrentValues, tbl);
                //    }
                //}
            }
        }
    }
}
