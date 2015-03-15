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

namespace MiBandImport.data
{
    class MiBandRawData
    {
        public UInt32 id { get; set; }
        public uint type { get; set; }
        public uint source { get; set; }
        public DateTime date { get; set; }
        public string summary { get; set; }
        public string index { get; set; }
        public string blob { get; set; }
        public uint sync { get; set; }

    }
}
