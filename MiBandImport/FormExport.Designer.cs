namespace MiBandImport
{
    partial class FormExport
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormExport));
            this.buttonStart = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.labelSeperator = new System.Windows.Forms.Label();
            this.textBoxSperator = new System.Windows.Forms.TextBox();
            this.checkBoxHeaderline = new System.Windows.Forms.CheckBox();
            this.labelFilename = new System.Windows.Forms.Label();
            this.textBoxFilename = new System.Windows.Forms.TextBox();
            this.buttonBrowse = new System.Windows.Forms.Button();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.labelExportData = new System.Windows.Forms.Label();
            this.comboBoxData = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // buttonStart
            // 
            resources.ApplyResources(this.buttonStart, "buttonStart");
            this.buttonStart.Name = "buttonStart";
            this.buttonStart.UseVisualStyleBackColor = true;
            this.buttonStart.Click += new System.EventHandler(this.buttonStart_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.buttonCancel, "buttonCancel");
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // labelSeperator
            // 
            resources.ApplyResources(this.labelSeperator, "labelSeperator");
            this.labelSeperator.Name = "labelSeperator";
            // 
            // textBoxSperator
            // 
            resources.ApplyResources(this.textBoxSperator, "textBoxSperator");
            this.textBoxSperator.Name = "textBoxSperator";
            // 
            // checkBoxHeaderline
            // 
            resources.ApplyResources(this.checkBoxHeaderline, "checkBoxHeaderline");
            this.checkBoxHeaderline.Name = "checkBoxHeaderline";
            this.checkBoxHeaderline.UseVisualStyleBackColor = true;
            // 
            // labelFilename
            // 
            resources.ApplyResources(this.labelFilename, "labelFilename");
            this.labelFilename.Name = "labelFilename";
            // 
            // textBoxFilename
            // 
            resources.ApplyResources(this.textBoxFilename, "textBoxFilename");
            this.textBoxFilename.Name = "textBoxFilename";
            // 
            // buttonBrowse
            // 
            resources.ApplyResources(this.buttonBrowse, "buttonBrowse");
            this.buttonBrowse.Name = "buttonBrowse";
            this.buttonBrowse.UseVisualStyleBackColor = true;
            this.buttonBrowse.Click += new System.EventHandler(this.buttonBrowse_Click);
            // 
            // saveFileDialog
            // 
            this.saveFileDialog.DefaultExt = "csv";
            this.saveFileDialog.FileName = "MiBandData";
            resources.ApplyResources(this.saveFileDialog, "saveFileDialog");
            // 
            // labelExportData
            // 
            resources.ApplyResources(this.labelExportData, "labelExportData");
            this.labelExportData.Name = "labelExportData";
            // 
            // comboBoxData
            // 
            this.comboBoxData.FormattingEnabled = true;
            resources.ApplyResources(this.comboBoxData, "comboBoxData");
            this.comboBoxData.Name = "comboBoxData";
            // 
            // FormExport
            // 
            this.AcceptButton = this.buttonStart;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ControlBox = false;
            this.Controls.Add(this.comboBoxData);
            this.Controls.Add(this.labelExportData);
            this.Controls.Add(this.buttonBrowse);
            this.Controls.Add(this.textBoxFilename);
            this.Controls.Add(this.labelFilename);
            this.Controls.Add(this.checkBoxHeaderline);
            this.Controls.Add(this.textBoxSperator);
            this.Controls.Add(this.labelSeperator);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonStart);
            this.Name = "FormExport";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonStart;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Label labelSeperator;
        private System.Windows.Forms.TextBox textBoxSperator;
        private System.Windows.Forms.CheckBox checkBoxHeaderline;
        private System.Windows.Forms.Label labelFilename;
        private System.Windows.Forms.TextBox textBoxFilename;
        private System.Windows.Forms.Button buttonBrowse;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private System.Windows.Forms.Label labelExportData;
        private System.Windows.Forms.ComboBox comboBoxData;
    }
}