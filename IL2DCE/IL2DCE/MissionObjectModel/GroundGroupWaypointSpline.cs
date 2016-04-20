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

namespace IL2DCE
{
    public class GroundGroupWaypointSpline : GroundGroupWaypoint
    {
        // Example: S 503 91 0.61 5.00 P 360207.22 223055.25  0 2 11.11
        // S ? ? ? ? P X Y  0 SubCount V
        private System.Text.RegularExpressions.Regex waypointLong = new System.Text.RegularExpressions.Regex(@"^([0-9]+) ([0-9]+) ([0-9]+[.0-9]*) ([-+]?[0-9]+[.0-9]*) P ([0-9]+[.0-9]*) ([0-9]+[.0-9]*)  ([0-9]+) ([0-9]+) ([0-9]+[.0-9]*)$");
        // Example: S 503 79 0.00 -1330.00 P 354764.81 223866.00
        // S ? ? ? ? P X Y
        private System.Text.RegularExpressions.Regex waypointShort = new System.Text.RegularExpressions.Regex(@"^([0-9]+) ([0-9]+) ([0-9]+[.0-9]*) ([-+]?[0-9]+[.0-9]*) P ([0-9]+[.0-9]*) ([0-9]+[.0-9]*)$");

        public override bool IsSubWaypoint(ISectionFile sectionFile, string id, int line)
        {
            string key;
            string value;
            sectionFile.get(id + "_Road", line, out key, out value);

            if (waypointShort.IsMatch(value) && !waypointLong.IsMatch(value))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #region Public constructors

        public GroundGroupWaypointSpline(ISectionFile sectionFile, string id, int line)
        {
            string key;
            string value;
            sectionFile.get(id + "_Road", line, out key, out value);
                        
            if (waypointLong.IsMatch(value))
            {
                System.Text.RegularExpressions.Match match = waypointLong.Match(value);

                if (match.Groups.Count == 10)
                {
                    _s = match.Groups[1].Value + " " + match.Groups[2].Value + " " + match.Groups[3].Value + " " + match.Groups[4].Value;

                    double x;
                    if(double.TryParse(match.Groups[5].Value, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture.NumberFormat, out x))
                    {
                        X = x;
                    }
                    double y;
                    if(double.TryParse(match.Groups[6].Value, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture.NumberFormat, out y))
                    {
                        Y = y;
                    }

                    double v;
                    if(double.TryParse(match.Groups[9].Value, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture.NumberFormat, out v))
                    {
                        V = v;
                    }
                }
            }
            else if (waypointShort.IsMatch(value))
            {
                System.Text.RegularExpressions.Match match = waypointShort.Match(value);

                if (match.Groups.Count == 7)
                {
                    _s = match.Groups[1].Value + " " + match.Groups[2].Value + " " + match.Groups[3].Value + " " + match.Groups[4].Value;

                    double x;
                    if(double.TryParse(match.Groups[5].Value, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture.NumberFormat, out x))
                    {
                        X = x;
                    }
                    double y;
                    if(double.TryParse(match.Groups[6].Value, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture.NumberFormat, out y))
                    {
                        Y = y;
                    }
                }
            }
        }

        #endregion

        #region Public properties

        public string S
        {
            get
            {
                return _s;
            }
        }
        private string _s;
        

        #endregion
    }
}