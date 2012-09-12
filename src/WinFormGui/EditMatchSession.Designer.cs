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
            this.matchPlayersDataGridView = new System.Windows.Forms.DataGridView();
            this.PlayerId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PlayerName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.EslPlayerId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Tags = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.descriptionDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.teamDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.skillDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.cheatingDataGridViewCheckBoxColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.matchSessionsPlayerBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.playerBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.playersDataGridView = new System.Windows.Forms.DataGridView();
            this.matchmediaContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.copyFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showInExplorerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.matchmediaBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.saveButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.nameDataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.typeDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.mapDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pathDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.createdDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label1 = new System.Windows.Forms.Label();
            this.primaryPlayerLabel = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.matchPlayersDataGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.matchSessionsPlayerBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.playerBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.playersDataGridView)).BeginInit();
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
            this.PlayerName,
            this.EslPlayerId,
            this.Tags,
            this.descriptionDataGridViewTextBoxColumn,
            this.teamDataGridViewTextBoxColumn,
            this.skillDataGridViewTextBoxColumn,
            this.cheatingDataGridViewCheckBoxColumn});
            this.matchPlayersDataGridView.DataSource = this.matchSessionsPlayerBindingSource;
            this.matchPlayersDataGridView.Location = new System.Drawing.Point(12, 37);
            this.matchPlayersDataGridView.Name = "matchPlayersDataGridView";
            this.matchPlayersDataGridView.Size = new System.Drawing.Size(728, 196);
            this.matchPlayersDataGridView.TabIndex = 0;
            this.matchPlayersDataGridView.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.matchPlayersDataGridView_DataError);
            this.matchPlayersDataGridView.SelectionChanged += new System.EventHandler(this.matchPlayersDataGridView_SelectionChanged);
            // 
            // PlayerId
            // 
            this.PlayerId.FillWeight = 50F;
            this.PlayerId.HeaderText = "Id";
            this.PlayerId.Name = "PlayerId";
            this.PlayerId.ReadOnly = true;
            this.PlayerId.Width = 50;
            // 
            // PlayerName
            // 
            this.PlayerName.HeaderText = "Name";
            this.PlayerName.Name = "PlayerName";
            // 
            // EslPlayerId
            // 
            this.EslPlayerId.FillWeight = 75F;
            this.EslPlayerId.HeaderText = "EslPlayerId";
            this.EslPlayerId.Name = "EslPlayerId";
            this.EslPlayerId.ReadOnly = true;
            this.EslPlayerId.Width = 70;
            // 
            // Tags
            // 
            this.Tags.HeaderText = "Tags";
            this.Tags.Name = "Tags";
            // 
            // descriptionDataGridViewTextBoxColumn
            // 
            this.descriptionDataGridViewTextBoxColumn.DataPropertyName = "Description";
            this.descriptionDataGridViewTextBoxColumn.FillWeight = 200F;
            this.descriptionDataGridViewTextBoxColumn.HeaderText = "Description";
            this.descriptionDataGridViewTextBoxColumn.Name = "descriptionDataGridViewTextBoxColumn";
            this.descriptionDataGridViewTextBoxColumn.Width = 200;
            // 
            // teamDataGridViewTextBoxColumn
            // 
            this.teamDataGridViewTextBoxColumn.DataPropertyName = "Team";
            this.teamDataGridViewTextBoxColumn.FillWeight = 50F;
            this.teamDataGridViewTextBoxColumn.HeaderText = "Team";
            this.teamDataGridViewTextBoxColumn.Name = "teamDataGridViewTextBoxColumn";
            this.teamDataGridViewTextBoxColumn.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.teamDataGridViewTextBoxColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.teamDataGridViewTextBoxColumn.Width = 50;
            // 
            // skillDataGridViewTextBoxColumn
            // 
            this.skillDataGridViewTextBoxColumn.DataPropertyName = "Skill";
            this.skillDataGridViewTextBoxColumn.FillWeight = 50F;
            this.skillDataGridViewTextBoxColumn.HeaderText = "Skill";
            this.skillDataGridViewTextBoxColumn.Name = "skillDataGridViewTextBoxColumn";
            this.skillDataGridViewTextBoxColumn.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.skillDataGridViewTextBoxColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.skillDataGridViewTextBoxColumn.Width = 50;
            // 
            // cheatingDataGridViewCheckBoxColumn
            // 
            this.cheatingDataGridViewCheckBoxColumn.DataPropertyName = "Cheating";
            this.cheatingDataGridViewCheckBoxColumn.FillWeight = 50F;
            this.cheatingDataGridViewCheckBoxColumn.HeaderText = "Cheating";
            this.cheatingDataGridViewCheckBoxColumn.Name = "cheatingDataGridViewCheckBoxColumn";
            this.cheatingDataGridViewCheckBoxColumn.Width = 50;
            // 
            // matchSessionsPlayerBindingSource
            // 
            this.matchSessionsPlayerBindingSource.DataSource = typeof(Yaaf.WirePlugin.WinFormGui.Database.MatchSessions_Player);
            // 
            // playerBindingSource
            // 
            this.playerBindingSource.DataSource = typeof(Yaaf.WirePlugin.WinFormGui.Database.Player);
            // 
            // playersDataGridView
            // 
            this.playersDataGridView.AllowDrop = true;
            this.playersDataGridView.AllowUserToAddRows = false;
            this.playersDataGridView.AutoGenerateColumns = false;
            this.playersDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.playersDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.nameDataGridViewTextBoxColumn1,
            this.typeDataGridViewTextBoxColumn,
            this.mapDataGridViewTextBoxColumn,
            this.pathDataGridViewTextBoxColumn,
            this.createdDataGridViewTextBoxColumn});
            this.playersDataGridView.ContextMenuStrip = this.matchmediaContextMenuStrip;
            this.playersDataGridView.DataSource = this.matchmediaBindingSource;
            this.playersDataGridView.Location = new System.Drawing.Point(12, 293);
            this.playersDataGridView.Name = "playersDataGridView";
            this.playersDataGridView.Size = new System.Drawing.Size(728, 207);
            this.playersDataGridView.TabIndex = 1;
            this.playersDataGridView.DragDrop += new System.Windows.Forms.DragEventHandler(this.dataGridView2_DragDrop);
            this.playersDataGridView.DragEnter += new System.Windows.Forms.DragEventHandler(this.dataGridView2_DragEnter);
            // 
            // matchmediaContextMenuStrip
            // 
            this.matchmediaContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyFileToolStripMenuItem,
            this.showInExplorerToolStripMenuItem});
            this.matchmediaContextMenuStrip.Name = "contextMenuStrip1";
            this.matchmediaContextMenuStrip.Size = new System.Drawing.Size(162, 48);
            // 
            // copyFileToolStripMenuItem
            // 
            this.copyFileToolStripMenuItem.Name = "copyFileToolStripMenuItem";
            this.copyFileToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.copyFileToolStripMenuItem.Text = "Copy File";
            this.copyFileToolStripMenuItem.Click += new System.EventHandler(this.copyFileToolStripMenuItem_Click);
            // 
            // showInExplorerToolStripMenuItem
            // 
            this.showInExplorerToolStripMenuItem.Name = "showInExplorerToolStripMenuItem";
            this.showInExplorerToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.showInExplorerToolStripMenuItem.Text = "Show in Explorer";
            this.showInExplorerToolStripMenuItem.Click += new System.EventHandler(this.showInExplorerToolStripMenuItem_Click);
            // 
            // matchmediaBindingSource
            // 
            this.matchmediaBindingSource.DataSource = typeof(Yaaf.WirePlugin.WinFormGui.Database.Matchmedia);
            // 
            // saveButton
            // 
            this.saveButton.Location = new System.Drawing.Point(391, 506);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(349, 41);
            this.saveButton.TabIndex = 2;
            this.saveButton.Text = "Save\r\n(Don\'t forget to also save in \"View Matchsessions Dialog\"!)";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Location = new System.Drawing.Point(241, 510);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(85, 32);
            this.cancelButton.TabIndex = 3;
            this.cancelButton.Text = "cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // nameDataGridViewTextBoxColumn1
            // 
            this.nameDataGridViewTextBoxColumn1.DataPropertyName = "Name";
            this.nameDataGridViewTextBoxColumn1.HeaderText = "Name";
            this.nameDataGridViewTextBoxColumn1.Name = "nameDataGridViewTextBoxColumn1";
            // 
            // typeDataGridViewTextBoxColumn
            // 
            this.typeDataGridViewTextBoxColumn.DataPropertyName = "Type";
            this.typeDataGridViewTextBoxColumn.HeaderText = "Type";
            this.typeDataGridViewTextBoxColumn.Name = "typeDataGridViewTextBoxColumn";
            // 
            // mapDataGridViewTextBoxColumn
            // 
            this.mapDataGridViewTextBoxColumn.DataPropertyName = "Map";
            this.mapDataGridViewTextBoxColumn.HeaderText = "Map";
            this.mapDataGridViewTextBoxColumn.Name = "mapDataGridViewTextBoxColumn";
            // 
            // pathDataGridViewTextBoxColumn
            // 
            this.pathDataGridViewTextBoxColumn.DataPropertyName = "Path";
            this.pathDataGridViewTextBoxColumn.FillWeight = 250F;
            this.pathDataGridViewTextBoxColumn.HeaderText = "Path";
            this.pathDataGridViewTextBoxColumn.Name = "pathDataGridViewTextBoxColumn";
            this.pathDataGridViewTextBoxColumn.ReadOnly = true;
            this.pathDataGridViewTextBoxColumn.Width = 250;
            // 
            // createdDataGridViewTextBoxColumn
            // 
            this.createdDataGridViewTextBoxColumn.DataPropertyName = "Created";
            this.createdDataGridViewTextBoxColumn.HeaderText = "Created";
            this.createdDataGridViewTextBoxColumn.Name = "createdDataGridViewTextBoxColumn";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 273);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(189, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Dragged Matchmedia will be added to:";
            // 
            // primaryPlayerLabel
            // 
            this.primaryPlayerLabel.AutoSize = true;
            this.primaryPlayerLabel.Location = new System.Drawing.Point(269, 273);
            this.primaryPlayerLabel.Name = "primaryPlayerLabel";
            this.primaryPlayerLabel.Size = new System.Drawing.Size(33, 13);
            this.primaryPlayerLabel.TabIndex = 5;
            this.primaryPlayerLabel.Text = "None";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 249);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(492, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Drag and drop files below to add them. Select a player from above to see his matc" +
    "hmedia for this match";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(15, 13);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(615, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Edit Players for this Match. Id, Name, EslPlayerId and Tags are global. Descripti" +
    "on Team Skill and Cheating flag is Match specific.";
            // 
            // EditMatchSession
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(752, 559);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.primaryPlayerLabel);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.matchPlayersDataGridView);
            this.Controls.Add(this.playersDataGridView);
            this.Controls.Add(this.saveButton);
            this.Controls.Add(this.cancelButton);
            this.Name = "EditMatchSession";
            this.Text = "Edit Matchsession";
            this.Load += new System.EventHandler(this.EditMatchSession_Load);
            ((System.ComponentModel.ISupportInitialize)(this.matchPlayersDataGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.matchSessionsPlayerBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.playerBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.playersDataGridView)).EndInit();
            this.matchmediaContextMenuStrip.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.matchmediaBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView matchPlayersDataGridView;
        private System.Windows.Forms.BindingSource playerBindingSource;
        private System.Windows.Forms.DataGridView playersDataGridView;
        private System.Windows.Forms.BindingSource matchmediaBindingSource;
        private System.Windows.Forms.BindingSource matchSessionsPlayerBindingSource;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.ContextMenuStrip matchmediaContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem copyFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showInExplorerToolStripMenuItem;
        private System.Windows.Forms.DataGridViewTextBoxColumn PlayerId;
        private System.Windows.Forms.DataGridViewTextBoxColumn PlayerName;
        private System.Windows.Forms.DataGridViewTextBoxColumn EslPlayerId;
        private System.Windows.Forms.DataGridViewTextBoxColumn Tags;
        private System.Windows.Forms.DataGridViewTextBoxColumn descriptionDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewComboBoxColumn teamDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewComboBoxColumn skillDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewCheckBoxColumn cheatingDataGridViewCheckBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn nameDataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn typeDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn mapDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn pathDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn createdDataGridViewTextBoxColumn;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label primaryPlayerLabel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
    }
}