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
using System.Diagnostics;
using System.Windows.Forms;

namespace MiBandImport
{
    static class performCMD
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Program));

        public static bool execute(string cmd, bool showError = false, bool showOutput = false, int timeout = 999999)
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
                    log.Debug("Prozess wird gestartet, Timeout in ms " + timeout);
                    process.WaitForExit(timeout);
                    log.Debug("Prozess wurde beendet");

                    if (!process.HasExited)
                    {
                        log.Error("Prozess wurde NICHT korrekt beendet");
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
                            log.Error("Fehler Kommando : " + error);

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
    }
}
