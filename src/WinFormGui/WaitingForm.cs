// ----------------------------------------------------------------------------
// This file (WaitingForm.cs) is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package (Yaaf.WirePlugin).
// Last Modified: 2012/09/10 14:08
// Created: 2012/08/26 20:57
// ----------------------------------------------------------------------------

namespace Yaaf.WirePlugin.WinFormGui
{
    using System;
    using System.Windows.Forms;

    using Microsoft.FSharp.Core;

    using Yaaf.WirePlugin.Primitives;

    public partial class WaitingForm : Form
    {
        private readonly Logging.LoggingInterfaces.ITracer logger;

        private readonly Task<Unit> task;

        public WaitingForm(Logging.LoggingInterfaces.ITracer logger, Task<Unit> task, string title)
        {
            this.logger = logger;
            this.task = task;
            InitializeComponent();
            Text = title;
        }

        public static void StartTask(Logging.LoggingInterfaces.ITracer logger, Task<Unit> task, string title)
        {
            var form = new WaitingForm(logger, task, title);
            form.ShowDialog();
        }

        private void WaitingForm_Load(object sender, EventArgs e)
        {
            task.Error += task_Error;
            task.Finished += task_Finished;
            task.Start();
        }

        private void task_Finished(object sender, Unit args)
        {
            logger.LogInformation("Task finished!");
            Invoke(new Action(Close));
        }

        private void task_Error(object sender, Exception args)
        {
            logger.LogError("Task finished with error: {0}", args);
            Invoke(new Action(Close));
        }
    }
}