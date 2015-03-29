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

using MiBand;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MiBandImport.DataPanels
{
    public class PanelDayDetail : Panel
    {
        private DataGridView dataGridView;

        public PanelDayDetail()
        {
            if (dataGridView == null)
            {
                dataGridView = new DataGridView();
                dataGridView.AllowUserToAddRows = false;
                dataGridView.AllowUserToDeleteRows = false;
                dataGridView.BorderStyle = BorderStyle.Fixed3D;
                dataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
                dataGridView.Dock = DockStyle.Fill;
                dataGridView.Name = "dataGridViewDayDetail";
                dataGridView.ReadOnly = true;
                dataGridView.RowHeadersVisible = false;

                dataGridView.ColumnCount = 4;
                dataGridView.Columns[0].DataPropertyName = "date";
                dataGridView.Columns[1].DataPropertyName = "category";
                dataGridView.Columns[2].DataPropertyName = "intensity";
                dataGridView.Columns[3].DataPropertyName = "steps";

                // die einzelnen Datensätze untersuchen
                foreach (DataGridViewColumn col in dataGridView.Columns)
                {
                    // Spaltenbreite auf Optimum setzen
                    col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

                    // die Überschrift und ggf. die Sichtbarkeit der Spalten setzen
                    switch (col.DataPropertyName)
                    {
                        case "date":
                            col.HeaderText = Properties.Resources.Datum;
                            break;
                        case "category":
                            col.HeaderText = Properties.Resources.Kategorie;
                            break;
                        case "intensity":
                            col.HeaderText = Properties.Resources.Intensiaet;
                            break;
                        case "steps":
                            col.HeaderText = Properties.Resources.Schritte;
                            break;
                    }
                }

                this.Controls.Add(dataGridView);                
            }
        }

        public void addListener()
        {
            ((Form1)this.TopLevelControl).selectectRowChanged += new Form1.SelectedDayChangedEventHandler(OnDayChanged);
        }

        private void OnDayChanged(object sender, EventArgsClasses.EventArgsSelectedDayChanged data)
        {
            dataGridView.Rows.Clear();

            List<MiBandDetail> detailList = data.data.detail;

            foreach (MiBandDetail detail in detailList)
            {
                // Zeile hinzufügen
                dataGridView.Rows.Add(new Object[] {detail.dateTime.ToString(),
                                                    detail.category,
                                                    detail.intensity,
                                                    detail.steps,});
            }
        }
    }
}
