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

            // neue Datenquelle für die Anzeige
            List<MiBandData> dataShow = new List<MiBandData>();

            // Daten für die Filterung der Anzeige prüfen
            foreach (MiBandData miData in data.data)
            {
                // prüfen ob Daten im Zeitraum liegen
                if (miData.date >= showFrom &&
                    miData.date <= showTo)
                {
                    // ja, dann für Anzeige übernehmen
                    dataShow.Add(miData);
                }
            }

            // Daten in Grid einfügen
            dataGridView.DataSource = dataShow;

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

                dataGridView.ColumnHeaderMouseClick += new DataGridViewCellMouseEventHandler(OnColumnHeaderMouseClick);

                this.Controls.Add(dataGridView);
            }
        }

        private void OnColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            /*DataGridViewColumn column = dataGridView.Columns[e.ColumnIndex];

            switch(column.Name)
            {
                /*case "date":
                    IComparer comp = new dateComparer();
                    dataGridView.Sort(comp);
                    break;*/
            /*case "lightSleepMin":
                col.HeaderText = "leichten Schlaf";
                break;

            case "deepSleepMin":
                col.HeaderText = "Tiefschlaf";
                break;

            case "awakeMin":
                col.HeaderText = "wach";
                break;

            case "runTimeMin":
                col.HeaderText = "Laufzeit";
                break;

            case "runDistanceMeter":
                col.HeaderText = "Laufen Entfernung";
                break;

            case "runBurnCalories":
                col.HeaderText = "Laufen Kalorien";
                break;

            case "walkTimeMin":
                col.HeaderText = "Gehen Zeit";
                break;

            case "dailySteps":
                col.HeaderText = "Schritte";
                break;

            case "dailyDistanceMeter":
                col.HeaderText = "Entfernung";
                break;

            case "dailyBurnCalories":
                col.HeaderText = "Kalorien";
                break;

            case "dailyGoal":
                col.HeaderText = "Ziel Schritte";
                break;

            case "sleepStartTime":
                col.HeaderText = "Einschlafzeit";
                break;

            case "sleepEndTime":
                col.HeaderText = "Aufwachzeit";
                break;

            case "sleepDuration":
                col.HeaderText = "Schlafdauer";
                break;

            case "sleepStart":
                col.Visible = false;
                break;

            case "sleepEnd":
                col.Visible = false;
                break;*/
            //}
            //DataGridViewColumn oldColumn = dataGridView.SortedColumn;
            //ListSortDirection direction = ListSortDirection.Ascending;

            /*           // If oldColumn is null, then the DataGridView is not sorted.
                       if (oldColumn != null)
                       {
                           // Sort the same column again, reversing the SortOrder.
                           if (oldColumn == newColumn &&
                               dataGridView.SortOrder == SortOrder.Ascending)
                           {
                               direction = ListSortDirection.Descending;
                           }
                           else
                           {
                               // Sort a new column and remove the old SortGlyph.
                               direction = ListSortDirection.Ascending;
                               oldColumn.HeaderCell.SortGlyphDirection = SortOrder.None;
                           }
                       }
                       else
                       {
                           direction = ListSortDirection.Ascending;
                       }*/

            // Sort the selected column.
            /*dataGridView.Sort(newColumn, direction);
            newColumn.HeaderCell.SortGlyphDirection =
                direction == ListSortDirection.Ascending ?
                SortOrder.Ascending : SortOrder.Descending;*/

            //this.dataGridView.Sort(dataGridView.Columns[0], ListSortDirection.Descending);
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
                var dailyGoal = Convert.ToInt32(row.Cells["dailyGoal"].Value);
                var steps = Convert.ToInt32(row.Cells["dailySteps"].Value);

                // wurde das Tagesziel erreicht
                if (steps > dailyGoal)
                {
                    // ja, dann grün
                    row.Cells["dailySteps"].Style.BackColor = Color.LightGreen;
                }
                else
                {
                    // nein, dann rot
                    row.Cells["dailySteps"].Style.BackColor = Color.Red;
                }

                // genug geschlafen?
                if ((TimeSpan)row.Cells["sleepDuration"].Value < sleepDuration)
                {
                    // nein
                    row.Cells["sleepDuration"].Style.BackColor = Color.Red;
                }
                else
                {
                    // ja
                    row.Cells["sleepDuration"].Style.BackColor = Color.LightGreen;
                }
            }
        }
    }
}