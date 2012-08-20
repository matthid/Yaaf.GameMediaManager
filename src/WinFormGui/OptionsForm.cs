namespace Yaaf.WirePlugin.WinFormGui
{
    using System;
    using System.Windows.Forms;
    using Yaaf.WirePlugin.WinFormGui.Properties;

    public partial class OptionsForm : Form
    {
        private readonly Action<System.Diagnostics.TraceEventType, string> logger;


        public OptionsForm(Action<System.Diagnostics.TraceEventType, string> logger)
        {
            this.logger = logger;
            this.InitializeComponent();
        }


        public bool Saved { get; set; }

        private void OptionsForm_Load(object sender, EventArgs e)
        {
            this.LoadData();
        }


        private void LoadData()
        {
            this.WarSaveInWire = Settings.Default.WarSaveInWire;
            this.PublicSaveInWire = Settings.Default.PublicSaveInWire;
            this.PublicFolderFormat = Settings.Default.PublicFolderFormat;
            this.WarFileFormat = Settings.Default.WarFileFormat;
            this.PublicFileFormat = Settings.Default.PublicFileFormat;

            PublicStyleTextBox_TextChanged(null, null);
            WarStyleTextBox_TextChanged(null, null);
            checkBox3_CheckedChanged(null, null);
            checkBox2_CheckedChanged(null, null);
        }

        private void SaveData()
        {
            Settings.Default.WarSaveInWire = this.WarSaveInWire;
            Settings.Default.PublicSaveInWire = this.PublicSaveInWire;
            Settings.Default.PublicFolderFormat = this.PublicFolderFormat;
            Settings.Default.WarFileFormat = this.WarFileFormat;
            Settings.Default.PublicFileFormat = this.PublicFileFormat;
            Settings.Default.Save();
        }

        public void CheckValid()
        {
            try
            {
                string.Format(
                    this.WarFileFormat,
                    DateTime.Now - TimeSpan.FromMinutes(3.56),
                    DateTime.Now,
                    "de_dust2",
                    "css",
                    3,
                    6,
                    1234567,
                    "RoXX");
            }
            catch (Exception e)
            {
                throw new FormatException(Resources.OptionsForm_CheckValid_Invalid_War_File_Format_, e);
            }

            try
            {
                string.Format(
                    this.PublicFileFormat,
                    DateTime.Now - TimeSpan.FromMinutes(3.56),
                    DateTime.Now,
                    "de_dust2",
                    "css",
                    3,
                    6);
            }
            catch (Exception e)
            {
                throw new FormatException(Resources.OptionsForm_CheckValid_Invalid_Public_File_Format_, e);
            }
            
            try
            {
                string.Format(
                    this.PublicFolderFormat,
                    DateTime.Now - TimeSpan.FromMinutes(3.56),
                    DateTime.Now,
                    "de_dust2",
                    "css",
                    3,
                    6);
            }
            catch (Exception e)
            {
                throw new FormatException(Resources.OptionsForm_CheckValid_Invalid_Public_Folder_Format_, e);
            }
        }

        public bool WarSaveInWire
        {
            get
            {
                return WarSaveInWireCheckBox.Checked;
            }
            set
            {
                WarSaveInWireCheckBox.Checked = value;
            }
        }

        public bool PublicSaveInWire
        {
            get
            {
                return PublicSaveInWireCheckBox.Checked;
            }
            set
            {
                PublicSaveInWireCheckBox.Checked = value;
            }
        }

        public string PublicFolderFormat
        {
            get
            {
                return PublicFolderFormatTextBox.Text;
            }
            set
            {
                PublicFolderFormatTextBox.Text = value;
            }
        }

        public string WarFileFormat
        {
            get
            {
                return WarStyleTextBox.Text;
            }
            set
            {
                WarStyleTextBox.Text = value;
            }
        }
        public string PublicFileFormat
        {
            get
            {
                return PublicStyleTextBox.Text;
            }
            set
            {
                PublicStyleTextBox.Text = value;
            }
        }


        private void WarStyleTextBox_TextChanged(object sender, EventArgs e)
        {
            try
            {
                this.WarStyleExampleLabel.Text = string.Format(
                    Resources.Example,
                    string.Format(this.WarStyleTextBox.Text, DateTime.Now - TimeSpan.FromMinutes(3.56), DateTime.Now, "de_dust2", "css", 3, 6, 1234567, "RoXX") + ".dem");
            }
            catch (Exception ex)
            {
                this.WarStyleExampleLabel.Text = 
                    string.Format(Resources.InvalidValue, ex.Message);
            }
        }


        private void PublicStyleTextBox_TextChanged(object sender, EventArgs e)
        {
            try
            {
                this.PublicStyleExampleLabel.Text = string.Format(
                    Resources.Example,
                    string.Format(this.PublicStyleTextBox.Text, DateTime.Now - TimeSpan.FromMinutes(3.56), DateTime.Now, "de_dust2", "css", 3, 6) + ".dem");
            }
            catch (Exception ex)
            {
                this.PublicStyleExampleLabel.Text =
                    string.Format(Resources.InvalidValue, ex.Message);
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            var enable = checkBox2.Checked;
            CustomWarFolderTextBox.Enabled = enable;
            CustomWarFolderBrowse.Enabled = enable;
        }

        private void CustomFolderBrowse_Click(object sender, EventArgs e)
        {
            var d = new FolderBrowserDialog();
            var res = d.ShowDialog();
            if (res == DialogResult.OK)
            {
                CustomWarFolderTextBox.Text = d.SelectedPath;
            }
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            var enable = checkBox3.Checked;
            CustomPublicFolderBrowse.Enabled = enable;
            CustomPublicFolderTextBox.Enabled = enable;
        }

        private void CustomPublicFolderBrowse_Click(object sender, EventArgs e)
        {
            var d = new FolderBrowserDialog();
            var res = d.ShowDialog();
            if (res == DialogResult.OK)
            {
                CustomPublicFolderTextBox.Text = d.SelectedPath;
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            try
            {
                this.CheckValid();
            }
            catch (FormatException ex)
            {
                logger(System.Diagnostics.TraceEventType.Warning, "Input Validation: " + ex);
                MessageBox.Show(ex.Message, Resources.OptionsForm_SaveButton_Click_Invalid_Input_,MessageBoxButtons.OK, MessageBoxIcon.Warning);
                
                return;
            }
            Saved = true;
            this.SaveData();
            this.Close();
        }

        private void AbortButton_Click(object sender, EventArgs e)
        {
            this.Saved = false;
            this.Close();
        }

        private void ResetButton_Click(object sender, EventArgs e)
        {
            this.LoadData();
        }
    }
}
