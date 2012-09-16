namespace Yaaf.WirePlugin.WinFormGui
{
    partial class EditMatchSession
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EditMatchSession));
            this.matchPlayersDataGridView = new System.Windows.Forms.DataGridView();
            this.PlayerId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.EslPlayerId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PlayerName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Tags = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Description = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Skill = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.Team = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.Cheating = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.matchSessionIdDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.playerIdDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.teamDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.skillDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.descriptionDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cheatingDataGridViewCheckBoxColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.matchSessionDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.playerDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.matchSessionsPlayerBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.matchmediaDataGridView = new System.Windows.Forms.DataGridView();
            this.nameDataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MatchmediaTagsColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.typeDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.mapDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pathDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.createdDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.matchmediaContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.copyFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showInExplorerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.matchmediaBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.saveButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.primaryPlayerLabel = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.matchnameTextBox = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.matchTagsTextBox = new System.Windows.Forms.TextBox();
            this.loadMatchDataButton = new System.Windows.Forms.Button();
            this.linkLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.matchPlayersDataGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.matchSessionsPlayerBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.matchmediaDataGridView)).BeginInit();
            this.matchmediaContextMenuStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.matchmediaBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // matchPlayersDataGridView
            // 
            this.matchPlayersDataGridView.AllowUserToOrderColumns = true;
            this.matchPlayersDataGridView.AutoGenerateColumns = false;
            this.matchPlayersDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.matchPlayersDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.PlayerId,
            this.EslPlayerId,
            this.PlayerName,
            this.Tags,
            this.Description,
            this.Skill,
            this.Team,
            this.Cheating,
            this.matchSessionIdDataGridViewTextBoxColumn,
            this.playerIdDataGridViewTextBoxColumn,
            this.teamDataGridViewTextBoxColumn,
            this.skillDataGridViewTextBoxColumn,
            this.descriptionDataGridViewTextBoxColumn,
            this.cheatingDataGridViewCheckBoxColumn,
            this.matchSessionDataGridViewTextBoxColumn,
            this.playerDataGridViewTextBoxColumn});
            this.matchPlayersDataGridView.DataSource = this.matchSessionsPlayerBindingSource;
            resources.ApplyResources(this.matchPlayersDataGridView, "matchPlayersDataGridView");
            this.matchPlayersDataGridView.Name = "matchPlayersDataGridView";
            this.matchPlayersDataGridView.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.matchPlayersDataGridView_DataError);
            this.matchPlayersDataGridView.SelectionChanged += new System.EventHandler(this.matchPlayersDataGridView_SelectionChanged);
            // 
            // PlayerId
            // 
            this.PlayerId.DataPropertyName = "Id";
            this.PlayerId.FillWeight = 50F;
            resources.ApplyResources(this.PlayerId, "PlayerId");
            this.PlayerId.Name = "PlayerId";
            // 
            // EslPlayerId
            // 
            this.EslPlayerId.DataPropertyName = "EslId";
            this.EslPlayerId.FillWeight = 75F;
            resources.ApplyResources(this.EslPlayerId, "EslPlayerId");
            this.EslPlayerId.Name = "EslPlayerId";
            // 
            // PlayerName
            // 
            this.PlayerName.DataPropertyName = "Name";
            resources.ApplyResources(this.PlayerName, "PlayerName");
            this.PlayerName.Name = "PlayerName";
            // 
            // Tags
            // 
            this.Tags.DataPropertyName = "Tags";
            resources.ApplyResources(this.Tags, "Tags");
            this.Tags.Name = "Tags";
            // 
            // Description
            // 
            this.Description.DataPropertyName = "Description";
            this.Description.FillWeight = 200F;
            resources.ApplyResources(this.Description, "Description");
            this.Description.Name = "Description";
            // 
            // Skill
            // 
            this.Skill.DataPropertyName = "Skill";
            this.Skill.FillWeight = 70F;
            resources.ApplyResources(this.Skill, "Skill");
            this.Skill.Name = "Skill";
            this.Skill.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Skill.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // Team
            // 
            this.Team.DataPropertyName = "Team";
            this.Team.FillWeight = 70F;
            resources.ApplyResources(this.Team, "Team");
            this.Team.Name = "Team";
            this.Team.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Team.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // Cheating
            // 
            this.Cheating.DataPropertyName = "Cheating";
            this.Cheating.FillWeight = 50F;
            resources.ApplyResources(this.Cheating, "Cheating");
            this.Cheating.Name = "Cheating";
            // 
            // matchSessionIdDataGridViewTextBoxColumn
            // 
            this.matchSessionIdDataGridViewTextBoxColumn.DataPropertyName = "MatchSessionId";
            resources.ApplyResources(this.matchSessionIdDataGridViewTextBoxColumn, "matchSessionIdDataGridViewTextBoxColumn");
            this.matchSessionIdDataGridViewTextBoxColumn.Name = "matchSessionIdDataGridViewTextBoxColumn";
            // 
            // playerIdDataGridViewTextBoxColumn
            // 
            this.playerIdDataGridViewTextBoxColumn.DataPropertyName = "PlayerId";
            resources.ApplyResources(this.playerIdDataGridViewTextBoxColumn, "playerIdDataGridViewTextBoxColumn");
            this.playerIdDataGridViewTextBoxColumn.Name = "playerIdDataGridViewTextBoxColumn";
            // 
            // teamDataGridViewTextBoxColumn
            // 
            this.teamDataGridViewTextBoxColumn.DataPropertyName = "Team";
            resources.ApplyResources(this.teamDataGridViewTextBoxColumn, "teamDataGridViewTextBoxColumn");
            this.teamDataGridViewTextBoxColumn.Name = "teamDataGridViewTextBoxColumn";
            // 
            // skillDataGridViewTextBoxColumn
            // 
            this.skillDataGridViewTextBoxColumn.DataPropertyName = "Skill";
            resources.ApplyResources(this.skillDataGridViewTextBoxColumn, "skillDataGridViewTextBoxColumn");
            this.skillDataGridViewTextBoxColumn.Name = "skillDataGridViewTextBoxColumn";
            // 
            // descriptionDataGridViewTextBoxColumn
            // 
            this.descriptionDataGridViewTextBoxColumn.DataPropertyName = "Description";
            resources.ApplyResources(this.descriptionDataGridViewTextBoxColumn, "descriptionDataGridViewTextBoxColumn");
            this.descriptionDataGridViewTextBoxColumn.Name = "descriptionDataGridViewTextBoxColumn";
            // 
            // cheatingDataGridViewCheckBoxColumn
            // 
            this.cheatingDataGridViewCheckBoxColumn.DataPropertyName = "Cheating";
            resources.ApplyResources(this.cheatingDataGridViewCheckBoxColumn, "cheatingDataGridViewCheckBoxColumn");
            this.cheatingDataGridViewCheckBoxColumn.Name = "cheatingDataGridViewCheckBoxColumn";
            // 
            // matchSessionDataGridViewTextBoxColumn
            // 
            this.matchSessionDataGridViewTextBoxColumn.DataPropertyName = "MatchSession";
            resources.ApplyResources(this.matchSessionDataGridViewTextBoxColumn, "matchSessionDataGridViewTextBoxColumn");
            this.matchSessionDataGridViewTextBoxColumn.Name = "matchSessionDataGridViewTextBoxColumn";
            // 
            // playerDataGridViewTextBoxColumn
            // 
            this.playerDataGridViewTextBoxColumn.DataPropertyName = "Player";
            resources.ApplyResources(this.playerDataGridViewTextBoxColumn, "playerDataGridViewTextBoxColumn");
            this.playerDataGridViewTextBoxColumn.Name = "playerDataGridViewTextBoxColumn";
            // 
            // matchSessionsPlayerBindingSource
            // 
            this.matchSessionsPlayerBindingSource.DataSource = typeof(Yaaf.WirePlugin.WinFormGui.Database.MatchSessions_Player);
            // 
            // matchmediaDataGridView
            // 
            this.matchmediaDataGridView.AllowDrop = true;
            this.matchmediaDataGridView.AllowUserToAddRows = false;
            this.matchmediaDataGridView.AutoGenerateColumns = false;
            this.matchmediaDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.matchmediaDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.nameDataGridViewTextBoxColumn1,
            this.MatchmediaTagsColumn,
            this.typeDataGridViewTextBoxColumn,
            this.mapDataGridViewTextBoxColumn,
            this.pathDataGridViewTextBoxColumn,
            this.createdDataGridViewTextBoxColumn});
            this.matchmediaDataGridView.ContextMenuStrip = this.matchmediaContextMenuStrip;
            this.matchmediaDataGridView.DataSource = this.matchmediaBindingSource;
            resources.ApplyResources(this.matchmediaDataGridView, "matchmediaDataGridView");
            this.matchmediaDataGridView.Name = "matchmediaDataGridView";
            this.matchmediaDataGridView.DragDrop += new System.Windows.Forms.DragEventHandler(this.matchmediaDataGridView_DragDrop);
            this.matchmediaDataGridView.DragEnter += new System.Windows.Forms.DragEventHandler(this.matchmediaDataGridView_DragEnter);
            // 
            // nameDataGridViewTextBoxColumn1
            // 
            this.nameDataGridViewTextBoxColumn1.DataPropertyName = "Name";
            this.nameDataGridViewTextBoxColumn1.FillWeight = 90F;
            resources.ApplyResources(this.nameDataGridViewTextBoxColumn1, "nameDataGridViewTextBoxColumn1");
            this.nameDataGridViewTextBoxColumn1.Name = "nameDataGridViewTextBoxColumn1";
            // 
            // MatchmediaTagsColumn
            // 
            this.MatchmediaTagsColumn.DataPropertyName = "MyTags";
            this.MatchmediaTagsColumn.FillWeight = 90F;
            resources.ApplyResources(this.MatchmediaTagsColumn, "MatchmediaTagsColumn");
            this.MatchmediaTagsColumn.Name = "MatchmediaTagsColumn";
            // 
            // typeDataGridViewTextBoxColumn
            // 
            this.typeDataGridViewTextBoxColumn.DataPropertyName = "Type";
            this.typeDataGridViewTextBoxColumn.FillWeight = 70F;
            resources.ApplyResources(this.typeDataGridViewTextBoxColumn, "typeDataGridViewTextBoxColumn");
            this.typeDataGridViewTextBoxColumn.Name = "typeDataGridViewTextBoxColumn";
            this.typeDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // mapDataGridViewTextBoxColumn
            // 
            this.mapDataGridViewTextBoxColumn.DataPropertyName = "Map";
            this.mapDataGridViewTextBoxColumn.FillWeight = 70F;
            resources.ApplyResources(this.mapDataGridViewTextBoxColumn, "mapDataGridViewTextBoxColumn");
            this.mapDataGridViewTextBoxColumn.Name = "mapDataGridViewTextBoxColumn";
            // 
            // pathDataGridViewTextBoxColumn
            // 
            this.pathDataGridViewTextBoxColumn.DataPropertyName = "Path";
            this.pathDataGridViewTextBoxColumn.FillWeight = 200F;
            resources.ApplyResources(this.pathDataGridViewTextBoxColumn, "pathDataGridViewTextBoxColumn");
            this.pathDataGridViewTextBoxColumn.Name = "pathDataGridViewTextBoxColumn";
            this.pathDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // createdDataGridViewTextBoxColumn
            // 
            this.createdDataGridViewTextBoxColumn.DataPropertyName = "Created";
            resources.ApplyResources(this.createdDataGridViewTextBoxColumn, "createdDataGridViewTextBoxColumn");
            this.createdDataGridViewTextBoxColumn.Name = "createdDataGridViewTextBoxColumn";
            // 
            // matchmediaContextMenuStrip
            // 
            this.matchmediaContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyFileToolStripMenuItem,
            this.showInExplorerToolStripMenuItem});
            this.matchmediaContextMenuStrip.Name = "contextMenuStrip1";
            resources.ApplyResources(this.matchmediaContextMenuStrip, "matchmediaContextMenuStrip");
            // 
            // copyFileToolStripMenuItem
            // 
            this.copyFileToolStripMenuItem.Name = "copyFileToolStripMenuItem";
            resources.ApplyResources(this.copyFileToolStripMenuItem, "copyFileToolStripMenuItem");
            this.copyFileToolStripMenuItem.Click += new System.EventHandler(this.copyFileToolStripMenuItem_Click);
            // 
            // showInExplorerToolStripMenuItem
            // 
            this.showInExplorerToolStripMenuItem.Name = "showInExplorerToolStripMenuItem";
            resources.ApplyResources(this.showInExplorerToolStripMenuItem, "showInExplorerToolStripMenuItem");
            this.showInExplorerToolStripMenuItem.Click += new System.EventHandler(this.showInExplorerToolStripMenuItem_Click);
            // 
            // matchmediaBindingSource
            // 
            this.matchmediaBindingSource.DataSource = typeof(Yaaf.WirePlugin.WinFormGui.Database.Matchmedia);
            // 
            // saveButton
            // 
            resources.ApplyResources(this.saveButton, "saveButton");
            this.saveButton.Name = "saveButton";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // cancelButton
            // 
            resources.ApplyResources(this.cancelButton, "cancelButton");
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // primaryPlayerLabel
            // 
            resources.ApplyResources(this.primaryPlayerLabel, "primaryPlayerLabel");
            this.primaryPlayerLabel.Name = "primaryPlayerLabel";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // matchnameTextBox
            // 
            resources.ApplyResources(this.matchnameTextBox, "matchnameTextBox");
            this.matchnameTextBox.Name = "matchnameTextBox";
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // matchTagsTextBox
            // 
            resources.ApplyResources(this.matchTagsTextBox, "matchTagsTextBox");
            this.matchTagsTextBox.Name = "matchTagsTextBox";
            // 
            // loadMatchDataButton
            // 
            resources.ApplyResources(this.loadMatchDataButton, "loadMatchDataButton");
            this.loadMatchDataButton.Name = "loadMatchDataButton";
            this.loadMatchDataButton.UseVisualStyleBackColor = true;
            this.loadMatchDataButton.Click += new System.EventHandler(this.loadMatchDataButton_Click);
            // 
            // linkLabel
            // 
            resources.ApplyResources(this.linkLabel, "linkLabel");
            this.linkLabel.Name = "linkLabel";
            // 
            // EditMatchSession
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.linkLabel);
            this.Controls.Add(this.matchmediaDataGridView);
            this.Controls.Add(this.loadMatchDataButton);
            this.Controls.Add(this.matchnameTextBox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.matchTagsTextBox);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.primaryPlayerLabel);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.saveButton);
            this.Controls.Add(this.matchPlayersDataGridView);
            this.Controls.Add(this.cancelButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "EditMatchSession";
            this.Load += new System.EventHandler(this.EditMatchSession_Load);
            ((System.ComponentModel.ISupportInitialize)(this.matchPlayersDataGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.matchSessionsPlayerBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.matchmediaDataGridView)).EndInit();
            this.matchmediaContextMenuStrip.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.matchmediaBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView matchPlayersDataGridView;
        private System.Windows.Forms.DataGridView matchmediaDataGridView;
        private System.Windows.Forms.BindingSource matchmediaBindingSource;
        private System.Windows.Forms.BindingSource matchSessionsPlayerBindingSource;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.ContextMenuStrip matchmediaContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem copyFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showInExplorerToolStripMenuItem;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label primaryPlayerLabel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox matchnameTextBox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox matchTagsTextBox;
        private System.Windows.Forms.Button loadMatchDataButton;
        private System.Windows.Forms.DataGridViewTextBoxColumn nameDataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn MatchmediaTagsColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn typeDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn mapDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn pathDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn createdDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn PlayerId;
        private System.Windows.Forms.DataGridViewTextBoxColumn EslPlayerId;
        private System.Windows.Forms.DataGridViewTextBoxColumn PlayerName;
        private System.Windows.Forms.DataGridViewTextBoxColumn Tags;
        private System.Windows.Forms.DataGridViewTextBoxColumn Description;
        private System.Windows.Forms.DataGridViewComboBoxColumn Skill;
        private System.Windows.Forms.DataGridViewComboBoxColumn Team;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Cheating;
        private System.Windows.Forms.Label linkLabel;
        private System.Windows.Forms.DataGridViewTextBoxColumn matchSessionIdDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn playerIdDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn teamDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn skillDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn descriptionDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewCheckBoxColumn cheatingDataGridViewCheckBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn matchSessionDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn playerDataGridViewTextBoxColumn;
    }
}