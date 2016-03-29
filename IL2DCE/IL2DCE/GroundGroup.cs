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

using System;
using System.Collections.Generic;

using maddox.game;
using maddox.GP;

namespace IL2DCE
{
    public enum EGroundGroupCountry
    {
        nn,
        gb,
        de,
    }

    public enum EGroundGroupType
    {
        Vehicle,
        Armor,
        Ship,
    }

    public class GroundGroup
    {
        public GroundGroup(ISectionFile sectionFile, string id)
        {
            _id = id;

            string value = sectionFile.get("Chiefs", id);

            // Class
            Class = value.Substring(0, value.IndexOf(" "));
            value = value.Remove(0, Class.Length + 1);

            // Army
            Country = (EGroundGroupCountry)Enum.Parse(typeof(EGroundGroupCountry), value.Substring(0, 2));
            value = value.Remove(0, 2);

            // Options
            Options = value.Trim();

            // Waypoints
            GroundGroupWaypoint lastWaypoint = null;
            for (int i = 0; i < sectionFile.lines(id + "_Road"); i++)
            {
                string key;
                sectionFile.get(id + "_Road", i, out key, out value);
                                
                if (!key.Contains("S"))
                {
                    GroundGroupWaypoint waypoint = new GroundGroupWaypoint(sectionFile, id, i);
                    lastWaypoint = waypoint;
                    Waypoints.Add(waypoint);
                }
                else if (key.Contains("S"))
                {
                    if (lastWaypoint != null)
                    {
                        GroundGroupSubWaypoint subWaypoint = new GroundGroupSubWaypoint(sectionFile, id, i);
                        lastWaypoint.SubWaypoints.Add(subWaypoint);
                    }
                }
            }

            if (Waypoints.Count > 0)
            {
                Position = new Point3d(Waypoints[0].X, Waypoints[0].Y, Waypoints[0].Z);
            }
        }

        public EGroundGroupType Type
        {
            get
            {
                // Type
                if (Class.StartsWith("Vehicle"))
                {
                    return EGroundGroupType.Vehicle;
                }
                else if (Class.StartsWith("Armor"))
                {
                    return EGroundGroupType.Armor;
                }
                else if (Class.StartsWith("Ship"))
                {
                    return EGroundGroupType.Ship;
                }
                else
                {
                    throw new System.FormatException("Unknown EType of GroundGroup");
                }
            }
        }

        public Point3d Position
        {
            get
            {
                return _position;
            }
            set
            {
                _position = value;
            }
        }
        private Point3d _position;

        public string Id
        {
            get
            {
                return _id;
            }
        }
        public string _id;

        public string Class
        {
            get;
            set;
        }

        public EGroundGroupCountry Country
        {
            get;
            set;
        }

        public int Army
        {
            get
            {
                if (Country == EGroundGroupCountry.gb)
                {
                    return 1;
                }
                else if (Country == EGroundGroupCountry.de)
                {
                    return 2;
                }
                else
                {
                    return 0;
                }
            }
        }

        public string Options
        {
            get;
            set;
        }

        public List<GroundGroupWaypoint> Waypoints
        {
            get
            {
                return _waypoints;
            }
        }
        private List<GroundGroupWaypoint> _waypoints = new List<GroundGroupWaypoint>();

        public void WriteTo(ISectionFile sectionFile)
        {
            if (Waypoints.Count > 1)
            {
                sectionFile.add("Chiefs", Id, Class + " " + Country.ToString() + " " + Options);
                double? _lastV = null;
                for (int i = 0; i < Waypoints.Count - 1; i++)
                {
                    if (Waypoints[i].V.HasValue)
                    {
                        _lastV = Waypoints[i].V.Value;
                        sectionFile.add(Id + "_Road", Waypoints[i].X.ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat), Waypoints[i].Y.ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat) + " " + Waypoints[i].Z.ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat) + "  0 " + (Waypoints[i].SubWaypoints.Count + 2).ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat) + " " + Waypoints[i].V.Value.ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat));
                        foreach (GroundGroupSubWaypoint subWaypoint in Waypoints[i].SubWaypoints)
                        {
                            if (subWaypoint.P.HasValue)
                            {
                                sectionFile.add(Id + "_Road", "S", subWaypoint.S + " P " + subWaypoint.P.Value.x.ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat) + " " + subWaypoint.P.Value.y.ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat));
                            }
                            else
                            {
                                sectionFile.add(Id + "_Road", "S", subWaypoint.S);
                            }
                        }
                    }
                    else if (_lastV != null && _lastV.HasValue)
                    {
                        sectionFile.add(Id + "_Road", Waypoints[i].X.ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat), Waypoints[i].Y.ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat) + " " + Waypoints[i].Z.ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat) + "  0 " + (Waypoints[i].SubWaypoints.Count + 2).ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat) + " " + _lastV.Value.ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat));
                        foreach (GroundGroupSubWaypoint subWaypoint in Waypoints[i].SubWaypoints)
                        {
                            if (subWaypoint.P.HasValue)
                            {
                                sectionFile.add(Id + "_Road", "S", subWaypoint.S + " P " + subWaypoint.P.Value.x.ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat) + " " + subWaypoint.P.Value.y.ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat));
                            }
                            else
                            {
                                sectionFile.add(Id + "_Road", "S", subWaypoint.S);
                            }
                        }
                    }
                }

                sectionFile.add(Id + "_Road", Waypoints[Waypoints.Count - 1].X.ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat), Waypoints[Waypoints.Count - 1].Y.ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat) + " " + Waypoints[Waypoints.Count - 1].Z.ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat));
            }
        }
    }
}