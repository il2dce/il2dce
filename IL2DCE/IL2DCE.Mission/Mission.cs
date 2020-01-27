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
    namespace Mission
    {
        abstract public class Mission : AMission
        {
            protected abstract Core Core
            {
                get;
            }

            /// <summary>
            /// React on the AircraftTookOff event.
            /// </summary>
            /// <param name="missionNumber"></param>
            /// <param name="shortName"></param>
            /// <param name="aircraft"></param>
            /// <remarks>
            /// Remove the player from the aircraft for a few ms. This is a workaround needed so that AI aicraft do not stay on the ground after the human player took off.
            /// </remarks>
            //public override void OnAircraftTookOff(int missionNumber, string shortName, AiAircraft aircraft)
            //{
            //    base.OnAircraftTookOff(missionNumber, shortName, aircraft);

            //    if (aircraft.Player(0) != null)
            //    {
            //        Player player = aircraft.Player(0);

            //        player.PlaceLeave(0);

            //        Timeout(0.1, () =>
            //        {
            //            player.PlaceEnter(aircraft, 0);
            //        });
            //    }
            //}
        }
    }
}