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

using System;
using System.Windows.Forms;

namespace MiBandDataPanel
{
    public partial class MiBandPanel: Panel 
    {
        public enum DataType { Global, Detail }

        protected DataType type;
        protected MiBand.MiBand data;    
        protected TimeSpan sleepDuration;
        protected DateTime showFrom;
        protected DateTime showTo;

        /// <summary>
        /// Konstruktor
        /// </summary>
        public MiBandPanel()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Daten für die Anzeige setzen
        /// </summary>
        /// <param name="_type"></param>
        /// <param name="_data"></param>
        /// <param name="_sleepDuration"></param>
        /// <param name="_showFrom"></param>
        /// <param name="_showTo"></param>
        public void setData(DataType _type, MiBand.MiBand _data, TimeSpan _sleepDuration, DateTime _showFrom, DateTime _showTo)
        {
            // Daten übernehmen
            type = _type;
            data = _data;    
            sleepDuration = _sleepDuration;
            showFrom = _showFrom;
            showTo = _showTo;

            // für die Datumsfelder die Zeit zurücksetzen
            showFrom = new DateTime(showFrom.Year, showFrom.Month, showFrom.Day, 0, 0, 0);
            showTo = new DateTime(showTo.Year, showTo.Month, showTo.Day, 23, 59, 59);
            
            // eigene Komponenten erzeugen
            initOwnComponents();

            // Anzeige aufbereiten
            if (data != null)
            {
                showData();
            }
        }

        /// <summary>
        /// Erzeugt die benötigten Elemente
        /// </summary>
        protected virtual void initOwnComponents() { }

        /// <summary>
        /// Zeigt die Daten an
        /// </summary>
        protected virtual void showData() { }

        /// <summary>
        /// Fügt etwaige Evtenhandler hinzu
        /// </summary>
        public virtual void addListener() { }

        /// <summary>
        /// Nimmt Änderungen an der gewünschten Schlafdauer entgegen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="sleepTime"></param>
        public void changeSleepTime(object sender, TimeSpan sleepTime)
        {
            // geänderte Zeit übernehmen
            sleepDuration = sleepTime;

            // Anzeige auffrischen
            initOwnComponents();
            showData();
        }

        /// <summary>
        /// Nimmt Änderungen an am Anzeigezeitraum entgegen 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        public void changeShowFromTo(object sender, DateTime from, DateTime to)
        {
            // geänderte Werte übernehmen
            showFrom = from;
            showTo = to;

            // Anzeige auffrischen
            initOwnComponents();
            showData();
        }
    }
}
