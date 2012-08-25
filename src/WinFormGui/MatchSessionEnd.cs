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
    using System.IO;

    using Microsoft.FSharp.Control;
    using Microsoft.FSharp.Core;

    using Yaaf.WirePlugin.Primitives;
    using Yaaf.WirePlugin.WinFormGui.Database;
    public interface IMatchSession
    {
        MatchSession Session { get; }

        FSharpAsync<Unit> LoadEslPlayers(string link);
    }

    public partial class MatchSessionEnd : Form
    {
        private readonly Logging.LoggingInterfaces.ITracer logger;

        private readonly LocalDatabaseWrapper context;

        private readonly MatchSession session;

        private IMatchSession myMatchSession;

        public MatchSessionEnd(
            Logging.LoggingInterfaces.ITracer logger, 
            LocalDatabaseWrapper context, 
            IMatchSession session)
        {
            this.logger = logger;
            this.context = context;
            this.myMatchSession = session;
            this.session = session.Session;
            InitializeComponent();
        }

        public bool DeleteMatchmedia { get; private set; }

        private void MatchSessionEnd_Load(object sender, EventArgs e)
        {
            Logging.setupLogging(logger); 
            session.MatchSessions_Tag.Load();

            tagTextBox.Text = string.Join(
                ",", (from assoc in session.MatchSessions_Tag select assoc.Tag.Name).ToArray());
            matchmediaBindingSource.DataSource = session.Matchmedia;
            eslMatchCheckBox.Checked = session.EslMatchLink != null;
            EslMatchIdTextBox.Text = 
                string.IsNullOrEmpty(session.EslMatchLink)
                ? "http://www.esl.eu/" : session.EslMatchLink;

            EslMatchIdTextBox.Enabled = session.EslMatchLink != null;
            int i = -1;
            foreach (var file in session.Matchmedia)
            {
                i++;
                file.Matchmedia_Tag.Load();
                var row = matchmediaDataGridView.Rows[i];
                row.Cells["Tags"].Value = String.Join(
                    ",", (from assoc in file.Matchmedia_Tag select assoc.Tag.Name).ToArray());
            }
        }

        private void saveMatchmediaButton_Click(object sender, EventArgs e)
        {
            try
            {
                this.SetEslMatchId();

                var tags = tagTextBox.Text.Split(',');
                foreach (var tag in tags)
                {
                    if (!(from assoc in session.MatchSessions_Tag
                          where assoc.Tag.Name == tag
                          select assoc).Any())
                    {
                        var association = new MatchSessions_Tag();
                        association.MatchSession = session;
                        association.Tag = context.GetTag(tag);
                        context.Context.MatchSessions_Tags.InsertOnSubmit(association);
                        session.MatchSessions_Tag.Add(association);
                    }
                }

                int i = -1;
                foreach (var file in session.Matchmedia)
                {
                    i++;
                    var row = matchmediaDataGridView.Rows[i];
                    var media_tags = row.Cells["Tags"].Value.ToString().Split(',');
                    foreach (var tag in media_tags)
                    {
                        if (!(from assoc in file.Matchmedia_Tag
                              where assoc.Tag.Name == tag
                              select assoc).Any())
                        {
                            var association = new Matchmedia_Tag();
                            association.Matchmedia = file;
                            association.Tag = context.GetTag(tag);
                            file.Matchmedia_Tag.Add(association);
                        }
                    }
                }

                this.SetupRemember(true);
                DeleteMatchmedia = false;
                this.Close();
            }
            catch (Exception ex)
            {
                logger.LogError("{0}", "Could not save Matchmedia, Ex: " + ex);
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void SetEslMatchId()
        {
            if (this.eslMatchCheckBox.Checked)
            {
                new Uri(this.EslMatchIdTextBox.Text);
                this.session.EslMatchLink = this.EslMatchIdTextBox.Text;
            }
            else
            {
                this.session.EslMatchLink = null;
            }
        }


        private void deleteMatchmediaButton_Click(object sender, EventArgs e)
        {
            this.SetupRemember(false);
            DeleteMatchmedia = true;
        }

        private void SetupRemember(bool saveData)
        {
            if (!this.rememberCheckBox.Checked)
            {
                return;
            }

            var game = this.session.Game;
            if (string.IsNullOrEmpty(this.session.EslMatchLink))
            { // Public Mode
                game.EnableMatchForm = false;
                game.PublicMatchFormSaveFiles = saveData;
            }
            else
            {
                game.EnableWarMatchForm = false;
                game.WarMatchFormSaveFiles = saveData;
            }
        }

        private void eslMatchCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            EslMatchIdTextBox.Enabled = eslMatchCheckBox.Checked;
        }

        private void addMatchmediaButton_Click(object sender, EventArgs e)
        {
            var fod = new OpenFileDialog();
            var res = fod.ShowDialog();
            if (res == DialogResult.OK)
            {
                AddMatchMedia(fod.SafeFileName);
            }
        }


        private void AddMatchMedia(string safeFileName)
        {
            var media = new Database.Matchmedia();
            media.Path = safeFileName;
            media.Map = Yaaf.WirePlugin.MediaAnalyser.analyseMedia(safeFileName).Map;
            media.Name = Path.GetFileNameWithoutExtension(safeFileName);
            media.Created = DateTime.Now;
            media.Type = Path.GetExtension(safeFileName);
            media.MatchSession = this.session;
            matchmediaBindingSource.Add(media);
        }

        private void managePlayersButton_Click(object sender, EventArgs e)
        {
            try
            {
                var managePlayer = new ManageMatchPlayers(logger, context, session);
                managePlayer.ShowDialog();
            }
            catch (Exception ex)
            {
                logger.LogError("{0}", "Could not open ManageMatchPlayers, Ex: " + ex);
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        void task_Finished(object sender, Unit args)
        {
            logger.LogInformation("Task finished!");
        }

        void task_Error(object sender, Exception args)
        {
            logger.LogError("Task finished with error: {0}", args);
        }

        private void EslMatchIdTextBox_Leave(object sender, EventArgs e)
        {
            // Load Enemies from ESL page and see if we can add new infos
            var async = this.myMatchSession.LoadEslPlayers(this.session.EslMatchLink);
            var task = new Task<Unit>(async);
            task.Error += this.task_Error;
            task.Finished += this.task_Finished;
            task.WaitForFinished();
        }
    }
}
