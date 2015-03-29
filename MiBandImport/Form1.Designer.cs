namespace MiBandImport
{
    partial class Form1
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
            if (disposing && (components != null))
            {
                components.Dispose();
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.textBoxWorkDirPhone = new System.Windows.Forms.TextBox();
            this.labelWorkDirPhone = new System.Windows.Forms.Label();
            this.dateTimePickerShowTo = new System.Windows.Forms.DateTimePicker();
            this.dateTimePickerShowFrom = new System.Windows.Forms.DateTimePicker();
            this.labelShowTo = new System.Windows.Forms.Label();
            this.labelShowFrom = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioButtonRoot = new System.Windows.Forms.RadioButton();
            this.radioButtonNoRoot = new System.Windows.Forms.RadioButton();
            this.maskedTextBoxSleepDur = new System.Windows.Forms.MaskedTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonShowOldData = new System.Windows.Forms.Button();
            this.buttonRead = new System.Windows.Forms.Button();
            this.tabControlMain = new System.Windows.Forms.TabControl();
            this.tabPageOrigin = new System.Windows.Forms.TabPage();
            this.tabControlOriginal = new System.Windows.Forms.TabControl();
            this.tabPageOriginTab = new System.Windows.Forms.TabPage();
            this.tabPageOriginGraphSteps = new System.Windows.Forms.TabPage();
            this.tabPageOriginGraphSleep = new System.Windows.Forms.TabPage();
            this.tabPageUserData = new System.Windows.Forms.TabPage();
            this.tabPageDayDetail = new System.Windows.Forms.TabPage();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tabControlMain.SuspendLayout();
            this.tabPageOrigin.SuspendLayout();
            this.tabControlOriginal.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            resources.ApplyResources(this.splitContainer1, "splitContainer1");
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.textBoxWorkDirPhone);
            this.splitContainer1.Panel1.Controls.Add(this.labelWorkDirPhone);
            this.splitContainer1.Panel1.Controls.Add(this.dateTimePickerShowTo);
            this.splitContainer1.Panel1.Controls.Add(this.dateTimePickerShowFrom);
            this.splitContainer1.Panel1.Controls.Add(this.labelShowTo);
            this.splitContainer1.Panel1.Controls.Add(this.labelShowFrom);
            this.splitContainer1.Panel1.Controls.Add(this.groupBox1);
            this.splitContainer1.Panel1.Controls.Add(this.maskedTextBoxSleepDur);
            this.splitContainer1.Panel1.Controls.Add(this.label1);
            this.splitContainer1.Panel1.Controls.Add(this.buttonShowOldData);
            this.splitContainer1.Panel1.Controls.Add(this.buttonRead);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.tabControlMain);
            // 
            // textBoxWorkDirPhone
            // 
            resources.ApplyResources(this.textBoxWorkDirPhone, "textBoxWorkDirPhone");
            this.textBoxWorkDirPhone.Name = "textBoxWorkDirPhone";
            // 
            // labelWorkDirPhone
            // 
            resources.ApplyResources(this.labelWorkDirPhone, "labelWorkDirPhone");
            this.labelWorkDirPhone.Name = "labelWorkDirPhone";
            // 
            // dateTimePickerShowTo
            // 
            resources.ApplyResources(this.dateTimePickerShowTo, "dateTimePickerShowTo");
            this.dateTimePickerShowTo.Name = "dateTimePickerShowTo";
            this.dateTimePickerShowTo.ValueChanged += new System.EventHandler(this.dateTimePickerShowFrom_ValueChanged);
            // 
            // dateTimePickerShowFrom
            // 
            resources.ApplyResources(this.dateTimePickerShowFrom, "dateTimePickerShowFrom");
            this.dateTimePickerShowFrom.Name = "dateTimePickerShowFrom";
            this.dateTimePickerShowFrom.ValueChanged += new System.EventHandler(this.dateTimePickerShowFrom_ValueChanged);
            // 
            // labelShowTo
            // 
            resources.ApplyResources(this.labelShowTo, "labelShowTo");
            this.labelShowTo.Name = "labelShowTo";
            // 
            // labelShowFrom
            // 
            resources.ApplyResources(this.labelShowFrom, "labelShowFrom");
            this.labelShowFrom.Name = "labelShowFrom";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioButtonRoot);
            this.groupBox1.Controls.Add(this.radioButtonNoRoot);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // radioButtonRoot
            // 
            resources.ApplyResources(this.radioButtonRoot, "radioButtonRoot");
            this.radioButtonRoot.Checked = true;
            this.radioButtonRoot.Name = "radioButtonRoot";
            this.radioButtonRoot.TabStop = true;
            this.radioButtonRoot.UseVisualStyleBackColor = true;
            // 
            // radioButtonNoRoot
            // 
            resources.ApplyResources(this.radioButtonNoRoot, "radioButtonNoRoot");
            this.radioButtonNoRoot.Name = "radioButtonNoRoot";
            this.radioButtonNoRoot.UseVisualStyleBackColor = true;
            // 
            // maskedTextBoxSleepDur
            // 
            resources.ApplyResources(this.maskedTextBoxSleepDur, "maskedTextBoxSleepDur");
            this.maskedTextBoxSleepDur.Name = "maskedTextBoxSleepDur";
            this.maskedTextBoxSleepDur.TextChanged += new System.EventHandler(this.maskedTextBoxSleepDur_TextChanged);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // buttonShowOldData
            // 
            resources.ApplyResources(this.buttonShowOldData, "buttonShowOldData");
            this.buttonShowOldData.Name = "buttonShowOldData";
            this.buttonShowOldData.UseVisualStyleBackColor = true;
            this.buttonShowOldData.Click += new System.EventHandler(this.buttonShowOldData_Click);
            // 
            // buttonRead
            // 
            resources.ApplyResources(this.buttonRead, "buttonRead");
            this.buttonRead.Name = "buttonRead";
            this.buttonRead.UseVisualStyleBackColor = true;
            this.buttonRead.Click += new System.EventHandler(this.buttonRead_Click);
            // 
            // tabControlMain
            // 
            this.tabControlMain.Controls.Add(this.tabPageOrigin);
            this.tabControlMain.Controls.Add(this.tabPageUserData);
            resources.ApplyResources(this.tabControlMain, "tabControlMain");
            this.tabControlMain.Name = "tabControlMain";
            this.tabControlMain.SelectedIndex = 0;
            // 
            // tabPageOrigin
            // 
            this.tabPageOrigin.Controls.Add(this.tabControlOriginal);
            resources.ApplyResources(this.tabPageOrigin, "tabPageOrigin");
            this.tabPageOrigin.Name = "tabPageOrigin";
            this.tabPageOrigin.UseVisualStyleBackColor = true;
            // 
            // tabControlOriginal
            // 
            this.tabControlOriginal.Controls.Add(this.tabPageOriginTab);
            this.tabControlOriginal.Controls.Add(this.tabPageOriginGraphSteps);
            this.tabControlOriginal.Controls.Add(this.tabPageOriginGraphSleep);
            this.tabControlOriginal.Controls.Add(this.tabPageDayDetail);
            resources.ApplyResources(this.tabControlOriginal, "tabControlOriginal");
            this.tabControlOriginal.Name = "tabControlOriginal";
            this.tabControlOriginal.SelectedIndex = 0;
            // 
            // tabPageOriginTab
            // 
            resources.ApplyResources(this.tabPageOriginTab, "tabPageOriginTab");
            this.tabPageOriginTab.Name = "tabPageOriginTab";
            this.tabPageOriginTab.UseVisualStyleBackColor = true;
            // 
            // tabPageOriginGraphSteps
            // 
            resources.ApplyResources(this.tabPageOriginGraphSteps, "tabPageOriginGraphSteps");
            this.tabPageOriginGraphSteps.Name = "tabPageOriginGraphSteps";
            this.tabPageOriginGraphSteps.UseVisualStyleBackColor = true;
            // 
            // tabPageOriginGraphSleep
            // 
            resources.ApplyResources(this.tabPageOriginGraphSleep, "tabPageOriginGraphSleep");
            this.tabPageOriginGraphSleep.Name = "tabPageOriginGraphSleep";
            this.tabPageOriginGraphSleep.UseVisualStyleBackColor = true;
            // 
            // tabPageUserData
            // 
            resources.ApplyResources(this.tabPageUserData, "tabPageUserData");
            this.tabPageUserData.Name = "tabPageUserData";
            this.tabPageUserData.UseVisualStyleBackColor = true;
            // 
            // tabPageDayDetail
            // 
            resources.ApplyResources(this.tabPageDayDetail, "tabPageDayDetail");
            this.tabPageDayDetail.Name = "tabPageDayDetail";
            this.tabPageDayDetail.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Name = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabControlMain.ResumeLayout(false);
            this.tabPageOrigin.ResumeLayout(false);
            this.tabControlOriginal.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonRead;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Button buttonShowOldData;
        private System.Windows.Forms.MaskedTextBox maskedTextBoxSleepDur;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radioButtonRoot;
        private System.Windows.Forms.RadioButton radioButtonNoRoot;
        private System.Windows.Forms.TabControl tabControlMain;
        private System.Windows.Forms.TabPage tabPageOrigin;
        private System.Windows.Forms.TabControl tabControlOriginal;
        private System.Windows.Forms.TabPage tabPageOriginTab;
        private System.Windows.Forms.TabPage tabPageOriginGraphSteps;
        private System.Windows.Forms.TabPage tabPageUserData;
        private System.Windows.Forms.DateTimePicker dateTimePickerShowFrom;
        private System.Windows.Forms.Label labelShowTo;
        private System.Windows.Forms.Label labelShowFrom;
        private System.Windows.Forms.DateTimePicker dateTimePickerShowTo;
        private System.Windows.Forms.TabPage tabPageOriginGraphSleep;
        private System.Windows.Forms.TextBox textBoxWorkDirPhone;
        private System.Windows.Forms.Label labelWorkDirPhone;
        private System.Windows.Forms.TabPage tabPageDayDetail;
    }
}

