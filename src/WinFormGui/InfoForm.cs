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

            Process.Start("https://github.com/matthid/Yaaf.WirePlugin");
        }
    }
}
