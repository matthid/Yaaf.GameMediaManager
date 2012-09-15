// ----------------------------------------------------------------------------
// This file (EditGames.cs) is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package (Yaaf.WirePlugin).
// Last Modified: 2012/09/10 14:08
// Created: 2012/08/26 20:57
// ----------------------------------------------------------------------------

namespace Yaaf.WirePlugin.WinFormGui
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Windows.Forms;

    using Yaaf.WirePlugin.Primitives;
    using Yaaf.WirePlugin.WinFormGui.Database;

    public partial class EditGames : Form
    {
        private readonly LocalDataContext context;

        private readonly Logging.LoggingInterfaces.ITracer logger;

        private readonly LocalDatabaseWrapper wrapper;

        private bool saveData = false;

        public EditGames(Logging.LoggingInterfaces.ITracer logger)
        {
            this.logger = logger;

            // this is a copy.. this way we can discard everything at the end, if we need to
            wrapper = FSharpInterop.Interop.GetNewContext();
            this.context = wrapper.Context;
            InitializeComponent();
        }

        private void EditGames_Load(object sender, EventArgs e)
        {
            Logging.setupLogging(logger);
            var games = from g in context.Games select g;

            foreach (var game in games)
            {
                game.MatchFormAction.Load();
            }
            gameBindingSource.DataSource = context.Games;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            saveData = true;
            Close();
        }

        private void SaveData()
        {
            try
            {
                // TODO: Check for invalid WatchFolder Entries (well they are not critical)
                foreach (var table in gameWatchFolders.Values)
                {
                    table.UpdateTable(wrapper.Context.WatchFolders);
                }

                wrapper.MySubmitChanges();
            }
            catch (Exception ex)
            {
                ex.ShowError(logger, "Can't save game changes");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            saveData = false;
            Close();
        }

        private Game OpenGameForm(Func<Game, Form> createForm)
        {
            var current = (Game)gameBindingSource.Current;
            if (current == null)
            {
                MessageBox.Show("Bitte ein Game auswählen");
                return null;
            }
            try
            {
                var form = createForm(current);
                if (form != null)
                {
                    form.ShowDialog();
                }
                return current;
            }
            catch (Exception ex)
            {
                ex.ShowError(logger, "Can't open Game form");
            }
            return current;
        }

        private Dictionary<Game, WrapperDataTable.WrapperTable<WatchFolder>> gameWatchFolders =
            new Dictionary<Game, WrapperDataTable.WrapperTable<WatchFolder>>();
        
        private void button3_Click(object sender, EventArgs e)
        {
            OpenGameForm((game) =>
            {
                WrapperDataTable.WrapperTable<WatchFolder> wrapperTable;
                if (!gameWatchFolders.TryGetValue(game, out wrapperTable))
                {
                    game.WatchFolder.Load();

                    wrapperTable =
                        game.WatchFolder.GetWrapper(WrapperDataTable.getFilterDelegate<PropertyInfo>(
                            new[] { "MyId", "GameId", "Folder", "Filter", "NotifyOnInactivity" }));
                    gameWatchFolders.Add(game, wrapperTable);
                }
                return new ManageWatchFolder(logger, wrapper, wrapperTable, game);
            });
        }

        private void button4_Click(object sender, EventArgs e)
        {
            OpenGameForm(
                (current) =>
                    {
                        MessageBox.Show("Not implemented");
                        return null;
                    });
        }

        private void EditGames_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (saveData)
            {
                SaveData();
            }
        }

        private void dataGridView1_UserAddedRow(object sender, DataGridViewRowEventArgs e)
        {
            try
            {
                var item = (Game)e.Row.DataBoundItem;
                if (item == null)
                {
                    item = (Game)gameBindingSource.Current;
                }
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
        
        private void dataGridView1_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            try
            {
                // TODO: Should we really delete all related Actions?
                var item = (Game)e.Row.DataBoundItem;
                if (item == null)
                {
                    item = (Game)gameBindingSource.Current;
                }

                context.MatchFormActions.DeleteAllOnSubmit(item.MatchFormAction);
            }
            catch (Exception ex)
            {
                ex.ShowError(logger, "Can't delete Associated Actions for game");
            }
        }
    }
}