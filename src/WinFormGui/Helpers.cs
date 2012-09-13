namespace Yaaf.WirePlugin.WinFormGui
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Windows.Forms;

    using Yaaf.WirePlugin.Primitives;
    using Yaaf.WirePlugin.WinFormGui.Database;
    public static class Helpers
    {
         public static void ShowError (this Exception ex, Logging.LoggingInterfaces.ITracer logger, string message)
         {
             logger.LogError("{0}: {1}", message, ex);
             MessageBox.Show(ex.Message, message, MessageBoxButtons.OK, MessageBoxIcon.Warning);
         }

        public static void AddDataFromFile(this Matchmedia media, string safeFileName)
        {
            media.Path = safeFileName;
            media.Map = MediaAnalyser.analyseMedia(safeFileName).Map;
            media.Name = Path.GetFileNameWithoutExtension(safeFileName);
            media.Created = DateTime.Now;
            media.Type = Path.GetExtension(safeFileName);
        }

        public static Tuple<List<T>, T> GetSelection<T>(this DataGridView gridView, BindingSource bindingSource) where T:class
        {
            var selectedItems =
                gridView.SelectedRows.Cast<DataGridViewRow>().Select<DataGridViewRow, T>(
                    f => (T)f.DataBoundItem).Where(t => t!=null);

            var matchSessionsPlayers = selectedItems as List<T> ?? selectedItems.ToList();

            var primaryPlayer = gridView.CurrentRow == null ? null : (T)gridView.CurrentRow.DataBoundItem;
            if (primaryPlayer == null)
            {
                primaryPlayer = (T)bindingSource.Current;
            }

            if (!matchSessionsPlayers.Any())
            {
                if (primaryPlayer != null)
                {
                    matchSessionsPlayers = new List<T>() { primaryPlayer };
                }
            }
            return Tuple.Create(matchSessionsPlayers, primaryPlayer);
        }

        public static Tuple<List<T2>, T2> MapSelection<T1, T2>(this Tuple<List<T1>, T1> original, Func<T1, T2> mapping)
            where T1 : class 
            where T2 : class
        {
            return Tuple.Create(original.Item1.Select(mapping).ToList(),original.Item2!=null? mapping(original.Item2):null);
        }

        public static Tuple<List<T>, T> FilterSelection<T>(this Tuple<List<T>, T> original, Func<T, bool> filter)
            where T : class
        {
            return Tuple.Create(original.Item1.Where(filter).ToList(), original.Item2 != null && filter(original.Item2) ? original.Item2 : null);
        }

        public static IEnumerable<EslGrabber.Player> ShowLoadMatchDataDialog(Logging.LoggingInterfaces.ITracer logger, string defaultLink)
        {
            var form = new FetchMatchdataDialog(logger, defaultLink);
            form.ShowDialog();
            return form.Result;

        }

        public static DataTable GetEnumTable(Type enumType)
        {
            var values = Enum.GetValues(enumType);
            var dataTable = new DataTable();
            dataTable.Columns.Add("value", enumType);
            dataTable.Columns.Add("name");
            foreach (var value in values)
            {
                dataTable.Rows.Add(value, value.ToString());
            }
            return dataTable;
        }
        public static Primitives.WrapperDataTable.WrapperTable<MatchSessions_Player> GetWrapper(IEnumerable<MatchSessions_Player> players)
        {
            return
                WrapperDataTable.getWrapperDelegate(
                    WrapperDataTable.getFilterDelegate<PropertyInfo>(
                        new[]
                            {
                                "MyTags", "MyName", "MyEslId", "MyMatchSessionId", "MyPlayerId", "MyTeam", "MySkill",
                                "Description", "Cheating"
                            }),
                    players);
        } 

        public static Primitives.WrapperDataTable.WrapperTable<Matchmedia> GetWrapper(IEnumerable<Matchmedia> medias)
        {
            return
                WrapperDataTable.getWrapperDelegate(
                    WrapperDataTable.getFilterDelegate<PropertyInfo>(
                        new[]
                            { "MyTags", "MyId", "Created", "Map", "Name", "Path", "Type", "PlayerId", "MatchsessionId" }),
                    medias);
        } 
    }
}