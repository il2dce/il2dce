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
    public class AircraftParametersInfo
    {
        public AircraftParametersInfo(string valuePart)
        {
            string[] parameters = valuePart.Split(new char[]{' '}, StringSplitOptions.RemoveEmptyEntries);
            if (parameters != null && parameters.Length == 1)
            {
                this.loadoutId = parameters[0];
            }
            else if (parameters != null && parameters.Length == 3)
            {
                this.loadoutId = parameters[0];
                minAltitude = double.Parse(parameters[1]);
                minAltitude = double.Parse(parameters[2]);
            }
            else
            {
                throw new FormatException(valuePart);
            }
        }

        public string LoadoutId
        {
            get
            {
                return this.loadoutId;
            }
        }
        private string loadoutId = "";

        public double? MinAltitude
        {
            get
            {
                return this.minAltitude;
            }
        }
        private double? minAltitude = null;

        public double? MaxAltitude
        {
            get
            {
                return this.maxAltitude;
            }
        }
        private double? maxAltitude = null;
    }
}