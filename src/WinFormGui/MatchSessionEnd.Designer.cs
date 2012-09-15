namespace Yaaf.WirePlugin.WinFormGui
{
    partial class MatchSessionEnd
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MatchSessionEnd));
            this.tagTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.saveMatchmediaButton = new System.Windows.Forms.Button();
            this.deleteMatchmediaButton = new System.Windows.Forms.Button();
            this.switchToAdvancedViewButton = new System.Windows.Forms.Button();
            this.rememberCheckBox = new System.Windows.Forms.CheckBox();
            this.matchmediaDataGridView = new System.Windows.Forms.DataGridView();
            this.matchmediaBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.fetchMatchDatabutton = new System.Windows.Forms.Button();
            this.linkLabel = new System.Windows.Forms.Label();
            this.nameDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Tags = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.typeDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.mapDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.createdDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label3 = new System.Windows.Forms.Label();
            this.matchNameTextBox = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.matchmediaDataGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.matchmediaBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // tagTextBox
            // 
            resources.ApplyResources(this.tagTextBox, "tagTextBox");
            this.tagTextBox.Name = "tagTextBox";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // saveMatchmediaButton
            // 
            resources.ApplyResources(this.saveMatchmediaButton, "saveMatchmediaButton");
            this.saveMatchmediaButton.Name = "saveMatchmediaButton";
            this.saveMatchmediaButton.UseVisualStyleBackColor = true;
            this.saveMatchmediaButton.Click += new System.EventHandler(this.saveMatchmediaButton_Click);
            // 
            // deleteMatchmediaButton
            // 
            this.deleteMatchmediaButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.deleteMatchmediaButton, "deleteMatchmediaButton");
            this.deleteMatchmediaButton.Name = "deleteMatchmediaButton";
            this.deleteMatchmediaButton.UseVisualStyleBackColor = true;
            this.deleteMatchmediaButton.Click += new System.EventHandler(this.deleteMatchmediaButton_Click);
            // 
            // switchToAdvancedViewButton
            // 
            resources.ApplyResources(this.switchToAdvancedViewButton, "switchToAdvancedViewButton");
            this.switchToAdvancedViewButton.Name = "switchToAdvancedViewButton";
            this.switchToAdvancedViewButton.Click += new System.EventHandler(this.switchToAdvancedViewButton_Click);
            // 
            // rememberCheckBox
            // 
            resources.ApplyResources(this.rememberCheckBox, "rememberCheckBox");
            this.rememberCheckBox.Name = "rememberCheckBox";
            this.rememberCheckBox.UseVisualStyleBackColor = true;
            // 
            // matchmediaDataGridView
            // 
            this.matchmediaDataGridView.AllowDrop = true;
            this.matchmediaDataGridView.AllowUserToAddRows = false;
            this.matchmediaDataGridView.AutoGenerateColumns = false;
            this.matchmediaDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.matchmediaDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.nameDataGridViewTextBoxColumn,
            this.Tags,
            this.typeDataGridViewTextBoxColumn,
            this.mapDataGridViewTextBoxColumn,
            this.createdDataGridViewTextBoxColumn});
            this.matchmediaDataGridView.DataSource = this.matchmediaBindingSource;
            resources.ApplyResources(this.matchmediaDataGridView, "matchmediaDataGridView");
            this.matchmediaDataGridView.Name = "matchmediaDataGridView";
            this.matchmediaDataGridView.DragDrop += new System.Windows.Forms.DragEventHandler(this.matchmediaDataGridView_DragDrop);
            this.matchmediaDataGridView.DragEnter += new System.Windows.Forms.DragEventHandler(this.matchmediaDataGridView_DragEnter);
            // 
            // matchmediaBindingSource
            // 
            this.matchmediaBindingSource.DataSource = typeof(Yaaf.WirePlugin.WinFormGui.Database.Matchmedia);
            // 
            // fetchMatchDatabutton
            // 
            resources.ApplyResources(this.fetchMatchDatabutton, "fetchMatchDatabutton");
            this.fetchMatchDatabutton.Name = "fetchMatchDatabutton";
            this.fetchMatchDatabutton.UseVisualStyleBackColor = true;
            this.fetchMatchDatabutton.Click += new System.EventHandler(this.fetchMatchDatabutton_Click);
            // 
            // linkLabel
            // 
            resources.ApplyResources(this.linkLabel, "linkLabel");
            this.linkLabel.Name = "linkLabel";
            // 
            // nameDataGridViewTextBoxColumn
            // 
            this.nameDataGridViewTextBoxColumn.DataPropertyName = "Name";
            this.nameDataGridViewTextBoxColumn.FillWeight = 150F;
            resources.ApplyResources(this.nameDataGridViewTextBoxColumn, "nameDataGridViewTextBoxColumn");
            this.nameDataGridViewTextBoxColumn.Name = "nameDataGridViewTextBoxColumn";
            // 
            // Tags
            // 
            this.Tags.DataPropertyName = "MyTags";
            resources.ApplyResources(this.Tags, "Tags");
            this.Tags.Name = "Tags";
            // 
            // typeDataGridViewTextBoxColumn
            // 
            this.typeDataGridViewTextBoxColumn.DataPropertyName = "Type";
            this.typeDataGridViewTextBoxColumn.FillWeight = 50F;
            resources.ApplyResources(this.typeDataGridViewTextBoxColumn, "typeDataGridViewTextBoxColumn");
            this.typeDataGridViewTextBoxColumn.Name = "typeDataGridViewTextBoxColumn";
            // 
            // mapDataGridViewTextBoxColumn
            // 
            this.mapDataGridViewTextBoxColumn.DataPropertyName = "Map";
            this.mapDataGridViewTextBoxColumn.FillWeight = 70F;
            resources.ApplyResources(this.mapDataGridViewTextBoxColumn, "mapDataGridViewTextBoxColumn");
            this.mapDataGridViewTextBoxColumn.Name = "mapDataGridViewTextBoxColumn";
            // 
            // createdDataGridViewTextBoxColumn
            // 
            this.createdDataGridViewTextBoxColumn.DataPropertyName = "Created";
            resources.ApplyResources(this.createdDataGridViewTextBoxColumn, "createdDataGridViewTextBoxColumn");
            this.createdDataGridViewTextBoxColumn.Name = "createdDataGridViewTextBoxColumn";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // matchNameTextBox
            // 
            resources.ApplyResources(this.matchNameTextBox, "matchNameTextBox");
            this.matchNameTextBox.Name = "matchNameTextBox";
            // 
            // MatchSessionEnd
            // 
            this.AcceptButton = this.saveMatchmediaButton;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.deleteMatchmediaButton;
            this.Controls.Add(this.label3);
            this.Controls.Add(this.matchNameTextBox);
            this.Controls.Add(this.linkLabel);
            this.Controls.Add(this.fetchMatchDatabutton);
            this.Controls.Add(this.matchmediaDataGridView);
            this.Controls.Add(this.rememberCheckBox);
            this.Controls.Add(this.switchToAdvancedViewButton);
            this.Controls.Add(this.deleteMatchmediaButton);
            this.Controls.Add(this.saveMatchmediaButton);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tagTextBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "MatchSessionEnd";
            this.Load += new System.EventHandler(this.MatchSessionEnd_Load);
            ((System.ComponentModel.ISupportInitialize)(this.matchmediaDataGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.matchmediaBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tagTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button saveMatchmediaButton;
        private System.Windows.Forms.Button deleteMatchmediaButton;
        private System.Windows.Forms.Button switchToAdvancedViewButton;
        private System.Windows.Forms.CheckBox rememberCheckBox;
        private System.Windows.Forms.DataGridView matchmediaDataGridView;
        private System.Windows.Forms.BindingSource matchmediaBindingSource;
        private System.Windows.Forms.Button fetchMatchDatabutton;
        private System.Windows.Forms.Label linkLabel;
        private System.Windows.Forms.DataGridViewTextBoxColumn nameDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn Tags;
        private System.Windows.Forms.DataGridViewTextBoxColumn typeDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn mapDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn createdDataGridViewTextBoxColumn;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox matchNameTextBox;
    }
}