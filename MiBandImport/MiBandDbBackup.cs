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
using System.IO;
using System.Windows.Forms;

namespace MiBandImport
{
    static class MiBandDbBackup
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Program));

        private static string pathBackup = Application.StartupPath + "\\dbBackup\\";

        private static string pathDB;
        private static string dateTime;

        /// <summary>
        /// Ein Backup der MiBand Datenbanken durchführen
        /// </summary>
        /// <param name="_pathDB"></param>
        public static void doBackup(string _pathDB)
        {
            // existiert das Backup-Verzeichnis schon
            if (!Directory.Exists(pathBackup))
            {
                // nein, dann anlegen
                log.Debug("Backupverzeichnis wird angelegt" + pathBackup);
                Directory.CreateDirectory(pathBackup);
            }

            // Pfad merken
            pathDB = _pathDB;

            // Datenbanken nach Backverzeichnis verschieben
            dateTime = generateBackupString();

            log.Debug("Erzeuge Backup der Mi-Datenbanken, Kennung " + dateTime);

            copy("origin_db");
            copy("origin_db-journal");
            copy("user-db");
            copy("user-db-journal");
        }

        /// <summary>
        /// Führt die Verschiebeaktion durch
        /// </summary>
        /// <param name="filename"></param>
        private static void move(string filename)
        {
            try
            {
                File.Move(pathDB + filename, pathBackup + dateTime + "_" + filename);
            }
            catch(IOException ex)
            {
                log.Error("Fehler beim sichern von " + filename);
                log.Error(ex.StackTrace);
            }
        }

        /// <summary>
        /// Führt die Kopieraktion durch
        /// </summary>
        /// <param name="filename"></param>
        private static void copy(string filename)
        {
            try
            {
                File.Copy(pathDB + filename, pathBackup + dateTime + "_" + filename);
            }
            catch (IOException ex)
            {
                log.Error("Fehler beim sichern von " + filename);
                log.Error(ex.StackTrace);
            }
        }

        /// <summary>
        /// Erzeugt die Kennung um die Backups der Datenbanken
        /// voneinander zu unterscheiden
        /// </summary>
        /// <returns></returns>
        private static string generateBackupString()
        {
            var now = DateTime.Now;

            string month = "0" + now.Month;
            string day = "0" + now.Day;
            string hour = "0" + now.Hour;
            string minute = "0" + now.Minute;
            string second = "0" + now.Second;

            string dateTime = "" +
                              now.Year +
                              month.Substring(month.Length - 2, 2) +
                              day.Substring(day.Length - 2, 2) +
                              hour.Substring(hour.Length - 2, 2) +
                              minute.Substring(minute.Length - 2, 2) +
                              second.Substring(second.Length - 2, 2);

            return dateTime;
        }
    }
}
