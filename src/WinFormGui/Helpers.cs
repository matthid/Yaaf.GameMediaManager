namespace Yaaf.WirePlugin.WinFormGui
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Windows.Forms;

    using Yaaf.WirePlugin.WinFormGui.Database;
    public interface IFSharpInterop
    {
        string GetMatchmediaPath(Matchmedia media);
    }
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
                    f => (T)f.DataBoundItem);

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
    }
}