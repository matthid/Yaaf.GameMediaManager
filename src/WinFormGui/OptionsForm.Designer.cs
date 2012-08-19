namespace Yaaf.WirePlugin.WinFormGui
{
    partial class OptionsForm
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.WarStyleTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.WarStyleExampleLabel = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.PublicStyleTextBox = new System.Windows.Forms.TextBox();
            this.PublicStyleExampleLabel = new System.Windows.Forms.Label();
            this.CustomPublicFolderTextBox = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.CustomWarFolderTextBox = new System.Windows.Forms.TextBox();
            this.WarSaveInWireCheckBox = new System.Windows.Forms.CheckBox();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.CustomWarFolderBrowse = new System.Windows.Forms.Button();
            this.CustomPublicFolderBrowse = new System.Windows.Forms.Button();
            this.checkBox3 = new System.Windows.Forms.CheckBox();
            this.PublicSaveInWireCheckBox = new System.Windows.Forms.CheckBox();
            this.PublicFolderFormatTextBox = new System.Windows.Forms.TextBox();
            this.SaveButton = new System.Windows.Forms.Button();
            this.AbortButton = new System.Windows.Forms.Button();
            this.ResetButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 200);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(197, 117);
            this.label1.TabIndex = 0;
            this.label1.Text = "War Variables:\r\n{0} - Datum\r\n{1} - Map\r\n{2} - Game\r\n{3} - increasing number (fore" +
    "ach filetype)\r\n{4} - global increasing number\r\n{5} - MatchId\r\n{6} - Enemy\r\n\r\n";
            // 
            // WarStyleTextBox
            // 
            this.WarStyleTextBox.Location = new System.Drawing.Point(149, 138);
            this.WarStyleTextBox.Name = "WarStyleTextBox";
            this.WarStyleTextBox.Size = new System.Drawing.Size(209, 20);
            this.WarStyleTextBox.TabIndex = 1;
            this.WarStyleTextBox.Text = "{0:HH_mm_ss} on {1}";
            this.WarStyleTextBox.TextChanged += new System.EventHandler(this.WarStyleTextBox_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 141);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "War Format:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(233, 200);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(197, 78);
            this.label3.TabIndex = 3;
            this.label3.Text = "Public Variables:\r\n{0} - Datum\r\n{1} - Map\r\n{2} - Game\r\n{3} - increasing number (f" +
    "oreach filetype)\r\n{4} - global increasing number\r\n";
            // 
            // WarStyleExampleLabel
            // 
            this.WarStyleExampleLabel.AutoSize = true;
            this.WarStyleExampleLabel.Location = new System.Drawing.Point(364, 141);
            this.WarStyleExampleLabel.Name = "WarStyleExampleLabel";
            this.WarStyleExampleLabel.Size = new System.Drawing.Size(53, 13);
            this.WarStyleExampleLabel.TabIndex = 4;
            this.WarStyleExampleLabel.Text = "Example: ";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 171);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(74, 13);
            this.label4.TabIndex = 5;
            this.label4.Text = "Public Format:";
            // 
            // PublicStyleTextBox
            // 
            this.PublicStyleTextBox.Location = new System.Drawing.Point(149, 168);
            this.PublicStyleTextBox.Name = "PublicStyleTextBox";
            this.PublicStyleTextBox.Size = new System.Drawing.Size(209, 20);
            this.PublicStyleTextBox.TabIndex = 6;
            this.PublicStyleTextBox.Text = "{0:HH_mm_ss} on {1}";
            this.PublicStyleTextBox.TextChanged += new System.EventHandler(this.PublicStyleTextBox_TextChanged);
            // 
            // PublicStyleExampleLabel
            // 
            this.PublicStyleExampleLabel.AutoSize = true;
            this.PublicStyleExampleLabel.Location = new System.Drawing.Point(364, 171);
            this.PublicStyleExampleLabel.Name = "PublicStyleExampleLabel";
            this.PublicStyleExampleLabel.Size = new System.Drawing.Size(53, 13);
            this.PublicStyleExampleLabel.TabIndex = 7;
            this.PublicStyleExampleLabel.Text = "Example: ";
            // 
            // CustomPublicFolderTextBox
            // 
            this.CustomPublicFolderTextBox.Location = new System.Drawing.Point(221, 91);
            this.CustomPublicFolderTextBox.Name = "CustomPublicFolderTextBox";
            this.CustomPublicFolderTextBox.Size = new System.Drawing.Size(209, 20);
            this.CustomPublicFolderTextBox.TabIndex = 11;
            this.CustomPublicFolderTextBox.Text = "PublicDemo\\{0} on {1}";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 69);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(71, 13);
            this.label5.TabIndex = 10;
            this.label5.Text = "Public Folder:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 9);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(62, 13);
            this.label6.TabIndex = 9;
            this.label6.Text = "War Folder:";
            // 
            // CustomWarFolderTextBox
            // 
            this.CustomWarFolderTextBox.Enabled = false;
            this.CustomWarFolderTextBox.Location = new System.Drawing.Point(221, 31);
            this.CustomWarFolderTextBox.Name = "CustomWarFolderTextBox";
            this.CustomWarFolderTextBox.Size = new System.Drawing.Size(209, 20);
            this.CustomWarFolderTextBox.TabIndex = 8;
            // 
            // WarSaveInWireCheckBox
            // 
            this.WarSaveInWireCheckBox.AutoSize = true;
            this.WarSaveInWireCheckBox.Checked = true;
            this.WarSaveInWireCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.WarSaveInWireCheckBox.Location = new System.Drawing.Point(100, 8);
            this.WarSaveInWireCheckBox.Name = "WarSaveInWireCheckBox";
            this.WarSaveInWireCheckBox.Size = new System.Drawing.Size(315, 17);
            this.WarSaveInWireCheckBox.TabIndex = 12;
            this.WarSaveInWireCheckBox.Text = "Save in Matchmedia Folder \"ESL Match Media\\{5}_(vs. {6})\"";
            this.WarSaveInWireCheckBox.UseVisualStyleBackColor = true;
            // 
            // checkBox2
            // 
            this.checkBox2.AutoSize = true;
            this.checkBox2.Enabled = false;
            this.checkBox2.Location = new System.Drawing.Point(100, 33);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(115, 17);
            this.checkBox2.TabIndex = 13;
            this.checkBox2.Text = "Use Custom Folder";
            this.checkBox2.UseVisualStyleBackColor = true;
            this.checkBox2.CheckedChanged += new System.EventHandler(this.checkBox2_CheckedChanged);
            // 
            // CustomWarFolderBrowse
            // 
            this.CustomWarFolderBrowse.Enabled = false;
            this.CustomWarFolderBrowse.Location = new System.Drawing.Point(457, 29);
            this.CustomWarFolderBrowse.Name = "CustomWarFolderBrowse";
            this.CustomWarFolderBrowse.Size = new System.Drawing.Size(75, 23);
            this.CustomWarFolderBrowse.TabIndex = 14;
            this.CustomWarFolderBrowse.Text = "Browse";
            this.CustomWarFolderBrowse.UseVisualStyleBackColor = true;
            this.CustomWarFolderBrowse.Click += new System.EventHandler(this.CustomFolderBrowse_Click);
            // 
            // CustomPublicFolderBrowse
            // 
            this.CustomPublicFolderBrowse.Enabled = false;
            this.CustomPublicFolderBrowse.Location = new System.Drawing.Point(457, 89);
            this.CustomPublicFolderBrowse.Name = "CustomPublicFolderBrowse";
            this.CustomPublicFolderBrowse.Size = new System.Drawing.Size(75, 23);
            this.CustomPublicFolderBrowse.TabIndex = 18;
            this.CustomPublicFolderBrowse.Text = "Browse";
            this.CustomPublicFolderBrowse.UseVisualStyleBackColor = true;
            this.CustomPublicFolderBrowse.Click += new System.EventHandler(this.CustomPublicFolderBrowse_Click);
            // 
            // checkBox3
            // 
            this.checkBox3.AutoSize = true;
            this.checkBox3.Enabled = false;
            this.checkBox3.Location = new System.Drawing.Point(100, 93);
            this.checkBox3.Name = "checkBox3";
            this.checkBox3.Size = new System.Drawing.Size(115, 17);
            this.checkBox3.TabIndex = 17;
            this.checkBox3.Text = "Use Custom Folder";
            this.checkBox3.UseVisualStyleBackColor = true;
            this.checkBox3.CheckedChanged += new System.EventHandler(this.checkBox3_CheckedChanged);
            // 
            // PublicSaveInWireCheckBox
            // 
            this.PublicSaveInWireCheckBox.AutoSize = true;
            this.PublicSaveInWireCheckBox.Checked = true;
            this.PublicSaveInWireCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.PublicSaveInWireCheckBox.Location = new System.Drawing.Point(100, 68);
            this.PublicSaveInWireCheckBox.Name = "PublicSaveInWireCheckBox";
            this.PublicSaveInWireCheckBox.Size = new System.Drawing.Size(155, 17);
            this.PublicSaveInWireCheckBox.TabIndex = 16;
            this.PublicSaveInWireCheckBox.Text = "Save in Matchmedia Folder";
            this.PublicSaveInWireCheckBox.UseVisualStyleBackColor = true;
            // 
            // PublicFolderFormatTextBox
            // 
            this.PublicFolderFormatTextBox.Location = new System.Drawing.Point(281, 66);
            this.PublicFolderFormatTextBox.Name = "PublicFolderFormatTextBox";
            this.PublicFolderFormatTextBox.Size = new System.Drawing.Size(149, 20);
            this.PublicFolderFormatTextBox.TabIndex = 19;
            this.PublicFolderFormatTextBox.Text = "PublicStuff\\{0:yyyy-MM-dd}";
            // 
            // SaveButton
            // 
            this.SaveButton.Location = new System.Drawing.Point(425, 270);
            this.SaveButton.Name = "SaveButton";
            this.SaveButton.Size = new System.Drawing.Size(107, 47);
            this.SaveButton.TabIndex = 20;
            this.SaveButton.Text = "Save";
            this.SaveButton.UseVisualStyleBackColor = true;
            this.SaveButton.Click += new System.EventHandler(this.SaveButton_Click);
            // 
            // AbortButton
            // 
            this.AbortButton.Location = new System.Drawing.Point(319, 289);
            this.AbortButton.Name = "AbortButton";
            this.AbortButton.Size = new System.Drawing.Size(85, 28);
            this.AbortButton.TabIndex = 21;
            this.AbortButton.Text = "Abort";
            this.AbortButton.UseVisualStyleBackColor = true;
            this.AbortButton.Click += new System.EventHandler(this.AbortButton_Click);
            // 
            // ResetButton
            // 
            this.ResetButton.Location = new System.Drawing.Point(215, 289);
            this.ResetButton.Name = "ResetButton";
            this.ResetButton.Size = new System.Drawing.Size(84, 28);
            this.ResetButton.TabIndex = 22;
            this.ResetButton.Text = "Reset";
            this.ResetButton.UseVisualStyleBackColor = true;
            this.ResetButton.Click += new System.EventHandler(this.ResetButton_Click);
            // 
            // OptionsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(553, 335);
            this.Controls.Add(this.ResetButton);
            this.Controls.Add(this.AbortButton);
            this.Controls.Add(this.SaveButton);
            this.Controls.Add(this.PublicFolderFormatTextBox);
            this.Controls.Add(this.CustomPublicFolderBrowse);
            this.Controls.Add(this.checkBox3);
            this.Controls.Add(this.PublicSaveInWireCheckBox);
            this.Controls.Add(this.CustomWarFolderBrowse);
            this.Controls.Add(this.checkBox2);
            this.Controls.Add(this.WarSaveInWireCheckBox);
            this.Controls.Add(this.CustomPublicFolderTextBox);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.CustomWarFolderTextBox);
            this.Controls.Add(this.PublicStyleExampleLabel);
            this.Controls.Add(this.PublicStyleTextBox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.WarStyleExampleLabel);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.WarStyleTextBox);
            this.Controls.Add(this.label1);
            this.Name = "OptionsForm";
            this.Text = "Options";
            this.Load += new System.EventHandler(this.OptionsForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox WarStyleTextBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label WarStyleExampleLabel;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox PublicStyleTextBox;
        private System.Windows.Forms.Label PublicStyleExampleLabel;
        private System.Windows.Forms.TextBox CustomPublicFolderTextBox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox CustomWarFolderTextBox;
        private System.Windows.Forms.CheckBox WarSaveInWireCheckBox;
        private System.Windows.Forms.CheckBox checkBox2;
        private System.Windows.Forms.Button CustomWarFolderBrowse;
        private System.Windows.Forms.Button CustomPublicFolderBrowse;
        private System.Windows.Forms.CheckBox checkBox3;
        private System.Windows.Forms.CheckBox PublicSaveInWireCheckBox;
        private System.Windows.Forms.TextBox PublicFolderFormatTextBox;
        private System.Windows.Forms.Button SaveButton;
        private System.Windows.Forms.Button AbortButton;
        private System.Windows.Forms.Button ResetButton;
    }
}

