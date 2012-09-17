namespace Yaaf.GameMediaManager.WinFormGui
{
    partial class ManagePlayers
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ManagePlayers));
            this.saveButton = new System.Windows.Forms.Button();
            this.playerDataGridView = new System.Windows.Forms.DataGridView();
            this.Id = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.eslPlayerIdDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.nameDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.playerBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.meLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.playerDataGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.playerBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // saveButton
            // 
            resources.ApplyResources(this.saveButton, "saveButton");
            this.saveButton.Name = "saveButton";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // playerDataGridView
            // 
            this.playerDataGridView.AllowUserToAddRows = false;
            this.playerDataGridView.AllowUserToDeleteRows = false;
            resources.ApplyResources(this.playerDataGridView, "playerDataGridView");
            this.playerDataGridView.AutoGenerateColumns = false;
            this.playerDataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.playerDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.playerDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Id,
            this.eslPlayerIdDataGridViewTextBoxColumn,
            this.nameDataGridViewTextBoxColumn});
            this.playerDataGridView.DataSource = this.playerBindingSource;
            this.playerDataGridView.Name = "playerDataGridView";
            this.playerDataGridView.ReadOnly = true;
            // 
            // Id
            // 
            this.Id.DataPropertyName = "Id";
            this.Id.FillWeight = 50F;
            resources.ApplyResources(this.Id, "Id");
            this.Id.Name = "Id";
            this.Id.ReadOnly = true;
            // 
            // eslPlayerIdDataGridViewTextBoxColumn
            // 
            this.eslPlayerIdDataGridViewTextBoxColumn.DataPropertyName = "EslPlayerId";
            this.eslPlayerIdDataGridViewTextBoxColumn.FillWeight = 70F;
            resources.ApplyResources(this.eslPlayerIdDataGridViewTextBoxColumn, "eslPlayerIdDataGridViewTextBoxColumn");
            this.eslPlayerIdDataGridViewTextBoxColumn.Name = "eslPlayerIdDataGridViewTextBoxColumn";
            this.eslPlayerIdDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // nameDataGridViewTextBoxColumn
            // 
            this.nameDataGridViewTextBoxColumn.DataPropertyName = "Name";
            this.nameDataGridViewTextBoxColumn.FillWeight = 150F;
            resources.ApplyResources(this.nameDataGridViewTextBoxColumn, "nameDataGridViewTextBoxColumn");
            this.nameDataGridViewTextBoxColumn.Name = "nameDataGridViewTextBoxColumn";
            this.nameDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // playerBindingSource
            // 
            this.playerBindingSource.DataSource = typeof(Yaaf.GameMediaManager.WinFormGui.Database.Player);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // meLabel
            // 
            resources.ApplyResources(this.meLabel, "meLabel");
            this.meLabel.Name = "meLabel";
            // 
            // ManagePlayers
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.meLabel);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.playerDataGridView);
            this.Controls.Add(this.saveButton);
            this.MaximizeBox = false;
            this.Name = "ManagePlayers";
            this.Load += new System.EventHandler(this.ManagePlayers_Load);
            ((System.ComponentModel.ISupportInitialize)(this.playerDataGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.playerBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.DataGridView playerDataGridView;
        private System.Windows.Forms.BindingSource playerBindingSource;
        private System.Windows.Forms.DataGridViewTextBoxColumn Id;
        private System.Windows.Forms.DataGridViewTextBoxColumn eslPlayerIdDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn nameDataGridViewTextBoxColumn;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label meLabel;
    }
}