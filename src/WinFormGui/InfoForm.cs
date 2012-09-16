// ----------------------------------------------------------------------------
// This file (InfoForm.cs) is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package (Yaaf.WirePlugin).
// Last Modified: 2012/09/10 14:08
// Created: 2012/08/20 07:49
// ----------------------------------------------------------------------------

namespace Yaaf.WirePlugin.WinFormGui
{
    using System;
    using System.Diagnostics;
    using System.Windows.Forms;

    using Yaaf.WirePlugin.WinFormGui.Properties;

    public partial class InfoForm : Form
    {
        private readonly Logging.LoggingInterfaces.ITracer logger;

        public InfoForm(Logging.LoggingInterfaces.ITracer logger)
        {
            this.logger = logger;
            InitializeComponent();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            TryStart("https://github.com/matthid/Yaaf.WirePlugin");
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
            TryStart("https://github.com/matthid/Yaaf.WirePlugin/downloads");
        }

        private void InfoForm_Load(object sender, EventArgs e)
        {
            this.SetupForm(logger);
            linkLabel2.Text = string.Format(
                Resources.InfoForm_InfoForm_Load_VersionString___0_, ProjectConstants.ProjectVersion);
        }
    }
}