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

namespace IL2DCE
{
    public class Waterway
    {
        public Waterway(ISectionFile sectionFile, string id)
        {
            // Waypoints
            GroundGroupWaypoint lastWaypoint = null;
            for (int i = 0; i < sectionFile.lines(id + "_Road"); i++)
            {
                string key;
                string value;
                sectionFile.get(id + "_Road", i, out key, out value);

                GroundGroupWaypoint waypoint = null;
                if (!key.Contains("S"))
                {
                    waypoint = new GroundGroupWaypointLine(sectionFile, id, i);
                }
                else if (key.Contains("S"))
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

        public List<GroundGroupWaypoint> Waypoints
        {
            get
            {
                return _waypoints;
            }
        }
        private List<GroundGroupWaypoint> _waypoints = new List<GroundGroupWaypoint>();

        public GroundGroupWaypoint Start
        {
            get
            {
                return Waypoints[0];
            }
        }

        public GroundGroupWaypoint End
        {
            get
            {
                return Waypoints[Waypoints.Count - 1];
            }
        }
    }
}