/**
 * Copyright (C) 2015 Ralf Joswig
 *
 * This program is free software; you can redistribute it and/or modify it under
 * the terms of the GNU General Public License as published by the Free Software
 * Foundation; either version 3 of the License, or (at your option) any later version.
 * This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
 * without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
 * See the GNU General Public License for more details.
 * You should have received a copy of the GNU General Public License along with this program;
 * if not, see <http://www.gnu.org/licenses/>
 */

using System.Drawing;
using System.Windows.Forms;
namespace MiBandImport.DataPanels
{
    class PanelStatistics : MiBandDataPanel.MiBandPanel
    {
        private Label labelAvgSleep;
        private TextBox textBoxAvgSleep;
        private GroupBox groupBoxSleep;
    
        protected override void initOwnComponents()
        {
            InitializeComponent();
        }

        protected override void showData()
        {
            
        }

        public override void addListener()
        {
            
        }

        private void InitializeComponent()
        {
            this.groupBoxSleep = new System.Windows.Forms.GroupBox();
            this.labelAvgSleep = new System.Windows.Forms.Label();
            this.textBoxAvgSleep = new System.Windows.Forms.TextBox();
            this.groupBoxSleep.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBoxSleep
            // 
            this.groupBoxSleep.Controls.Add(this.labelAvgSleep);
            this.groupBoxSleep.Controls.Add(this.textBoxAvgSleep);
            this.groupBoxSleep.Location = new System.Drawing.Point(0, 0);
            this.groupBoxSleep.Name = "groupBoxSleep";
            this.groupBoxSleep.Size = new System.Drawing.Size(400, 400);
            this.groupBoxSleep.TabIndex = 0;
            this.groupBoxSleep.TabStop = false;
            this.groupBoxSleep.Text = "Sleep";
            // 
            // labelAvgSleep
            // 
            this.labelAvgSleep.AutoSize = true;
            this.labelAvgSleep.Location = new System.Drawing.Point(10, 15);
            this.labelAvgSleep.Name = "labelAvgSleep";
            this.labelAvgSleep.Size = new System.Drawing.Size(47, 13);
            this.labelAvgSleep.TabIndex = 0;
            this.labelAvgSleep.Text = "Average";
            // 
            // textBoxAvgSleep
            // 
            this.textBoxAvgSleep.Location = new System.Drawing.Point(80, 15);
            this.textBoxAvgSleep.Name = "textBoxAvgSleep";
            this.textBoxAvgSleep.Size = new System.Drawing.Size(20, 20);
            this.textBoxAvgSleep.TabIndex = 0;
            // 
            // PanelStatistics
            // 
            this.BackColor = System.Drawing.Color.LightGray;
            this.Controls.Add(this.groupBoxSleep);
            this.groupBoxSleep.ResumeLayout(false);
            this.groupBoxSleep.PerformLayout();
            this.ResumeLayout(false);

        }

    }
}
