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
    using Yaaf.WirePlugin.WinFormGui.Database;

    public partial class MatchSessionEnd : Form
    {
        private readonly MatchSession session;

        private readonly IEnumerable<Matchmedia> mediaFiles;

        private readonly int? eslWarId;


        public MatchSessionEnd(Database.MatchSession session, IEnumerable<Database.Matchmedia> mediaFiles, int? eslWarId)
        {
            this.session = session;
            this.mediaFiles = mediaFiles;
            this.eslWarId = eslWarId;
            InitializeComponent();
        }


        public IEnumerable<Database.Matchmedia> ResultMedia { get; private set; } 
    }
}
