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
using log4net.Appender;
using log4net.Config;
using log4net.Core;
using log4net.Layout;
using log4net.Repository.Hierarchy;
using System;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace MiBandImport
{
    static class Program
    {
        static readonly ILog log = LogManager.GetLogger(typeof(Program));

        /// <summary>
        /// Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //Thread.CurrentThread.CurrentUICulture = new CultureInfo("en");

            // Logger initialisieren
            try
            {
                initLogger();
                log.Info("Programm gestartet ---------------------------------");
                log.Info("Version: " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.StackTrace);
                Application.Exit();
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }

        /// <summary>
        /// Das Log initialisieren
        /// </summary>
        private static void initLogger()
        {
            BasicConfigurator.Configure();
            Logger l = (Logger)log.Logger;
            FileAppender appender = new FileAppender();
            string logname = Application.ProductName + ".log";
            appender.Name = logname;

            appender.File = Path.Combine(Application.StartupPath, logname);
            appender.AppendToFile = true;
            PatternLayout layout = new PatternLayout();
            layout.ConversionPattern = "%-5p%d{yyyy-MM-dd HH:mm:ss} - %m%n";
            layout.ActivateOptions();
            appender.Layout = layout;
            appender.ActivateOptions();
            l.AddAppender(appender);

            setLogLevel();
        }

        /// <summary>
        /// Setzt die Stufe für das Logging
        /// </summary>
        public static void setLogLevel()
        {
            switch (Properties.Settings.Default.LogLevel)
            {
                case 0:
                    LogManager.GetRepository().Threshold = Level.Info;
                    break;
                case 1:
                    LogManager.GetRepository().Threshold = Level.Error;
                    break;
                case 2:
                    LogManager.GetRepository().Threshold = Level.Debug;
                    break;
            }

            log.Info("LogLevel wurde gesetzt: " + LogManager.GetRepository().Threshold.ToString());
        }
    }
}
