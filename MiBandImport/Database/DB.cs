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
using MiBand;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MiBandImport.DBClass
{
    public class DB
    {
        protected static readonly ILog log = LogManager.GetLogger(typeof(Program));

        protected static string dataSource = System.Windows.Forms.Application.ProductName + ".sqlite3";
        protected static SQLiteConnection connection = null;
        protected static DB instance = null;

        /// <summary>
        /// Aufstellung der grundsätzlichen Einstellungen
        /// </summary>
        public enum Prop
        {
            DbVer
        }

        /// <summary>
        /// Felder Tabelle der Tagesübersichten
        /// </summary>
        private enum FieldsDaily
        {
            date,
            lightSleepMin,
            deepSleepMin,
            awakeMin,
            runTimeMin,
            runDistanceMeter,
            runBurnCalories,
            walkTimeMin,
            dailySteps,
            dailyDistanceMeter,
            dailyBurnCalories,
            dailyGoal,
            sleepStartTime,
            sleepEndTime,
            sleepDuration,
            sleepStart,
            sleepEnd
        }

        private enum FieldsDetail
        {
            dateTime,
            category,
            intensity,
            steps,
            mode,
            runDistance,
            runCalories,
            walkDistance,
            discription,
            walkCalories
        }

        private enum FieldsDaySum
        {
            id,
            dateTime,
            type,
            right,
            index,
            json_string,
            script_version,
            lua_action_script,
            text1,
            text2,
            start,
            stop,
            expire_time,
            typeText
        }

        /// <summary>
        /// Privater Konstruktor
        /// </summary>
        private DB()
        {
            openDB();
        }

        /// <summary>
        /// Gibt eine Instanz die Datenbank zurück.
        /// </summary>
        /// <returns>Die Instanz der Datenbank</returns>
        public static DB getInstance()
        {
            if (NestedSingleton.singleton == null)
            {
                NestedSingleton.singleton = new DB();
            }

            return NestedSingleton.singleton;
        }

        /// <summary>
        /// Interne Klasse um die Datenbankinstanz zu kapseln
        /// </summary>
        class NestedSingleton
        {
            internal static DB singleton = new DB();

            /// <summary>
            /// Der Konstruktor der Singelton-Klasse für die Datenbank
            /// </summary>
            static NestedSingleton()
            {
                log.Debug("Neue DB Instanz erzeugt");

                // Datenbankverbindung öffnen
                singleton.openDB();
            }

            /// <summary>
            /// Öffnet die Datenbank neu
            /// </summary>
            static void reload()
            {
                singleton = new DB();
                singleton.openDB();
            }
        }

        /// <summary>
        /// Schließt die Verbindung zur DB
        /// </summary>
        public static void leaveInstance()
        {
            if (instance != null)
            {
                if (instance != null)
                {
                    // Datenbank schließen
                    log.Debug("Schließe DB");

                    instance.closeDB();

                    // Instanz vernichten
                    log.Debug("vernichte DB-Instanz");
                }
            }
        }

        /// <summary>
        /// Gibt den PFad zur Datenbank zurück
        /// </summary>
        /// <returns>Path zur Datenbank</returns>
        public static string getDbPath()
        {
            string path = Path.Combine(Application.StartupPath, dataSource);
            log.Debug("DB-Pfad: " + path);

            return path;
        }

        /// <summary>
        /// Prüft ob die Datenbank vorhanden ist
        /// </summary>
        /// <returns>true wenn Datenbank existiert</returns>
        public bool exsits()
        {
            log.Debug("Prüfe ob DB vorhanden");

            // Da durch das öffnen der Datenbank ggf. eine neue, leere, angelegt wird,
            // wird abgefragt ob es Werte in der Properties-Tabelle gibt
            bool exsist = true;
            try
            {
                // Wenn nötig Instanz der Datenbank besorgen
                if (instance == null)
                {
                    instance = DB.getInstance();
                }

                SQLiteCommand command = new SQLiteCommand(connection);
                SQLiteDataReader reader = null;
                try
                {
                    // Prüfen ob Tabelle vorhanden ist
                    command.CommandText = "SELECT COUNT(type) FROM sqlite_master WHERE type='table' AND name='properties';";

                    log.Debug("SQL Befehl: " + command.CommandText);

                    reader = command.ExecuteReader();

                    // gefundene Datensätze verarbeiten
                    while (reader.Read())
                    {
                        int count = reader.GetInt16(0);
                        if (count > 0)
                        {
                            exsist = true;
                        }
                        else
                        {
                            exsist = false;
                        }
                    }
                }
                catch (Exception e)
                {
                    // Fehler beim Lesen des Fahrers
                    log.Error("SQL Fehler:", e);
                }
                finally
                {
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

            }
            catch (SQLiteException)
            {
                // es gibt eine grundlegende Einstellung nicht, dann gibt es wohl alle Tabellen nicht
                exsist = false;
            }

            // Datenbank gab es bisher nicht, daher Dummy löschen
            if (exsist == false)
            {
                closeDB();
                delete();
            }

            return exsist;
        }

        /// <summary>
        /// Erzeugt wenn nötig eine neue Datenbank
        /// </summary>
        public void createDB()
        {
            // prüfen ob die Datenbank bereits vorhanden ist
            if (exsits())
            {
                // ja, dann keine neue erzeugen
                return;
            }

            // Erstellen der Tabelle
            log.Debug("erzeuge Tabellen");

            execDbCommandNoResult("CREATE TABLE [daily] ( [" + FieldsDaily.date               + "] DATE     NOT NULL PRIMARY KEY, " +
                                                         "[" + FieldsDaily.lightSleepMin      + "] INTEGER  NULL, " +
                                                         "[" + FieldsDaily.deepSleepMin       + "] INTEGER  NULL, " +
                                                         "[" + FieldsDaily.awakeMin           + "] INTEGER  NULL, " +
                                                         "[" + FieldsDaily.runTimeMin         + "] INTEGER  NULL, " +
                                                         "[" + FieldsDaily.runDistanceMeter   + "] INTEGER  NULL, " +
                                                         "[" + FieldsDaily.runBurnCalories    + "] INTEGER  NULL, " +
                                                         "[" + FieldsDaily.walkTimeMin        + "] INTEGER  NULL, " +
                                                         "[" + FieldsDaily.dailySteps         + "] INTEFER  NULL, " +
                                                         "[" + FieldsDaily.dailyDistanceMeter + "] INTEGER  NULL, " +
                                                         "[" + FieldsDaily.dailyBurnCalories  + "] INTEFER  NULL, " +
                                                         "[" + FieldsDaily.dailyGoal          + "] INTEGER  NULL, " +
                                                         "[" + FieldsDaily.sleepStartTime     + "] TIME     NULL, " +
                                                         "[" + FieldsDaily.sleepEndTime       + "] TIME     NULL, " +
                                                         "[" + FieldsDaily.sleepDuration      + "] TIME     NULL, " +
                                                         "[" + FieldsDaily.sleepStart         + "] DATETIME NULL, " +
                                                         "[" + FieldsDaily.sleepEnd           + "] DATETIME NULL);");

            execDbCommandNoResult("CREATE TABLE [details] ( [" + FieldsDetail.dateTime     + "] DATETIME NULL PRIMARY KEY, " +
                                                           "[" + FieldsDetail.category     + "] INTEGER  NULL, " +
                                                           "[" + FieldsDetail.intensity    + "] INTEGER  NULL, " +
                                                           "[" + FieldsDetail.steps        + "] INTEGER  NULL, " +
                                                           "[" + FieldsDetail.mode         + "] INTEGER  NULL, " +
                                                           "[" + FieldsDetail.runDistance  + "] FLOAT    NULL, " +
                                                           "[" + FieldsDetail.runCalories  + "] FLOAT    NULL, " +
                                                           "[" + FieldsDetail.walkDistance + "] FLOAT    NULL, " +
                                                           "[" + FieldsDetail.discription  + "] TEXT     NULL, " +
                                                           "[" + FieldsDetail.walkCalories + "] FLOAT    NULL);");

            execDbCommandNoResult("CREATE TABLE [daySum] ( [" + FieldsDaySum.id                + "] INTEGER  NULL PRIMARY KEY, " +
                                                          "[" + FieldsDaySum.dateTime          + "] DATETIME NULL, " +
                                                          "[" + FieldsDaySum.type              + "] INTEGER  NULL, " +
                                                          "[" + FieldsDaySum.right             + "] TEXT     NULL, " +
                                                          "[" + FieldsDaySum.index             + "] TEXT     NULL, " +
                                                          "[" + FieldsDaySum.json_string       + "] TEXT     NULL, " +
                                                          "[" + FieldsDaySum.script_version    + "] TEXT     NULL, " +
                                                          "[" + FieldsDaySum.lua_action_script + "] TEXT     NULL, " +
                                                          "[" + FieldsDaySum.text1             + "] TEXT     NULL, " +
                                                          "[" + FieldsDaySum.text2             + "] TEXT     NULL, " +
                                                          "[" + FieldsDaySum.start             + "] INTEGER  NULL, " +
                                                          "[" + FieldsDaySum.stop              + "] INTEGER  NULL, " +
                                                          "[" + FieldsDaySum.expire_time       + "] DATETIME NULL, " +
                                                          "[" + FieldsDaySum.typeText          + "] TEXT     NULL);");
            
            execDbCommandNoResult("CREATE TABLE [properties] ( [key]   TEXT UNIQUE NOT NULL PRIMARY KEY, " +
                                                              "[value] TEXT NULL );");

            // Voreinstellungen festlegen
            log.Debug("Befülle Tabelle mit Einstellungen");

            string guid = Program.getAssemblyGUID();
            setPropertie(guid, Prop.DbVer, "1.0");

            log.Debug("Datenbank erfolgreich angelegt");
        }

        /// <summary>
        /// Setzt den Wert für einen bestimmten Parameter
        /// </summary>
        /// <param name="guid">Eindeutiger Schlüssel der die Quellanwendung identifiziert</param>
        /// <param name="key">Eindeutiger Schlüssel für den Parameter</param>
        /// <param name="value">neuer Parameterwert</param>
        public void setPropertie(string guid, Prop key, string value)
        {
            // Schlüssel auf der DB aus Guid und dem übergebenen Schlüssel zusammensetzen
            string dbKey = guid + ":" + key;

            // Wert auf Datenbank schreiben
            execDbCommandNoResult("INSERT OR REPLACE INTO properties (key, value) VALUES ('" + dbKey + "', '" + value + "');");
        }

        /// <summary>
        /// Gibt den aktuellen Wert eines Parameters zurück
        /// </summary>
        /// <param name="guid">Eindeutiger Schlüssel der die Quellanwendung identifiziert</param>
        /// <param name="prop">Eindeutiger Schlüssel eines Parameters</param>
        /// <returns>Der aktuelle Wert des Parameters</returns>
        public string getPropertie(string guid, Prop prop)
        {
            if (instance == null)
            {
                instance = DB.getInstance();
            }

            // Key für Datenbank aus Guid und übergebenen Schlüssel zusammensetzen
            string dbKey = guid + ":" + prop;

            string ret = null;

            // übergebenen Parameter in der Tabelle lesen
            SQLiteCommand command = new SQLiteCommand(connection);
            SQLiteDataReader reader = null;

            try
            {
                command.CommandText = "SELECT value FROM properties WHERE key = '" + dbKey + "';";

                log.Debug("SQL Befehl: " + command.CommandText);

                reader = command.ExecuteReader();

                while (reader.Read())
                {
                    ret = reader[0].ToString();
                }
            }
            catch (Exception e)
            {
                // Parameter ist nicht vorhanden
                log.Error("SQL Fehler:", e);
            }
            finally
            {
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

            // ermittelten Wert zurück geben
            return ret;
        }

        /// <summary>
        /// Führt einen SQL-Befehl der kein Ergebnis liefert aus
        /// </summary>
        /// <param name="com">Der SQL-Befehl</param>
        public void execDbCommandNoResult(string com)
        {
            if (instance == null)
            {
                instance = DB.getInstance();
            }

            // Befehl ausführen
            SQLiteCommand command = new SQLiteCommand(connection);

            try
            {
                command.CommandText = com;

                log.Debug("SQL Befehl: " + command.CommandText);

                command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                // Die Datenbank meldet einen Fehler
                log.Error("SQL Fehler: " + com, e);
            }
            finally
            {
                if (command != null)
                {
                    command.Dispose();
                }
            }
        }

        /// <summary>
        /// Öffnet eine Verbindung zur Datenbank
        /// </summary>
        private void openDB()
        {
            // Prüfen ob ein Pfad zur DB hinterlegt ist
            if (getDbPath() == dataSource)
            {
                // keiner da, dann keine Datenbank anlegen
                return;
            }

            /*if(!exsits())
            {
                createDB();
            }*/

            // wenn nötig Verbindung erstellen
            if (connection == null)
            {
                if (log.IsDebugEnabled)
                {
                    try
                    {
                        log.Debug("öffne DB: " + connection.ConnectionString);
                    }
                    catch (Exception)
                    {
                    }
                }

                try
                {
                    connection = new SQLiteConnection();
                    connection.ConnectionString = "Data Source=" + getDbPath();
                    connection.Open();
                }
                catch (SQLiteException e)
                {
                    log.Error("Keine Verbindung zur DB möglich: ", e);
                }

                                // Version der Datenbank prüfen
                string dbVer = getPropertie(Program.getAssemblyGUID(), DB.Prop.DbVer);

                if (dbVer == null)
                {
                    // Version kann nicht gelesen werden, daher gehen wir davon aus das es die
                    // Datenbank noch nicht gibt
                    return;
                }

                // DB-Version passt, dann keine Anpassung vornehmen
                if (dbVer == "1.0")
                {
                    return;
                }

                // ab hier kommen Beispiel wie auf eine neue Version geupdated wird
                /*if (dbVer == "1.0")
                {
                    // alte Version, es fehlen nur zwei Parameter
                    setPropertie(guid, Database.Prop.StartInWindowmode, Convert.ToString(false));
                    setPropertie(guid, Database.Prop.WindowHeight, "600");
                    setPropertie(guid, Database.Prop.WindowWidth, "800");
                    setPropertie(guid, Database.Prop.DbVer, "1.1");
                    dbVer = "1.1";
                }
                if (dbVer == "1.1")
                {
                    setPropertie(guid, Database.Prop.CheckNewRlvs, Convert.ToString(false));
                    setPropertie(guid, Database.Prop.CheckNewPgmf, Convert.ToString(false));
                    setPropertie(guid, Database.Prop.DbVer, "1.2");
                    dbVer = "1.2";
                }
                if (dbVer != "1.2")
                {
                    // Datenbankversion passt nicht zum Programm
                    // entsprechende Mendung ausgeben
                    log.Fatal("Datenbank hat höhere Version als vom Programm benötigte Version");

                    MessageBox.Show(resManager.GetString("s0016"),
                                    resManager.GetString("s0071"),
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);

                    // Programm beenden
                    closeDB();
                    System.Environment.Exit(1);
                }*/
            }
        }

        /// <summary>
        /// Löscht die Datenbank
        /// </summary>
        public static void delete()
        {
            log.Info("Datenbank wird gelöscht");
            leaveInstance();
            File.Delete(getDbPath());
        }

        /// <summary>
        /// Verbindung zur Datenbank schließen
        /// </summary>
        public void closeDB()
        {
            log.Debug("schließe Datenbank");

            // Datenbankverbindung schließen
            if (connection != null)
            {
                try
                {
                    connection.Close();
                    connection.Dispose();
                    connection = null;
                }
                catch (SQLiteException e)
                {
                    log.Error("DB-Vedrbindung konnte nicht geschlossen werden: ", e);
                }
            }

            // Instanz vernichten
            instance = null;
            NestedSingleton.singleton = null;
        }

        /// <summary>
        /// Löscht alle Daten in den Tabellen
        /// </summary>
        public void clear()
        {
            log.Debug("Lösche DB- Inhalt");

            execDbCommandNoResult("DELETE FROM daily;");
            execDbCommandNoResult("DELETE FROM details;");
            execDbCommandNoResult("DELETE FROM daySum;");
        }


        public int saveDaySum(MiBandUser daySum)
        {

            // wenn nötig eine Instanz der DB besorgen
            if (instance == null)
            {
                instance = DB.getInstance();
            }
/*
            // aktuell höchste ID ermitteln
            int id = 0;
            SQLiteCommand command = new SQLiteCommand(connection);
            SQLiteDataReader reader = null;

            try
            {
                // gößte ID lesen
                command.CommandText = "SELECT MAX(" + FieldsTerrain.id + ") FROM terrains;";

                log.Debug("SQL Befehl: " + command.CommandText);
                
                reader = command.ExecuteReader();
                reader.Read();

                // es wurde etwas gefunden
                if (reader.HasRows)
                {
                    // aber kein gültiger Wert
                    if (reader.IsDBNull(0))
                    {
                        // dann ID mit null vorbelegen
                        id = 0;
                    }
                    else
                    {
                        // gefundenen ID übernehmen
                        id = reader.GetInt16(0);
                    }
                }
                else
                {
                    // es konnte keine ID ermittelt werden
                    log.Error("Nächste ID für Terrain kann nicht ermittelt werden.");
                }

                // ID um ein erhöhren
                id++;

                // Neues Terrain einfügen
                reader.Close();
                reader.Dispose();
            */
            SQLiteCommand command = null;
            string sqlCommand = string.Empty;
            try
            {
                string format = "yyyy-MM-dd HH:MM:ss"; 
                command = new SQLiteCommand(connection);
                sqlCommand = "INSERT INTO daySum (" + FieldsDaySum.id       + ", " +
                                                      FieldsDaySum.dateTime + ", " +
                                                      FieldsDaySum.type              + ", " +
                    FieldsDaySum.right             + //", " +
                    //FieldsDaySum.index             + //", " +
                    //FieldsDaySum.json_string       + ", " +
                    //FieldsDaySum.script_version    + ", " +
                    //FieldsDaySum.lua_action_script + ", " +
                    //FieldsDaySum.text1             + ", " +
                    //FieldsDaySum.text2             + ", " +
                    //FieldsDaySum.start             + ", " +
                    //FieldsDaySum.stop              + ", " +
                    //FieldsDaySum.expire_time       + ", " +
                    //FieldsDaySum.typeText          +
                                                          ") VALUES (\"" + daySum.id                        + "\", \"" +
                                                                           daySum.dateTime.ToString(format) + "\", \"" +
                                                                           daySum.type              + "\", \"" +
                    daySum.right             + //"\", \"" +
                    //daySum.index             + //"\", \"" +
                    //daySum.json_string       + "\", \"" +
                    //daySum.script_version    + "\", \"" +
                    //daySum.lua_action_script + "\", \"" +
                    //daySum.text1             + "\", \"" +
                    //daySum.text2             + "\", \"" +
                    //daySum.start             + "\", \"" +
                    //daySum.stop              + "\", \"" +
                    //daySum.expire_time       + "\", \"" +
                                                                          "\");";//daySum.typeText          + "\");";
                command.CommandText = sqlCommand;

                log.Debug("SQL Befehl: " + command.CommandText);
                
                command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                // Parameter ist nicht vorhanden
                log.Debug("SQL Befehl: " + sqlCommand);
                log.Error("SQL Fehler:", e);
            }
            finally
            {
                if (command != null)
                {
                    command.Dispose();
                }
            }

            // ID des neuen Terrains zurückgeben
            return 0;
        }

        /// <summary>
        /// Liest ein Terrain von der Datenbank und legt auf Wunsch ein
        /// Neues auf der DB an wenn es noch nicht vorhanden ist
        /// </summary>
        /// <param name="name">Name des Terrain</param>
        /// <param name="createIfNew">Wenn true und das Terrain nicht vorhanden wird es neu angelegt</param>
        /// <returns>Das Terrain</returns>
        /*public Terrain getTerrain(string name, bool createIfNew)
        {
            log.Debug("Lese Terrain mit Namen " + name);

            // Wenn nötig Instanz der Datenbank besorgen
            if (instance == null)
            {
                instance = Database.getInstance();
            }

            Terrain terrain = null;
            SQLiteCommand command = new SQLiteCommand(connection);
            SQLiteDataReader reader = null;

            try
            {
                // Terrain mit mit Namen lesen
                command.CommandText = "SELECT * FROM terrains WHERE " + FieldsTerrain.name_uppercase + " = \"" + name.ToUpper() + "\";";

                log.Debug("SQL Befehl: " + command.CommandText);

                reader = command.ExecuteReader();
                reader.Read();

                // gefundene Datensätze verarbeiten
                if (reader.HasRows)
                {
                    // Es wurde was gefunden, dann Terrain anlegen
                    terrain = new Terrain(reader.GetInt16(getOrdinal(reader, FieldsTerrain.id.ToString())),
                                          reader.GetString(getOrdinal(reader, FieldsTerrain.name.ToString())));
                }
                else if (createIfNew)
                {
                    // Terrain gibt es noch nicht, dann neues erzeugen
                    terrain = new Terrain(insertTerrain(name), name);
                }
            }
            catch (Exception e)
            {
                // Fehler beim Lesen des Terrains
                log.Error("SQL Fehler:", e);
            }
            finally
            {
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

            // Terrain zurückgeben
            return terrain;
        }*/

        /// <summary>
        /// Liest einen Fahrer zur ID von der Datenbank
        /// </summary>
        /// <param name="id">ID des Fahrers</param>
        /// <returns>Der gefundene Fahrer</returns>
        /*public Rider getRider(int id)
        {
            log.Debug("Lese Fahrer zur ID " + id);

            // Wenn nötig Instanz der Datenbank besorgen
            if (instance == null)
            {
                instance = Database.getInstance();
            }

            Rider rider = null;
            SQLiteCommand command = new SQLiteCommand(connection);
            SQLiteDataReader reader = null;

            try
            {
                // Fahrer mit mit ID lesen
                command.CommandText = "SELECT * FROM rider WHERE " + FieldsRider.id + " = '" + id + "';";

                log.Debug("SQL Befehl: " + command.CommandText);

                reader = command.ExecuteReader();
                reader.Read();

                // gefundene Datensätze verarbeiten
                if (reader.HasRows)
                {
                    // Es wurde was gefunden, dann Fahrer anlegen
                    rider = new Rider(reader.GetInt16(getOrdinal(reader, FieldsRider.id.ToString())),
                                      reader.GetString(getOrdinal(reader, FieldsRider.name.ToString())),
                                      getTeam(reader.GetInt16(getOrdinal(reader, FieldsRider.team.ToString()))));
                }
            }
            catch (Exception e)
            {
                // Fehler beim Lesen des Fahrers
                log.Error("SQL Fehler:", e);
            }
            finally
            {
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

            return rider;
        }*/

        /// <summary>
        /// Liest ein Team zur ID von der Datenbank
        /// </summary>
        /// <param name="id">ID des Teams</param>
        /// <returns>Das gelesene Team</returns>
        /*public Team getTeam(int id)
        {
            log.Debug("Lese Team zur ID " + id);
            
            // Wenn nötig Instanz der Datenbank besorgen
            if (instance == null)
            {
                instance = Database.getInstance();
            }

            Team team = null;

            SQLiteCommand command = new SQLiteCommand(connection);
            SQLiteDataReader reader = null;

            try
            {
                // Terrain mit über ID lesen
                command.CommandText = "SELECT * FROM team WHERE " + FieldsTeam.id + "  = '" + id + "';";

                log.Debug("SQL Befehl: " + command.CommandText);
                
                reader = command.ExecuteReader();
                reader.Read();

                // gefundene Datensätze verarbeiten
                if (reader.HasRows)
                {
                    // Es wurde was gefunden, dann Team anlegen
                    team = new Team(reader.GetInt16(getOrdinal(reader, FieldsTeam.id.ToString())),
                                    reader.GetString(getOrdinal(reader, FieldsTeam.name.ToString())));
                }
            }
            catch (Exception e)
            {
                // Fehler beim Lesen des Teams
                log.Error("SQL Fehler:", e);
            }
            finally
            {
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

            return team;
        }*/


        /// <summary>
        /// Gibt eine Liste der bekannt Gebiete zurück
        /// </summary>
        /// <returns></returns>
        /*public List<String> getTerrains()
        {
            // Wenn nötig Instanz der Datenbank besorgen
            if (instance == null)
            {
                instance = Database.getInstance();
            }

            List<String> ret = new List<String>();

            SQLiteCommand command = new SQLiteCommand(connection);
            SQLiteDataReader reader = null;
            try
            {
                // Strecken von DB lesen
                command.CommandText = "SELECT name FROM terrains;";

                log.Debug("SQL Befehl: " + command.CommandText);
                
                reader = command.ExecuteReader();

                // gefundene Datensätze verarbeiten
                while (reader.Read())
                {
                    ret.Add(reader.GetString(getOrdinal(reader, FieldsTerrain.name.ToString())));
                }
            }
            catch (Exception e)
            {
                // Fehler beim Lesen des Fahrers
                log.Error("SQL Fehler:", e);
            }
            finally
            {
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

            return ret;
        }*/

        /// <summary>
        /// Liest ein Terrain zur ID von der Datenbank
        /// </summary>
        /// <param name="id">ID des Terrains</param>
        /// <returns>Das gelesene Terrain</returns>
        /*public Terrain getTerrain(int id)
        {
            log.Debug("Lese Terrain zur ID " + id);
            
            // Wenn nötig Instanz der Datenbank besorgen
            if (instance == null)
            {
                instance = Database.getInstance();
            }

            Terrain terrain = null;
            SQLiteCommand command = new SQLiteCommand(connection);
            SQLiteDataReader reader = null;

            try
            {
                // Terrain mit mit Namen lesen
                command.CommandText = "SELECT * FROM terrains WHERE " + FieldsTerrain.id + " = '" + id + "';";

                log.Debug("SQL Befehl: " + command.CommandText);
                
                reader = command.ExecuteReader();
                reader.Read();

                // gefundene Datensätze verarbeiten
                if (reader.HasRows)
                {
                    // Es wurde was gefunden, dann Terrain anlegen
                    terrain = new Terrain(reader.GetInt16(getOrdinal(reader, FieldsTerrain.id.ToString())),
                                          reader.GetString(getOrdinal(reader, FieldsTerrain.name.ToString())));
                }
            }
            catch (Exception e)
            {
                // Fehler beim Lesen des Terrains
                log.Error("SQL Fehler:", e);
            }
            finally
            {
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

            // Terrain zurückgeben
            return terrain;
        }*/

        

        /// <summary>
        /// Liest einen Faher von der Datenbank
        /// </summary>
        /// <param name="team">Team des Fahrers</param>
        /// <param name="name">Naem des Fahrers</param>
        /// <param name="createIfNew">Soll der Fahrer neu angelegt werden wenn nicht vorhanden</param>
        /// <returns></returns>
        /*public Rider getRider(string team, string name, bool createIfNew)
        {
            log.Debug("Lese Fahrer zu " + team + " / " + name);
            
            // Wenn nötig Instanz der Datenbank besorgen
            if (instance == null)
            {
                instance = Database.getInstance();
            }

            Rider rider = null;
            SQLiteCommand command = new SQLiteCommand(connection);
            SQLiteDataReader reader = null;

            try
            {
                // Team von DB lesen
                Team dbTeam = getTeam(team, createIfNew);

                // Fahrer von DB lesen
                command.CommandText = "SELECT * FROM rider WHERE " + FieldsRider.team + "= '" + dbTeam.getId() + "' AND " +
                                                                     FieldsRider.name_uppercase + " = \"" + name.ToUpper() + "\";";

                log.Debug("SQL Befehl: " + command.CommandText);
                
                reader = command.ExecuteReader();
                reader.Read();

                // gefundene Datensätze verarbeiten
                if (reader.HasRows)
                {
                    // Es wurde was gefunden, dann Fahrer anlegen
                    rider = new Rider(reader.GetInt16(getOrdinal(reader, FieldsRider.id.ToString())),
                                      reader.GetString(getOrdinal(reader, FieldsRider.name.ToString())),
                                      dbTeam);

                }
                else if (createIfNew)
                {
                    // Fahrer gibt es noch nicht, dann neue erzeugen
                    rider = new Rider(insertRider(dbTeam, name), name, dbTeam);
                }
            }
            catch (Exception e)
            {
                // Fehler beim Lesen des Fahrers
                log.Error("SQL Fehler:", e);
            }
            finally
            {
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

            // Fahrer zurückgeben
            return rider;
        }*/

        /// <summary>
        /// Fügt einen neuen Fahrer in die Datenbank ein
        /// </summary>
        /// <param name="team">Team ds Fahrers</param>
        /// <param name="name">Name des Fahrers</param>
        /// <returns>Die ID des neuen Fahrers</returns>
        /*private int insertRider(Team team, string name)
        {
            // wenn nötig eine Instanz der DB besorgen
            if (instance == null)
            {
                instance = Database.getInstance();
            }

            // aktuell höchste ID ermitteln
            int id = 0;
            SQLiteCommand command = new SQLiteCommand(connection);
            SQLiteDataReader reader = null;

            try
            {
                // gößte ID lesen
                command.CommandText = "SELECT MAX(" + FieldsRider.id + ") FROM rider;";

                log.Debug("SQL Befehl: " + command.CommandText);
                
                reader = command.ExecuteReader();
                reader.Read();

                // es wurde etwas gefunden
                if (reader.HasRows)
                {
                    // aber kein gültiger Wert
                    if (reader.IsDBNull(0))
                    {
                        // dann ID mit null vorbelegen
                        id = 0;
                    }
                    else
                    {
                        // gefundenen ID übernehmen
                        id = reader.GetInt16(0);
                    }
                }
                else
                {
                    // es konnte keine ID ermittelt werden
                    log.Error("Nächste ID für Rider kann nicht ermittelt werden.");
                }

                reader.Dispose();
                reader.Close();

                // ID um ein erhöhren
                id++;

                // Neuen Fahrer einfügen
                command.CommandText = "INSERT INTO rider (" + FieldsRider.id + ", " +
                                                              FieldsRider.team + ", " +
                                                              FieldsRider.name + ", " +
                                                              FieldsRider.name_uppercase +
                                                        ") VALUES ('" + id + "', '" +
                                                                        team.getId() + "', \"" +
                                                                        name + "\", \"" +
                                                                        name.ToUpper() + "\");";

                log.Debug("SQL Befehl: " + command.CommandText);
                
                command.ExecuteNonQuery();

            }
            catch (Exception e)
            {
                // Parameter ist nicht vorhanden
                log.Error("SQL Fehler:", e);
            }
            finally
            {
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

            // ID des neuen Fahrers zurückgeben
            return id;
        }*/

        /// <summary>
        /// Ermittelte die Nummer einer Spalte zum SQLDataReader
        /// </summary>
        /// <param name="reader">Der SQLDataReader</param>
        /// <param name="field">Bezeichung der Spalte</param>
        /// <returns>Nummer der Spalte</returns>
        /*private int getOrdinal(SQLiteDataReader reader, string field)
        {
            int ordinal = reader.GetOrdinal(field);

            if (ordinal < 0)
            {
                throw new ArgumentOutOfRangeException("Spalte unbekannt: " + field);
            }

            return ordinal;
        }*/

        /// <summary>
        /// Liest ein Team von der Datenbank
        /// </summary>
        /// <param name="name">Name des Teams</param>
        /// <param name="createIfNew">Team neu anlegen wenn nicht vorhanden</param>
        /// <returns>Das Team</returns>
        /*private Team getTeam(string name, bool createIfNew)
        {
            log.Debug("Lese Team mit Name " + name);
            
            // Wenn nötig Instanz der Datenbank besorgen
            if (instance == null)
            {
                instance = Database.getInstance();
            }

            Team team = null;
            SQLiteCommand command = new SQLiteCommand(connection);
            SQLiteDataReader reader = null;

            try
            {
                // Team von DB lesen
                command.CommandText = "SELECT * FROM team WHERE " + FieldsTeam.name_uppercase + " = \"" + name.ToUpper() + "\";";

                log.Debug("SQL Befehl: " + command.CommandText);
                
                reader = command.ExecuteReader();
                reader.Read();

                // gefundene Datensätze verarbeiten
                if (reader.HasRows)
                {
                    // Es wurde was gefunden, dann Fahrer anlegen
                    team = new Team(reader.GetInt16(getOrdinal(reader, FieldsTeam.id.ToString())),
                                    reader.GetString(getOrdinal(reader, FieldsTeam.name.ToString())));
                }
                else if (createIfNew)
                {
                    // Team gibt es noch nicht, dann neue erzeugen
                    team = new Team(insertTeam(name), name);
                }
            }
            catch (Exception e)
            {
                // Fehler beim Lesen des Teams
                log.Error("SQL Fehler:", e);
            }
            finally
            {
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

            // Team zurückgeben
            return team;
        }*/

        /// <summary>
        /// Legt ein neues Team an
        /// </summary>
        /// <param name="name">Name des Teams</param>
        /// <returns>Die ID des neu angelegten Teams</returns>
        /*private int insertTeam(string name)
        {
            // wenn nötig eine Instanz der DB besorgen
            if (instance == null)
            {
                instance = Database.getInstance();
            }

            // aktuell höchste ID ermitteln
            int id = 0;
            SQLiteCommand command = new SQLiteCommand(connection);
            SQLiteDataReader reader = null;

            try
            {
                // gößte ID lesen
                command.CommandText = "SELECT MAX(" + FieldsTeam.id + ") FROM team;";

                log.Debug("SQL Befehl: " + command.CommandText);
                
                reader = command.ExecuteReader();
                reader.Read();

                // es wurde etwas gefunden
                if (reader.HasRows)
                {
                    // aber kein gültiger Wert
                    if (reader.IsDBNull(0))
                    {
                        // dann ID mit null vorbelegen
                        id = 0;
                    }
                    else
                    {
                        // gefundenen ID übernehmen
                        id = reader.GetInt16(0);
                    }
                }
                else
                {
                    // es konnte keine ID ermittelt werden
                    log.Error("Nächste ID für Team kann nicht ermittelt werden.");
                }

                reader.Dispose();
                reader.Close();

                // ID um ein erhöhren
                id++;

                // Neues Team einfügen
                command.CommandText = "INSERT INTO team (" + FieldsTeam.id + ", " +
                                                             FieldsTeam.name + ", " +
                                                             FieldsTeam.name_uppercase +
                                                        ") VALUES ('" + id + "', \"" +
                                                                        name + "\", \"" +
                                                                        name.ToUpper() + "\");";

                log.Debug("SQL Befehl: " + command.CommandText);
                
                command.ExecuteNonQuery();

            }
            catch (Exception e)
            {
                // Parameter ist nicht vorhanden
                log.Error("SQL Fehler:", e);
            }
            finally
            {
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

            // ID des neuen Teams zurückgeben
            return id;
        }*/

        /// <summary>
        /// Startet eine Transaktion
        /// </summary>
        /*public void beginTransaction()
        {
            if (connection != null &&
                transaction == null)
            {
                log.Debug("Starte eine Transaktion");
                
                transaction = connection.BeginTransaction();
            }
        }*/

        /// <summary>
        /// Beendet eine Transaktion mit Commit
        /// </summary>
        /*public void endTransaction()
        {

            if (transaction != null)
            {
                log.Debug("Beende eine Transaktion mit Commit");
                
                transaction.Commit();
                transaction.Dispose();
                transaction = null;
            }
        }*/

        /// <summary>
        /// Nimmt alle Änderungen an der Datenbank zurück
        /// </summary>
        /*public void rollback()
        {
            if (transaction != null)
            {
                log.Debug("Führere ein Rollback durch");
                
                transaction.Rollback();
                transaction.Dispose();
                transaction = null;
            }
        }*/

        /// <summary>
        /// Liest Statistikdaten aus der Datebank
        /// </summary>
        /// <returns>Statistik zur Datenbank</returns>
        /*public DbStat getDbStat()
        {
            log.Debug("Ermittle Datenbankstatistik");
            
            // die verschiedenen Statistikwerte ermitteln
            int countRun = selectCount("course", "" + FieldsCourse.type + " = '" + (int)Course.CourseType.Run + "'");
            int countCourse = selectCount("course", "" + FieldsCourse.type + " = '" + (int)Course.CourseType.Route + "'");
            int countTeam = selectCount("team", null);
            int countRider = selectCount("rider", null);
            int countTerrain = selectCount("terrains", null);

            // neues Objekt mit den Werten erzeugen und zurückgeben
            return new DbStat(countRun, countCourse, countTeam, countRider, countTerrain);
        }*/

        /// <summary>
        /// Liest per SELECT COUNT die Anzahl von Datensätzen zu einer Tabelle. Dabei kann, muss aber nicht,
        /// eine Bedinung übergeben werden
        /// </summary>
        /// <param name="table">Die Tabelle aus der die Anzahl ermittelt werden soll</param>
        /// <param name="condition">Bedingung mit der die Anzahl der Sätze ermittelt wird</param>
        /// <returns></returns>
        /*private int selectCount(string table, string condition)
        {
            // wenn nötig eine Instanz der DB besorgen
            if (instance == null)
            {
                instance = Database.getInstance();
            }

            SQLiteCommand command = new SQLiteCommand(connection);
            SQLiteDataReader reader = null;

            // Abfrage mit Tabelle aufbauen
            string com = "SELECT COUNT (*) FROM " + table;

            // wenn eine Bedingung übergeben wurde, Abfrage erweitern
            if (condition != null)
            {
                com += " WHERE " + condition + ";";
            }

            // Anzahl passender Datensätze lesen
            int count = 0;
            try
            {
                command.CommandText = com;

                log.Debug("SQL Befehl: " + command.CommandText);
                
                reader = command.ExecuteReader();
                reader.Read();

                if (reader.HasRows)
                {
                    // aber kein gültiger Wert
                    if (reader.IsDBNull(0))
                    {
                        count = 0;
                    }
                    else
                    {
                        count = reader.GetInt16(0);
                    }
                }
                else
                {
                    log.Error("Anzahl Datensätze in Tabelle " + table + " konnte nicht ermittelt werden.");
                }
            }
            catch (Exception e)
            {
                // Parameter ist nicht vorhanden
                log.Error("SQL Fehler:", e);
            }
            finally
            {
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

            // und Anzahl zurückmelden
            return count;
        }*/

        /// <summary>
        /// Fügt eine Strecke oder ein Ergebnis in die Datenbank ein
        /// </summary>
        /// <param name="course">Der neue Kurs</param>
        /// <returns>Konnte der Kurs eingefügt werden</returns>
        /*public bool insertCourse(Course course)
        {
            log.Debug("Füge Strecke ein: " + course.getName());
            
            // wenn nötig eine Instanz der DB besorgen
            if (instance == null)
            {
                instance = Database.getInstance();
            }

            // Team, Terrain und Fahrer sollten bereits angelegt sein
            SQLiteCommand command = new SQLiteCommand(connection);

            try
            {
                // Daten für Strecke in Tabelle einfügen
                int fin;
                if (course.isFinished())
                {
                    fin = 1;
                }
                else
                {
                    fin = 0;
                }

                // prüfen ob eine Strecke bereits vorhanden ist, dann muss sie nicht eingefügt werden
                if (courseExists(course.getHash()))
                {
                    // Strecke gibt es bereits, wenn gewünscht Meldung ins Log
                    log.Debug("Strecke bereits vorhanden: " + course.getTerrain() + ", " + course.getName());
                }
                else
                {
                    // Strecke ist noch nicht da, dann einfügen
                    command.CommandText = "INSERT INTO course (" + FieldsCourse.hash + ", " +
                                                                   FieldsCourse.terrain + ",  " +
                                                                   FieldsCourse.name + ", " +
                                                                   FieldsCourse.type + ", " +
                                                                   FieldsCourse.distance + ", " +
                                                                   FieldsCourse.duration + ", " +
                                                                   FieldsCourse.file + ", " +
                                                                   FieldsCourse.path + ", " +
                                                                   FieldsCourse.date + ", " +
                                                                   FieldsCourse.rider + ", " +
                                                                   FieldsCourse.calibration + ", " +
                                                                   FieldsCourse.avgWatt + ", " +
                                                                   FieldsCourse.avgCadence + ", " +
                                                                   FieldsCourse.avgSpeed + ", " +
                                                                   FieldsCourse.finished + ", " +
                                                                   FieldsCourse.windstrength + ", " +
                                                                   FieldsCourse.winddirection + ", " +
                                                                   FieldsCourse.name_uppercase + ", " +
                                                                   FieldsCourse.sumDecrease + ", " +
                                                                   FieldsCourse.sumIncrease + ") VALUES ('" +
                                           course.getHash() + "', '" +
                                           course.getTerrain().getId() + "', \"" +
                                           course.getName() + "\", '" +
                                           (int)course.getType() + "', '" +
                                           string.Format(cultureInfoProvider, "{0:F3}", course.getDistance()) + "', '" +
                                           TimeSpan.FromSeconds(Math.Round(course.getDuration(), MidpointRounding.AwayFromZero)).ToString() + "', \"" +
                                           course.getFile() + "\", \"" +
                                           course.getPath() + "\", '" +
                                           course.getDate().ToString("yyyy-MM-dd HH:mm:ss") + "', '" +
                                           course.getRider().getId() + "', '" +
                                           string.Format(cultureInfoProvider, "{0:F2}", course.getCalibartion()) + "', '" +
                                           string.Format(cultureInfoProvider, "{0:F3}", course.getAvgWatt()) + "', '" +
                                           string.Format(cultureInfoProvider, "{0:F3}", course.getAvgCadence()) + "', '" +
                                           string.Format(cultureInfoProvider, "{0:F3}", course.getAvgSpeed()) + "', '" +
                                           fin + "', '" +
                                           course.getWindstrength() + "', '" +
                                           course.getWinddirection() + "', \"" +
                                           course.getName().ToUpper() + "\", '" +
                                           course.getSumDecrease() + "', '" +
                                           course.getSumIncrease() + "');";

                    log.Debug("SQL Befehl: " + command.CommandText);
                    
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                // Parameter ist nicht vorhanden
                log.Error("SQL Fehler:", e);
                return false;
            }
            finally
            {
                if (command != null)
                {
                    command.Dispose();
                }
            }

            // ID des neuen Fahrers zurückgeben
            return true;
        }*/

        /*private bool courseExists(string hash)
        {
            // Wenn nötig Instanz der Datenbank besorgen
            if (instance == null)
            {
                instance = Database.getInstance();
            }

            SQLiteCommand command = new SQLiteCommand(connection);
            SQLiteDataReader reader = null;
            try
            {
                // Strecke von DB lesen
                command.CommandText = "SELECT * FROM course WHERE " + FieldsCourse.hash + " = '" + hash + "';";

                log.Debug("SQL Befehl: " + command.CommandText);
                
                reader = command.ExecuteReader();
            }
            catch (Exception e)
            {
                // Fehler beim Lesen des Fahrers
                log.Error("SQL Fehler:", e);
            }
            finally
            {
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

            return reader.HasRows;
        }*/

        /// <summary>
        /// List alle Ergebnisse ein
        /// </summary>
        /// <returns>Liste der Ergebnisse</returns>
        /*public List<Course> getRuns()
        {
            log.Debug("Lese Ergebnisse");
            
            return getCourse((int)Course.CourseType.Run);
        }*/

        /// <summary>
        /// List alle Catalyst-Programme ein
        /// </summary>
        /// <returns>Liste der Catalyst-Programme</returns>
        /*public List<Course> getPgmf()
        {
            log.Debug("Lese Catalyst-Programme");

            return getCourse((int)Course.CourseType.Pgmf);
        }*/

        /// <summary>
        /// List die alle Pilotroutes ein
        /// </summary>
        /// <returns>Eine Liste der Pilotroutes</returns>
        /*public List<Course> getPilotRoutes()
        {
            log.Debug("Lese Pilotroutes");
            
            return getCourse((int)Course.CourseType.Route);
        }*/

        /// <summary>
        /// List den gewünschten Streckentypen
        /// </summary>
        /// <param name="type">Streckentyp</param>
        /// <returns>Liste der Streckedn</returns>
        /*private List<Course> getCourse(int type)
        {
            // Wenn nötig Instanz der Datenbank besorgen
            if (instance == null)
            {
                instance = Database.getInstance();
            }

            List<Course> ret = new List<Course>();

            SQLiteCommand command = new SQLiteCommand(connection);
            SQLiteDataReader reader = null;
            try
            {
                // Strecken von DB lesen
                command.CommandText = "SELECT * FROM course WHERE " + FieldsCourse.type + " = '" + type + "';";

                log.Debug("SQL Befehl: " + command.CommandText);
                
                reader = command.ExecuteReader();

                // gefundene Datensätze verarbeiten
                while (reader.Read())
                {

                    TimeSpan duration = TimeSpan.Parse(reader.GetString(getOrdinal(reader, FieldsCourse.duration.ToString())));
                    double duration2 = duration.Seconds + duration.Minutes * 60 + duration.Hours * 3600;
                    bool fin;
                    if (reader.GetInt16(getOrdinal(reader, FieldsCourse.finished.ToString())) == 1)
                    {
                        fin = true;
                    }
                    else
                    {
                        fin = false;
                    }

                    // Es wurde was gefunden, dann Strecke anlegen
                    Course course = new Course(reader.GetString(getOrdinal(reader, FieldsCourse.hash.ToString())),
                                               getTerrain(reader.GetInt16(getOrdinal(reader, FieldsCourse.terrain.ToString()))),
                                               duration2,
                                               DateTime.Parse(reader.GetString(getOrdinal(reader, FieldsCourse.date.ToString()))),
                                               getRider(reader.GetInt16(getOrdinal(reader, FieldsCourse.rider.ToString()))),
                                               reader.GetString(getOrdinal(reader, FieldsCourse.name.ToString())),
                                               reader.GetString(getOrdinal(reader, FieldsCourse.file.ToString())),
                                               reader.GetString(getOrdinal(reader, FieldsCourse.path.ToString())),
                                               (Course.CourseType)reader.GetInt16(getOrdinal(reader, FieldsCourse.type.ToString())),
                                               reader.GetInt16(getOrdinal(reader, FieldsCourse.windstrength.ToString())),
                                               reader.GetInt16(getOrdinal(reader, FieldsCourse.winddirection.ToString())),
                                               reader.GetDouble(getOrdinal(reader, FieldsCourse.distance.ToString())),
                                               reader.GetFloat(getOrdinal(reader, FieldsCourse.calibration.ToString())),
                                               reader.GetFloat(getOrdinal(reader, FieldsCourse.avgWatt.ToString())),
                                               reader.GetFloat(getOrdinal(reader, FieldsCourse.avgCadence.ToString())),
                                               reader.GetFloat(getOrdinal(reader, FieldsCourse.avgSpeed.ToString())),
                                               fin,
                                               reader.GetInt16(getOrdinal(reader, FieldsCourse.sumDecrease.ToString())),
                                               reader.GetInt16(getOrdinal(reader, FieldsCourse.sumIncrease.ToString()))
                                               );
                    ret.Add(course);
                }
            }
            catch (Exception e)
            {
                // Fehler beim Lesen des Fahrers
                log.Error("SQL Fehler:", e);
            }
            finally
            {
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

            return ret;
        }*/

        /// <summary>
        /// Liest eine Strecke von der Datenbank
        /// </summary>
        /// <param name="hash">Schlüssel für die Strecke</param>
        /// <returns></returns>
        /*public Course getCourse(string hash)
        {
            // Wenn nötig Instanz der Datenbank besorgen
            if (instance == null)
            {
                instance = Database.getInstance();
            }

            SQLiteCommand command = new SQLiteCommand(connection);
            SQLiteDataReader reader = null;
            Course course = null;
            try
            {
                // Strecke von DB lesen
                command.CommandText = "SELECT * FROM course WHERE " + FieldsCourse.hash + " = '" + hash + "';";

                log.Debug("SQL Befehl: " + command.CommandText);
                
                reader = command.ExecuteReader();

                // gefundene Datensätze verarbeiten
                while (reader.Read())
                {

                    TimeSpan duration = TimeSpan.Parse(reader.GetString(getOrdinal(reader, FieldsCourse.duration.ToString())));
                    double duration2 = duration.Seconds + duration.Minutes * 60 + duration.Hours * 3600;
                    bool fin;
                    if (reader.GetInt16(getOrdinal(reader, FieldsCourse.finished.ToString())) == 1)
                    {
                        fin = true;
                    }
                    else
                    {
                        fin = false;
                    }

                    // Es wurde was gefunden, dann Fahrer anlegen
                    course = new Course(reader.GetString(getOrdinal(reader, FieldsCourse.hash.ToString())),
                                               getTerrain(reader.GetInt16(getOrdinal(reader, FieldsCourse.terrain.ToString()))),
                                               duration2,
                                               DateTime.Parse(reader.GetString(getOrdinal(reader, FieldsCourse.date.ToString()))),
                                               getRider(reader.GetInt16(getOrdinal(reader, FieldsCourse.rider.ToString()))),
                                               reader.GetString(getOrdinal(reader, FieldsCourse.name.ToString())),
                                               reader.GetString(getOrdinal(reader, FieldsCourse.file.ToString())),
                                               reader.GetString(getOrdinal(reader, FieldsCourse.path.ToString())),
                                               (Course.CourseType)reader.GetInt16(getOrdinal(reader, FieldsCourse.type.ToString())),
                                               reader.GetInt16(getOrdinal(reader, FieldsCourse.windstrength.ToString())),
                                               reader.GetInt16(getOrdinal(reader, FieldsCourse.winddirection.ToString())),
                                               reader.GetDouble(getOrdinal(reader, FieldsCourse.distance.ToString())),
                                               reader.GetFloat(getOrdinal(reader, FieldsCourse.calibration.ToString())),
                                               reader.GetFloat(getOrdinal(reader, FieldsCourse.avgWatt.ToString())),
                                               reader.GetFloat(getOrdinal(reader, FieldsCourse.avgCadence.ToString())),
                                               reader.GetFloat(getOrdinal(reader, FieldsCourse.avgSpeed.ToString())),
                                               fin,
                                               reader.GetInt16(getOrdinal(reader, FieldsCourse.sumDecrease.ToString())),
                                               reader.GetInt16(getOrdinal(reader, FieldsCourse.sumIncrease.ToString()))
                                        );
                }
            }
            catch (Exception e)
            {
                // Fehler beim Lesen des Fahrers
                log.Error("SQL Fehler:", e);
            }
            finally
            {
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

            return course;
        }*/

        /// <summary>
        /// Löscht einen Kurs
        /// </summary>
        /// <param name="course">Der zu löschende Kurs</param>
        /*public void deleteCourse(Course course)
        {
            log.Debug("Lösche Strecke " + course.getName());
            
            string command = "DELETE FROM course WHERE " + FieldsCourse.hash + " = \"" + course.getHash() + "\";";

            execDbCommandNoResult(command);
        }*/

        /// <summary>
        /// Liest alle vorhandenen Teams
        /// </summary>
        /// <returns>Liste der Teams</returns>
        /*private List<Team> getTeams()
        {
            // Wenn nötig Instanz der Datenbank besorgen
            if (instance == null)
            {
                instance = Database.getInstance();
            }

            List<Team> ret = new List<Team>();

            SQLiteCommand command = new SQLiteCommand(connection);
            SQLiteDataReader reader = null;
            try
            {
                // Teams von DB lesen
                command.CommandText = "SELECT * FROM team;";

                log.Debug("SQL Befehl: " + command.CommandText);
                
                reader = command.ExecuteReader();

                // gefundene Datensätze verarbeiten
                while (reader.Read())
                {
                    // Es wurde was gefunden, dann Team anlegen
                    Team team = new Team(reader.GetInt16(getOrdinal(reader, FieldsTeam.id.ToString())),
                                         reader.GetString(getOrdinal(reader, FieldsTeam.name.ToString())));
                    ret.Add(team);
                }
            }
            catch (Exception e)
            {
                // Fehler beim Lesen des Fahrers
                log.Error("SQL Fehler:", e);
            }
            finally
            {
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

            return ret;
        }*/

        /// <summary>
        /// Prüft ob zu allen Teams ein Faher vorhanden ist
        /// </summary>
        /*public void checkTeamWithoutRider()
        {
            log.Info("Prüfe Teams ohne Fahrer");

            // alle Teams lesen
            List<Team> teams = getTeams();

            // prüfen ob zu jedem Team min. ein Fahrer vorhanden ist
            foreach (Team team in teams)
            {
                SQLiteCommand command = new SQLiteCommand(connection);
                SQLiteDataReader reader = null;
                try
                {
                    // Teams von DB lesen
                    command.CommandText = "SELECT COUNT(*) FROM rider WHERE " + FieldsRider.team + " = '" + team.getId() + "';";

                    log.Debug("SQL Befehl: " + command.CommandText);
                    
                    reader = command.ExecuteReader();

                    // gefundene Datensätze verarbeiten
                    reader.Read();

                    bool hasRiders = false;

                    if (reader.HasRows)
                    {
                        // prüfen ob Fahrer gefunden wurden
                        if (reader.GetInt16(0) > 0)
                        {
                            hasRiders = true;
                        }
                    }

                    // wenn es keine Fahrer gibt Team löschen
                    if (!hasRiders)
                    {
                        deleteTeam(team);
                    }
                }
                catch (Exception e)
                {
                    // Fehler beim Lesen des Fahrers
                    log.Error("SQL Fehler:", e);
                }
                finally
                {
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
            }
        }*/

        /// <summary>
        /// Löscht ein Team
        /// </summary>
        /// <param name="team">Das zu löschende Team</param>
        /*public void deleteTeam(Team team)
        {
            log.Debug("Lösche Team " + team.getName());
            
            string command = "DELETE FROM team WHERE " + FieldsTeam.id + " = '" + team.getId() + "';";

            execDbCommandNoResult(command);
        }*/

        /// <summary>
        /// Prüft auf Fahrer ohne Strecken
        /// </summary>
        /*public void checkRiderWithoutCourse()
        {
            log.Info("Prüfe Fahrer ohne Strecke");

            // alle Fahrer lesen
            List<Rider> riders = getRiders();

            // prüfen ob zu jedem Team min. ein Fahrer vorhanden ist
            foreach (Rider rider in riders)
            {
                SQLiteCommand command = new SQLiteCommand(connection);
                SQLiteDataReader reader = null;
                try
                {
                    // Fahrer von DB lesen
                    command.CommandText = "SELECT COUNT(*) FROM course WHERE " + FieldsCourse.rider + " = '" + rider.getId() + "';";

                    log.Debug("SQL Befehl: " + command.CommandText);
                    
                    reader = command.ExecuteReader();

                    // gefundene Datensätze verarbeiten
                    reader.Read();

                    bool hasCourse = false;

                    if (reader.HasRows)
                    {
                        // prüfen ob Strecken gefunden wurden
                        if (reader.GetInt16(0) > 0)
                        {
                            hasCourse = true;
                        }
                    }

                    // wenn es keine Strecke gibt Fahrer löschen
                    if (!hasCourse)
                    {
                        deleteRider(rider);
                    }
                }
                catch (Exception e)
                {
                    // Fehler beim Lesen des Fahrers
                    log.Error("SQL Fehler:", e);
                }
                finally
                {
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
            }
        }*/

        /// <summary>
        /// Löscht einen Fahrer
        /// </summary>
        /// <param name="rider">Der zu löschende Fahrer</param>
        /*public void deleteRider(Rider rider)
        {
            log.Debug("Lösche Fahrer " + rider.getName());
            
            string command = "DELETE FROM rider WHERE " + FieldsRider.id + " = \"" + rider.getId() + "\";";

            execDbCommandNoResult(command);
        }*/

        /// <summary>
        /// Liest alle Fahrer
        /// </summary>
        /// <returns>Eine Liste der Fahrer</returns>
        /*private List<Rider> getRiders()
        {
            // Wenn nötig Instanz der Datenbank besorgen
            if (instance == null)
            {
                instance = Database.getInstance();
            }

            List<Rider> ret = new List<Rider>();

            SQLiteCommand command = new SQLiteCommand(connection);
            SQLiteDataReader reader = null;
            try
            {
                // Fahrer von DB lesen
                command.CommandText = "SELECT * FROM rider;";

                log.Debug("SQL Befehl: " + command.CommandText);
                
                reader = command.ExecuteReader();

                // gefundene Datensätze verarbeiten
                while (reader.Read())
                {
                    // Es wurde was gefunden, dann Fahrer anlegen
                    Rider rider = new Rider(reader.GetInt16(getOrdinal(reader, FieldsRider.id.ToString())),
                                            reader.GetString(getOrdinal(reader, FieldsRider.name.ToString())),
                                            getTeam(reader.GetInt16(getOrdinal(reader, FieldsRider.team.ToString()))));
                    ret.Add(rider);
                }
            }
            catch (Exception e)
            {
                // Fehler beim Lesen des Fahrers
                log.Error("SQL Fehler:", e);
            }
            finally
            {
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

            return ret;
        }*/

        /// <summary>
        /// Prüft auf Fahrer ohne Team
        /// </summary>
        /*public void checkRiderWithoutTeam()
        {
            log.Info("Prüfe Fahrer ohne Team");

            // alle Fahrer lesen
            List<Rider> riders = getRiders();

            // prüfen ob zu jedem Fahrer ein Team vorhanden ist
            foreach (Rider rider in riders)
            {
                SQLiteCommand command = new SQLiteCommand(connection);
                SQLiteDataReader reader = null;
                try
                {
                    bool hasTeam = false;

                    if (rider.getTeam() != null)
                    {
                        // Team von DB lesen
                        command.CommandText = "SELECT COUNT(*) FROM team WHERE " + FieldsTeam.id + " = \"" + rider.getTeam().getId() + "\";";

                        log.Debug("SQL Befehl: " + command.CommandText);
                        
                        reader = command.ExecuteReader();

                        // gefundene Datensätze verarbeiten
                        reader.Read();

                        if (reader.HasRows)
                        {
                            // prüfen ob ein Team gefunden wurden
                            if (reader.GetInt16(0) > 0)
                            {
                                hasTeam = true;
                            }
                        }
                    }
                    // wenn es kein Team gibt, Team hinzufügen
                    if (!hasTeam)
                    {
                        Team team = getTeam("Tacx", true);
                        rider.setTeam(team);
                        updaeRider(rider);
                    }
                }
                catch (Exception e)
                {
                    // Fehler beim Lesen des Fahrers
                    log.Error("SQL Fehler:", e);
                }
                finally
                {
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
            }
        }*/

        /// <summary>
        /// Setzt neue Werte für einen Fahrer
        /// </summary>
        /// <param name="rider">Der Fahrer</param>
        /*public void updaeRider(Rider rider)
        {
            // wenn nötig eine Instanz der DB besorgen
            if (instance == null)
            {
                instance = Database.getInstance();
            }

            SQLiteCommand command = new SQLiteCommand(connection);

            try
            {
                // Datensatz updaten
                command.CommandText = "UPDATE rider SET " + FieldsRider.team + " = \"" + rider.getTeam().getId() + "\", " +
                                                            FieldsRider.name + " = \"" + rider.getName() + "\", " +
                                                            FieldsRider.name_uppercase + " = \"" + rider.getName().ToUpper() + "\" " +
                                                   "WHERE " + FieldsRider.id + " = \"" + rider.getId() + "\";";

                log.Debug("SQL Befehl: " + command.CommandText);
                
                command.ExecuteNonQuery();

            }
            catch (Exception e)
            {
                // Parameter ist nicht vorhanden
                log.Error("SQL Fehler:", e);
            }
            finally
            {
                if (command != null)
                {
                    command.Dispose();
                }
            }
        }*/

        /// <summary>
        /// Gibt eine Tabelle der Strecken zurück
        /// </summary>
        /// <returns>Tabelle mit Strecken</returns>
        /*public DataTable getPilotRoutesDataTable()
        {
            // wenn nötig eine Instanz der DB besorgen
            if (instance == null)
            {
                instance = Database.getInstance();
            }

            // Strecken von DB lesen
            string sql = "SELECT * FROM v_pilotroutes;";
            SQLiteCommand cmd = new SQLiteCommand(sql, connection);
            SQLiteDataAdapter da = new SQLiteDataAdapter(cmd);
            DataSet ds = new DataSet();
            DataTable dt = null;
            try
            {
                da.Fill(ds);
                dt = ds.Tables[0];
            }
            catch (Exception e)
            {
                log.Error(e);
            }

            // Spalte mit Windstärke als Text hinzufügen
            addColumnWindStrength(dt);

            // Spalte mit Windrichtung als Text hinzufügen
            addColumnWindDirection(dt);

            // Spalte mit Anzahl der Egebnisse hinzufügen
            addColumnRuns(dt);

            return dt;
        }*/

        /// <summary>
        /// Gibt eine Tabelle der Ergenissen zurück
        /// </summary>
        /// <returns>Tabelle mit Ergebnissen</returns>
        /*public DataTable getResultsDataTable(Terrain terrain, string name)
        {
            // wenn nötig eine Instanz der DB besorgen
            if (instance == null)
            {
                instance = Database.getInstance();
            }

            // Strecken von DB lesen
            string sql = "SELECT * FROM v_runs WHERE " + FieldsViewRun.terrain + " = \"" + terrain.getName() + "\" AND " + 
                                                         FieldsViewRun.course + " = \"" + name + "\";";
            SQLiteCommand cmd = new SQLiteCommand(sql, connection);
            SQLiteDataAdapter da = new SQLiteDataAdapter(cmd);
            DataSet ds = new DataSet();
            DataTable dt = null;
            try
            {
                da.Fill(ds);
                dt = ds.Tables[0];
            }
            catch (Exception e)
            {
                log.Error(e);
            }

            // Spalte mit Windstärke als Text hinzufügen
            addColumnWindStrength(dt);

            // Spalte mit Windrichtung als Text hinzufügen
            addColumnWindDirection(dt);

            return dt;
        }*/

        /// <summary>
        /// Fügt der Tabelle eine Spalte mit der Anzahl der Ergebnisse hinzu
        /// </summary>
        /// <param name="dt">Tabelle mit den Strecken</param>
        /*private void addColumnRuns(DataTable dt)
        {
            // Spalte hinzufügen
            string col = Enum.GetName(typeof(FieldsViewPilotAdd), 0);
            dt.Columns.Add(col, typeof(int));

            // Zeilen besorgen
            DataRowCollection rows = dt.Rows;

            // Wenn nötig Instanz der Datenbank besorgen
            if (instance == null)
            {
                instance = Database.getInstance();
            }

            // alle Zeilen bearbeiten
            foreach (DataRow row in rows)
            {
                SQLiteCommand command = new SQLiteCommand(connection);
                SQLiteDataReader reader = null;
                try
                {
                    // Anzahl der Ergebnisse ermitteln
                    command.CommandText = "SELECT COUNT(*) FROM course " +
                                                          "WHERE " + FieldsCourse.terrain + " = (SELECT " + FieldsTerrain.id + " FROM terrains " +
                                                                                              "WHERE " + FieldsTerrain.name + " = \"" + row[Enum.GetName(typeof(FieldsViewPilot), 1)] + "\") AND " +
                                                                     FieldsCourse.name + " = \"" + row[Enum.GetName(typeof(FieldsViewPilot), 2)] + "\" AND " +
                                                                     FieldsCourse.type + " = \"" + (int)Course.CourseType.Run + "\";";

                    log.Debug("SQL Befehl: " + command.CommandText);
                    
                    reader = command.ExecuteReader();

                    // Anzahl ermitteln
                    while (reader.Read())
                    {
                        row[col] = reader.GetInt16(0);
                        break;
                    }
                }
                catch (Exception e)
                {
                    // Fehler beim Lesen des Fahrers
                    log.Error("SQL Fehler:", e);
                }
                finally
                {
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
            }
        }*/

        /// <summary>
        /// Fügt der Tabelle eine Spalte mir der Windrichtung als Text hinzu
        /// </summary>
        /// <param name="dataTable">Tabelle mit den Strecken</param>
        /*private void addColumnWindDirection(DataTable dataTable)
        {
            // Spalte hinzufügen
            string col = Enum.GetName(typeof(FieldsViewPilotAdd), 1);
            dataTable.Columns.Add(col, typeof(string));

            // Zeilen besorgen
            DataRowCollection rows = dataTable.Rows;

            // alle Zeilen bearbeiten
            foreach (DataRow row in rows)
            {
                // Winkelwert in Text umsetzen
                row[col] = TranslateWindDirection.text(Convert.ToInt16(row[Enum.GetName(typeof(FieldsViewPilot), 12)].ToString()));
            }
        }*/

        /// <summary>
        /// Fügt eine Spalte mit der Windstärke als Text ein
        /// </summary>
        /// <param name="dataTable">Table mit den Ergebnissen</param>
        /*private void addColumnWindStrength(DataTable dataTable)
        {
            // Spalte einfügen
            string col = Enum.GetName(typeof(FieldsViewPilotAdd), 2);
            dataTable.Columns.Add(col, typeof(string));

            // Zeilen holen
            DataRowCollection rows = dataTable.Rows;

            // alle Zeilen bearbeiten
            foreach (DataRow row in rows)
            {
                // Zahlenwert in Text umsetzen
                row[col] = TranslateWindStrength.text(row[Enum.GetName(typeof(FieldsViewPilot), 11)].ToString());
            }
        }*/

        /*public void checkCourseWithoutRider()
        {
            throw new NotImplementedException();
        }*/
    }
}
