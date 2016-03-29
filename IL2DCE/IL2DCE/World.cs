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
using maddox.GP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IL2DCE
{
    public class World
    {
        public World(ISectionFile templateFile, CampaignInfo campaignInfo)
        {
            _roads.Clear();
            _waterways.Clear();

            _redFrontMarkers.Clear();
            _blueFrontMarkers.Clear();
            _redAirGroups.Clear();
            _blueAirGroups.Clear();
            _redGroundGroups.Clear();
            _blueGroundGroups.Clear();

            _redStationaries.Clear();
            _blueStationaries.Clear();

            for (int i = 0; i < templateFile.lines("Stationary"); i++)
            {
                string key;
                string value;
                templateFile.get("Stationary", i, out key, out value);

                // Radar
                string[] valueParts = value.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (valueParts.Length > 4)
                {
                    if (valueParts[0] == "Stationary.Radar.EnglishRadar1")
                    {
                        double x;
                        double y;
                        double.TryParse(valueParts[2], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture.NumberFormat, out x);
                        double.TryParse(valueParts[3], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture.NumberFormat, out y);
                        Stationary radar = new Stationary(key, x, y);
                        _redStationaries.Add(radar);
                    }
                }
            }

            for (int i = 0; i < templateFile.lines("FrontMarker"); i++)
            {
                string key;
                string value;
                templateFile.get("FrontMarker", i, out key, out value);

                string[] valueParts = value.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (valueParts.Length == 3)
                {
                    double x;
                    double y;
                    int army;
                    if (double.TryParse(valueParts[0], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture.NumberFormat, out x)
                        && double.TryParse(valueParts[1], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture.NumberFormat, out y)
                        && int.TryParse(valueParts[2], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture.NumberFormat, out army))
                    {
                        if (army == 0)
                        {
                            _neutralFrontMarkers.Add(new Point3d(x, y, 0.0));
                        }
                        else if (army == 1)
                        {
                            _redFrontMarkers.Add(new Point3d(x, y, 0.0));
                        }
                        else if (army == 2)
                        {
                            _blueFrontMarkers.Add(new Point3d(x, y, 0.0));
                        }
                    }
                }
            }


            for (int i = 0; i < templateFile.lines("AirGroups"); i++)
            {
                string key;
                string value;
                templateFile.get("AirGroups", i, out key, out value);

                AirGroup airGroup = new AirGroup(templateFile, key, campaignInfo);

                if (AirGroupInfo.GetAirGroupInfo(1, airGroup.AirGroupKey) != null)
                {
                    _redAirGroups.Add(airGroup);
                }
                else if (AirGroupInfo.GetAirGroupInfo(2, airGroup.AirGroupKey) != null)
                {
                    _blueAirGroups.Add(airGroup);
                }
            }


            for (int i = 0; i < templateFile.lines("Chiefs"); i++)
            {
                string key;
                string value;
                templateFile.get("Chiefs", i, out key, out value);

                GroundGroup groundGroup = new GroundGroup(templateFile, key);

                if (groundGroup.Army == 1)
                {
                    _redGroundGroups.Add(groundGroup);
                }
                else if (groundGroup.Army == 2)
                {
                    _blueGroundGroups.Add(groundGroup);
                }
                else
                {
                    Waterway road = new Waterway(templateFile, key);
                    if (value.StartsWith("Vehicle") || value.StartsWith("Armor"))
                    {
                        _roads.Add(road);
                    }
                    else if (value.StartsWith("Ship"))
                    {
                        _waterways.Add(road);
                    }
                }
            }
        }

        public IList<Waterway> Roads
        {
            get
            {
                return _roads;
            }
        }

        public IList<Waterway> Waterways
        {
            get
            {
                return _waterways;
            }
        }
        
        public IList<Point3d> RedFrontMarkers
        {
            get
            {
                return _redFrontMarkers;
            }
        }

        public IList<Point3d> BlueFrontMarkers
        {
            get
            {
                return _blueFrontMarkers;
            }
        }
        
        public IList<AirGroup> AirGroups
        {
            get
            {
                List<AirGroup> airGroups = new List<AirGroup>();
                airGroups.AddRange(_redAirGroups);
                airGroups.AddRange(_blueAirGroups);
                return airGroups;
            }
        }

        public IList<AirGroup> RedAirGroups
        {
            get
            {
                List<AirGroup> airGroups = new List<AirGroup>();
                airGroups.AddRange(_redAirGroups);
                return airGroups;
            }
        }

        public IList<AirGroup> BlueAirGroups
        {
            get
            {
                List<AirGroup> airGroups = new List<AirGroup>();
                airGroups.AddRange(_blueAirGroups);
                return airGroups;
            }
        }

        public IList<GroundGroup> GroundGroups
        {
            get
            {
                List<GroundGroup> groundGroups = new List<GroundGroup>();
                groundGroups.AddRange(_redGroundGroups);
                groundGroups.AddRange(_blueGroundGroups);
                return groundGroups;
            }
        }

        public IList<GroundGroup> RedGroundGroups
        {
            get
            {
                List<GroundGroup> groundGroups = new List<GroundGroup>();
                groundGroups.AddRange(_redGroundGroups);
                return groundGroups;
            }
        }

        public IList<GroundGroup> BlueGroundGroups
        {
            get
            {
                List<GroundGroup> groundGroups = new List<GroundGroup>();
                groundGroups.AddRange(_blueGroundGroups);
                return groundGroups;
            }
        }
        
        public IList<GroundGroup> GetGroundGroups(int armyIndex)
        {
            if (armyIndex == 1)
            {
                return _redGroundGroups;
            }
            else if (armyIndex == 2)
            {
                return _blueGroundGroups;
            }
            else
            {
                return new List<GroundGroup>();
            }
        }

        public IList<AirGroup> GetAirGroups(int armyIndex)
        {
            if (armyIndex == 1)
            {
                return _redAirGroups;
            }
            else if (armyIndex == 2)
            {
                return _blueAirGroups;
            }
            else
            {
                return new List<AirGroup>();
            }
        }

        public IList<Stationary> GetFriendlyRadars(int armyIndex)
        {
            if (armyIndex == 1)
            {
                return _redStationaries;
            }
            else if (armyIndex == 2)
            {
                return _blueStationaries;
            }
            else
            {
                return new List<Stationary>();
            }
        }

        public IList<Stationary> GetEnemyRadars(int armyIndex)
        {
            if (armyIndex == 1)
            {
                return _blueStationaries;
            }
            else if (armyIndex == 2)
            {
                return _redStationaries;
            }
            else
            {
                return new List<Stationary>();
            }
        }

        public IList<Point3d> GetFriendlyMarkers(int armyIndex)
        {
            if (armyIndex == 1)
            {
                return _redFrontMarkers;
            }
            else if (armyIndex == 2)
            {
                return _blueFrontMarkers;
            }
            else
            {
                return new List<Point3d>();
            }
        }

        public IList<Point3d> GetEnemyMarkers(int armyIndex)
        {
            if (armyIndex == 1)
            {
                return _blueFrontMarkers;
            }
            else if (armyIndex == 2)
            {
                return _redFrontMarkers;
            }
            else
            {
                return new List<Point3d>();
            }
        }

        private List<Point3d> _redFrontMarkers = new List<Point3d>();
        private List<Point3d> _blueFrontMarkers = new List<Point3d>();
        private List<Point3d> _neutralFrontMarkers = new List<Point3d>();
        
        private List<Waterway> _roads = new List<Waterway>();
        private List<Waterway> _waterways = new List<Waterway>();

        private List<AirGroup> _redAirGroups = new List<AirGroup>();
        private List<AirGroup> _blueAirGroups = new List<AirGroup>();

        private List<GroundGroup> _redGroundGroups = new List<GroundGroup>();
        private List<GroundGroup> _blueGroundGroups = new List<GroundGroup>();

        private List<Stationary> _redStationaries = new List<Stationary>();
        private List<Stationary> _blueStationaries = new List<Stationary>();

    }
}
