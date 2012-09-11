// ----------------------------------------------------------------------------
// This file (MatchSessionEnd.cs) is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package (Yaaf.WirePlugin).
// Last Modified: 2012/09/10 14:08
// Created: 2012/08/26 20:57
// ----------------------------------------------------------------------------

namespace Yaaf.WirePlugin.WinFormGui
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Windows.Forms;

    using Microsoft.FSharp.Control;
    using Microsoft.FSharp.Core;

    using Yaaf.WirePlugin.Primitives;
    using Yaaf.WirePlugin.WinFormGui.Database;

    public interface IMatchSession
    {
        MatchSession Session { get; }

        Player IdentityPlayer { get; }
        FSharpAsync<Unit> LoadEslPlayers(string link);
    }

    public partial class MatchSessionEnd : Form
    {
        private readonly LocalDatabaseWrapper context;

        private readonly Logging.LoggingInterfaces.ITracer logger;

        private readonly IMatchSession myMatchSession;

        private readonly MatchSession session;

        public MatchSessionEnd(
            Logging.LoggingInterfaces.ITracer logger, LocalDatabaseWrapper context, IMatchSession session)
        {
            this.logger = logger;
            this.context = context;
            myMatchSession = session;
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
            EslMatchIdTextBox.Text = string.IsNullOrEmpty(session.EslMatchLink)
                                         ? "http://www.esl.eu/"
                                         : session.EslMatchLink;

            EslMatchIdTextBox.Enabled = session.EslMatchLink != null;
            var i = -1;
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
                SetEslMatchId();

                var tags = tagTextBox.Text.Split(',');
                foreach (var tag in tags)
                {
                    if (!(from assoc in session.MatchSessions_Tag where assoc.Tag.Name == tag select assoc).Any())
                    {
                        var association = new MatchSessions_Tag();
                        association.MatchSession = session;
                        association.Tag = context.GetTag(tag);
                        context.Context.MatchSessions_Tags.InsertOnSubmit(association);
                        session.MatchSessions_Tag.Add(association);
                    }
                }

                var i = -1;
                foreach (var file in session.Matchmedia)
                {
                    i++;
                    var row = matchmediaDataGridView.Rows[i];
                    var mytags = row.Cells["Tags"].Value;
                    var media_tags = (mytags == null ? "" : mytags.ToString()).Split(',');
                    foreach (var tag in media_tags)
                    {
                        if (!(from assoc in file.Matchmedia_Tag where assoc.Tag.Name == tag select assoc).Any())
                        {
                            var association = new Matchmedia_Tag();
                            association.Matchmedia = file;
                            association.Tag = context.GetTag(tag);
                            file.Matchmedia_Tag.Add(association);
                        }
                    }
                }

                SetupRemember(true);
                DeleteMatchmedia = false;
                Close();
            }
            catch (Exception ex)
            {
                logger.LogError("{0}", "Could not save Matchmedia, Ex: " + ex);
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetEslMatchId()
        {
            if (eslMatchCheckBox.Checked)
            {
                new Uri(EslMatchIdTextBox.Text);
                session.EslMatchLink = EslMatchIdTextBox.Text;
            }
            else
            {
                session.EslMatchLink = null;
            }
        }

        private void deleteMatchmediaButton_Click(object sender, EventArgs e)
        {
            SetupRemember(false);
            DeleteMatchmedia = true;
        }

        private void SetupRemember(bool saveData)
        {
            if (!rememberCheckBox.Checked)
            {
                return;
            }

            var game = session.Game;
            if (string.IsNullOrEmpty(session.EslMatchLink))
            {
                // Public Mode
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
            try
            {
                var fod = new OpenFileDialog();
                var res = fod.ShowDialog();
                if (res == DialogResult.OK)
                {
                    AddMatchMedia(fod.FileName);
                }
            }
            catch (Exception ex)
            {
                logger.LogError("{0}", "Could not add Matchmedia, Ex: " + ex);
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AddMatchMedia(string safeFileName)
        {
            var media = (Matchmedia)matchmediaBindingSource.AddNew();
            if (media == null)
            {
                throw new InvalidOperationException("Couldn't add new Matchmedia (was null)");
            }

            media.Player = myMatchSession.IdentityPlayer;
            media.Path = safeFileName;
            media.Map = MediaAnalyser.analyseMedia(safeFileName).Map;
            media.Name = Path.GetFileNameWithoutExtension(safeFileName);
            media.Created = DateTime.Now;
            media.Type = Path.GetExtension(safeFileName);
            media.MatchSession = session;
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

        private void EslMatchIdTextBox_Leave(object sender, EventArgs e)
        {
            try
            {
                // Load Enemies from ESL page and see if we can add new infos
                SetEslMatchId();
                var async = myMatchSession.LoadEslPlayers(session.EslMatchLink);
                var task = new Task<Unit>(async);
                task.Error +=
                    (senderTask, eTask) =>
                    MessageBox.Show(eTask.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                WaitingForm.StartTask(logger, task, "Loading Players...");
            }
            catch (Exception ex)
            {
                logger.LogError("{0}", "Could not start player grabbing task, Ex: " + ex);
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}