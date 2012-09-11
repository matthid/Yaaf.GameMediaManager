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
    public partial class ViewMatchSessions : Form
    {
        private readonly Logging.LoggingInterfaces.ITracer logger;

        private readonly LocalDatabaseWrapper context;

        public ViewMatchSessions(
            Logging.LoggingInterfaces.ITracer logger, LocalDatabaseWrapper context)
        {
            this.logger = logger;
            this.context = context;
            InitializeComponent();
        }

        private void ViewMatchSessions_Load(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
