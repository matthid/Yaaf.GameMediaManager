// ----------------------------------------------------------------------------
// This file (InfoForm.cs) is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package (Yaaf.GameMediaManager).
// Last Modified: 2012/09/10 14:08
// Created: 2012/08/20 07:49
// ----------------------------------------------------------------------------

namespace Yaaf.GameMediaManager.WinFormGui
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Net;
    using System.Reflection;
    using System.Windows.Forms;

    using Yaaf.GameMediaManager.WinFormGui.Properties;

    public partial class InfoForm : Form
    {
        private readonly Logging.LoggingInterfaces.ITracer logger;

        private bool loaded;

        public InfoForm(Logging.LoggingInterfaces.ITracer logger)
        {
            this.logger = logger;
            InitializeComponent();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            TryStart("https://github.com/matthid/Yaaf.GameMediaManager");
        }

        private void TryStart(string link)
        {
            try
            {
                Process.Start(link);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, Resources.InfoForm_TryStart_Could_not_start_browser);
                logger.LogError("{0}", e.ToString());
            }
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            TryStart("https://github.com/matthid/Yaaf.GameMediaManager/downloads");
        }

        private void InfoForm_Load(object sender, EventArgs e)
        {
            this.SetupForm(logger);
            try
            {
                loaded = true;
                linkLabel2.Text = string.Format(
                    Resources.InfoForm_InfoForm_Load_VersionString___0_, ProjectConstants.ProjectVersion);
                var downloadReleaseNotesTask = Primitives.Task.FromDelegate(
                    () =>
                    {
                        const string Url = "https://raw.github.com/matthid/Yaaf.GameMediaManager/master/Releasenotes.txt";
                        var client = new WebClient();
                        var raw = client.DownloadString(Url);
                        return raw.Replace("\n", "\r\n");
                    });
                downloadReleaseNotesTask.Finished += downloadReleaseNotesTask_Finished;
                downloadReleaseNotesTask.Error += downloadReleaseNotesTask_Error;
                downloadReleaseNotesTask.Start();
            }
            catch (Exception ex)
            {
                ex.ShowError(logger, "Can't load Info-Form");
                Close();
            }
        }

        void downloadReleaseNotesTask_Error(object sender, Exception args)
        {
            args.ShowError(logger, "Error while fetching releasenotes");

            if (!loaded) return;
            Invoke(
                new Action(
                    () =>
                    { this.releaseNotesTextBlock.Text = args.ToString(); }));
        }

        void downloadReleaseNotesTask_Finished(object sender, string args)
        {
            if (!loaded) return;
            Invoke(
                new Action(
                    () =>
                        { this.releaseNotesTextBlock.Text = args; }));
        }

        private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            TryStart("https://github.com/matthid/Yaaf.GameMediaManager/wiki");
        }

        private void linkLabel4_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var loc = Assembly.GetExecutingAssembly().Location;
            TryStart(Path.Combine(Path.GetDirectoryName(loc), "Scripting"));
        }

        private void linkLabel5_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            TryStart("https://github.com/matthid/Yaaf.GameMediaManager/issues");
        }

        private void linkLabel6_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            TryStart("mailto:matthi.d@googlemail.com");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            loaded = false;
            this.Close();
        }
    }
}