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
using System.Data.SQLite;
using System.IO;

namespace MiBand
{
    public class MiBand
    {
        public double weight_in_kg { get { return getWeight(); } set { setWeight(value); } }
        public double height_in_cm { get { return getHeight(); } set { setHeight(value); } }

        private double _weight_in_kg;
        private double _height_in_cm;
        private string pathToDb = null;
        private string pathOrigin = null;
        private string pathUser = null;
        private DateTime initDateTime = new DateTime(0);

        protected static SQLiteConnection connection = null;
        private List<MiBandRawData> rawData = new List<MiBandRawData>();
        private List<MiBandRawUser> rawUser = new List<MiBandRawUser>();

        public List<MiBandData> data = new List<MiBandData>();
        public List<MiBandUser> userData = new List<MiBandUser>();

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="_pathToDB"></param>
        public MiBand(string _pathToDB)
        {
            // Pfade merken
            pathToDb = _pathToDB;
            pathOrigin = pathToDb + "origin_db";
            pathUser = pathToDb + "user-db";

            // prüfen ob Datenbank vorhanden
            if (!File.Exists(pathOrigin))
            {
                throw new FileNotFoundException("Datenbank unter " + pathOrigin + " nicht gefunden.");
            }
            if (!File.Exists(pathUser))
            {
                throw new FileNotFoundException("Datenbank unter " + pathUser + " nicht gefunden.");
            }
        }

        /// <summary>
        /// Liest die Daten aus der Datenbank
        /// </summary>
        public void read()
        {
            // Datenbank öffnen
            openDB(pathOrigin);

            // Daten lesen
            readData();

            // Datenbank schließen
            closeDB();

            // Datenbank öffnen
            openDB(pathUser);

            // Daten lesen
            readDataUser();

            // Datenbank schließen
            closeDB();

            // Raw-Daten umsetzen
            convertRawData();

        }

        /// <summary>
        /// Liest die aufbereiteten Daten ein
        /// </summary>
        private void readDataUser()
        {
            SQLiteCommand command = new SQLiteCommand(connection);
            SQLiteDataReader reader = null;

            // Daten auslesen und speichern
            var com = "select * from lua_list order by date, time;";
            command.CommandText = com;

            reader = command.ExecuteReader();

            // gefundene Datensätze verarbeiten
            while (reader.Read())
            {
                var raw = new MiBandRawUser();

                raw.id = readUIntFromDB(reader, "_id");
                raw.date = readDateTimeFromDB(reader, "DATE");
                raw.time = readDateTimeFromDB(reader, "TIME");
                raw.type = readTextAsUIntFromDB(reader, "TYPE");
                raw.right = readStringFromDB(reader, "RIGHT");
                raw.index = readStringFromDB(reader, "INDEX");
                raw.json_string = readStringFromDB(reader, "JSON_STRING");
                raw.script_version = readStringFromDB(reader, "SCRIPT_VERSION");
                raw.lua_action_script = readStringFromDB(reader, "LUA_ACTION_SCRIPT");
                raw.text1 = readStringFromDB(reader, "TEXT1");
                raw.text2 = readStringFromDB(reader, "TEXT2");
                raw.start = readTextAsUIntFromDB(reader, "START");
                raw.stop = readTextAsUIntFromDB(reader, "STOP");
                raw.expire_time = readDateTimeFromDB(reader, "EXPIRE_TIME");

                // Datensatz merken
                rawUser.Add(raw);
            }

            // nach getaner Arbeit Ressourcen freigeben
            if (reader != null)
            {
                reader.Dispose();
                reader.Close();
            }
            if (command != null)
            {
                command.Dispose();
            }
        }

        /// <summary>
        /// Liest einen Ganzzahl-Wert von der Datenbank
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        private UInt32 readUIntFromDB(SQLiteDataReader reader, string column)
        {
            // Index der Spalte ermitteln
            int index = reader.GetOrdinal(column);

            // enthält das Feld einen Wert auf der DB
            if (reader.IsDBNull(index))
            {
                // nein, dann null zurückmelden
                return 0;
            }
            else
            {
                // Wert auslesen
                return (UInt32)reader.GetInt32(index);
            }
        }

        /// <summary>
        /// Liest einen als Text gespeicherten Ganzzahl-Wert von der Datenbank
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        private UInt32 readTextAsUIntFromDB(SQLiteDataReader reader, string column)
        {
            // Index der Spalte ermitteln
            int index = reader.GetOrdinal(column);

            // enthält das Feld einen Wert auf der DB
            if (reader.IsDBNull(index))
            {
                // nein, dann null zurückmelden
                return 0;
            }
            else
            {
                // Wert auslesen
                return Convert.ToUInt32(reader.GetString(index));
            }
        }

        /// <summary>
        /// Liest eine Zeit/Datum Wert von der Datenbank
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        private DateTime readDateTimeFromDB(SQLiteDataReader reader, string column)
        {
            // Index der Spalte ermitteln
            int index = reader.GetOrdinal(column);

            // enthält das Feld einen Wert auf der DB
            if (reader.IsDBNull(index))
            {
                // nein, dann einen initiales Datum melden
                return new DateTime();
            }
            else
            {
                // Wert als Text lesen um die Gültigkeit zu prüfen
                string dateTime = reader.GetString(index);

                // wurde ein Wert gelesen
                if (dateTime == null ||
                    dateTime.Equals(string.Empty))
                {
                    // nein, dann einen initiales Datum melden
                    return new DateTime();
                }
                else
                {
                    // Wert lesen
                    return reader.GetDateTime(index);
                }

            }
        }

        /// <summary>
        /// Liest einen Text von der Datenbank
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        private string readStringFromDB(SQLiteDataReader reader, string column)
        {
            // Index der Spalte ermitteln
            int index = reader.GetOrdinal(column);

            // enthält das Feld einen Wert auf der DB
            if (reader.IsDBNull(index))
            {
                // nein, dann leeren String melden
                return string.Empty;
            }
            else
            {
                // Text lesen
                return reader.GetString(index);
            }
        }

        /// <summary>
        /// Schließt die Datenbankverbindung
        /// </summary>
        private void closeDB()
        {
            // Datenbankverbindung schließen
            if (connection != null)
            {
                connection.Close();
                connection.Dispose();
                connection = null;
            }
        }

        /// <summary>
        /// Öffnet die Datenbankverbindung
        /// </summary>
        private void openDB(string db)
        {
            // wenn nötig Verbindung erstellen
            if (connection == null)
            {
                connection = new SQLiteConnection();
                connection.ConnectionString = "Data Source=" + db;
                connection.Open();
            }
        }

        /// <summary>
        /// Liest die Daten aus der Datenbank aus
        /// </summary>
        private void readData()
        {
            SQLiteCommand command = new SQLiteCommand(connection);
            SQLiteDataReader reader = null;

            // Daten auslesen und speichern
            var com = "select * from date_data order by date;";
            command.CommandText = com;

            reader = command.ExecuteReader();

            // gefundene Datensätze verarbeiten
            while (reader.Read())
            {
                var raw = new MiBandRawData();

                raw.id = readUIntFromDB(reader, "id");
                raw.type = readUIntFromDB(reader, "type");
                raw.source = readUIntFromDB(reader, "source");
                raw.date = readDateTimeFromDB(reader, "date");
                raw.summary = readStringFromDB(reader, "summary");
                raw.index = readStringFromDB(reader, "index");
                raw.data = readByteArrayFromDB(reader, "data", 4320);
                raw.sync = readUIntFromDB(reader, "sync");

                // Datensatz merken
                rawData.Add(raw);
            }

            // nach getaner Arbeit Ressourcen freigeben
            if (reader != null)
            {
                reader.Dispose();
                reader.Close();
            }
            if (command != null)
            {
                command.Dispose();
            }

        }

        /// <summary>
        /// Liest Bytes in ein Array ein
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="column"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        private byte[] readByteArrayFromDB(SQLiteDataReader reader, string column, int count)
        {
            // Index der Spalte ermitteln
            int index = reader.GetOrdinal(column);
            byte[] buffer = new byte[count];

            // enthält das Feld einen Wert auf der DB
            if (reader.IsDBNull(index))
            {
                // nein, dann null zurückmelden
                return buffer;
            }
            else
            {
                // Werte auslesen
                reader.GetBytes(index, 0, buffer, 0, count);
                return buffer;
            }
        }

        /// <summary>
        /// Setzt die Rohdaten um
        /// </summary>
        private void convertRawData()
        {
            // die einzelnen Rohdatensätze bearbeiten
            foreach (MiBandRawData raw in rawData)
            {
                var miData = new MiBandData();

                // Datum übernehmen
                miData.date = raw.date;

                // Schlafdaten ermitteln
                string stringSleep = findMainPart(raw.summary, "slp");
                miData.sleepStart = findSubPart(stringSleep, "st");
                miData.sleepEnd = findSubPart(stringSleep, "ed");
                miData.deepSleepMin = findSubPart(stringSleep, "dp");
                miData.lightSleepMin = findSubPart(stringSleep, "lt");
                miData.awakeMin = findSubPart(stringSleep, "wk");

                // Schrittdaten ermitteln
                string stringStep = findMainPart(raw.summary, "stp");
                miData.runTimeMin = findSubPart(stringStep, "rn");
                miData.runDistanceMeter = findSubPart(stringStep, "runDist");
                miData.runBurnCalories = findSubPart(stringStep, "runCal");
                miData.walkTimeMin = findSubPart(stringStep, "wk");
                miData.dailySteps = findSubPart(stringStep, "ttl");
                miData.dailyDistanceMeter = findSubPart(stringStep, "dis");
                miData.dailyBurnCalories = findSubPart(stringStep, "cal");

                // Tagesziel ermitteln
                miData.dailyGoal = findSubPart(raw.summary, "goal");

                // Detaildaten umsetzen
                miData.detail = convertDetailData(raw.data, miData.date);

                // Datensatz merken
                data.Add(miData);
            }

            // Rohdatensätze für Benutzer umsetzen
            foreach (MiBandRawUser raw in rawUser)
            {
                var data = new MiBandUser();

                data.id = raw.id;
                data.dateTime = raw.getDateTime();
                data.type = raw.type;
                data.right = raw.right;
                data.index = raw.index;
                data.json_string = raw.json_string;
                data.script_version = raw.script_version;
                data.lua_action_script = raw.lua_action_script;
                data.text1 = raw.text1;
                data.text2 = raw.text2;
                data.start = raw.start;
                data.stop = raw.stop;
                data.expire_time = data.expire_time;

                userData.Add(data);
            }
        }

        /// <summary>
        /// Konvertiert die Detaildaten
        /// </summary>
        /// <param name="data"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        private List<MiBandDetail> convertDetailData(byte[] data, DateTime date)
        {
            // Rückgabe vorbereiten
            List<MiBandDetail> list = new List<MiBandDetail>();

            // das Array durchlaufen
            int index = 0;
            int minute = 0;
            while (index < data.Length - 1)
            {
                // einzelnen Satz aufbereiten
                var record = new MiBandDetail();
                record.dateTime = date.AddMinutes(minute++);
                record.category = data[index++];
                record.intensity = data[index++];
                record.steps = data[index++];

                // und Datensatz merken
                list.Add(record);
            }

            return list;
        }

        /// <summary>
        /// Sucht einen Unterpunkt in den Rohdaten
        /// </summary>
        /// <param name="data"></param>
        /// <param name="mark"></param>
        /// <returns></returns>
        private uint findSubPart(string data, string mark)
        {
            var start = data.IndexOf(mark);
            var from = start + 1;
            var to = data.Length;
            for (int index = from; index <= to; index++)
            {
                var test = data.Substring(index, 1);
                if (test.Equals(":"))
                {
                    from = index + 1;
                }
                else if (test.Equals(",") ||
                         test.Equals("}"))
                {
                    to = index;
                }
            }

            return Convert.ToUInt32(data.Substring(from, to - from));
        }

        /// <summary>
        /// Findet einen Hauptpunkt in den Rohdaten
        /// </summary>
        /// <param name="data"></param>
        /// <param name="mark"></param>
        /// <returns></returns>
        private string findMainPart(string data, string mark)
        {
            var start = data.IndexOf(mark);
            var from = start + 5;
            var to = data.Length;
            for (int index = from; index <= to; index++)
            {
                var test = data.Substring(index, 1);
                if (test.Equals("{"))
                {
                    from = index + 1;
                }
                else if (test.Equals("}"))
                {
                    to = index;
                }
            }

            return data.Substring(from, to - from + 1);
        }

        /// <summary>
        /// Setzt die Größe
        /// </summary>
        /// <param name="value"></param>
        private void setHeight(double value)
        {
            _height_in_cm = value;

            MiBandDetail.height_in_cm = _height_in_cm;

            // da die Größe geändert wurde, in allen Datensätzen die Berechnung der
            // abhängigen Werte zurücksetzen
            foreach (MiBandData record in data)
            {
                foreach (MiBandDetail detail in record.detail)
                {
                    detail.resetMarker();
                }
            }
        }

        /// <summary>
        /// Setzt das Gewicht
        /// </summary>
        /// <param name="value"></param>
        private void setWeight(double value)
        {
            _weight_in_kg = value;

            MiBandDetail.weight_in_kg = _weight_in_kg;

            // da die Größe geändert wurde, in allen Datensätzen die Berechnung der
            // abhängigen Werte zurücksetzen
            foreach (MiBandData record in data)
            {
                foreach (MiBandDetail detail in record.detail)
                {
                    detail.resetMarker();
                }
            }
        }

        /// <summary>
        /// Gibt das Gewicht zurück
        /// </summary>
        /// <returns></returns>
        private double getWeight()
        {
            return _weight_in_kg;
        }

        /// <summary>
        /// Gibt die Größe zurück
        /// </summary>
        /// <returns></returns>
        private double getHeight()
        {
            return _height_in_cm;
        }

        /// <summary>
        /// Speichert die Daten in einer Datei
        /// </summary>
        /// <param name="seperator"></param>
        /// <param name="filename"></param>
        /// <param name="datapart"></param>
        /// <param name="withHeaderline"></param>
        /// <returns></returns>
        public string export(string seperator, string filename, int datapart, bool withHeaderline)
        {
            // Datei zur Ausgabe öffnen
            FileStream file = new FileStream(filename, FileMode.Create);
            try
            {
                // Ausgabe vorbereiten
                using (StreamWriter sw = new StreamWriter(file, System.Text.Encoding.Default))
                {
                    // verschiedene Ausgaben durchführen
                    switch (datapart)
                    {
                        case 0:
                            // Tagesübersicht
                            exportDailyOverview(sw, seperator, withHeaderline);
                            break;
                        case 1:
                            // Rohdaten
                            exportRawData(sw, seperator, withHeaderline);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                // fehlerhafte Datei löschen
                File.Delete(filename);

                // Fehler zurückmelden
                return ex.Message;
            }

            //Datei schließen
            file.Close();

            // korrektes Speichern zurückmelden
            return null;
        }

        /// <summary>
        /// Speichert die Tagesübersicht
        /// </summary>
        /// <param name="sw"></param>
        /// <param name="seperator"></param>
        /// <param name="withHeaderline"></param>
        private void exportDailyOverview(StreamWriter sw, string seperator, bool withHeaderline)
        {
            // wenn gewünscht die Kopfzeile ausgeben
            if (withHeaderline)
            {
                string output = "Date" +
                                seperator +
                                "Light sleeping" +
                                seperator +
                                "Deep sleep" +
                                seperator +
                                "wake" +
                                seperator +
                                "Run-time" +
                                seperator +
                                "Run distance" +
                                seperator +
                                "Running calories" +
                                seperator +
                                "Walk time" +
                                seperator +
                                "Steps" +
                                seperator +
                                "Distance" +
                                seperator +
                                "Calories" +
                                seperator +
                                "Goal steps" +
                                seperator +
                                "Sleep time" +
                                seperator +
                                "Wake-up time" +
                                seperator +
                                "Sleep duration";
                sw.WriteLine(output);
            }

            // alle Datensätze bearbeiten
            foreach (MiBandData miData in data)
            {
                // wenn kein Schlafstart,-ende oder -dauer gesetzt ist auch keine anzeigen
                string sleepStartTime = string.Empty;
                if (miData.sleepStartTime.CompareTo(initDateTime) != 0)
                {
                    sleepStartTime = miData.sleepStartTime.ToShortTimeString();
                }

                string sleepEndTime = string.Empty;
                if (miData.sleepEndTime.CompareTo(initDateTime) != 0)
                {
                    sleepEndTime = miData.sleepEndTime.ToShortTimeString();
                }

                string sleepDuration = string.Empty;
                if (miData.sleepEndTime.CompareTo(initDateTime) != 0)
                {
                    sleepDuration = miData.sleepDuration.ToString();
                }

                // Zeile zusammenfügen
                string output = string.Concat(miData.date.ToShortDateString(),
                                              seperator,
                                              miData.lightSleepMin,
                                              seperator,
                                              miData.deepSleepMin,
                                              seperator,
                                              miData.awakeMin,
                                              seperator,
                                              miData.runTimeMin,
                                              seperator,
                                              miData.runDistanceMeter,
                                              seperator,
                                              miData.runBurnCalories,
                                              seperator,
                                              miData.walkTimeMin,
                                              seperator,
                                              miData.dailySteps,
                                              seperator,
                                              miData.dailyDistanceMeter,
                                              seperator,
                                              miData.dailyBurnCalories,
                                              seperator,
                                              miData.dailyGoal,
                                              seperator,
                                              sleepStartTime,
                                              seperator,
                                              sleepEndTime,
                                              seperator, 
                                              sleepDuration);

                // Daten ausgeben
                sw.WriteLine(output);
            }
        }

        /// <summary>
        /// Speichert die Detaildaten
        /// </summary>
        /// <param name="sw"></param>
        /// <param name="seperator"></param>
        /// <param name="withHeaderline"></param>
        private void exportRawData(StreamWriter sw, string seperator, bool withHeaderline)
        {
            // wenn gewünscht die Kopfzeile ausgeben
            if (withHeaderline)
            {
                string output = "date" +
                                seperator +
                                "description" +
                                seperator +
                                "walkDistance" +
                                seperator +
                                "steps" +
                                seperator +
                                "walkCalories" +
                                seperator +
                                "runDistance" +
                                seperator +
                                "runCalories" +
                                seperator +
                                "rawActivity" +
                                seperator +
                                "rawSensorData";
                sw.WriteLine(output);
            }

            // alle Datensätze bearbeiten
            foreach (MiBandData record in data)
            {
                // Einzeldaten durchlaufen
                foreach (MiBandDetail detail in record.detail)
                {
                    // Zeile hinzufügen
                    string output = string.Concat(detail.dateTime.ToString(),
                                                  seperator,
                                                  detail.discription,
                                                  seperator,
                                                  detail.steps,
                                                  seperator,
                                                  Math.Round(detail.walkDistance, 2),
                                                  seperator,
                                                  Math.Round(detail.runDistance, 2),
                                                  seperator,
                                                  Math.Round(detail.walkCalories, 2),
                                                  seperator,
                                                  Math.Round(detail.runCalories, 2),
                                                  seperator,
                                                  detail.category,
                                                  seperator,
                                                  detail.intensity);
                    // Daten ausgeben
                    sw.WriteLine(output);
                }
            }
        }
    }
}
