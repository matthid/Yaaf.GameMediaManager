using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Yaaf.GameMediaManager.WinFormGui
{
    using System.Reflection;

    using Yaaf.GameMediaManager.Primitives;
    using Yaaf.GameMediaManager.WinFormGui.Database;
    using Yaaf.GameMediaManager.WinFormGui.Properties;

    using WrapperMatchmediaTable = Primitives.WrapperDataTable.WrapperTable<Database.Matchmedia>;
    using WrapperPlayerTable = Primitives.WrapperDataTable.WrapperTable<Database.MatchSessions_Player>;
    using SessionData = Tuple<Primitives.WrapperDataTable.WrapperTable<Database.Matchmedia>, Primitives.WrapperDataTable.WrapperTable<Database.MatchSessions_Player>>;
    public partial class ViewMatchSessions : Form
    {
        private readonly Logging.LoggingInterfaces.ITracer logger;

        private readonly LocalDatabaseWrapper context;

        public ViewMatchSessions(
            Logging.LoggingInterfaces.ITracer logger)
        {
            this.logger = logger;
            this.context = FSharpInterop.Interop.GetNewContext();
            InitializeComponent();
        }

        private void ViewMatchSessions_Load(object sender, EventArgs e)
        {
            this.SetupForm(logger);
            try
            {
                copyMatchSessionsTable = context.Context.MatchSessions.GetWrapper();
                copyMatchSessionsTable.ItemChanged += copyMatchSessionsTable_ItemChanged;
                matchSessionBindingSource.DataSource = copyMatchSessionsTable.SourceTable;
                matchSessionDataGridView.Sort(matchSessionDataGridView.Columns["startdateDataGridViewTextBoxColumn"], ListSortDirection.Descending);
            }
            catch(Exception ex)
            {
                ex.ShowError(logger, "Couldn't load ViewMatchSessions Form");
                Close();
            }
        }

        void copyMatchSessionsTable_ItemChanged(object sender, WrapperDataTable.ItemChangedData<MatchSession> args)
        {
            var copyItem = args.Items.Copy;
            var enteredName = copyItem.MyGame.Name;

            // Try to find the game
            var game = FSharpInterop.Interop.Database.GetGame(context, enteredName);
            if (game != null)
            {
                copyItem.MyGame.Name = game.Name;
                args.Items.Original.MyGame = game; // Save here because we only commit on save
                args.ChangedCopy.Value = true;
            }
        }

        private Dictionary<MatchSession, SessionData> sessionData = new Dictionary<MatchSession, SessionData>();

        private WrapperDataTable.WrapperTable<MatchSession> copyMatchSessionsTable;

        private void matchSessionDataGridView_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                var selectedRow = (DataRowView)matchSessionBindingSource.Current;
                if (selectedRow == null) return;
                var selectedSessionCopy = copyMatchSessionsTable.GetCopyItem(selectedRow.Row);
                if (selectedSessionCopy.IsNone()) return;
                var selectedSession = copyMatchSessionsTable.GetItem(selectedRow.Row);
                SessionData wrapperTable;
                if (!sessionData.TryGetValue(selectedSession, out wrapperTable))
                {
                    var wrapperMatchmediaTable = Helpers.GetWrapper(selectedSession.Matchmedia);
                    var wrapperPlayerTable = Helpers.GetWrapper(selectedSession.MatchSessions_Player);
                    wrapperTable = Tuple.Create(wrapperMatchmediaTable, wrapperPlayerTable);
                    sessionData.Add(selectedSession, wrapperTable);
                }

                var editForm = new EditMatchSession(logger, context, wrapperTable, selectedSession, false);
                editForm.ShowDialog();
                copyMatchSessionsTable.UpdateItem(selectedSession, selectedSession);
            }
            catch (Exception ex)
            {
                ex.ShowError(logger, "Couldn't Show EditWindow");
            }
        }

        private void matchSessionDataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                var selectedSession = (DataRowView)matchSessionBindingSource.Current;
                if (selectedSession == null) return;
            }
            catch (Exception ex)
            {
                ex.ShowError(logger, "Couldn't Handle CellClick");
            }
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            try
            {
                var invalidItem = (from copy in copyMatchSessionsTable.CopyLinqData
                                   let orig = copyMatchSessionsTable.get_CopyItemToOriginal(copy)
                                   where context.Context.Games.GetOriginalEntityState(orig.MyGame) == null
                                   select copy).FirstOrDefault();
                if (invalidItem != null)
                {
                    MessageBox.Show(
                        string.Format(
                            Resources.ViewMatchSessions_saveButton_Click_The_entry__0__has_an_invalid_game__please_enter_a_valid_game__short_gamename__gamename_or_id_, 
                            invalidItem));
                    return;
                }

                copyMatchSessionsTable.UpdateTable(context.Context.MatchSessions);

                foreach (var wrapperTables in sessionData.Values)
                {
                    var mediaTable = wrapperTables.Item1;
                    mediaTable.UpdateTable(context.Context.Matchmedias);
                    var playerTable = wrapperTables.Item2;
                    context.UpdateMatchSessionPlayerTable(playerTable);
                }

                var changes = context.Context.GetChangeSet();

                foreach (var media in changes.Deletes.Select(o => o as Matchmedia).Where(o => o != null))
                {
                    if (System.IO.File.Exists(media.Path))
                    {
                        System.IO.File.Delete(media.Path);
                    }
                }

                var mediaToCopy = changes.Inserts.Select(o => o as Matchmedia).Where(o => o != null).ToList();

                context.MySubmitChanges();

                try
                {
                    foreach (var media in mediaToCopy)
                    {
                        var newPath = FSharpInterop.Interop.GetMatchmediaPath(media);
                        if (System.IO.File.Exists(newPath))
                        {
                            throw new InvalidOperationException(string.Format("{0} should not exists, delete this file cleanup your database!", newPath));
                        }
                        System.IO.File.Move(media.Path, newPath);
                        media.Path = newPath;
                    }
                }
                finally
                {
                    // submit at least the moved items
                    context.MySubmitChanges();
                }

                Close();
            }
            catch (Exception ex)
            {
                ex.ShowError(logger, "Couldn't Save Data!");
            }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }



    }
}
