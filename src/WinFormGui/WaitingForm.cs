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
            this.Text = title;
        }

        public static void StartTask(Logging.LoggingInterfaces.ITracer logger, Task<Unit> task, string title)
        {
            var form = new WaitingForm(logger, task, title);
            form.ShowDialog();
        }

        private void WaitingForm_Load(object sender, EventArgs e)
        {
            task.Error += this.task_Error;
            task.Finished += this.task_Finished;
            task.Start();
        }


        void task_Finished(object sender, Unit args)
        {
            logger.LogInformation("Task finished!");
            this.Invoke(new Action(this.Close));
        }

        void task_Error(object sender, Exception args)
        {
            logger.LogError("Task finished with error: {0}", args);
            this.Invoke(new Action(this.Close));
        }
    }
}
