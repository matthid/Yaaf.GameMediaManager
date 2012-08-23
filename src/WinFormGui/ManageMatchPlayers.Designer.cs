namespace Yaaf.WirePlugin.WinFormGui
{
    partial class ManageMatchPlayers
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ManageMatchPlayers));
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.matchPlayersDataGridView = new System.Windows.Forms.DataGridView();
            this.matchSessionsPlayerBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.PlayerId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PlayerName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.EslPlayerId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Tags = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.descriptionDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.teamDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.skillDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.cheatingDataGridViewCheckBoxColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.matchPlayersDataGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.matchSessionsPlayerBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // button1
            // 
            resources.ApplyResources(this.button1, "button1");
            this.button1.Name = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            resources.ApplyResources(this.button2, "button2");
            this.button2.Name = "button2";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // matchPlayersDataGridView
            // 
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
            resources.ApplyResources(this.matchPlayersDataGridView, "matchPlayersDataGridView");
            this.matchPlayersDataGridView.Name = "matchPlayersDataGridView";
            this.matchPlayersDataGridView.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.matchPlayersDataGridView_DataError);
            // 
            // matchSessionsPlayerBindingSource
            // 
            this.matchSessionsPlayerBindingSource.DataSource = typeof(Yaaf.WirePlugin.WinFormGui.Database.MatchSessions_Player);
            // 
            // PlayerId
            // 
            this.PlayerId.FillWeight = 50F;
            resources.ApplyResources(this.PlayerId, "PlayerId");
            this.PlayerId.Name = "PlayerId";
            // 
            // PlayerName
            // 
            resources.ApplyResources(this.PlayerName, "PlayerName");
            this.PlayerName.Name = "PlayerName";
            // 
            // EslPlayerId
            // 
            resources.ApplyResources(this.EslPlayerId, "EslPlayerId");
            this.EslPlayerId.Name = "EslPlayerId";
            // 
            // Tags
            // 
            resources.ApplyResources(this.Tags, "Tags");
            this.Tags.Name = "Tags";
            // 
            // descriptionDataGridViewTextBoxColumn
            // 
            this.descriptionDataGridViewTextBoxColumn.DataPropertyName = "Description";
            this.descriptionDataGridViewTextBoxColumn.FillWeight = 250F;
            resources.ApplyResources(this.descriptionDataGridViewTextBoxColumn, "descriptionDataGridViewTextBoxColumn");
            this.descriptionDataGridViewTextBoxColumn.Name = "descriptionDataGridViewTextBoxColumn";
            // 
            // teamDataGridViewTextBoxColumn
            // 
            this.teamDataGridViewTextBoxColumn.DataPropertyName = "Team";
            this.teamDataGridViewTextBoxColumn.FillWeight = 50F;
            resources.ApplyResources(this.teamDataGridViewTextBoxColumn, "teamDataGridViewTextBoxColumn");
            this.teamDataGridViewTextBoxColumn.Items.AddRange(new object[] {
            "",
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10"});
            this.teamDataGridViewTextBoxColumn.Name = "teamDataGridViewTextBoxColumn";
            this.teamDataGridViewTextBoxColumn.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.teamDataGridViewTextBoxColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // skillDataGridViewTextBoxColumn
            // 
            this.skillDataGridViewTextBoxColumn.DataPropertyName = "Skill";
            this.skillDataGridViewTextBoxColumn.FillWeight = 50F;
            resources.ApplyResources(this.skillDataGridViewTextBoxColumn, "skillDataGridViewTextBoxColumn");
            this.skillDataGridViewTextBoxColumn.Items.AddRange(new object[] {
            "",
            "0",
            "10",
            "20",
            "30",
            "40",
            "5ß",
            "60",
            "70",
            "80",
            "90",
            "100"});
            this.skillDataGridViewTextBoxColumn.Name = "skillDataGridViewTextBoxColumn";
            this.skillDataGridViewTextBoxColumn.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.skillDataGridViewTextBoxColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // cheatingDataGridViewCheckBoxColumn
            // 
            this.cheatingDataGridViewCheckBoxColumn.DataPropertyName = "Cheating";
            this.cheatingDataGridViewCheckBoxColumn.FillWeight = 50F;
            resources.ApplyResources(this.cheatingDataGridViewCheckBoxColumn, "cheatingDataGridViewCheckBoxColumn");
            this.cheatingDataGridViewCheckBoxColumn.Name = "cheatingDataGridViewCheckBoxColumn";
            // 
            // ManageMatchPlayers
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.matchPlayersDataGridView);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "ManageMatchPlayers";
            this.Load += new System.EventHandler(this.ManageMatchPlayers_Load);
            ((System.ComponentModel.ISupportInitialize)(this.matchPlayersDataGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.matchSessionsPlayerBindingSource)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.DataGridView matchPlayersDataGridView;
        private System.Windows.Forms.BindingSource matchSessionsPlayerBindingSource;
        private System.Windows.Forms.DataGridViewTextBoxColumn PlayerId;
        private System.Windows.Forms.DataGridViewTextBoxColumn PlayerName;
        private System.Windows.Forms.DataGridViewTextBoxColumn EslPlayerId;
        private System.Windows.Forms.DataGridViewTextBoxColumn Tags;
        private System.Windows.Forms.DataGridViewTextBoxColumn descriptionDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewComboBoxColumn teamDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewComboBoxColumn skillDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewCheckBoxColumn cheatingDataGridViewCheckBoxColumn;
    }
}