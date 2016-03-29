// IL2DCE: A dynamic campaign engine for IL-2 Sturmovik: Cliffs of Dover
// Copyright (C) 2016 Stefan Rothdach
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as
// published by the Free Software Foundation, either version 3 of the
// License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Affero General Public License for more details.
//
// You should have received a copy of the GNU Affero General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.


using maddox.game;

namespace IL2DCE
{
    public class Config
    {
        public Config(ISectionFile confFile)
        {
            SpawnParked = false;
            if (confFile.exist("Core", "forceSetOnPark"))
            {
                string value = confFile.get("Core", "forceSetOnPark");
                if (value == "1")
                {
                    SpawnParked = true;
                }
                else
                {
                    SpawnParked = false;
                }
            }

            _additionalAirOperations = 0;
            if (confFile.exist("Core", "additionalAirOperations"))
            {
                string value = confFile.get("Core", "additionalAirOperations");
                int.TryParse(value, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture.NumberFormat, out _additionalAirOperations);
            }

            _additionalGroundOperations = 0;
            if (confFile.exist("Core", "additionalGroundOperations"))
            {
                string value = confFile.get("Core", "additionalGroundOperations");
                int.TryParse(value, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture.NumberFormat, out _additionalGroundOperations);
            }

            _flightSize = 1.0;
            if (confFile.exist("Core", "flightSize"))
            {
                string value = confFile.get("Core", "flightSize");
                double.TryParse(value, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture.NumberFormat, out _flightSize);
            }

            _flightCount = 1.0;
            if (confFile.exist("Core", "flightCount"))
            {
                string value = confFile.get("Core", "flightCount");
                double.TryParse(value, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture.NumberFormat, out _flightCount);
            }

            _debug = 0;
            if (confFile.exist("Core", "debug"))
            {
                string value = confFile.get("Core", "debug");
                int.TryParse(value, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture.NumberFormat, out _debug);
            }

            if (confFile.exist("Main", "campaignsFolder"))
            {
                string _campaignsFolder = confFile.get("Main", "campaignsFolder");
            }
        }

        public int AdditionalAirOperations
        {
            get
            {
                return _additionalAirOperations;
            }
        }
        
        public int AdditionalGroundOperations
        {
            get
            {
                return _additionalGroundOperations;
            }
        }
        
        public double FlightSize
        {
            get
            {
                return _flightSize;
            }

        }
        
        public double FlightCount
        {
            get
            {
                return _flightCount;
            }
        }
        
        public bool SpawnParked
        {
            get
            {
                return _spawnParked;
            }
            set
            {
                _spawnParked = value;
            }
        }

        public int Debug
        {
            get
            {
                return _debug;
            }
            set
            {
                _debug = value;
            }
        }
        
        public string CampaignsFolder
        {
            get
            {
                return _campaignsFolder;
            }
        }

        private int _additionalAirOperations = 0;
        private int _additionalGroundOperations = 0;
        private double _flightSize = 1.0;
        private double _flightCount = 1.0;
        public static bool _spawnParked = false;
        private int _debug = 0;
        private string _campaignsFolder = "$home/parts/IL2DCE/Campaigns";
    }
}
