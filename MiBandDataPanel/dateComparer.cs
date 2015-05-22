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
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace MiBandDataPanel
{
    public class dateComparer : IComparer
    {
        private static int sortOrderModifier = 1;

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="sortOrder"></param>
        public dateComparer(ListSortDirection sortOrder)
        {
            // Richtungsmodifizierer setzen
            if (sortOrder == ListSortDirection.Descending)
            {
                sortOrderModifier = -1;
            }
            else if (sortOrder == ListSortDirection.Ascending)
            {
                sortOrderModifier = 1;
            }
        }

        /// <summary>
        /// Vergleicht zwei Datumswerte
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public int Compare(object x, object y)
        {
            // die beiden zu sortierenden Zeilen holen
            DataGridViewRow DataGridViewRow1 = (DataGridViewRow)x;
            DataGridViewRow DataGridViewRow2 = (DataGridViewRow)y;

            // Datum liegt als Text vor, in Datum umwandeln
            DateTime value1 = DateTime.Parse((string)DataGridViewRow1.Cells[0].Value);
            DateTime value2 = DateTime.Parse((string)DataGridViewRow2.Cells[0].Value);

            //  Datumswerte vergleichen
            return value1.CompareTo(value2) * sortOrderModifier;
        }
    }
}
