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
    public class AircraftLoadoutInfo
    {
        public AircraftLoadoutInfo(ISectionFile aircraftInfoFile, string aircraft, string loadoutId)
        {
            if (aircraftInfoFile.exist(aircraft + "_" + loadoutId))
            {
                if (aircraftInfoFile.exist(aircraft + "_" + loadoutId, "Weapons"))
                {
                    // Weapons
                    string weaponsLine = aircraftInfoFile.get(aircraft + "_" + loadoutId, "Weapons");
                    string[] weaponsList = weaponsLine.Split(new char[] { ' ' });
                    if (weaponsList != null && weaponsList.Length > 0)
                    {
                        this.weapons = new int[weaponsList.Length];
                        for (int i = 0; i < weaponsList.Length; i++)
                        {
                            this.weapons[i] = int.Parse(weaponsList[i]);
                        }
                    }
                }
                else
                {
                    throw new FormatException(aircraft + "_" + loadoutId + ".Weapons");
                }

                for (int i = 0; i < aircraftInfoFile.lines(aircraft + "_" + loadoutId); i++)
                {
                    string key;
                    string value;
                    aircraftInfoFile.get(aircraft + "_" + loadoutId, i, out key, out value);
                    if (key == "Detonator")
                    {
                        this.detonator.Add(value);
                    }
                }
            }
            else
            {
                throw new ArgumentException(aircraft + "_" + loadoutId);
            }
        }

        public int[] Weapons
        {
            get
            {
                return this.weapons;
            }
        }
        private int[] weapons = null;

        public List<string> Detonator
        {
            get
            {
                return this.detonator;
            }
        }
        private List<string> detonator = new List<string>();
    }
}