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
        Train,
    }

    public class GroundGroup
    {
        public GroundGroup(string id, string @class, EGroundGroupCountry country, string options, List<GroundGroupWaypoint> waypoints = null)
        {
            _id = id;

            Class = @class;
            Country = country;
            Options = options;

            if(waypoints != null && waypoints.Count > 0)
            {
                // Copy the items in list, not the list itself.
                Waypoints.AddRange(waypoints);                
            }
        }

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

                GroundGroupWaypoint waypoint = null;
                if(!key.Contains("S"))
                {
                    waypoint = new GroundGroupWaypointLine(sectionFile, id, i);
                }
                else if(key.Contains("S"))
                {
                    waypoint = new GroundGroupWaypointSpline(sectionFile, id, i);
                }
                
                // Check if it's a subwaypoint or the last waypoint (which looks like a subwaypoint but is none).
                if (waypoint.IsSubWaypoint(sectionFile, id, i) && i < sectionFile.lines(id + "_Road") - 1)
                {
                    if (lastWaypoint != null)
                    {
                        lastWaypoint.SubWaypoints.Add(waypoint);
                    }
                    else
                    {
                        throw new FormatException();
                    }
                }
                else
                {   
                    Waypoints.Add(waypoint);
                    lastWaypoint = waypoint;
                }
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
                else if (Class.StartsWith("Train"))
                {
                    return EGroundGroupType.Train;
                }
                else
                {
                    throw new System.FormatException("Unknown EType of GroundGroup");
                }
            }
        }

        public Point2d? Position
        {
            get
            {
                if(Waypoints.Count > 0)
                {
                    return new Point2d(Waypoints[0].X, Waypoints[0].Y);
                }
                else
                {
                    return null;
                }
            }
        }

        public string Id
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value;
            }
        }

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

        public void WriteTo(ISectionFile sectionFile)
        {
            if (Waypoints.Count > 1)
            {
                sectionFile.add("Chiefs", Id, Class + " " + Country.ToString() + " " + Options);
                // Write all waypoints except for the last one.
                for (int i = 0; i < Waypoints.Count - 1; i++)
                {
                    if (Waypoints[i] is GroundGroupWaypointLine)
                    {
                        if (Waypoints[i].V.HasValue)
                        {
                            sectionFile.add(Id + "_Road", Waypoints[i].X.ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat), Waypoints[i].Y.ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat) + " " + (Waypoints[i] as GroundGroupWaypointLine).Z.ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat) + "  0 " + (Waypoints[i].SubWaypoints.Count + 2).ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat) + " " + Waypoints[i].V.Value.ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat));
                        }
                    }
                    else if (Waypoints[i] is GroundGroupWaypointSpline)
                    {
                        if (Waypoints[i].V.HasValue)
                        {
                            sectionFile.add(Id + "_Road", "S", (Waypoints[i] as GroundGroupWaypointSpline).S + " P " + Waypoints[i].X.ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat) + " " + Waypoints[i].Y.ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat) + "  0 " + (Waypoints[i].SubWaypoints.Count + 2).ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat) + " " + Waypoints[i].V.Value.ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat));
                        }
                    }

                    foreach (GroundGroupWaypoint subWaypoint in Waypoints[i].SubWaypoints)
                    {
                        if (subWaypoint is GroundGroupWaypointLine)
                        {
                            sectionFile.add(Id + "_Road", subWaypoint.X.ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat), subWaypoint.Y.ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat) + " " + (subWaypoint as GroundGroupWaypointLine).Z.ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat));
                        }
                        else if (subWaypoint is GroundGroupWaypointSpline)
                        {
                            sectionFile.add(Id + "_Road", "S", (subWaypoint as GroundGroupWaypointSpline).S + " P " + subWaypoint.X.ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat) + " " + subWaypoint.Y.ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat));
                        }
                    }
                }

                // For the last waypoint don't write the subwaypoint count and the speed.
                if (Waypoints[Waypoints.Count - 1] is GroundGroupWaypointLine)
                {
                    sectionFile.add(Id + "_Road", Waypoints[Waypoints.Count - 1].X.ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat), Waypoints[Waypoints.Count - 1].Y.ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat) + " " + (Waypoints[Waypoints.Count - 1] as GroundGroupWaypointLine).Z.ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat));
                }
                else if (Waypoints[Waypoints.Count - 1] is GroundGroupWaypointSpline)
                {
                    sectionFile.add(Id + "_Road", "S", (Waypoints[Waypoints.Count - 1] as GroundGroupWaypointSpline).S + " P " + Waypoints[Waypoints.Count - 1].X.ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat) + " " + Waypoints[Waypoints.Count - 1].Y.ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat));
                }
            }
        }

        private List<GroundGroupWaypoint> _waypoints = new List<GroundGroupWaypoint>();
        private string _id;
    }
}
