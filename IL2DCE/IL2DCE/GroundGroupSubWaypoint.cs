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
    public class GroundGroupSubWaypoint
    {
        #region Public constructors

        public GroundGroupSubWaypoint(string s, Point2d? p)
        {
            _s = s;
            _p = p;
        }

        public GroundGroupSubWaypoint(ISectionFile sectionFile, string id, int line)
        {
            string key;
            string value;
            sectionFile.get(id + "_Road", line, out key, out value);

            System.Text.RegularExpressions.Regex subWaypointLong = new System.Text.RegularExpressions.Regex(@"^([0-9]+) ([0-9]+) ([0-9]+[.0-9]*) ([0-9]+[.0-9]*) P ([0-9]+[.0-9]*) ([0-9]+[.0-9]*)$");
            System.Text.RegularExpressions.Regex subWaypointShort = new System.Text.RegularExpressions.Regex(@"^([0-9]+) ([0-9]+) ([0-9]+[.0-9]*) ([0-9]+[.0-9]*)$");

            if (subWaypointLong.IsMatch(value))
            {
                System.Text.RegularExpressions.Match match = subWaypointLong.Match(value);

                if (match.Groups.Count == 7)
                {
                    _s = match.Groups[1].Value + " " + match.Groups[2].Value + " " + match.Groups[3].Value + " " + match.Groups[4].Value;

                    double x;
                    double y;
                    double.TryParse(match.Groups[5].Value, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture.NumberFormat, out x);
                    double.TryParse(match.Groups[6].Value, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture.NumberFormat, out y);
                    _p = new Point2d(x, y);
                }
            }
            if (subWaypointLong.IsMatch(value))
            {
                System.Text.RegularExpressions.Match match = subWaypointLong.Match(value);

                if (match.Groups.Count == 5)
                {
                    _s = match.Groups[1].Value + " " + match.Groups[2].Value + " " + match.Groups[3].Value + " " + match.Groups[4].Value;
                    _p = null;
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

        public Point2d? P
        {
            get
            {
                return _p;
            }
        }
        private Point2d? _p;

        #endregion
    }
}