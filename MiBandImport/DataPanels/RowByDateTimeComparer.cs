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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows.Forms;

namespace MiBandImport
{
    public class RowByDateTimeComparer : IComparer
    {

        private static int sortOrderModifier = 1;

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="sortOrder"></param>
        public RowByDateTimeComparer(ListSortDirection sortOrder)
        {
            // anhand der Sortierrichtung den Modifizierer festlegen
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
        /// Vergleicht zwei Datumwerte
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public int Compare(object x, object y)
        {
            // Die übergebenen Objekte in DataGrid-Zeigen umwandeln
            DataGridViewRow DataGridViewRow1 = (DataGridViewRow)x;
            DataGridViewRow DataGridViewRow2 = (DataGridViewRow)y;

            // Den ersten Wert in den beiden Zeilen in ein Datum umwandeln
            DateTime date1 = DateTime.Parse(DataGridViewRow1.Cells[0].Value.ToString());
            DateTime date2 = DateTime.Parse(DataGridViewRow2.Cells[0].Value.ToString());

            // die beiden Datumswerte vergleichen
            int CompareResult = date1.CompareTo(date2);

            // Ergebnis mit dem Modifizierer für die Sortierrichtung bearbeiten
            return CompareResult * sortOrderModifier;
        }
    }
}
