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
    using System.Diagnostics;

    using Yaaf.WirePlugin.WinFormGui.Properties;

    public partial class InfoForm : Form
    {
        private readonly Action<TraceEventType, string> logger;

        public InfoForm(Action<System.Diagnostics.TraceEventType, string> logger)
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
                logger(TraceEventType.Error, e.ToString());
            }
        }


        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.TryStart("https://github.com/matthid/Yaaf.WirePlugin/downloads");
        }

        private void InfoForm_Load(object sender, EventArgs e)
        {
            linkLabel2.Text = string.Format(
                Resources.InfoForm_InfoForm_Load_VersionString___0_,
                System.Reflection.Assembly.GetExecutingAssembly().GetName().Version);
        }
    }
}
