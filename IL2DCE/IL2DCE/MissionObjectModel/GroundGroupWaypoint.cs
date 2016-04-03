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

using System.Collections.Generic;

using maddox.game;
using maddox.GP;

namespace IL2DCE
{
    public class GroundGroupWaypoint
    {
        #region Public constructors

        public GroundGroupWaypoint(Point3d position, double v)
        {
            X = position.x;
            Y = position.y;
            Z = position.z;
            V = v;
        }

        public GroundGroupWaypoint(double x, double y, double z, double v)
        {
            X = x;
            Y = y;
            Z = z;
            V = v;
        }

        public GroundGroupWaypoint(ISectionFile sectionFile, string id, int line)
        {
            string key;
            string value;
            sectionFile.get(id + "_Road", line, out key, out value);

            System.Text.RegularExpressions.Regex waypointLong = new System.Text.RegularExpressions.Regex(@"^([0-9]+[.0-9]*) ([0-9]+[.0-9]*)  ([0-9]+) ([0-9]+) ([0-9]+[.0-9]*)$");
            System.Text.RegularExpressions.Regex waypointShort = new System.Text.RegularExpressions.Regex(@"^([0-9]+[.0-9]*) ([0-9]+[.0-9]*)$");

            if (waypointLong.IsMatch(value))
            {
                System.Text.RegularExpressions.Match match = waypointLong.Match(value);

                if(match.Groups.Count == 6)
                {
                    double.TryParse(key, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture.NumberFormat, out X);
                    double.TryParse(match.Groups[1].Value, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture.NumberFormat, out Y);
                    double.TryParse(match.Groups[2].Value, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture.NumberFormat, out Z);

                    double v;
                    if (double.TryParse(match.Groups[5].Value, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture.NumberFormat, out v))
                    {
                        V = v;
                    }
                }
            }
            else if(waypointShort.IsMatch(value))
            {
                System.Text.RegularExpressions.Match match = waypointShort.Match(value);

                if (match.Groups.Count == 3)
                {
                    double.TryParse(key, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture.NumberFormat, out X);
                    double.TryParse(match.Groups[1].Value, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture.NumberFormat, out Y);
                    double.TryParse(match.Groups[2].Value, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture.NumberFormat, out Z);
                }
            }
        }

        #endregion

        #region Public properties

        public List<GroundGroupSubWaypoint> SubWaypoints
        {
            get
            {
                return _subWaypoints;
            }
        }
        private List<GroundGroupSubWaypoint> _subWaypoints = new List<GroundGroupSubWaypoint>();

        public Point3d Position
        {
            get
            {
                return new Point3d(X, Y, Z);
            }
        }

        public double X;

        public double Y;

        public double Z;
        
        public double? V
        {
            get
            {
                return _v;
            }
            set
            {
                _v = value;
            }
        }
        public double? _v;

        #endregion
    }
}