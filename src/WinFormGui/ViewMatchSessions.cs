﻿using System;
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

    public partial class ViewMatchSessions : Form
    {
        private readonly Logging.LoggingInterfaces.ITracer logger;

        private readonly LocalDatabaseWrapper context;

        private readonly IFSharpInterop interop;

        public ViewMatchSessions(
            Logging.LoggingInterfaces.ITracer logger, LocalDatabaseWrapper context, IFSharpInterop interop)
        {
            this.logger = logger;
            this.context = context;
            this.interop = interop;
            InitializeComponent();
        }

        private void ViewMatchSessions_Load(object sender, EventArgs e)
        {
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
        private Dictionary<MatchSession, Primitives.WrapperDataTable.WrapperTable<Matchmedia>> sessionData = new Dictionary<MatchSession, WrapperDataTable.WrapperTable<Matchmedia>>(); 
        private void matchSessionDataGridView_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                var selectedSession = (MatchSession)matchSessionBindingSource.Current;
                if (selectedSession == null) return;
                WrapperDataTable.WrapperTable<Matchmedia> wrapperTable;
                if (!sessionData.TryGetValue(selectedSession, out wrapperTable))
                {
                    selectedSession.Matchmedia.Load();

                    wrapperTable =
                        WrapperDataTable.getWrapperDelegate(
                            WrapperDataTable.getFilterDelegate<PropertyInfo>(
                                new[] { "MyId", "Created", "Map", "Name", "Path", "Type", "PlayerId", "MatchsessionId" }),
                            selectedSession.Matchmedia);
                    sessionData.Add(selectedSession, wrapperTable);
                }

                var editForm = new EditMatchSession(logger, context, wrapperTable, selectedSession);
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
                foreach (var wrapperTable in sessionData.Values)
                {
                    wrapperTable.UpdateTable(context.Context.Matchmedias);
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
                foreach (var media in mediaToCopy)
                {
                    var newPath = interop.GetMatchmediaPath(media);
                    System.IO.File.Copy(media.Path, newPath);
                    media.Path = newPath;
                }

                context.MySubmitChanges();
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
