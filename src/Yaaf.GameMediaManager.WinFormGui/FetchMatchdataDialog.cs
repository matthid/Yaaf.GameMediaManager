﻿using System;
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
    public partial class FetchMatchdataDialog : Form
    {
        private readonly Logging.LoggingInterfaces.ITracer logger;

        public FetchMatchdataDialog(Logging.LoggingInterfaces.ITracer logger, string defaultLink)
        {
            this.logger = logger;
            Result = null;
            InitializeComponent();
            linkTextBox.Text = defaultLink;
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            try
            {
                var linkString = linkTextBox.Text;
                var uri = new System.Uri(linkString);
                var task = FSharpInterop.Interop.GetFetchPlayerTask(linkString);
                WaitingForm.StartTask(logger, task, "Fetching player data...");
                if (task.Result.IsNone())
                {
                    if (task.ErrorObj.IsNone())
                    {
                        throw new InvalidOperationException("Task finished without result or error!");
                    }
                    throw new TargetException("Task produced an error: " + task.ErrorObj.Value.Message, task.ErrorObj.Value);
                }
                Result = Tuple.Create(task.Result.Value, linkString);

                Close();
            }
            catch (Exception ex)
            {
                ex.ShowError(logger, "Could not fetch data");
            }
        }

        public Tuple<IEnumerable<EslGrabber.Player>, string> Result { get; private set; }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void FetchMatchdataDialog_Load(object sender, EventArgs e)
        {
            this.SetupForm(logger);
        }
    }
}