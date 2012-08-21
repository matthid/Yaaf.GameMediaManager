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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MatchSessionEnd));
            this.eslMatchCheckBox = new System.Windows.Forms.CheckBox();
            this.EslMatchIdTextBox = new System.Windows.Forms.TextBox();
            this.tagTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.matchmediaListView = new System.Windows.Forms.ListView();
            this.label2 = new System.Windows.Forms.Label();
            this.saveMatchmediaButton = new System.Windows.Forms.Button();
            this.deleteMatchmediaButton = new System.Windows.Forms.Button();
            this.Name = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Tags = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Type = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Map = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.addMatchmediaButton = new System.Windows.Forms.Button();
            this.managePlayersButton = new System.Windows.Forms.Button();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // eslMatchCheckBox
            // 
            resources.ApplyResources(this.eslMatchCheckBox, "eslMatchCheckBox");
            this.eslMatchCheckBox.Name = "eslMatchCheckBox";
            this.eslMatchCheckBox.UseVisualStyleBackColor = true;
            // 
            // EslMatchIdTextBox
            // 
            resources.ApplyResources(this.EslMatchIdTextBox, "EslMatchIdTextBox");
            this.EslMatchIdTextBox.Name = "EslMatchIdTextBox";
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
            // matchmediaListView
            // 
            this.matchmediaListView.CheckBoxes = true;
            this.matchmediaListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.Name,
            this.Tags,
            this.Type,
            this.Map});
            this.matchmediaListView.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            ((System.Windows.Forms.ListViewItem)(resources.GetObject("matchmediaListView.Items"))),
            ((System.Windows.Forms.ListViewItem)(resources.GetObject("matchmediaListView.Items1")))});
            this.matchmediaListView.LabelEdit = true;
            resources.ApplyResources(this.matchmediaListView, "matchmediaListView");
            this.matchmediaListView.Name = "matchmediaListView";
            this.matchmediaListView.UseCompatibleStateImageBehavior = false;
            this.matchmediaListView.View = System.Windows.Forms.View.Details;
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
            // 
            // deleteMatchmediaButton
            // 
            this.deleteMatchmediaButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.deleteMatchmediaButton, "deleteMatchmediaButton");
            this.deleteMatchmediaButton.Name = "deleteMatchmediaButton";
            this.deleteMatchmediaButton.UseVisualStyleBackColor = true;
            // 
            // Name
            // 
            resources.ApplyResources(this.Name, "Name");
            // 
            // Tags
            // 
            resources.ApplyResources(this.Tags, "Tags");
            // 
            // Type
            // 
            resources.ApplyResources(this.Type, "Type");
            // 
            // Map
            // 
            resources.ApplyResources(this.Map, "Map");
            // 
            // addMatchmediaButton
            // 
            resources.ApplyResources(this.addMatchmediaButton, "addMatchmediaButton");
            this.addMatchmediaButton.Name = "addMatchmediaButton";
            this.addMatchmediaButton.UseVisualStyleBackColor = true;
            // 
            // managePlayersButton
            // 
            resources.ApplyResources(this.managePlayersButton, "managePlayersButton");
            this.managePlayersButton.Name = "managePlayersButton";
            // 
            // checkBox1
            // 
            resources.ApplyResources(this.checkBox1, "checkBox1");
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // MatchSessionEnd
            // 
            this.AcceptButton = this.saveMatchmediaButton;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.deleteMatchmediaButton;
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.managePlayersButton);
            this.Controls.Add(this.addMatchmediaButton);
            this.Controls.Add(this.deleteMatchmediaButton);
            this.Controls.Add(this.saveMatchmediaButton);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.matchmediaListView);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tagTextBox);
            this.Controls.Add(this.EslMatchIdTextBox);
            this.Controls.Add(this.eslMatchCheckBox);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox eslMatchCheckBox;
        private System.Windows.Forms.TextBox EslMatchIdTextBox;
        private System.Windows.Forms.TextBox tagTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListView matchmediaListView;
        private System.Windows.Forms.ColumnHeader Name;
        private System.Windows.Forms.ColumnHeader Tags;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button saveMatchmediaButton;
        private System.Windows.Forms.Button deleteMatchmediaButton;
        private System.Windows.Forms.ColumnHeader Type;
        private System.Windows.Forms.ColumnHeader Map;
        private System.Windows.Forms.Button addMatchmediaButton;
        private System.Windows.Forms.Button managePlayersButton;
        private System.Windows.Forms.CheckBox checkBox1;
    }
}