// ----------------------------------------------------------------------------
// This file (WaitingForm.cs) is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package (Yaaf.GameMediaManager).
// Last Modified: 2012/09/10 14:08
// Created: 2012/08/26 20:57
// ----------------------------------------------------------------------------

namespace Yaaf.GameMediaManager.WinFormGui
{
    using System;
    using System.Reflection;
    using System.Threading;
    using System.Windows.Forms;

    using Microsoft.FSharp.Core;

    using Yaaf.GameMediaManager.Primitives;

    public partial class WaitingForm : Form
    {
        private readonly Logging.LoggingInterfaces.ITracer logger;

        private readonly ITask<object> task;
        private bool taskStarted = false;

        private string myTitle;

        public WaitingForm(Logging.LoggingInterfaces.ITracer logger, ITask<object> task, string title)
        {
            this.logger = logger;
            this.task = task;
            InitializeComponent();
            myTitle = title;
            Text = title;
        }

        public static T StartTask<T>(Logging.LoggingInterfaces.ITracer logger, ITask<T> task, string title)
        {
            var form = new WaitingForm(logger, task.MapTask(t => (object)t), title);
            form.ShowDialog();
            if (task.ErrorObj.IsSome())
            {
                throw new TargetInvocationException("Task produced an error", task.ErrorObj.Value);
            }
            return task.Result.Value;
        }
        
        public static void StartTaskAsync<T>(Logging.LoggingInterfaces.ITracer logger, ITask<T> task, string title, Action onFinished)
        {
            var thread = 
                new Thread(() =>
                    {
                        try
                        {
                            StartTask(logger, task, title);
                        }
                        catch (Exception error)
                        {
                            logger.LogError("Error: {0}", error);
                        }
                        finally
                        {
                            try
                            {
                                onFinished();
                            }
                            catch (Exception e)
                            {
                                logger.LogError("Error in onFinished Code: {0}", e);
                            }
                        }
                    });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }

        private void WaitingForm_Load(object sender, EventArgs e)
        {
            this.SetupForm(logger); 
            if (!taskStarted)
            {
                task.Error += task_Error;
                task.Finished += task_Finished;
                task.Update += task_Update;

                task.Start();
                taskStarted = true;
            }
        }

        void task_Update(object sender, string args)
        {
            logger.LogInformation("Task Update: {0} - {1}", myTitle, args);
            var newTitle = string.Format("{0} - {1}", myTitle, args);
            Invoke(
                new Action(
                    () =>
                        { Text = newTitle; }));
        }

        private void task_Finished(object sender, object args)
        {
            logger.LogInformation("Task finished!");
            Invoke(new Action(Close));
        }

        private void task_Error(object sender, Exception args)
        {
            logger.LogWarning("Task finished with error: {0}", args);
            Invoke(new Action(Close));
        }
    }
}