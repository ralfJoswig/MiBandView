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
using MiBandDataPanel;
using MiBandImport.data;
using MiBandImport.EventArgsClasses;
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
        public delegate void SelectedDayChangedEventHandler(object sender, EventArgsSelectedDayChanged data);

        public event SelectedDayChangedEventHandler selectectRowChanged;

        private DataGridView dataGridView;
        private SortOrder sortDirectionDate = SortOrder.None;
        private DateTime initDateTime = new DateTime(0);

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

            // alte Daten löschen
            dataGridView.Rows.Clear();

            // Daten für die Filterung der Anzeige prüfen
            foreach (MiBandData miData in data.data)
            {
                // prüfen ob Daten im Zeitraum liegen
                if (miData.date >= showFrom &&
                    miData.date <= showTo)
                {
                    // wenn kein Schlafstart,-ende oder -dauer gesetzt ist auch keine anzeigen
                    string sleepStartTime = string.Empty;
                    if (miData.sleepStartTime.CompareTo(initDateTime) != 0) 
                    {
                        sleepStartTime = miData.sleepStartTime.ToShortTimeString();
                    }

                    string sleepEndTime = string.Empty;
                    if (miData.sleepEndTime.CompareTo(initDateTime) != 0)
                    {
                        sleepEndTime = miData.sleepEndTime.ToShortTimeString();
                    }

                    string sleepDuration = string.Empty;
                    if (miData.sleepEndTime.CompareTo(initDateTime) != 0)
                    {
                        sleepDuration = miData.sleepDuration.ToString();
                    }

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
                                                        sleepStartTime,
                                                        sleepEndTime,
                                                        sleepDuration,
                                                        miData.sleepStart,
                                                        miData.sleepEnd,
                                                        miData.detail,
                                                        miData});
                }
            }

            // Grid für die modifizieren
            modifyDataGrid();
        }

        /// <summary>
        /// Festlegen auf welche Änderungen die Tabelle reagieren soll
        /// </summary>
        public override void addListener()
        {
            // geänderte Schlafdauer
            ((Form1)this.TopLevelControl).sleepDurationChanged += new Form1.SleepDurationChangedEventHandler(OnSleepDurationChanged);

            // geänderte Auswahl der Tage
            ((Form1)this.TopLevelControl).showSpanChanged += new Form1.ShowSpanChangedEventHandler(OnShowSpanChanged);
        }

        /// <summary>
        /// Reagiert auf Änderungen in der Auswahl der anzuzeigenden Tage
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="days"></param>
        private void OnShowSpanChanged(object sender, EventArgsDaysToDisplay days)
        {
            // merken welche Tage angezeigt werden sollen
            showFrom = days.DisplayFrom;
            showTo = days.DisplayTo;

            // für die Datumsfelder die Zeit zurücksetzen
            showFrom = new DateTime(showFrom.Year, showFrom.Month, showFrom.Day, 0, 0, 0);
            showTo = new DateTime(showTo.Year, showTo.Month, showTo.Day, 23, 59, 59);

            // wenn Daten vorhanden sind, diese neu anzeigen
            if (data != null)
            {
                showData();
            }
        }

        /// <summary>
        /// Reagiert auf Änderungen der Schlafdauer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="duration"></param>
        private void OnSleepDurationChanged(object sender, EventArgsSleepDurationChanged duration)
        {
            // geänderte Schlafdauer merken
            sleepDuration = duration.SleepDuration;

            // wenn Daten vorhanden sind, diese neu anzeigen
            if (data != null)
            {
                showData();
            }
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
                dataGridView.RowHeadersVisible = false;
                dataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                this.Dock = DockStyle.Fill;

                // Spalten festlegen
                dataGridView.ColumnCount = 19;
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
                dataGridView.Columns[17].DataPropertyName = "detail";
                dataGridView.Columns[18].DataPropertyName = "data";

                // auf einen Klick auf die Spaltenköpfe reagieren
                dataGridView.ColumnHeaderMouseClick += new DataGridViewCellMouseEventHandler(OnColumnHeaderMouseClick);

                // auf geänderte Zeilenauswahl reagieren
                dataGridView.SelectionChanged += new EventHandler(selectionChanged);

                // DataGridView zum Control hinzufügen
                this.Controls.Add(dataGridView);
            }
        }

        /// <summary>
        /// Auf eine geänderte Zeilenauswahl reagieren
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void selectionChanged(object sender, EventArgs e)
        {
            // wenn keine Zeile ausgewählt ist
            if (dataGridView == null ||
                dataGridView.SelectedRows == null ||
                dataGridView.SelectedRows.Count == 0)
            {
                // doch nichts tun
                return;
            }

            // ausgewählte Zeile holen
            DataGridViewRow row = (DataGridViewRow)dataGridView.SelectedRows[0];

            // zur Sicherheit prüfen ob Daten in der Zeile hinterlegt sind
            if (row.Cells[18].Value is MiBandData)
            {
                // Daten sind hinterlegt, dann Daten holen
                MiBandData miBand = (MiBandData)row.Cells[18].Value;

                // Parameter für den Aufruf der Zuhörer erzeugen
                EventArgsSelectedDayChanged eventArg = new EventArgsSelectedDayChanged();
                eventArg.data = miBand;

                // Gibt es Zuhörer
                if (selectectRowChanged != null)
                {
                    // ja, dann benachrichtigen
                    selectectRowChanged(this, eventArg);
                }
            }
        }

        /// <summary>
        /// Reagiert auf den Klick auf einen Spaltenkopf
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            // wurde auf die Datumsspalte geklickt
            DataGridViewColumn column = dataGridView.Columns[e.ColumnIndex];
            if (column == null ||
                column.DataPropertyName != "date")
            {
                // nein, dann die Standartsortiermethode verwenden
                dataGridView.Columns[0].HeaderCell.SortGlyphDirection = SortOrder.None;
                sortDirectionDate = SortOrder.None;
                return;
            }

            // Sortierrichtung bestimmen
            ListSortDirection direction;
            if (sortDirectionDate == SortOrder.None ||
                sortDirectionDate == SortOrder.Descending)
            {
                // keine oder absteigend sortiert, dann jetzt aufsteigend
                direction = ListSortDirection.Ascending;
                sortDirectionDate = SortOrder.Ascending;
            }
            else
            {
                // bisher aufsteigend sortiert, dann jetzt absteigen
                direction = ListSortDirection.Descending;
                sortDirectionDate = SortOrder.Descending;
            }

            // Grid sortieren
            dataGridView.Sort(new dateComparer(direction));

            // Kennzeichen für Sortierrichtung setzen
            column.HeaderCell.SortGlyphDirection = sortDirectionDate;
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

                    case "detail":
                        col.Visible = false;
                        break;

                    case "data":
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
                if ((string)row.Cells[14].Value == string.Empty)
                {
                    row.Cells[14].Style.BackColor = Color.White;
                }
                else
                {
                    TimeSpan test = TimeSpan.Parse((string)row.Cells[14].Value);
                    if (test < sleepDuration)
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
}