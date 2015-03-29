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
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace MiBandImport.DataPanels
{
    class PanelGeneralGraphSteps : MiBandDataPanel.MiBandPanel
    {

        private Chart chart;
        private ChartArea chartArea;
        private Series seriesSteps;
        private Series seriesGoal;

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
            foreach (var miData in data.data)
            {
                // prüfen ob Daten im Zeitraum liegen
                if (miData.date >= showFrom &&
                    miData.date <= showTo)
                {
                    // Anzahl Schritte einfügen
                    seriesSteps.Points.AddXY(miData.date.ToOADate(), (double)miData.dailySteps);

                    // tägliches Ziel einfügen
                    seriesGoal.Points.AddXY(miData.date.ToOADate(), (double)miData.dailyGoal);
                }
            }

            // Größe und Position festlegen
            chartArea.InnerPlotPosition.X = 5;
            chartArea.InnerPlotPosition.Y = 0;
            chartArea.InnerPlotPosition.Height = 95;
            chartArea.InnerPlotPosition.Width = 90;

        }

        public override void addListener()
        {

        }

        /// <summary>
        /// Eigene Komponenten initialisieren
        /// </summary>
        protected override void initOwnComponents()
        {
            // wenn nötig Chart erzeugen
            if (chart == null)
            {
                chart = new Chart();
                chart.Name = "chartS";
                chart.Dock = DockStyle.Fill;
                Controls.Add(chart);
            }

            // wenn nötig Zeichenbereich erzeugen
            if (chartArea == null)
            {
                chartArea = new ChartArea();
                chartArea.AxisX.Title = Properties.Resources.Datum;
                chartArea.AxisY.Title = Properties.Resources.Schritte;
                chartArea.AxisX.IntervalAutoMode = IntervalAutoMode.FixedCount;
                chartArea.AxisX.Interval = 1;

                // Position festlegen
                chartArea.InnerPlotPosition.X = 0;
                chartArea.InnerPlotPosition.Y = 0;
                chartArea.InnerPlotPosition.Height = 100;
                chartArea.InnerPlotPosition.Width = 100;

                chart.ChartAreas.Add(chartArea);
            }

            // Datenserie für Schritte erzeugen
            if (seriesSteps == null)
            {
                seriesSteps = new Series();
                seriesSteps.XValueType = ChartValueType.DateTime;
                seriesSteps.Color = Color.Red;
                seriesSteps.ChartType = SeriesChartType.Line;

                chart.Series.Add(seriesSteps);
            }
            else
            {
                seriesSteps.Points.Clear();
            }

            // Datenserie für das tägliche Ziel erzeugen
            if (seriesGoal == null)
            {
                seriesGoal = new Series();
                seriesGoal.XValueType = ChartValueType.Date;
                seriesGoal.Color = Color.Green;
                seriesGoal.ChartType = SeriesChartType.Line;

                chart.Series.Add(seriesGoal);
            }
            else
            {
                seriesGoal.Points.Clear();
            }
        }
    }
}
