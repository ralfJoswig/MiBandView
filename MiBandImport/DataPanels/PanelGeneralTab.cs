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

using MiBandDataPanel;
using MiBandImport.data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Resources;
using System.Windows.Forms;

namespace MiBandImport.DataPanels
{
    internal class PanelGeneralTab : MiBandDataPanel.MiBandPanel
    {
        private DataGridView dataGridView;

        /// <summary>
        /// Zeigt die Daten an
        /// </summary>
        protected override void showData()
        {
            // sind schon Daten vorhanden
            if (data == null)
            {
                return;
            }

            // Daten für die Filterung der Anzeige prüfen
            foreach (MiBandData miData in data.data)
            {
                // prüfen ob Daten im Zeitraum liegen
                if (miData.date >= showFrom &&
                    miData.date <= showTo)
                {
                    // Zeile hinzufügen
                    dataGridView.Rows.Add(new Object[] {miData.date.ToShortDateString(),
                                                        miData.lightSleepMin,
                                                        miData.deepSleepMin,
                                                        miData.awakeMin,
                                                        miData.runTimeMin,
                                                        miData.runDistanceMeter,
                                                        miData.runBurnCalories,
                                                        miData.walkTimeMin,
                                                        miData.dailySteps,
                                                        miData.dailyDistanceMeter,
                                                        miData.dailyBurnCalories,
                                                        miData.dailyGoal,
                                                        miData.sleepStartTime,
                                                        miData.sleepEndTime,
                                                        miData.sleepDuration,
                                                        miData.sleepStart,
                                                        miData.sleepEnd});
                }
            }

            // Grid für die modifizieren
            modifyDataGrid();
        }

        /// <summary>
        /// Eigene Komponenten initialisieren
        /// </summary>
        protected override void initOwnComponents()
        {
            // wenn nötig, den GridView erzeugen
            if (dataGridView == null)
            {
                dataGridView = new DataGridView();
                dataGridView.AllowUserToAddRows = false;
                dataGridView.AllowUserToDeleteRows = false;
                dataGridView.BorderStyle = BorderStyle.Fixed3D;
                dataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
                dataGridView.Dock = DockStyle.Fill;
                dataGridView.Name = "dataGridViewGeneralTab";
                dataGridView.ReadOnly = true;

                dataGridView.ColumnCount = 17;
                dataGridView.Columns[0].DataPropertyName = "date";
                dataGridView.Columns[1].DataPropertyName = "lightSleepMin";
                dataGridView.Columns[2].DataPropertyName = "deepSleepMin";
                dataGridView.Columns[3].DataPropertyName = "awakeMin";
                dataGridView.Columns[4].DataPropertyName = "runTimeMin";
                dataGridView.Columns[5].DataPropertyName = "runDistanceMeter";
                dataGridView.Columns[6].DataPropertyName = "runBurnCalories";
                dataGridView.Columns[7].DataPropertyName = "walkTimeMin";
                dataGridView.Columns[8].DataPropertyName = "dailySteps";
                dataGridView.Columns[9].DataPropertyName = "dailyDistanceMeter";
                dataGridView.Columns[10].DataPropertyName = "dailyBurnCalories";
                dataGridView.Columns[11].DataPropertyName = "dailyGoal";
                dataGridView.Columns[12].DataPropertyName = "sleepStartTime";
                dataGridView.Columns[13].DataPropertyName = "sleepEndTime";
                dataGridView.Columns[14].DataPropertyName = "sleepDuration";
                dataGridView.Columns[15].DataPropertyName = "sleepStart";
                dataGridView.Columns[16].DataPropertyName = "sleepEnd";

                dataGridView.ColumnHeaderMouseClick += new DataGridViewCellMouseEventHandler(OnColumnHeaderMouseClick);

                this.Controls.Add(dataGridView);
            }
        }

        private void OnColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            // wurde auf die Datumsspalte geklickt
            DataGridViewColumn column = dataGridView.Columns[e.ColumnIndex];
            if (column == null ||
                column.DataPropertyName != "date")
            {
                // nein, dann die Standartsortiermethode verwenden
                return;
            }

            // Sortierrichtung bestimmen
            ListSortDirection direction;
            if (column.HeaderCell.SortGlyphDirection == SortOrder.None ||
                column.HeaderCell.SortGlyphDirection == SortOrder.Descending)
            {
                // keine oder absteigend sortiert, dann jetzt aufsteigend
                direction = ListSortDirection.Ascending;
            }
            else
            {
                // bisher aufsteigend sortiert, dann jetzt absteigen
                direction = ListSortDirection.Descending;
            }

            // Grid sortieren
            dataGridView.Sort(new dateComparer(direction));

            // Kennzeichen für Sortierrichtung setzen
            column.HeaderCell.SortGlyphDirection = direction == ListSortDirection.Ascending ?
                                                                SortOrder.Ascending : 
                                                                SortOrder.Descending;
        }

        /// <summary>
        /// Anzeige mit den Einzeldaten aufbereiten
        /// </summary>
        private void modifyDataGrid()
        {
            // die einzelnen Datensätze untersuchen
            foreach (DataGridViewColumn col in dataGridView.Columns)
            {
                // Spaltenbreite auf Optimum setzen
                col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

                //col.SortMode = DataGridViewColumnSortMode.Automatic;

                // die Überschrift und ggf. die Sichtbarkeit der Spalten setzen
                switch (col.DataPropertyName)
                {
                    case "date":
                        col.HeaderText = Properties.Resources.Datum;
                        break;

                    case "lightSleepMin":
                        col.HeaderText = Properties.Resources.LeichtenSchlaf;
                        break;

                    case "deepSleepMin":
                        col.HeaderText = Properties.Resources.Tiefschlaf;
                        break;

                    case "awakeMin":
                        col.HeaderText = Properties.Resources.Wach;
                        break;

                    case "runTimeMin":
                        col.HeaderText = Properties.Resources.Laufzeit;
                        break;

                    case "runDistanceMeter":
                        col.HeaderText = Properties.Resources.LaufenEntfernung;
                        break;

                    case "runBurnCalories":
                        col.HeaderText = Properties.Resources.LaufenKalorien;
                        break;

                    case "walkTimeMin":
                        col.HeaderText = Properties.Resources.GehenZeit;
                        break;

                    case "dailySteps":
                        col.HeaderText = Properties.Resources.Schritte;
                        break;

                    case "dailyDistanceMeter":
                        col.HeaderText = Properties.Resources.Entfernung;
                        break;

                    case "dailyBurnCalories":
                        col.HeaderText = Properties.Resources.Kalorien;
                        break;

                    case "dailyGoal":
                        col.HeaderText = Properties.Resources.ZielSchritte;
                        break;

                    case "sleepStartTime":
                        col.HeaderText = Properties.Resources.Einschlafzeit;
                        break;

                    case "sleepEndTime":
                        col.HeaderText = Properties.Resources.Aufwachzeit;
                        break;

                    case "sleepDuration":
                        col.HeaderText = Properties.Resources.Schlafdauer;
                        break;

                    case "sleepStart":
                        col.Visible = false;
                        break;

                    case "sleepEnd":
                        col.Visible = false;
                        break;
                }
            }

            // Farben einzelner Zellen setzen
            foreach (DataGridViewRow row in dataGridView.Rows)
            {
                // Tagesziel und tägliche Schritte holen
                var dailyGoal = Convert.ToInt32(row.Cells[11].Value);
                var steps = Convert.ToInt32(row.Cells[8].Value);

                // wurde das Tagesziel erreicht
                if (steps > dailyGoal)
                {
                    // ja, dann grün
                    row.Cells[8].Style.BackColor = Color.LightGreen;
                }
                else
                {
                    // nein, dann rot
                    row.Cells[8].Style.BackColor = Color.Red;
                }

                // genug geschlafen?
                if ((TimeSpan)row.Cells[14].Value < sleepDuration)
                {
                    // nein
                    row.Cells[14].Style.BackColor = Color.Red;
                }
                else
                {
                    // ja
                    row.Cells[14].Style.BackColor = Color.LightGreen;
                }
            }
        }
    }
}