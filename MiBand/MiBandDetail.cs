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
using System.Collections.Generic;
using System.Resources;

namespace MiBand
{
    public class MiBandDetail
    {
        public static double weight_in_kg { get { return getWeight(); } set { setWeight(value); } }
        public static double height_in_cm { get { return getHeight(); } set { setHeight(value); } }
        
        public DateTime dateTime { get; set; }
        public int category { get; set; }
        public int intensity { get; set; }
        public int steps { get; set; }
        public int mode { get { return getMode(); } private set { _mode = value; } }
        public double runDistance { get { return getRunDistance(); } private set { _runDistance = value; } }
        public double runCalories { get { return getRunCalories(); } private set { _runCalories = value; } }
        public double walkDistance { get { return getWalkDistance(); } private set { _walkDistance = value; } }
        public string discription { get { return getDescription(); } private set { _description = value; } }
        public double walkCalories { get { return getWalkCalories(); } private set { _walkCalories = value; } }

        private static double _weight_in_kg;
        private static double _height_in_cm;
        private double _walkDistance;
        private double _walkCalories;
        private int _mode;
        private double _runDistance;
        private double _runCalories;
        private int step;
        private int runs;
        private int activity;
        private bool isModeCalculated = false;
        private bool isRunDistanceCalculated = false;
        private bool isWalkDistanceCalculated = false;
        private bool isWalkCaloriesCalculated = false;
        private string _description = string.Empty;

        private static List<double> calList = new List<double>();
        private static List<double> calList2 = new List<double>();

        /// <summary>
        /// Statischer Konstruktor
        /// </summary>
        static MiBandDetail()
        {
            // Parameterliste zur Berechnung der Kalorien Teil 1
            calList.Add(40.233d);
            calList.Add(53.645d);
            calList.Add(67.056d);
            calList.Add(80.467d);
            calList.Add(93.878d);
            calList.Add(107.29d);
            calList.Add(120.7d);
            calList.Add(134.11d);
            calList.Add(160.94d);
            calList.Add(187.76d);
            calList.Add(214.58d);
            calList.Add(241.4d);
            calList.Add(268.23d);
            calList.Add(295.05d);
            calList.Add(321.87d);
            calList.Add(348.69d);
            calList.Add(375.52d);

            // Parameterliste zur Berechnung der Kalorien Teil 1
            calList2.Add(0.95d);
            calList2.Add(1.19d);
            calList2.Add(1.41d);
            calList2.Add(1.57d);
            calList2.Add(1.78d);
            calList2.Add(2.36d);
            calList2.Add(2.97d);
            calList2.Add(3.79d);
            calList2.Add(4.67d);
            calList2.Add(5.24d);
            calList2.Add(5.62d);
            calList2.Add(6.1d);
            calList2.Add(6.91d);
            calList2.Add(7.62d);
            calList2.Add(9.05d);
            calList2.Add(9.43d);
            calList2.Add(10.95d);
        }

        /// <summary>
        /// Ermittelt die beim Gehen verbrauchten Kalorien
        /// </summary>
        /// <returns></returns>
        private double getWalkCalories()
        {
            if (!isWalkDistanceCalculated)
            {
                getWalkDistance();
            }

            double comp = -1d;
            int index = 0;
            while (comp <= walkDistance &&
                   index < calList.Count)
            {
                comp = calList[index];
                index++;
            }
            index--;

            _walkCalories = (_weight_in_kg * 2.2046 * walkDistance * calList2[index]) / (60 * calList[index]);

            isWalkCaloriesCalculated = true;

            return _walkCalories;
        }

        /// <summary>
        /// Ermittelt die Aktivität
        /// </summary>
        /// <returns></returns>
        private int getMode()
        {
            if (category == 0x7f)
            {
                _mode = category;
            }
            else
            {
                _mode = category & 0xf;
                if (steps > 0)
                {
                    runs = category >> 4;
                }
                activity = intensity;
                step = steps;
                isModeCalculated = true;
            }

            return _mode;
        }

        /// <summary>
        /// Ermittelt die zurückgelegte Gehen-Strecke
        /// </summary>
        /// <returns></returns>
        private double getWalkDistance()
        {
            if (!isWalkDistanceCalculated)
            {
                if (!isModeCalculated)
                {
                    getMode();
                }

                if (step == 0)
                {
                    _walkDistance = 0;
                }
                else
                {
                    double h1 = _height_in_cm * 0.42d / 100d;
                    if (steps > 0 &&
                        category != 4 &&
                        category != 5)
                        if (steps <= 90)
                        {
                            _walkDistance = steps * h1 * 0.9;
                        }
                        else
                        {
                            double q;
                            if (steps > 120)
                            {
                                q = 125;
                            }
                            else
                            {
                                q = 100;
                            }
                            _walkDistance = steps * h1 / q;
                        }
                    else
                    {
                        _walkDistance = 0;
                    }
                }
                isWalkDistanceCalculated = true;
            }

            return _walkDistance;
        }

        /// <summary>
        /// Ermittelt die zurückgelegte Laufstrecke
        /// </summary>
        /// <returns></returns>
        private double getRunDistance()
        {
            if (!isRunDistanceCalculated)
            {
                if (!isWalkDistanceCalculated)
                {
                    getWalkDistance();
                }

                double runs;
                if (steps > 0)
                {
                    runs = category >> 4;
                }
                else
                {
                    runs = 0;
                }
                _runDistance = (((runs * 2) + 3) * _walkDistance) / 15;

                isRunDistanceCalculated = true;
            }
            return _runDistance;
        }

        /// <summary>
        /// Ermittelt die beim Laufen verbrauchten Kalorien
        /// </summary>
        /// <returns></returns>
        private double getRunCalories()
        {
            if (_runCalories == 0)
            {
                if (!isRunDistanceCalculated)
                {
                    getRunDistance();
                }

                if (!isWalkCaloriesCalculated)
                {
                    getWalkCalories();
                }

                _runCalories = (3d + runs * 2d) / 15d * _walkCalories;
            }

            return _runCalories;

        }

        /// <summary>
        /// Gibt eine Beschreibung für die Aktivität zurück
        /// </summary>
        /// <returns></returns>
        private string getDescription()
        {
            if (category == 4 ||
                category == 5)
            {
                _description = Properties.Resources.Sleep;
            }
            else
            {
                if (category > 15 &&
                    steps > 0)
                {
                    _description = Properties.Resources.Run;
                }
                else
                {
                    if (category > 0 &&
                        steps > 0)
                    {
                        _description = Properties.Resources.Walk;
                    }
                    else
                    {
                        _description = Properties.Resources.Idle;
                    }
                }
            }
            return _description;
        }


        public void resetMarker()
        {
            isModeCalculated = false;
            isRunDistanceCalculated = false;
            isWalkDistanceCalculated = false;
            isWalkCaloriesCalculated = false;
        }

        private static void setHeight(double value)
        {
            _height_in_cm = value;

            //getRunDistance();
        }


        private static void setWeight(double value)
        {
            _weight_in_kg = value;

            //getWaldColories();
        }


        private static double getWeight()
        {
            return _weight_in_kg;
        }

        private static double getHeight()
        {
            return _height_in_cm;
        }
    }
}
