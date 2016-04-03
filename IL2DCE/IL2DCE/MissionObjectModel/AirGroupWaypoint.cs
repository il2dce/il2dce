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
using System.Collections;
using System.Collections.Generic;

using maddox.game;
using maddox.game.world;
using maddox.GP;

namespace IL2DCE
{

    public class AirGroupWaypoint
    {
        # region Public enums

        public enum AirGroupWaypointTypes
        {
            NORMFLY,
            TAKEOFF,
            LANDING,
            COVER,
            HUNTING,
            RECON,
            GATTACK_POINT,
            GATTACK_TARG,
            ESCORT,
            AATTACK_FIGHTERS,
            AATTACK_BOMBERS,
        };

        #endregion

        #region Public constructors

        public AirGroupWaypoint(AirGroupWaypointTypes type, Point3d position, double v, string target = null)
        {
            Type = type;
            X = position.x;
            Y = position.y;
            Z = position.z;
            V = v;
            Target = target;
        }

        public AirGroupWaypoint(AirGroupWaypointTypes type, double x, double y, double z, double v, string target = null)
        {
            Type = type;
            X = x;
            Y = y;
            Z = z;
            V = v;
            Target = target;
        }

        public AirGroupWaypoint(ISectionFile sectionFile, string id, int line)
        {
            string key;
            string value;
            sectionFile.get(id + "_Way", line, out key, out value);

            string[] valueList = value.Split(new char[] { ' ' });
            if (valueList != null && valueList.Length == 4)
            {
                Type = (AirGroupWaypointTypes)Enum.Parse(typeof(AirGroupWaypointTypes), key);
                double.TryParse(valueList[0], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture.NumberFormat, out X);
                double.TryParse(valueList[1], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture.NumberFormat, out Y);
                double.TryParse(valueList[2], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture.NumberFormat, out Z);
                double.TryParse(valueList[3], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture.NumberFormat, out V);
            }
        }

        #endregion

        #region Public properties

        public AirGroupWaypointTypes Type
        {
            get;
            set;
        }

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

        public double V;

        public string Target
        {
            get;
            set;
        }

        #endregion
    }
}