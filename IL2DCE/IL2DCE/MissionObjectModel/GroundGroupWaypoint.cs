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
    public abstract class GroundGroupWaypoint
    {
        public abstract bool IsSubWaypoint(ISectionFile sectionFile, string id, int line);

        #region Public properties

        public List<GroundGroupWaypoint> SubWaypoints
        {
            get
            {
                return _subWaypoints;
            }
        }

        public Point2d Position
        {
            get
            {
                return new Point2d(X, Y);
            }
        }

        public double X
        {
            get;
            set;
        }

        public double Y
        {
            get;
            set;
        }

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

        public double Count
        {
            get
            {
                return SubWaypoints.Count + 2;
            }
        }

        protected List<GroundGroupWaypoint> _subWaypoints = new List<GroundGroupWaypoint>();
        protected double? _v;
        
        #endregion
    }
}