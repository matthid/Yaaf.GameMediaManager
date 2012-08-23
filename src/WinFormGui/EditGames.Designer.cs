namespace Yaaf.WirePlugin.WinFormGui
{
    partial class EditGames
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EditGames));
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.idDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.nameDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.shortnameDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.enableNotificationDataGridViewCheckBoxColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.enableMatchFormDataGridViewCheckBoxColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.enablePublicNotificationDataGridViewCheckBoxColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.enableWarMatchFormDataGridViewCheckBoxColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.warMatchFormSaveFilesDataGridViewCheckBoxColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.publicMatchFormSaveFilesDataGridViewCheckBoxColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.gameBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gameBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.AutoGenerateColumns = false;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.idDataGridViewTextBoxColumn,
            this.nameDataGridViewTextBoxColumn,
            this.shortnameDataGridViewTextBoxColumn,
            this.enableNotificationDataGridViewCheckBoxColumn,
            this.enableMatchFormDataGridViewCheckBoxColumn,
            this.enablePublicNotificationDataGridViewCheckBoxColumn,
            this.enableWarMatchFormDataGridViewCheckBoxColumn,
            this.warMatchFormSaveFilesDataGridViewCheckBoxColumn,
            this.publicMatchFormSaveFilesDataGridViewCheckBoxColumn});
            this.dataGridView1.DataSource = this.gameBindingSource;
            resources.ApplyResources(this.dataGridView1, "dataGridView1");
            this.dataGridView1.Name = "dataGridView1";
            // 
            // idDataGridViewTextBoxColumn
            // 
            this.idDataGridViewTextBoxColumn.DataPropertyName = "Id";
            this.idDataGridViewTextBoxColumn.FillWeight = 50F;
            resources.ApplyResources(this.idDataGridViewTextBoxColumn, "idDataGridViewTextBoxColumn");
            this.idDataGridViewTextBoxColumn.Name = "idDataGridViewTextBoxColumn";
            // 
            // nameDataGridViewTextBoxColumn
            // 
            this.nameDataGridViewTextBoxColumn.DataPropertyName = "Name";
            resources.ApplyResources(this.nameDataGridViewTextBoxColumn, "nameDataGridViewTextBoxColumn");
            this.nameDataGridViewTextBoxColumn.Name = "nameDataGridViewTextBoxColumn";
            // 
            // shortnameDataGridViewTextBoxColumn
            // 
            this.shortnameDataGridViewTextBoxColumn.DataPropertyName = "Shortname";
            resources.ApplyResources(this.shortnameDataGridViewTextBoxColumn, "shortnameDataGridViewTextBoxColumn");
            this.shortnameDataGridViewTextBoxColumn.Name = "shortnameDataGridViewTextBoxColumn";
            // 
            // enableNotificationDataGridViewCheckBoxColumn
            // 
            this.enableNotificationDataGridViewCheckBoxColumn.DataPropertyName = "EnableNotification";
            resources.ApplyResources(this.enableNotificationDataGridViewCheckBoxColumn, "enableNotificationDataGridViewCheckBoxColumn");
            this.enableNotificationDataGridViewCheckBoxColumn.Name = "enableNotificationDataGridViewCheckBoxColumn";
            // 
            // enableMatchFormDataGridViewCheckBoxColumn
            // 
            this.enableMatchFormDataGridViewCheckBoxColumn.DataPropertyName = "EnableMatchForm";
            resources.ApplyResources(this.enableMatchFormDataGridViewCheckBoxColumn, "enableMatchFormDataGridViewCheckBoxColumn");
            this.enableMatchFormDataGridViewCheckBoxColumn.Name = "enableMatchFormDataGridViewCheckBoxColumn";
            // 
            // enablePublicNotificationDataGridViewCheckBoxColumn
            // 
            this.enablePublicNotificationDataGridViewCheckBoxColumn.DataPropertyName = "EnablePublicNotification";
            resources.ApplyResources(this.enablePublicNotificationDataGridViewCheckBoxColumn, "enablePublicNotificationDataGridViewCheckBoxColumn");
            this.enablePublicNotificationDataGridViewCheckBoxColumn.Name = "enablePublicNotificationDataGridViewCheckBoxColumn";
            // 
            // enableWarMatchFormDataGridViewCheckBoxColumn
            // 
            this.enableWarMatchFormDataGridViewCheckBoxColumn.DataPropertyName = "EnableWarMatchForm";
            resources.ApplyResources(this.enableWarMatchFormDataGridViewCheckBoxColumn, "enableWarMatchFormDataGridViewCheckBoxColumn");
            this.enableWarMatchFormDataGridViewCheckBoxColumn.Name = "enableWarMatchFormDataGridViewCheckBoxColumn";
            // 
            // warMatchFormSaveFilesDataGridViewCheckBoxColumn
            // 
            this.warMatchFormSaveFilesDataGridViewCheckBoxColumn.DataPropertyName = "WarMatchFormSaveFiles";
            resources.ApplyResources(this.warMatchFormSaveFilesDataGridViewCheckBoxColumn, "warMatchFormSaveFilesDataGridViewCheckBoxColumn");
            this.warMatchFormSaveFilesDataGridViewCheckBoxColumn.Name = "warMatchFormSaveFilesDataGridViewCheckBoxColumn";
            // 
            // publicMatchFormSaveFilesDataGridViewCheckBoxColumn
            // 
            this.publicMatchFormSaveFilesDataGridViewCheckBoxColumn.DataPropertyName = "PublicMatchFormSaveFiles";
            resources.ApplyResources(this.publicMatchFormSaveFilesDataGridViewCheckBoxColumn, "publicMatchFormSaveFilesDataGridViewCheckBoxColumn");
            this.publicMatchFormSaveFilesDataGridViewCheckBoxColumn.Name = "publicMatchFormSaveFilesDataGridViewCheckBoxColumn";
            // 
            // gameBindingSource
            // 
            this.gameBindingSource.DataSource = typeof(Yaaf.WirePlugin.WinFormGui.Database.Game);
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
            // button3
            // 
            resources.ApplyResources(this.button3, "button3");
            this.button3.Name = "button3";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button4
            // 
            resources.ApplyResources(this.button4, "button4");
            this.button4.Name = "button4";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // EditGames
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.dataGridView1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "EditGames";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.EditGames_FormClosing);
            this.Load += new System.EventHandler(this.EditGames_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gameBindingSource)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridViewTextBoxColumn idDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn nameDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn shortnameDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewCheckBoxColumn enableNotificationDataGridViewCheckBoxColumn;
        private System.Windows.Forms.DataGridViewCheckBoxColumn enableMatchFormDataGridViewCheckBoxColumn;
        private System.Windows.Forms.DataGridViewCheckBoxColumn enablePublicNotificationDataGridViewCheckBoxColumn;
        private System.Windows.Forms.DataGridViewCheckBoxColumn enableWarMatchFormDataGridViewCheckBoxColumn;
        private System.Windows.Forms.DataGridViewCheckBoxColumn warMatchFormSaveFilesDataGridViewCheckBoxColumn;
        private System.Windows.Forms.DataGridViewCheckBoxColumn publicMatchFormSaveFilesDataGridViewCheckBoxColumn;
        private System.Windows.Forms.BindingSource gameBindingSource;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
    }
}