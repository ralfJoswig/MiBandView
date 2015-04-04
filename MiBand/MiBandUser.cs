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

namespace MiBand
{
    public class MiBandUser
    {
        public enum Type {UNLOCK = 1003, 
                          RECORD = 2001, 
                          GOOL = 2002, 
                          DAILY_GOOL = 2004, 
                          WEEK_SUM = 2006, 
                          MONTH_SUM = 2007, 
                          STEPS = 3002, 
                          ACTIFITY_MIN = 3003, 
                          SLEEP = 4001, 
                          CONNECTION = 5004 }

        public UInt32 id { get; set; }
        public DateTime dateTime { get; set; }
        public UInt32 type { get; set; }
        public string right { get; set; }
        public string index { get; set; }
        public string json_string { get; set; }
        public string script_version { get; set; }
        public string lua_action_script { get; set; }
        public string text1 { get; set; }
        public string text2 { get; set; }
        public UInt32 start { get; set; }
        public UInt32 stop { get; set; }
        public DateTime expire_time { get; set; }
        public string typeText { get { return getTypeAsText(); } set { typeText = value; } }

        private string getTypeAsText()
        {
            switch (type)
            {
                case (uint)Type.ACTIFITY_MIN:
                    return Properties.Resources.aktiv;
                case (uint)Type.STEPS:
                    return Properties.Resources.gehen;
                case (uint)Type.SLEEP:
                    return Properties.Resources.schlafen;
                case (uint)Type.MONTH_SUM:
                    return Properties.Resources.Monatszusammenfassung;
                case (uint)Type.RECORD:
                    return Properties.Resources.Rekord;
                case (uint)Type.WEEK_SUM:
                    return Properties.Resources.Wochenzusammenfassung;
                case (uint)Type.UNLOCK:
                    return Properties.Resources.Entsperren;
                case (uint)Type.GOOL:
                    return Properties.Resources.ZielErreicht;
                case (uint)Type.CONNECTION:
                    return Properties.Resources.Verbindungsinfo;
                case (uint)Type.DAILY_GOOL:
                    return Properties.Resources.Tagesziel;
            }

            return string.Empty;
        }
    }
}
