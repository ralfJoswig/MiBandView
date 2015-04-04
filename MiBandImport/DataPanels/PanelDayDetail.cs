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

        /// <summary>
        /// Konstruktor
        /// </summary>
        public PanelDayDetail()
        {
            // ist das Datagrid bereits vorhanden
            if (dataGridView == null)
            {
                // nein, dann anlegen
                dataGridView = new DataGridView();
                dataGridView.AllowUserToAddRows = false;
                dataGridView.AllowUserToDeleteRows = false;
                dataGridView.BorderStyle = BorderStyle.Fixed3D;
                dataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
                dataGridView.Dock = DockStyle.Fill;
                dataGridView.Name = "dataGridViewDayDetail";
                dataGridView.ReadOnly = true;
                dataGridView.RowHeadersVisible = false;
                dataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                this.Dock = DockStyle.Fill;

                // Spalten einfügen
                dataGridView.ColumnCount = 9;
                dataGridView.Columns[0].DataPropertyName = "date";
                dataGridView.Columns[1].DataPropertyName = "description";
                dataGridView.Columns[2].DataPropertyName = "steps";
                dataGridView.Columns[3].DataPropertyName = "walkDistance";
                dataGridView.Columns[4].DataPropertyName = "runDistance";
                dataGridView.Columns[5].DataPropertyName = "walkCalories";
                dataGridView.Columns[6].DataPropertyName = "runCalories";
                dataGridView.Columns[7].DataPropertyName = "rawActivity";
                dataGridView.Columns[8].DataPropertyName = "rawSensorData";

                // die einzelnen Spalten bearbeiten
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
                        case "description":
                            col.HeaderText = Properties.Resources.Beschreibung;
                            break;
                        case "walkDistance":
                            col.HeaderText = Properties.Resources.GehenEntfernung;
                            break;
                        case "steps":
                            col.HeaderText = Properties.Resources.Schritte;
                            break;
                        case "walkCalories":
                            col.HeaderText = Properties.Resources.GehenKalorien;
                            break;
                        case "runDistance":
                            col.HeaderText = Properties.Resources.LaufenEntfernung;
                            break;
                        case "runCalories":
                            col.HeaderText = Properties.Resources.LaufenKalorien;
                            break;
                        case "rawActivity":
                            col.HeaderText = Properties.Resources.RohdatenAktivität;
                            break;
                        case "rawSensorData":
                            col.HeaderText = Properties.Resources.RohdatenSensorwert;
                            break;
                    }
                }

                // Datagrid hinzufügen
                this.Controls.Add(dataGridView);                
            }
        }

        /// <summary>
        /// Control soll mitbekommen wenn der ausgewählte Tag geändert wird
        /// </summary>
        public void addListener()
        {
            ((Form1)this.TopLevelControl).selectectRowChanged += new Form1.SelectedDayChangedEventHandler(OnDayChanged);
        }

        /// <summary>
        /// Reagiert auf eine geänderte Auswahl des Tages
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        private void OnDayChanged(object sender, EventArgsClasses.EventArgsSelectedDayChanged data)
        {
            // alte Anzeige löschen
            dataGridView.Rows.Clear();

            // Liste mit den Detaildaten holen
            List<MiBandDetail> detailList = data.data.detail;

            // die Zeilen in den Grid einfügen
            foreach (MiBandDetail detail in detailList)
            {
                // Zeile hinzufügen
                dataGridView.Rows.Add(new Object[] {detail.dateTime.ToString(),
                                                    detail.discription,
                                                    detail.steps,
                                                    Math.Round(detail.walkDistance, 2),
                                                    Math.Round(detail.runDistance, 2),
                                                    Math.Round(detail.walkCalories, 2),
                                                    Math.Round(detail.runCalories, 2),
                                                    detail.category,
                                                    detail.intensity});
                
            }
        }
    }
}
