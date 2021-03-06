﻿// ----------------------------------------------------------------------------
// This file (ManageWatchFolder.cs) is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package (Yaaf.GameMediaManager).
// Last Modified: 2012/09/10 14:08
// Created: 2012/08/26 20:57
// ----------------------------------------------------------------------------

namespace Yaaf.GameMediaManager.WinFormGui
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Forms;

    using Yaaf.GameMediaManager.Primitives;
    using Yaaf.GameMediaManager.WinFormGui.Database;

    public partial class ManageWatchFolder : Form
    {
        private readonly LocalDatabaseWrapper context;

        private readonly WrapperDataTable.WrapperTable<WatchFolder> wrapperTable;

        private readonly Game game;

        private readonly Logging.LoggingInterfaces.ITracer logger;
        
        private WrapperDataTable.WrapperTable<WatchFolder> wrapperTableCopy;

        public ManageWatchFolder(Logging.LoggingInterfaces.ITracer logger, LocalDatabaseWrapper context, WrapperDataTable.WrapperTable<WatchFolder> wrapperTable, Game game)
        {
            this.logger = logger;
            this.context = context;
            this.wrapperTable = wrapperTable;
            this.wrapperTableCopy = wrapperTable.Clone();
            wrapperTableCopy.UserAddedRow += wrapperTableCopy_UserAddedRow;
            wrapperTableCopy.DeletedRow += wrapperTableCopy_DeletedRow;
            this.game = game;
            InitializeComponent();
        }

        void wrapperTableCopy_DeletedRow(object sender, WrapperDataTable.DeletedRowData<WatchFolder> args)
        {
            wrapperTableCopy.get_CopyItemToOriginal(args.CopyItem).Game = null;
        }

        void wrapperTableCopy_UserAddedRow(object sender, WrapperDataTable.UserAddedRowData<WatchFolder> args)
        {
            args.OriginalItem.MyGame = game;
        }


        private void ManageWatchFolder_Load(object sender, EventArgs e)
        {
            this.SetupForm(logger);
            try
            {
                watchFolderBindingSource.DataSource = wrapperTableCopy.SourceTable;
            }
            catch (Exception ex)
            {
                ex.ShowError(logger, "Can't load watchfolder data");
                Close();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                // Add all new, delete all deleted and update all changed games.
                wrapperTable.ImportChanges(wrapperTableCopy);
                Close();
            }
            catch (Exception ex)
            {
                ex.ShowError(logger, "Can't change watchfolder changes");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}