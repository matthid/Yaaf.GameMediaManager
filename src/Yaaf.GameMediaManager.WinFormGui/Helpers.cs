namespace Yaaf.GameMediaManager.WinFormGui
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Data.Linq;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Windows.Forms;

    using Yaaf.GameMediaManager.Primitives;
    using Yaaf.GameMediaManager.WinFormGui.Database;
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

        public static Tuple<IEnumerable<EslGrabber.Player>, string> ShowLoadMatchDataDialog(Logging.LoggingInterfaces.ITracer logger, string defaultLink)
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

        public interface ILinqEntity : INotifyPropertyChanging
        {
            // This is just a marker interface for Extension Methods
        }

        /// <summary>
        /// Obtain the DataContext providing this entity
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static LocalDataContext GetContext(this ILinqEntity obj)
        {
            FieldInfo fEvent = obj.GetType().GetField("PropertyChanging", BindingFlags.NonPublic | BindingFlags.Instance);
            MulticastDelegate dEvent = (MulticastDelegate)fEvent.GetValue(obj);
            if (dEvent == null)
            {
                throw new ArgumentException("Entity is not attached!");
            }
            Delegate[] onChangingHandlers = dEvent.GetInvocationList();

            // Obtain the ChangeTracker
            foreach (Delegate handler in onChangingHandlers)
            {
                if (handler.Target.GetType().Name == "StandardChangeTracker")
                {
                    // Obtain the 'services' private field of the 'tracker'
                    object tracker = handler.Target;
                    object services = tracker.GetType().GetField("services", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(tracker);

                    // Get the Context
                    var context = services.GetType().GetProperty("Context").GetValue(services, null) as LocalDataContext;
                    return context;
                }
            }

            // Not found
            throw new ArgumentException("Entity is not attached!");
        }

        public static WrapperDataTable.WrapperTable<MatchSessions_Player> GetWrapper(this IEnumerable<MatchSessions_Player> players)
        {
            var createColumns =
                new Func<IEnumerable<DataColumn>>(
                    () => new[] {
                            new DataColumn("Name", typeof(string)), new DataColumn("Tags", typeof(string)),
                            new DataColumn("EslId", typeof(int)), new DataColumn("Id", typeof(int)),
                            new DataColumn("Team", typeof(PlayerTeam)), new DataColumn("Skill", typeof(PlayerSkill)),
                            new DataColumn("Description", typeof(string)), new DataColumn("Cheating", typeof(bool)),
                        });

            WrapperDataTable.WrapperTable<MatchSessions_Player> table = null;
            table = players.CreateTable(
                createColumns,
                (initOriginal) => new MatchSessions_Player(){Player = new Player()},
                (player, row) =>
                    {
                        row["Name"] = player.Player.Name.ConvertValueToDb();
                        row["EslId"] = player.Player.EslPlayerId.ConvertValueToDb();
                        row["Team"] = player.MyTeam.ConvertValueToDb();
                        row["Description"] = player.Description.ConvertValueToDb();
                        row["Tags"] = player.Player.MyTags.ConvertValueToDb();
                        row["Id"] = player.Player.MyId.ConvertValueToDb();
                        row["Skill"] = player.MySkill.ConvertValueToDb();
                        row["Cheating"] = player.Cheating.ConvertValueToDb();
                    },
                (row, player) =>
                    {
                        player.Player.Name = row["Name"].ConvertValueBack("");
                        player.Player.EslPlayerId = row["EslId"].ConvertValueBack((int?)null);
                        player.MyTeam = row["Team"].ConvertValueBack(PlayerTeam.Team11);
                        player.Description = row["Description"].ConvertValueBack("");
                        player.Player.MyTags = row["Tags"].ConvertValueBack("");
                        player.Player.MyId = row["Id"].ConvertValueBack(0);
                        player.MySkill = row["Skill"].ConvertValueBack(PlayerSkill.Unknown);
                        player.Cheating = row["Cheating"].ConvertValueBack(false);
                    },
                (isOriginal, targetPlayer, sourcePlayer) =>
                    {
                        targetPlayer.Player.EslPlayerId = sourcePlayer.Player.EslPlayerId;
                        targetPlayer.Player.Name = sourcePlayer.Player.Name;
                        targetPlayer.Player.MyTags = sourcePlayer.Player.MyTags;
                        targetPlayer.MyTeam = sourcePlayer.MyTeam;
                        targetPlayer.Player.MyId = sourcePlayer.Player.MyId;
                        targetPlayer.Description = sourcePlayer.Description;
                        targetPlayer.MySkill = sourcePlayer.MySkill;
                        targetPlayer.Cheating = sourcePlayer.Cheating;
                    });
            table.DeletedRow += (sender, args) => { args.Player = null; };
            return table;
        }

        public static WrapperDataTable.WrapperTable<Matchmedia> GetWrapper(IEnumerable<Matchmedia> medias)
        {
            return
                medias.GetWrapper(
                    WrapperDataTable.getFilterDelegate<PropertyInfo>(
                        new[] { "MyTags", "MyId", "Created", "Map", "Name", "Path", "Type" }));
        }

        public static Matchmedia MediaFromFile(string safeFileName, MatchSessions_Player myMatchSessionsPlayer)
        {
            var media = new Matchmedia();
            media.SetOriginal();
            media.AddDataFromFile(safeFileName);
            media.MyMatchSessionsPlayer = myMatchSessionsPlayer;
            return media;
        }

        public static void SetupForm(this Form form, Logging.LoggingInterfaces.ITracer logger)
        {
            Logging.setupLogging(logger);
            form.Icon = Properties.Resources.reddragonIco;
        }
    }
}