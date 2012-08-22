namespace Yaaf.WirePlugin.WinFormGui
{
    partial class ManageWatchFolder
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ManageWatchFolder));
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.folderDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.filterDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.notifyOnInativityDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.watchFolderBindingSource = new System.Windows.Forms.BindingSource(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.watchFolderBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.AutoGenerateColumns = false;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.folderDataGridViewTextBoxColumn,
            this.filterDataGridViewTextBoxColumn,
            this.notifyOnInativityDataGridViewTextBoxColumn});
            this.dataGridView1.DataSource = this.watchFolderBindingSource;
            resources.ApplyResources(this.dataGridView1, "dataGridView1");
            this.dataGridView1.Name = "dataGridView1";
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
            // folderDataGridViewTextBoxColumn
            // 
            this.folderDataGridViewTextBoxColumn.DataPropertyName = "Folder";
            this.folderDataGridViewTextBoxColumn.FillWeight = 500F;
            resources.ApplyResources(this.folderDataGridViewTextBoxColumn, "folderDataGridViewTextBoxColumn");
            this.folderDataGridViewTextBoxColumn.Name = "folderDataGridViewTextBoxColumn";
            // 
            // filterDataGridViewTextBoxColumn
            // 
            this.filterDataGridViewTextBoxColumn.DataPropertyName = "Filter";
            resources.ApplyResources(this.filterDataGridViewTextBoxColumn, "filterDataGridViewTextBoxColumn");
            this.filterDataGridViewTextBoxColumn.Name = "filterDataGridViewTextBoxColumn";
            // 
            // notifyOnInativityDataGridViewTextBoxColumn
            // 
            this.notifyOnInativityDataGridViewTextBoxColumn.DataPropertyName = "NotifyOnInativity";
            resources.ApplyResources(this.notifyOnInativityDataGridViewTextBoxColumn, "notifyOnInativityDataGridViewTextBoxColumn");
            this.notifyOnInativityDataGridViewTextBoxColumn.Name = "notifyOnInativityDataGridViewTextBoxColumn";
            // 
            // watchFolderBindingSource
            // 
            this.watchFolderBindingSource.DataSource = typeof(Yaaf.WirePlugin.WinFormGui.Database.WatchFolder);
            // 
            // ManageWatchFolder
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.dataGridView1);
            this.Name = "ManageWatchFolder";
            this.Load += new System.EventHandler(this.ManageWatchFolder_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.watchFolderBindingSource)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.BindingSource watchFolderBindingSource;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.DataGridViewTextBoxColumn folderDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn filterDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn notifyOnInativityDataGridViewTextBoxColumn;
    }
}