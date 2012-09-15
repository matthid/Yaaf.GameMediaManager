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
    using System.Reflection;

    using Yaaf.WirePlugin.Primitives;
    using Yaaf.WirePlugin.WinFormGui.Database;
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
            Logging.setupLogging(logger);
            try
            {
                matchSessionBindingSource.DataSource = context.Context.MatchSessions;
            }
            catch(Exception ex)
            {
                ex.ShowError(logger, "Couldn't load ViewMatchSessions Form");
                Close();
            }
        }
        private Dictionary<MatchSession, SessionData> sessionData = new Dictionary<MatchSession, SessionData>();
        private void matchSessionDataGridView_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                var selectedSession = (MatchSession)matchSessionBindingSource.Current;
                if (selectedSession == null) return;
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
                var selectedSession = (MatchSession)matchSessionBindingSource.Current;
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
