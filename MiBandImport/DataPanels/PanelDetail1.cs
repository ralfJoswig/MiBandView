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
using System.Windows.Forms;

namespace MiBandImport.DataPanels
{
    class PanelDetail1 : MiBandDataPanel.MiBandPanel
    {
        private DataGridView dataGridView;

        protected override void showData()
        {
            // sind schon Daten vorhanden
            if (data == null)
            {
                return;
            }

            // Daten für die Filterung der Anzeige prüfen
            foreach (MiBandUser miData in data.userData)
            {
                // prüfen ob Daten im Zeitraum liegen
                if (miData.dateTime >= showFrom &&
                    miData.dateTime <= showTo)
                {
                    // Zeile hinzufügen
                    dataGridView.Rows.Add(new Object[] {miData.id,
                                                        miData.dateTime,
                                                        miData.type,
                                                        miData.right,
                                                        miData.index,
                                                        miData.json_string,
                                                        miData.script_version,
                                                        miData.lua_action_script,
                                                        miData.text1,
                                                        miData.text2,
                                                        miData.start,
                                                        miData.stop,
                                                        miData.expire_time,
                                                        miData.typeText});
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
                dataGridView.Name = "dataGridViewPanel1";
                dataGridView.ReadOnly = true;

                dataGridView.ColumnCount = 14;
                dataGridView.Columns[0].DataPropertyName = "id";
                dataGridView.Columns[1].DataPropertyName = "dateTime";
                dataGridView.Columns[2].DataPropertyName = "type";
                dataGridView.Columns[3].DataPropertyName = "right";
                dataGridView.Columns[4].DataPropertyName = "index";
                dataGridView.Columns[5].DataPropertyName = "json_string";
                dataGridView.Columns[6].DataPropertyName = "script_version";
                dataGridView.Columns[7].DataPropertyName = "lua_action_script";
                dataGridView.Columns[8].DataPropertyName = "text1";
                dataGridView.Columns[9].DataPropertyName = "text2";
                dataGridView.Columns[10].DataPropertyName = "start";
                dataGridView.Columns[11].DataPropertyName = "stop";
                dataGridView.Columns[12].DataPropertyName = "expire_time";
                dataGridView.Columns[13].DataPropertyName = "typeText";

                this.Controls.Add(dataGridView);
            }
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
                    case "id":
                        col.HeaderText = Properties.Resources.ID;
                        col.Visible = false;
                        break;
                    case "dateTime":
                        col.HeaderText = Properties.Resources.DatumUhrzeit;
                        break;
                    case "type":
                        col.HeaderText = Properties.Resources.Typ;
                        col.Visible = false;
                        break;
                    case "right":
                        col.HeaderText = Properties.Resources.Berechtigung;
                        col.Visible = false;
                        break;
                    case "index":
                        col.HeaderText = Properties.Resources.Index;
                        col.Visible = false;
                        break;
                    case "json_string":
                        col.HeaderText = Properties.Resources.JsonString;
                        col.Visible = false;
                        break;
                    case "script_version":
                        col.HeaderText = Properties.Resources.ScriptVersion;
                        col.Visible = false;
                        break;
                    case "lua_action_script":
                        col.HeaderText = Properties.Resources.LuaScript;
                        col.Visible = false;
                        break;
                    case "text1":
                        col.HeaderText = Properties.Resources.Schritte;
                        break;
                    case "text2":
                        col.HeaderText = Properties.Resources.Info;
                        break;
                    case "start":
                        col.HeaderText = Properties.Resources.Start;
                        col.Visible = false;
                        break;
                    case "stop":
                        col.HeaderText = Properties.Resources.Ende;
                        col.Visible = false;
                        break;
                    case "expire_time":
                        col.HeaderText = Properties.Resources.Ablaufzeit;
                        col.Visible = false;
                        break;
                    case "typeText":
                        col.HeaderText = Properties.Resources.Typ;
                        break;
                }
            }
        }
    }
}
