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
using System;
using System.Collections.Generic;

namespace IL2DCE
{
    /// <summary>
    /// The generator for the next mission.
    /// </summary>
    /// <remarks>
    /// This is a interface for the different generators responsible for different parts in the mission generation.
    /// </remarks>
    class Generator
    {
        internal GeneratorAirOperation GeneratorAirOperation
        {
            get;
            set;
        }

        internal GeneratorGroundOperation GeneratorGroundOperation
        {
            get;
            set;
        }

        internal GeneratorBriefing GeneratorBriefing
        {
            get;
            set;
        }

        private Random rand = new Random();
        
        private Core _core;

        private Core Core
        {
            get
            {
                return _core;
            }
        }
        
        private IGamePlay GamePlay
        {
            get
            {
                return _core.GamePlay;
            }
        }

        private Config Config
        {
            get
            {
                return _core.Config;
            }

        }

        private Career Career
        {
            get
            {
                return _core.CurrentCareer;
            }
        }

        public Generator(Core core)
        {
            _core = core;
        }

        /// <summary>
        /// Generates the next mission template based on the previous mission template. 
        /// </summary>
        /// <param name="staticTemplateFileName"></param>
        /// <param name="missionTemplateFile"></param>
        /// <remarks>
        /// For now it has a simplified implementaiton. It only generated random supply ships and air groups.
        /// </remarks>
        public void GenerateMissionTemplate(string staticTemplateFileName, string templateFileName, out ISectionFile missionTemplateFile)
        {
            MissionFile staticTemplateFile = new MissionFile(GamePlay.gpLoadSectionFile(staticTemplateFileName));

            // Use the campaign template to initialise the mission template.
            missionTemplateFile = GamePlay.gpLoadSectionFile(templateFileName);

            // Remove the ground groups but keep the air groups.
            if (missionTemplateFile.exist("Chiefs"))
            {
                // Delete all ground groups from the template file.
                for (int i = 0; i < missionTemplateFile.lines("Chiefs"); i++)
                {
                    string key;
                    string value;
                    missionTemplateFile.get("Chiefs", i, out key, out value);
                    missionTemplateFile.delete(key + "_Road");
                }
                missionTemplateFile.delete("Chiefs");
            }

            // Generate supply ships and trains.

            // For now generate a random supply ship on one of the routes to a harbour.
            int chiefIndex = 0;
            
            // TODO: Only create a random (or decent) amount of supply ships.
            foreach (Waterway waterway in staticTemplateFile.Waterways)
            {
                // For waterways only the end must be in friendly territory.
                if (GamePlay.gpFrontArmy(waterway.End.X, waterway.End.Y) == 1)
                {
                    string id = chiefIndex.ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat) + "_Chief";
                    chiefIndex++;

                    // For red army
                    GroundGroup supplyShip = new GroundGroup(id, "Ship.Tanker_Medium1", EGroundGroupCountry.gb, "/sleep 0/skill 2/slowfire 1", waterway.Waypoints);
                    supplyShip.WriteTo(missionTemplateFile);
                }
                else if(GamePlay.gpFrontArmy(waterway.End.X, waterway.End.Y) == 2)
                {
                    string id = chiefIndex.ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat) + "_Chief";
                    chiefIndex++;

                    // For blue army
                    GroundGroup supplyShip = new GroundGroup(id, "Ship.Tanker_Medium1", EGroundGroupCountry.de, "/sleep 0/skill 2/slowfire 1", waterway.Waypoints);
                    supplyShip.WriteTo(missionTemplateFile);
                }
            }

            foreach (Waterway railway in staticTemplateFile.Railways)
            {
                // For railways the start and the end must be in friendly territory.
                if (GamePlay.gpFrontArmy(railway.Start.X, railway.Start.Y) == 1 && GamePlay.gpFrontArmy(railway.End.X, railway.End.Y) == 1)
                {
                    string id = chiefIndex.ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat) + "_Chief";
                    chiefIndex++;

                    // For red army
                    // TODO: Use british train for red
                    GroundGroup supplyShip = new GroundGroup(id, "Train.57xx_0-6-0PT_c0", EGroundGroupCountry.gb, "", railway.Waypoints);
                    supplyShip.WriteTo(missionTemplateFile);
                }
                else if (GamePlay.gpFrontArmy(railway.Start.X, railway.Start.Y) == 2 && GamePlay.gpFrontArmy(railway.End.X, railway.End.Y) == 2)
                {
                    string id = chiefIndex.ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat) + "_Chief";
                    chiefIndex++;

                    // For blue army
                    GroundGroup supplyShip = new GroundGroup(id, "Train.BR56-00_c2", EGroundGroupCountry.de, "", railway.Waypoints);
                    supplyShip.WriteTo(missionTemplateFile);
                }
            }
        }

        public void GenerateMission(string missionTemplateFileName, string missionId, out ISectionFile missionFile, out BriefingFile briefingFile)
        {
            MissionFile missionTemplateFile = new MissionFile(GamePlay.gpLoadSectionFile(missionTemplateFileName));

            GeneratorAirOperation = new GeneratorAirOperation(this, Career.CampaignInfo, missionTemplateFile, Core.GamePlay, Core.Config);
            GeneratorGroundOperation = new GeneratorGroundOperation(this, Career.CampaignInfo, missionTemplateFile, Core.GamePlay, Core.Config);
            GeneratorBriefing = new GeneratorBriefing(Core, this);
            
            missionFile = GamePlay.gpLoadSectionFile(missionTemplateFileName);
            briefingFile = new BriefingFile();

            briefingFile.MissionName = missionId;
            briefingFile.MissionDescription = "";

            // Delete everything from the template file.
            
            if (missionFile.exist("AirGroups"))
            {
                // Delete all air groups from the template file.
                for (int i = 0; i < missionFile.lines("AirGroups"); i++)
                {
                    string key;
                    string value;
                    missionFile.get("AirGroups", i, out key, out value);
                    missionFile.delete(key);
                    missionFile.delete(key + "_Way");
                }
                missionFile.delete("AirGroups");
            }

            if (missionFile.exist("Chiefs"))
            {
                // Delete all ground groups from the template file.
                for (int i = 0; i < missionFile.lines("Chiefs"); i++)
                {
                    string key;
                    string value;
                    missionFile.get("Chiefs", i, out key, out value);
                    missionFile.delete(key + "_Road");
                }
                missionFile.delete("Chiefs");
            }

            for (int i = 0; i < missionFile.lines("MAIN"); i++)
            {
                // Delete player from the template file.
                string key;
                string value;
                missionFile.get("MAIN", i, out key, out value);
                if (key == "player")
                {
                    missionFile.delete("MAIN", i);
                    break;
                }
            }

            // Add things to the template file.

            int randomTime = rand.Next(5, 21);
            missionFile.set("MAIN", "TIME", randomTime.ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat));

            int randomWeatherIndex = rand.Next(0, 3);
            missionFile.set("MAIN", "WeatherIndex", randomWeatherIndex.ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat));

            int randomCloudsHeight = rand.Next(5, 15);
            missionFile.set("MAIN", "CloudsHeight", (randomCloudsHeight * 100).ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat));

            string weatherString = "";
            if (randomWeatherIndex == 0)
            {
                weatherString = "Clear";
            }
            else if (randomWeatherIndex == 1)
            {
                weatherString = "Light clouds at " + randomCloudsHeight * 100 + "m";
            }
            else if (randomWeatherIndex == 2)
            {
                weatherString = "Medium clouds at " + randomCloudsHeight * 100 + "m";
            }

            briefingFile.MissionDescription += this.Career.CampaignInfo.Name + "\n";
            briefingFile.MissionDescription += "Date: " + this.Career.Date.Value.ToShortDateString() + "\n";
            briefingFile.MissionDescription += "Time: " + randomTime + ":00\n";
            briefingFile.MissionDescription += "Weather: " + weatherString + "\n";

            // Create a air operation for the player.

            foreach (AirGroup airGroup in GeneratorAirOperation.AvailableAirGroups)
            {
                if ((airGroup.ArmyIndex == Career.ArmyIndex) && (airGroup.AirGroupKey + "." + airGroup.SquadronIndex.ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat)) == Career.AirGroup)
                {
                    GeneratorAirOperation.CreateRandomAirOperation(missionFile, briefingFile, airGroup);

                    // Determine the aircraft that is controlled by the player.
                    List<string> aircraftOrder = determineAircraftOrder(airGroup);

                    string playerAirGroupKey = airGroup.AirGroupKey;
                    int playerSquadronIndex = airGroup.SquadronIndex;
                    if (aircraftOrder.Count > 0)
                    {
                        string playerPosition = aircraftOrder[aircraftOrder.Count - 1];

                        double factor = aircraftOrder.Count / 6;
                        int playerPositionIndex = (int)(Math.Floor(Career.RankIndex * factor));

                        playerPosition = aircraftOrder[aircraftOrder.Count - 1 - playerPositionIndex];

                        if (missionFile.exist("MAIN", "player"))
                        {
                            missionFile.set("MAIN", "player", playerAirGroupKey + "." + playerSquadronIndex.ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat) + playerPosition);
                        }
                        else
                        {
                            missionFile.add("MAIN", "player", playerAirGroupKey + "." + playerSquadronIndex.ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat) + playerPosition);
                        }
                    }
                    break;
                }
            }

            // Add additional air operations.

            if (GeneratorAirOperation.AvailableAirGroups != null && GeneratorAirOperation.AvailableAirGroups.Count > 0)
            {
                for (int i = 0; i < Config.AdditionalAirOperations; i++)
                {
                    if (GeneratorAirOperation.AvailableAirGroups.Count > 0)
                    {
                        int randomAirGroupIndex = rand.Next(GeneratorAirOperation.AvailableAirGroups.Count);
                        AirGroup randomAirGroup = GeneratorAirOperation.AvailableAirGroups[randomAirGroupIndex];
                        GeneratorAirOperation.CreateRandomAirOperation(missionFile, briefingFile, randomAirGroup);
                    }
                }
            }

            // Add additional ground operations.

            if (GeneratorGroundOperation.AvailableGroundGroups != null && GeneratorGroundOperation.AvailableGroundGroups.Count > 0)
            {
                for (int i = 0; i < Config.AdditionalGroundOperations; i++)
                {
                    if (GeneratorGroundOperation.AvailableGroundGroups.Count > 0)
                    {
                        int randomGroundGroupIndex = rand.Next(GeneratorGroundOperation.AvailableGroundGroups.Count);
                        GroundGroup randomGroundGroup = GeneratorGroundOperation.AvailableGroundGroups[randomGroundGroupIndex];
                        GeneratorGroundOperation.AvailableGroundGroups.Remove(randomGroundGroup);

                        GeneratorGroundOperation.CreateRandomGroundOperation(missionFile, randomGroundGroup);
                    }
                }
            }
        }

        private static List<string> determineAircraftOrder(AirGroup airGroup)
        {
            List<string> aircraftOrder = new List<string>();
            if (airGroup.AirGroupInfo.FlightSize % 3 == 0)
            {
                for (int i = 0; i < 3; i++)
                {
                    foreach (int key in airGroup.Flights.Keys)
                    {
                        if (airGroup.Flights[key].Count > i)
                        {
                            aircraftOrder.Add(key.ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat) + i.ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat));
                        }
                    }

                    foreach (int key in airGroup.Flights.Keys)
                    {
                        if (airGroup.Flights[key].Count > i + 3)
                        {
                            aircraftOrder.Add(key.ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat) + (i + 3).ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat));
                        }
                    }
                }
            }
            else if (airGroup.AirGroupInfo.FlightSize % 2 == 0)
            {
                for (int i = 0; i < 2; i++)
                {
                    foreach (int key in airGroup.Flights.Keys)
                    {
                        if (airGroup.Flights[key].Count > i)
                        {
                            aircraftOrder.Add(key.ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat) + i.ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat));
                        }
                    }

                    foreach (int key in airGroup.Flights.Keys)
                    {
                        if (airGroup.Flights[key].Count > i + 2)
                        {
                            aircraftOrder.Add(key.ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat) + (i + 2).ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat));
                        }
                    }
                }
            }
            else if (airGroup.AirGroupInfo.FlightSize % 1 == 0)
            {
                foreach (int key in airGroup.Flights.Keys)
                {
                    if (airGroup.Flights[key].Count == 1)
                    {
                        aircraftOrder.Add(key.ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat) + "0");
                    }
                }
            }

            return aircraftOrder;
        }
    }
}
