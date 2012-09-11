namespace Yaaf.WirePlugin.WinFormGui
{
    using System;
    using System.Linq;
    using System.Windows.Forms;

    using Yaaf.WirePlugin.WinFormGui.Database;

    public class ManageMatchmediaHelper
    {
        private readonly LocalDatabaseWrapper localDatabaseWrapper;

        private readonly MatchSession session;

        private readonly BindingSource matchmediaBindingSource;

        private readonly DataGridView matchmediaDataGridView;

        private readonly LocalDatabaseWrapper context;

        private readonly Logging.LoggingInterfaces.ITracer logger;

        public ManageMatchmediaHelper(Logging.LoggingInterfaces.ITracer logger,
            LocalDatabaseWrapper localDatabaseWrapper, MatchSession session, BindingSource matchmediaBindingSource, DataGridView matchmediaDataGridView)
        {
            this.logger = logger;
            this.localDatabaseWrapper = localDatabaseWrapper;
            this.session = session;
            this.matchmediaBindingSource = matchmediaBindingSource;
            this.matchmediaDataGridView = matchmediaDataGridView;
        }

        public void Load()
        {
            matchmediaBindingSource.DataSource = session.Matchmedia;
            var i = -1;
            foreach (var file in session.Matchmedia)
            {
                i++;
                file.Matchmedia_Tag.Load();
                var row = matchmediaDataGridView.Rows[i];
                row.Cells["Tags"].Value = String.Join(
                    ",", (from assoc in file.Matchmedia_Tag select assoc.Tag.Name).ToArray());
            }
        }

        public void Save()
        {
            var i = -1;
            foreach (var file in session.Matchmedia)
            {
                i++;
                var row = matchmediaDataGridView.Rows[i];
                var mytags = row.Cells["Tags"].Value;
                var media_tags = (mytags == null ? "" : mytags.ToString()).Split(',');
                var toRemove = file.Matchmedia_Tag.ToDictionary(t => t.Tag.Name, t => t);
                foreach (var tag in media_tags)
                {
                    toRemove.Remove(tag);
                    if (!(from assoc in file.Matchmedia_Tag where assoc.Tag.Name == tag select assoc).Any())
                    {
                        var association = new Matchmedia_Tag();
                        association.Matchmedia = file;
                        association.Tag = context.GetTag(tag);
                        file.Matchmedia_Tag.Add(association);
                    }
                }

                foreach (var matchSessionsTag in toRemove.Values)
                {
                    file.Matchmedia_Tag.Remove(matchSessionsTag);
                }
            }

        }
    }
}