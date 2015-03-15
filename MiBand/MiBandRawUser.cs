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
    class MiBandRawUser
    {
        public UInt32 id { get; set; }
        public DateTime date { get; set; }
        public DateTime time { get; set; }
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

        public DateTime getDateTime()
        {
            return new DateTime(date.Year, date.Month, date.Day, time.Hour, time.Minute, time.Second);
        }

    }
}
