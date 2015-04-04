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

namespace MiBandImport.data
{
    public class MiBandData
    {
        public DateTime date { get; set; }
        public UInt32 lightSleepMin { get; set; }
        public UInt32 deepSleepMin { get; set; }
        public UInt32 awakeMin { get; set; }
        public UInt32 runTimeMin { get; set; }
        public UInt32 runDistanceMeter { get; set; }
        public UInt32 runBurnCalories { get; set; }
        public UInt32 walkTimeMin { get; set; }
        public UInt32 dailySteps { get; set; }
        public UInt32 dailyDistanceMeter { get; set; }
        public UInt32 dailyBurnCalories { get; set; }
        public UInt32 dailyGoal { get; set; }
        public DateTime sleepStartTime { get { return getSleepStartDate(); } set { sleepEndTime = value; } }
        public DateTime sleepEndTime { get { return getSleepEndDate(); } set { sleepEndTime = value; } }
        public TimeSpan sleepDuration { get { return getSleepDuration(); } set { sleepDuration = value; } }
        public UInt32 sleepStart { get; set; }
        public UInt32 sleepEnd { get; set; }
        public List<MiBandDetail> detail { get; set; }

        /// <summary>
        /// Liefert die Uhrzeit für den Schlafbeginn als DateTime
        /// </summary>
        /// <returns></returns>
        private DateTime getSleepStartDate()
        {
            // Schlafbeginn ungleich -ende
            if (sleepStart != sleepEnd)
            {
                // ja, dann Startzeit aufbereiten
                DateTime sleepDate = new DateTime(1970, 1, 1);
                return sleepDate.AddSeconds(sleepStart).ToLocalTime();
            }
            else
            {
                // nein, dann ein initiales Objekt erzeugen
                return new DateTime();
            }
        }

        /// <summary>
        /// Liefert die Uhrzeit für das Schlafende als DateTime
        /// </summary>
        /// <returns></returns>
        private DateTime getSleepEndDate()
        {
            // Schlafbeginn ungleich -ende
            if (sleepStart != sleepEnd)
            {
                // ja, dann Endzeit aufbereiten
                DateTime sleepDate = new DateTime(1970, 1, 1);
                return sleepDate.AddSeconds(sleepEnd).ToLocalTime();
            }
            else
            {
                // nein, dann ein initiales Objekt erzeugen
                return new DateTime();
            }
        }

        /// <summary>
        /// Schlafdauer als Zeitraum zurückgeben
        /// </summary>
        /// <returns></returns>
        public TimeSpan getSleepDuration()
        {
            return new TimeSpan(getSleepEndDate().Subtract(getSleepStartDate()).Ticks);
        }
    }
}
