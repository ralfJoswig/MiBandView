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

using log4net;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MiBandImport
{
    public partial class FormExport : Form
    {
        protected static readonly ILog log = LogManager.GetLogger(typeof(Program));

        private MiBand.MiBand miband;

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="_miband"></param>
        public FormExport(MiBand.MiBand _miband)
        {
            // Daten merken
            miband = _miband;

            // Elemente initialisieren
            InitializeComponent();

            // Auswahl der möglichen Daten vorbelegen
            comboBoxData.Items.Add(Properties.Resources.Tagesuebersicht);
            comboBoxData.Items.Add(Properties.Resources.RawData);

            // ersten Eintrag auswählen
            comboBoxData.SelectedIndex = 0;
        }

        /// <summary>
        /// Nach Dateinamen suchen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonBrowse_Click(object sender, EventArgs e)
        {            
            // Dialog anzeigen
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                // Dialog wurde mit der OK-Taste beendet, Dateinamen übernehmen
                textBoxFilename.Text = saveFileDialog.FileName;
            }
        }

        /// <summary>
        /// Datenexport soll durchgeführt werden
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonStart_Click(object sender, EventArgs e)
        {
            // Export ausführen
            var meldung = miband.export(textBoxSperator.Text, textBoxFilename.Text, comboBoxData.SelectedIndex, checkBoxHeaderline.Checked);

            // gab es beim Export eine Meldung
            if (meldung == null)
            {
                // nein, dann war alles i.O.
                MessageBox.Show("Daten erfolgreich exportiert",
                                "Export erfolgreich",
                                 MessageBoxButtons.OK,
                                 MessageBoxIcon.Information);

                // Fenster schließen
                Close();
            }
            else
            {
                // es gabe eine Meldung, dann ab ins Log
                log.Info("Export meldet: " + meldung);

                // und Meldung ausgeben
                MessageBox.Show(meldung,
                                "Beim Export trat eine Meldung auf",
                                 MessageBoxButtons.OK,
                                 MessageBoxIcon.Information);
            }
        }
    }
}
