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

using Google.Apis.Auth.OAuth2;
using Google.Apis.Fitness.v1;
using Google.Apis.Util.Store;
using log4net;
using System.IO;
using System.Threading;
using System.Windows.Forms;


namespace MiBandImport.GoogleFit
{
    class GoogleAuth
    {
        protected static readonly ILog log = LogManager.GetLogger(typeof(Program));

        public bool isAuthenticated { get; private set; }

        public GoogleAuth()
        {
            isAuthenticated = false;

            UserCredential credential;
            string path = Path.Combine(Application.StartupPath, "GoogleFit", "client_secret.json");

            using (var stream = new FileStream(path, FileMode.Open,
                                    FileAccess.Read))
            {
                try
                {
                    //GoogleWebAuthorizationBroker.Folder = "Tasks.Auth.Store";
                    credential = GoogleWebAuthorizationBroker.AuthorizeAsync(GoogleClientSecrets.Load(stream).Secrets,
                                                                              new[] { FitnessService.Scope.FitnessActivityRead,
                                                                                  FitnessService.Scope.FitnessActivityWrite },
                                                                              "user",
                                                                              CancellationToken.None,
                                                                              new FileDataStore("Drive.Auth.Store")).Result;

                    isAuthenticated = true;
                }
                catch (System.AggregateException )
                {
                    log.Error("Anwender hat Zugriff auf Gogle-Fit nicht erlaubt.");
                }

                
            }
        }
    }
}
