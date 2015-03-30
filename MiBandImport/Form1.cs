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
using MiBandImport.data;
using MiBandImport.DataPanels;
using MiBandImport.EventArgsClasses;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace MiBandImport
{
    public partial class Form1 : Form
    {
        public delegate void SleepDurationChangedEventHandler(object sender, EventArgsSleepDurationChanged duration);
        public delegate void ShowSpanChangedEventHandler(object sender, EventArgsDaysToDispay days);
        public delegate void SelectedDayChangedEventHandler(object sender, EventArgsSelectedDayChanged data);

        public event SleepDurationChangedEventHandler sleepDurationChanged;
        public event ShowSpanChangedEventHandler showSpanChanged;
        public event SelectedDayChangedEventHandler selectectRowChanged;

        protected static readonly ILog log = LogManager.GetLogger(typeof(Program));

        private MiBand.MiBand miband;

        private PanelDetail1 panelDetail1;
        private PanelGeneralTab panelGeneralTab;
        private PanelGeneralGraphSteps panelGeneralGraphSteps;
        private PanelGeneralGraphSleep panelGeneralGraphSleep;
        private PanelDayDetail panelDayDetail;

        private string pathDBshort = ".\\db\\";
        private string pathDB = Application.StartupPath + "\\db\\";
        private string pathLib = Application.StartupPath + "\\lib\\";

        /// <summary>
        /// Konstruktor 
        /// </summary>
        public Form1()
        {
            InitializeComponent();

            // Programmtitel um Version erweitern
            this.Text = this.Text + " " + Application.ProductVersion;
            
            // Fensterstatus wiederherstellen
            this.WindowState = (FormWindowState)Properties.Settings.Default.WindowState;

            // Position wiederherstellen
            this.DesktopLocation = new Point(Properties.Settings.Default.PosX, Properties.Settings.Default.PosY);
            this.Size = new Size(Properties.Settings.Default.SizeWidth, Properties.Settings.Default.SizeHight);

            // Einstellungen übernehmen
            radioButtonRoot.Checked = Properties.Settings.Default.Root;
            radioButtonNoRoot.Checked = !Properties.Settings.Default.Root;
            textBoxWorkDirPhone.Text = Properties.Settings.Default.WorkDirOnPhone;

            // Schlafdauer zurückholen
            maskedTextBoxSleepDur.Text = Properties.Settings.Default.SleepDuration;

            // Panles für die Daten erzeugen
            initDataPanles();            
        }


        /// <summary>
        /// Initialisiert die einzelnen Tabs
        /// </summary>
        private void initDataPanles()
        {
            // Erfasste Schlafzeit in eine Zeitspanne umsetzen
            var timeSpanSleep = new TimeSpan(Convert.ToInt16(maskedTextBoxSleepDur.Text.Substring(0, 2)),
                                             Convert.ToInt16(maskedTextBoxSleepDur.Text.Substring(3, 2)),
                                             0);

            // Tab mit den Details erzeugen wenn noch nicht geschehen
            if (panelDetail1 == null)
            {
                panelDetail1 = new PanelDetail1();
                tabPageUserData.Controls.Add(panelDetail1);
                panelDetail1.addListener();
            }

            // Panel hinzufügen
            panelDetail1.setData(PanelDetail1.DataType.Detail, miband, timeSpanSleep, dateTimePickerShowFrom.Value, dateTimePickerShowTo.Value);

            //Tab mit den allg. Daten erzeugen wenn noch nicht geschehen
            if (panelGeneralTab == null)
            {
                panelGeneralTab = new PanelGeneralTab();
                tabPageOriginTab.Controls.Add(panelGeneralTab);
                panelGeneralTab.addListener();

                // wenn ein Tag ausgewählt wird, wollen wir das wissen
                panelGeneralTab.selectectRowChanged += new PanelGeneralTab.SelectedDayChangedEventHandler(OnDayChanged);
            }

            // Daten anzeigen
            panelGeneralTab.setData(PanelDetail1.DataType.Global, miband, timeSpanSleep, dateTimePickerShowFrom.Value, dateTimePickerShowTo.Value);

            // Tab mit der Grafik für die Schritte erzeugen wenn noch nicht geschehen
            if (panelGeneralGraphSteps == null)
            {
                panelGeneralGraphSteps = new PanelGeneralGraphSteps();
                tabPageOriginGraphSteps.Controls.Add(panelGeneralGraphSteps);
                panelGeneralGraphSteps.addListener();
            }

            // Daten anzeigen
            panelGeneralGraphSteps.setData(PanelDetail1.DataType.Global, miband, timeSpanSleep, dateTimePickerShowFrom.Value, dateTimePickerShowTo.Value);

            // Tab mit der Grafik mit der Schlafdauer
            if (panelGeneralGraphSleep == null)
            {
                panelGeneralGraphSleep = new PanelGeneralGraphSleep();
                tabPageOriginGraphSleep.Controls.Add(panelGeneralGraphSleep);
                panelGeneralGraphSleep.addListener();
            }

            // Panel hinzufügen
            panelGeneralGraphSleep.setData(PanelDetail1.DataType.Global, miband, timeSpanSleep, dateTimePickerShowFrom.Value, dateTimePickerShowTo.Value);

            // Tab mit Tagesdetails
            if (panelDayDetail == null)
            {
                panelDayDetail = new PanelDayDetail();
                tabPageDayDetail.Controls.Add(panelDayDetail);
                panelDayDetail.addListener();
            }
        }

        /// <summary>
        /// Gibt eine geänderte Auswahl für den Tag weiter
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        private void OnDayChanged(object sender, EventArgsClasses.EventArgsSelectedDayChanged data)
        {
            // Gibt es Zuhörer
            if (selectectRowChanged != null)
            {
                // ja, dann benachrichtigen
                selectectRowChanged(this, data);
            }
        }

        /// <summary>
        /// Drucktaste zum Neulesen der Daten vom SmartPhone
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonRead_Click(object sender, EventArgs e)
        {
            // Datenbanken von SmartPhone lesen
            if (readDbFromPhone())
            {
                // Rohdaten konnten gelesen werden, dann für Anzeige konvertieren
                readData();
            }
        }

        /// <summary>
        /// Liest die Datenbanken vom SmartPhone lesen
        /// </summary>
        private bool readDbFromPhone()
        {
            // Lesen mit Root-Rechten ?
            if (radioButtonNoRoot.Checked == false)
            {
                // mit Root-Rechten lesen
                return readAsRoot();
            }
            else
            {
                // ohne Root
                return readNonRoot();
            }
        }

        /// <summary>
        /// Liest die Datenbanken per Backup-Funktion ohne Root-Rechte
        /// </summary>
        private bool readNonRoot()
        {
            // Hinweis ausgeben das Meldung anzeigen das Backup auf dem SmartPhone
            // bestätigt werden muss
            if (DialogResult.OK == MessageBox.Show(Properties.Resources.BackupBestätigen,
                                                   Properties.Resources.AktionErforderlich,
                                                   MessageBoxButtons.OKCancel,
                                                   MessageBoxIcon.Information))
            {
                // Backup auf Smartphone erstellen und auf PC ablegen
                adbCommand("backup -f " + pathDB + "mi.ab -noapk -noshared com.xiaomi.hm.health");
                performCMD(pathLib + "tail -c +25 " + pathDB + "mi.ab > " + pathDB + "mi.zlb");

                // Datenbanken extrahieren
                performCMD(pathLib + "deflate d " + pathDB + "mi.zlb " + pathDB + "mi.tar");
                performCMD(pathLib + "tar xf " + pathDBshort + "mi.tar apps/com.xiaomi.hm.health/db/origin_db*");
                performCMD(pathLib + "tar xf " + pathDBshort + "mi.tar apps/com.xiaomi.hm.health/db/user-db*");

                // Datenbanken in Arbeitsverzeichnis kopieren
                performCMD("copy /Y apps\\com.xiaomi.hm.health\\db\\* db\\.");

                // aufräumen
                Directory.Delete(Application.StartupPath + "\\apps", true);
                File.Delete(pathDB + "mi.ab");
                File.Delete(pathDB + "mi.zlb");
                File.Delete(pathDB + "mi.tar");

                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Führt ein Shell-Kommando aus
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="showError"></param>
        /// <param name="showOutput"></param>
        private bool performCMD(string cmd, bool showError = false, bool showOutput = false, int timeout = 99999)
        {
            log.Debug("Kommando wird ausgeführt: " + cmd);

            // Prozess vorbereiten
            ProcessStartInfo psi = new ProcessStartInfo();
            psi.CreateNoWindow = true;
            psi.UseShellExecute = false;

            psi.FileName = "CMD.exe";

            psi.Arguments = "/c " + cmd;

            psi.ErrorDialog = true;
            psi.WorkingDirectory = Application.StartupPath;
            psi.WindowStyle = ProcessWindowStyle.Hidden;
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardError = true;

            // Prozess ausführen
            using (Process process = Process.Start(psi))
            {
                try
                {
                    // auf Beenden des Kommandos warten
                    process.WaitForExit(timeout);

                    if (!process.HasExited)
                    {
                        return false;
                    }

                    // Exit-Code ermitteln
                    var exitCode = process.ExitCode;

                    log.Debug("Ergebnis Kommando : " + exitCode);

                    // wenn das Programm ohne Fehlercode beendet wurde
                    if (exitCode == 0)
                    {
                        // Ausgabe des Programms holen
                        var output = process.StandardOutput.ReadToEnd();

                        // Wenn was ermittelt wurde und auch ausgegeben werden soll
                        if (output.Length > 0)
                        {
                            log.Debug("Ausgabe Kommando : " + output);

                            if (showOutput)
                            {
                                // Meldung ausgeben
                                MessageBox.Show(output, 
                                                Properties.Resources.ErgebnisAktion, 
                                                MessageBoxButtons.OK,
                                                MessageBoxIcon.Information);
                                
                            }
                        }

                        return true;
                    }
                    else
                    {
                        // Programm wurde mit Fehlercode beendet
                        var error = process.StandardError.ReadToEnd();

                        // Wenn was ermittelt wurde und auch ausgegeben werden soll
                        if (error.Length > 0)
                        {
                            log.Debug("Fehler Kommando : " + error);

                            if (showError)
                            {
                                // Meldung ausgeben
                                MessageBox.Show(error, 
                                                Properties.Resources.FehlerMsg, 
                                                MessageBoxButtons.OK, 
                                                MessageBoxIcon.Error);
                            }
                        }

                        return false;
                    }
                }
                catch (Exception ex)
                {
                    // richtiger Fehler aufgetreten, ausgeben und ab ins Log
                    MessageBox.Show(ex.Message,
                                    Properties.Resources.FehlerMsg, 
                                    MessageBoxButtons.OK, 
                                    MessageBoxIcon.Error);

                    log.Error(ex.Message);
                    log.Error(ex.StackTrace);

                    return false;
                }
            }
        }

        /// <summary>
        /// Datenbanken vom SmartPhone mit Root-Rechten lesen
        /// </summary>
        private bool readAsRoot()
        {
            string workDir = textBoxWorkDirPhone.Text;

            // erfasstes Verzeichnis prüfen
            if (!checkWorkDirPhone())
            {
                // nein, dann Meldung und abbrechen
                MessageBox.Show(Properties.Resources.FehlerWorkDirPhone,
                                Properties.Resources.FehlerMsg,
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                return false;
            }
            
            // prüfen ob Smartphone erreichbar ist
            if (!phoneIsAvabile())
            {
                // nein, dann Meldung und abbrechen
                MessageBox.Show(Properties.Resources.FehlerPhoneNotAvabile,
                                Properties.Resources.FehlerMsg,
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                return false;
            }


            //adbCommand("kill-server");

            // Zwischenverzeichnis zur Sicherheit anlegen
            adbCommand("shell \"mkdir " + workDir + "\"", Properties.Settings.Default.PhoneTimeOut);

            // Datenbank innerhalb des SmartPhones kopieren
            adbCommand("shell \"su -c 'cp /data/data/com.xiaomi.hm.health/databases/origin_db " + workDir + "'\"", Properties.Settings.Default.PhoneTimeOut);
            adbCommand("shell \"su -c 'cp /data/data/com.xiaomi.hm.health/databases/origin_db-journal " + workDir + "'\"", Properties.Settings.Default.PhoneTimeOut);
            adbCommand("shell \"su -c 'cp /data/data/com.xiaomi.hm.health/databases/user-db " + workDir + "'\"", Properties.Settings.Default.PhoneTimeOut);
            adbCommand("shell \"su -c 'cp /data/data/com.xiaomi.hm.health/databases/user-db-journal " + workDir + "'\"", Properties.Settings.Default.PhoneTimeOut);

            // Daten vom Phone auf Rechner kopieren
            adbCommand("pull " + workDir + "origin_db  " + pathDB + "origin_db", Properties.Settings.Default.PhoneTimeOut);
            adbCommand("pull " + workDir + "origin_db-journal " + pathDB + "origin_db-journal", Properties.Settings.Default.PhoneTimeOut);
            adbCommand("pull " + workDir + "user-db  " + pathDB + "user-db", Properties.Settings.Default.PhoneTimeOut);
            adbCommand("pull " + workDir + "user-db-journal " + pathDB + "user-db-journal", Properties.Settings.Default.PhoneTimeOut);

            // alles anscheinend ohne Fehler verlaufen
            return true;
        }

        /// <summary>
        /// Prüft ob das Smartphone erreichbar ist
        /// </summary>
        /// <returns></returns>
        private bool phoneIsAvabile()
        {
            return adbCommand("shell \"su -c 'ls'\"", Properties.Settings.Default.PhoneTimeOut);
        }

        /// <summary>
        /// Überprüft ob das Arbeitsverzeichnis auf dem Smartphone vorhanden ist
        /// </summary>
        /// <returns></returns>
        private bool checkWorkDirPhone()
        {
            return true;
        }

        /// <summary>
        /// Daten aus den Datenbanken einlesen
        /// </summary>
        private void readData()
        {
            // Daten einlesen
            try
            {
                miband = new MiBand.MiBand(pathDB);
                miband.read();
            }
            catch (Exception ex)
            {
                // Fehler ist aufgetreten, anzeigen und ab ins Log
                MessageBox.Show(ex.Message,
                                Properties.Resources.FehlerMsg,
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);

                log.Error(ex.Message);
                log.Error(ex.StackTrace);
            }

            // Anzeige Ab-Datum ermitteln und setzen
            DateTime last = DateTime.Now;
            foreach (MiBandData data in miband.data)
            {
                if (data.date < last)
                {
                    last = data.date;
                }
            }
            dateTimePickerShowFrom.Value = last;

            // wenn nötig Panles für Daten initialisieren
            initDataPanles();
        }

        /// <summary>
        /// führt ein Kommando über ADB ausführen
        /// </summary>
        /// <param name="arg"></param>
        private bool adbCommand(string arg, int timeout = 99999)
        {
            return performCMD(pathLib + "adb " + arg, timeout: timeout);
        }

        /// <summary>
        /// Es sollen bereits vom SmartPhone eingelesene Daten anzeigen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonShowOldData_Click(object sender, EventArgs e)
        {
            readData();
        }

        /// <summary>
        /// Führt Aktionen beim Schließen des Fensters aus
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Zugriffsmodus speichern
            Properties.Settings.Default.Root = radioButtonRoot.Checked;

            // Fensterposition speichern
            Properties.Settings.Default.SizeHight = this.Size.Height;
            Properties.Settings.Default.SizeWidth = this.Size.Width;
            Properties.Settings.Default.PosY = this.DesktopLocation.Y;
            Properties.Settings.Default.PosX = this.DesktopLocation.X;

            // Anzeigestaus Fenster speichern
            Properties.Settings.Default.WindowState = (int)this.WindowState;

            // Schlafdauer speichern
            Properties.Settings.Default.SleepDuration = maskedTextBoxSleepDur.Text;

            // Arbeitsverzeichnis auf dem Smartphone
            Properties.Settings.Default.WorkDirOnPhone = textBoxWorkDirPhone.Text;


            // Einstellungen speichern
            Properties.Settings.Default.Save();

            log.Info("Anwendung wird beendet");
        }

        /// <summary>
        /// Text in Feld für die Schlafdauer wurde geändert
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void maskedTextBoxSleepDur_TextChanged(object sender, EventArgs e)
        {
            // wenn die Eingabe das passende Format hat
            if (sleepDurationChanged != null &&
                maskedTextBoxSleepDur.Text.Length == 5)
            {
                // Eingabe in eine Zeitspanne umwandeln
                var timeSpanSleep = new TimeSpan(Convert.ToInt16(maskedTextBoxSleepDur.Text.Substring(0, 2)),
                                                 Convert.ToInt16(maskedTextBoxSleepDur.Text.Substring(3, 2)),
                                                 0);

                // und die geänderte Schlafdauer an alle mitteilen die es wissen wollen
                if (sleepDurationChanged != null)
                {
                    EventArgsSleepDurationChanged args = new EventArgsSleepDurationChanged();
                    args.SleepDuration = timeSpanSleep;
                    sleepDurationChanged(this, args);
                }
            }
        }

        /// <summary>
        /// Zeitpunkt ab dem die Daten angezeigt werden soll wurde geändert
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dateTimePickerShowFrom_ValueChanged(object sender, EventArgs e)
        {
            // Aktuellen Wert an alle verteilen die es wissen wollen
            if (showSpanChanged != null)
            {
                EventArgsDaysToDispay args = new EventArgsDaysToDispay();
                args.DisplayFrom = dateTimePickerShowFrom.Value;
                args.DisplayTo = dateTimePickerShowTo.Value;
                showSpanChanged(this, args);
            }
        }
    }
}
