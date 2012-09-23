namespace Yaaf.GameMediaManager.WinFormGui
{
    partial class ViewMatchSessions
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ViewMatchSessions));
            this.matchSessionDataGridView = new System.Windows.Forms.DataGridView();
            this.matchSessionBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.saveButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.idDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MatchsessionTagColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.gameIdDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.startdateDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.durationDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.matchSessionDataGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.matchSessionBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // matchSessionDataGridView
            // 
            this.matchSessionDataGridView.AllowUserToOrderColumns = true;
            resources.ApplyResources(this.matchSessionDataGridView, "matchSessionDataGridView");
            this.matchSessionDataGridView.AutoGenerateColumns = false;
            this.matchSessionDataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.matchSessionDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.matchSessionDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.idDataGridViewTextBoxColumn,
            this.MatchsessionTagColumn,
            this.gameIdDataGridViewTextBoxColumn,
            this.startdateDataGridViewTextBoxColumn,
            this.durationDataGridViewTextBoxColumn});
            this.matchSessionDataGridView.DataSource = this.matchSessionBindingSource;
            this.matchSessionDataGridView.Name = "matchSessionDataGridView";
            this.matchSessionDataGridView.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.matchSessionDataGridView_CellClick);
            this.matchSessionDataGridView.DoubleClick += new System.EventHandler(this.matchSessionDataGridView_DoubleClick);
            // 
            // matchSessionBindingSource
            // 
            this.matchSessionBindingSource.DataSource = typeof(Yaaf.GameMediaManager.WinFormGui.Database.MatchSession);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
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
            // idDataGridViewTextBoxColumn
            // 
            this.idDataGridViewTextBoxColumn.DataPropertyName = "Name";
            this.idDataGridViewTextBoxColumn.FillWeight = 75F;
            resources.ApplyResources(this.idDataGridViewTextBoxColumn, "idDataGridViewTextBoxColumn");
            this.idDataGridViewTextBoxColumn.Name = "idDataGridViewTextBoxColumn";
            // 
            // MatchsessionTagColumn
            // 
            this.MatchsessionTagColumn.DataPropertyName = "Tags";
            this.MatchsessionTagColumn.FillWeight = 75F;
            resources.ApplyResources(this.MatchsessionTagColumn, "MatchsessionTagColumn");
            this.MatchsessionTagColumn.Name = "MatchsessionTagColumn";
            // 
            // gameIdDataGridViewTextBoxColumn
            // 
            this.gameIdDataGridViewTextBoxColumn.DataPropertyName = "Game";
            resources.ApplyResources(this.gameIdDataGridViewTextBoxColumn, "gameIdDataGridViewTextBoxColumn");
            this.gameIdDataGridViewTextBoxColumn.Name = "gameIdDataGridViewTextBoxColumn";
            // 
            // startdateDataGridViewTextBoxColumn
            // 
            this.startdateDataGridViewTextBoxColumn.DataPropertyName = "Startdate";
            this.startdateDataGridViewTextBoxColumn.FillWeight = 50F;
            resources.ApplyResources(this.startdateDataGridViewTextBoxColumn, "startdateDataGridViewTextBoxColumn");
            this.startdateDataGridViewTextBoxColumn.Name = "startdateDataGridViewTextBoxColumn";
            // 
            // durationDataGridViewTextBoxColumn
            // 
            this.durationDataGridViewTextBoxColumn.DataPropertyName = "Duration";
            this.durationDataGridViewTextBoxColumn.FillWeight = 50F;
            resources.ApplyResources(this.durationDataGridViewTextBoxColumn, "durationDataGridViewTextBoxColumn");
            this.durationDataGridViewTextBoxColumn.Name = "durationDataGridViewTextBoxColumn";
            // 
            // ViewMatchSessions
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.saveButton);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.matchSessionDataGridView);
            this.Name = "ViewMatchSessions";
            this.Load += new System.EventHandler(this.ViewMatchSessions_Load);
            ((System.ComponentModel.ISupportInitialize)(this.matchSessionDataGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.matchSessionBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView matchSessionDataGridView;
        private System.Windows.Forms.BindingSource matchSessionBindingSource;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.DataGridViewTextBoxColumn idDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn MatchsessionTagColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn gameIdDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn startdateDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn durationDataGridViewTextBoxColumn;
    }
}