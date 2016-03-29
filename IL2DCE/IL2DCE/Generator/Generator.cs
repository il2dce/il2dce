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
    class Generator
    {
        private Random rand = new Random();

        private IList<AirGroup> availableAirGroups = new List<AirGroup>();
        private IList<GroundGroup> availableGroundGroups = new List<GroundGroup>();

        private Core _core;

        private World World
        {
            get
            {
                return _core.World;
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

        public void Generate(string templateFileName, string missionId, out ISectionFile missionFile, out BriefingFile briefingFile)
        {
            availableAirGroups.Clear();
            availableGroundGroups.Clear();

            foreach (AirGroup airGroup in World.AirGroups)
            {
                availableAirGroups.Add(airGroup);
            }

            foreach (GroundGroup groundGroup in World.GroundGroups)
            {
                availableGroundGroups.Add(groundGroup);
            }

            missionFile = GamePlay.gpLoadSectionFile(templateFileName);
            briefingFile = new BriefingFile();

            briefingFile.MissionName = missionId;
            briefingFile.MissionDescription = "";

            // Delete everything from the template file.
            if (missionFile.exist("AirGroups"))
            {

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

            foreach (AirGroup airGroup in World.GetAirGroups(Career.ArmyIndex))
            {
                if ((airGroup.AirGroupKey + "." + airGroup.SquadronIndex.ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat)) == Career.AirGroup)
                {
                    createRandomAirOperation(missionFile, briefingFile, airGroup);

                    // Determine the aircraft that is controlled by the player.

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

            if (availableAirGroups != null && availableAirGroups.Count > 0)
            {
                for (int i = 0; i < Config.AdditionalAirOperations; i++)
                {
                    if (availableAirGroups.Count > 0)
                    {
                        int randomAirGroupIndex = rand.Next(availableAirGroups.Count);
                        AirGroup randomAirGroup = availableAirGroups[randomAirGroupIndex];
                        createRandomAirOperation(missionFile, briefingFile, randomAirGroup);
                    }
                }
            }

            // Add additional ground operations.

            if (availableGroundGroups != null && availableGroundGroups.Count > 0)
            {
                for (int i = 0; i < Config.AdditionalGroundOperations; i++)
                {
                    if (availableGroundGroups.Count > 0)
                    {
                        int randomGroundGroupIndex = rand.Next(availableGroundGroups.Count);
                        GroundGroup randomGroundGroup = availableGroundGroups[randomGroundGroupIndex];
                        availableGroundGroups.Remove(randomGroundGroup);

                        createRandomGroundOperation(missionFile, randomGroundGroup);
                    }
                }
            }
        }
        


        private void findPath(GroundGroup groundGroup, Point2d start, Point2d end)
        {
            IRecalcPathParams pathParams = null;
            if (groundGroup.Type == EGroundGroupType.Armor || groundGroup.Type == EGroundGroupType.Vehicle)
            {
                pathParams = GamePlay.gpFindPath(start, 10.0, end, 20.0, PathType.GROUND, groundGroup.Army);
            }
            else if (groundGroup.Type == EGroundGroupType.Ship)
            {
                pathParams = GamePlay.gpFindPath(start, 10.0, end, 20.0, PathType.WATER, groundGroup.Army);
            }

            if (pathParams != null)
            {
                while (pathParams.State == RecalcPathState.WAIT)
                {
                    //Game.gpLogServer(new Player[] { Game.gpPlayer() }, "Wait for path.", null);
                    System.Threading.Thread.Sleep(100);
                }

                if (pathParams.State == RecalcPathState.SUCCESS)
                {
                    //Game.gpLogServer(new Player[] { Game.gpPlayer() }, "Path found (" + pathParams.Path.Length.ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat) + ").", null);

                    GroundGroupWaypoint lastGroundGroupWaypoint = null;
                    foreach (maddox.game.world.AiWayPoint aiWayPoint in pathParams.Path)
                    {
                        if (aiWayPoint is maddox.game.world.AiGroundWayPoint)
                        {
                            maddox.game.world.AiGroundWayPoint aiGroundWayPoint = aiWayPoint as maddox.game.world.AiGroundWayPoint;

                            if (aiGroundWayPoint.P.z == -1)
                            {
                                GroundGroupWaypoint groundGroupWaypoint = new GroundGroupWaypoint(aiGroundWayPoint.P.x, aiGroundWayPoint.P.y, aiGroundWayPoint.roadWidth, aiGroundWayPoint.Speed);
                                lastGroundGroupWaypoint = groundGroupWaypoint;
                                groundGroup.Waypoints.Add(groundGroupWaypoint);
                            }
                            else if (lastGroundGroupWaypoint != null)
                            {
                                string s = aiGroundWayPoint.P.x.ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat) + " " + aiGroundWayPoint.P.y.ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat) + " " + aiGroundWayPoint.P.z.ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat) + " " + aiGroundWayPoint.roadWidth.ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
                                GroundGroupSubWaypoint groundGroupSubWaypoint = new GroundGroupSubWaypoint(s, null);
                                lastGroundGroupWaypoint.SubWaypoints.Add(groundGroupSubWaypoint);
                            }
                        }
                    }
                }
                else if (pathParams.State == RecalcPathState.FAILED)
                {
                    //Game.gpLogServer(new Player[] { Game.gpPlayer() }, "Path not found.", null);
                }
            }
        }

        private void findRoad(GroundGroup groundGroup, Point2d start, Point2d end, IList<Waterway> roads)
        {
            if (roads != null && roads.Count > 0)
            {
                Waterway closestRoad = null;
                double closestRoadDistance = 0.0;
                foreach (Waterway road in roads)
                {
                    if (road.Start != null && road.End != null)
                    {
                        Point2d roadStart = new Point2d(road.Start.Position.x, road.Start.Position.y);
                        double distanceStart = start.distance(ref roadStart);
                        Point2d roadEnd = new Point2d(road.End.Position.x, road.End.Position.y);
                        double distanceEnd = end.distance(ref roadEnd);

                        Point2d p = new Point2d(end.x, end.y);
                        if (distanceEnd < start.distance(ref p))
                        {
                            if (closestRoad == null)
                            {
                                closestRoad = road;
                                closestRoadDistance = distanceStart + distanceEnd;
                            }
                            else
                            {
                                if (distanceStart + distanceEnd < closestRoadDistance)
                                {
                                    closestRoad = road;
                                    closestRoadDistance = distanceStart + distanceEnd;
                                }
                            }
                        }
                    }
                }

                if (closestRoad != null)
                {
                    //findPath(groundGroup, start, new Point2d(closestRoad.Start.X, closestRoad.Start.Y));

                    groundGroup.Waypoints.AddRange(closestRoad.Waypoints);

                    List<Waterway> availableRoads = new List<Waterway>(roads);
                    availableRoads.Remove(closestRoad);

                    findRoad(groundGroup, new Point2d(closestRoad.End.Position.x, closestRoad.End.Position.y), end, availableRoads);
                }
            }
        }
        


        private double getRandomAltitude(AircraftParametersInfo missionParameters)
        {
            if (missionParameters.MinAltitude != null && missionParameters.MinAltitude.HasValue && missionParameters.MaxAltitude != null && missionParameters.MaxAltitude.HasValue)
            {
                return (double)rand.Next((int)missionParameters.MinAltitude.Value, (int)missionParameters.MaxAltitude.Value);
            }
            else
            {
                // Use some default altitudes
                return (double)rand.Next(300, 7000);
            }
        }

        private void getRandomFlightSize(AirGroup airGroup, EMissionType missionType)
        {
            airGroup.Flights.Clear();
            int aircraftNumber = 1;

            int flightCount = (int)Math.Ceiling(airGroup.AirGroupInfo.FlightCount * this.Config.FlightCount);
            int flightSize = (int)Math.Ceiling(airGroup.AirGroupInfo.FlightSize * this.Config.FlightSize);

            if (missionType == EMissionType.RECON || missionType == EMissionType.MARITIME_RECON)
            {
                for (int i = 0; i < 1; i++)
                {
                    List<string> aircraftNumbers = new List<string>();
                    for (int j = 0; j < 1; j++)
                    {
                        aircraftNumbers.Add(aircraftNumber.ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat));
                        aircraftNumber++;
                    }
                    airGroup.Flights[i] = aircraftNumbers;
                }
            }
            else if (missionType == EMissionType.ARMED_RECON || missionType == EMissionType.ARMED_MARITIME_RECON)
            {
                for (int i = 0; i < 1; i++)
                {
                    List<string> aircraftNumbers = new List<string>();
                    for (int j = 0; j < flightSize; j++)
                    {
                        aircraftNumbers.Add(aircraftNumber.ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat));
                        aircraftNumber++;
                    }
                    airGroup.Flights[i] = aircraftNumbers;
                }
            }
            else if (missionType == EMissionType.ESCORT || missionType == EMissionType.INTERCEPT)
            {
                if (airGroup.TargetAirGroup != null)
                {
                    if (airGroup.TargetAirGroup.Flights.Count < flightCount)
                    {
                        flightCount = airGroup.TargetAirGroup.Flights.Count;
                    }

                    for (int i = 0; i < flightCount; i++)
                    {
                        List<string> aircraftNumbers = new List<string>();
                        for (int j = 0; j < flightSize; j++)
                        {
                            aircraftNumbers.Add(aircraftNumber.ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat));
                            aircraftNumber++;
                        }
                        airGroup.Flights[i] = aircraftNumbers;
                    }
                }
            }
            else
            {
                for (int i = 0; i < flightCount; i++)
                {
                    List<string> aircraftNumbers = new List<string>();
                    for (int j = 0; j < flightSize; j++)
                    {
                        aircraftNumbers.Add(aircraftNumber.ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat));
                        aircraftNumber++;
                    }
                    airGroup.Flights[i] = aircraftNumbers;
                }
            }
        }

        

        private void createRandomAirOperation(ISectionFile sectionFile, BriefingFile briefingFile, AirGroup airGroup)
        {
            IList<EMissionType> missionTypes = airGroup.AircraftInfo.MissionTypes;
            if (missionTypes != null && missionTypes.Count > 0)
            {
                List<EMissionType> availableMissionTypes = new List<EMissionType>();
                foreach (EMissionType missionType in missionTypes)
                {
                    if (isMissionTypeAvailable(airGroup, missionType))
                    {
                        availableMissionTypes.Add(missionType);
                    }
                }

                if (availableMissionTypes.Count > 0)
                {
                    int randomMissionTypeIndex = rand.Next(availableMissionTypes.Count);
                    EMissionType randomMissionType = availableMissionTypes[randomMissionTypeIndex];

                    createAirOperation(sectionFile, briefingFile, airGroup, randomMissionType, true, null);
                }
            }
        }
        
        public void createAirOperation(ISectionFile sectionFile, BriefingFile briefingFile, AirGroup airGroup, EMissionType missionType, bool allowIntercept, AirGroup forcedEscortAirGroup)
        {
            if (isMissionTypeAvailable(airGroup, missionType))
            {
                availableAirGroups.Remove(airGroup);

                IList<AircraftParametersInfo> aircraftParametersInfos = airGroup.AircraftInfo.GetAircraftParametersInfo(missionType);
                int aircraftParametersInfoIndex = rand.Next(aircraftParametersInfos.Count);
                AircraftParametersInfo randomAircraftParametersInfo = aircraftParametersInfos[aircraftParametersInfoIndex];
                AircraftLoadoutInfo aircraftLoadoutInfo = airGroup.AircraftInfo.GetAircraftLoadoutInfo(randomAircraftParametersInfo.LoadoutId);
                airGroup.Weapons = aircraftLoadoutInfo.Weapons;
                airGroup.Detonator = aircraftLoadoutInfo.Detonator;

                AirGroup escortAirGroup = null;
                if (isMissionTypeEscorted(missionType))
                {
                    if (forcedEscortAirGroup == null)
                    {
                        escortAirGroup = getAvailableRandomEscortAirGroup(airGroup);
                    }
                    else
                    {
                        escortAirGroup = forcedEscortAirGroup;
                    }
                }

                if (missionType == EMissionType.COVER)
                {
                    AirGroup offensiveAirGroup = getAvailableRandomOffensiveAirGroup(airGroup.ArmyIndex);
                    if (offensiveAirGroup != null)
                    {
                        List<EMissionType> availableOffensiveMissionTypes = new List<EMissionType>();
                        foreach (EMissionType targetMissionType in offensiveAirGroup.AircraftInfo.MissionTypes)
                        {
                            if (isMissionTypeAvailable(offensiveAirGroup, targetMissionType) && isMissionTypeOffensive(targetMissionType))
                            {
                                availableOffensiveMissionTypes.Add(targetMissionType);
                            }
                        }

                        if (availableOffensiveMissionTypes.Count > 0)
                        {
                            int offensiveMissionTypeIndex = rand.Next(availableOffensiveMissionTypes.Count);
                            EMissionType randomOffensiveMissionType = availableOffensiveMissionTypes[offensiveMissionTypeIndex];
                            createAirOperation(sectionFile, briefingFile, offensiveAirGroup, randomOffensiveMissionType, false, null);

                            if (offensiveAirGroup.Altitude != null && offensiveAirGroup.Altitude.HasValue && offensiveAirGroup.TargetGroundGroup != null)
                            {
                                airGroup.Cover(missionType, offensiveAirGroup.TargetGroundGroup, offensiveAirGroup.Altitude.Value);
                            }
                            else if (offensiveAirGroup.Altitude != null && offensiveAirGroup.Altitude.HasValue && offensiveAirGroup.TargetStationary != null)
                            {
                                airGroup.Cover(missionType, offensiveAirGroup.TargetStationary, offensiveAirGroup.Altitude.Value);
                            }
                            else if (offensiveAirGroup.Altitude != null && offensiveAirGroup.Altitude.HasValue && offensiveAirGroup.TargetArea != null && offensiveAirGroup.TargetArea.HasValue)
                            {
                                airGroup.Cover(missionType, offensiveAirGroup.TargetArea.Value, offensiveAirGroup.Altitude.Value);
                            }
                        }
                    }
                }
                else if (missionType == EMissionType.ARMED_MARITIME_RECON)
                {
                    GroundGroup groundGroup = getAvailableRandomEnemyGroundGroup(airGroup.ArmyIndex, new List<EGroundGroupType> { EGroundGroupType.Ship });
                    createRandomGroundOperation(sectionFile, groundGroup);
                    double altitude = getRandomAltitude(randomAircraftParametersInfo);

                    airGroup.GroundAttack(missionType, groundGroup, altitude, escortAirGroup);
                }
                else if (missionType == EMissionType.ARMED_RECON)
                {
                    GroundGroup groundGroup = getAvailableRandomEnemyGroundGroup(airGroup.ArmyIndex, new List<EGroundGroupType> { EGroundGroupType.Armor, EGroundGroupType.Vehicle });
                    createRandomGroundOperation(sectionFile, groundGroup);
                    double altitude = getRandomAltitude(randomAircraftParametersInfo);

                    airGroup.GroundAttack(missionType, groundGroup, altitude, escortAirGroup);
                }
                else if (missionType == EMissionType.ATTACK_ARMOR)
                {
                    GroundGroup groundGroup = getAvailableRandomEnemyGroundGroup(airGroup.ArmyIndex, new List<EGroundGroupType> { EGroundGroupType.Armor });
                    createRandomGroundOperation(sectionFile, groundGroup);
                    double altitude = getRandomAltitude(randomAircraftParametersInfo);

                    airGroup.GroundAttack(missionType, groundGroup, altitude, escortAirGroup);
                }
                else if (missionType == EMissionType.ATTACK_RADAR)
                {
                    IList<Stationary> radars = World.GetEnemyRadars(airGroup.ArmyIndex);
                    if (radars.Count > 0)
                    {
                        int radarIndex = rand.Next(radars.Count);
                        Stationary radar = radars[radarIndex];
                        double altitude = getRandomAltitude(randomAircraftParametersInfo);

                        airGroup.GroundAttack(missionType, radar, altitude, escortAirGroup);
                    }
                }
                else if (missionType == EMissionType.ATTACK_SHIP)
                {
                    GroundGroup groundGroup = getAvailableRandomEnemyGroundGroup(airGroup.ArmyIndex, new List<EGroundGroupType> { EGroundGroupType.Ship });
                    createRandomGroundOperation(sectionFile, groundGroup);
                    double altitude = getRandomAltitude(randomAircraftParametersInfo);

                    airGroup.GroundAttack(missionType, groundGroup, altitude, escortAirGroup);
                }
                else if (missionType == EMissionType.ATTACK_VEHICLE)
                {
                    GroundGroup groundGroup = getAvailableRandomEnemyGroundGroup(airGroup.ArmyIndex, new List<EGroundGroupType> { EGroundGroupType.Vehicle });
                    createRandomGroundOperation(sectionFile, groundGroup);
                    double altitude = getRandomAltitude(randomAircraftParametersInfo);

                    airGroup.GroundAttack(missionType, groundGroup, altitude, escortAirGroup);
                }
                else if (missionType == EMissionType.ESCORT)
                {
                    AirGroup escortedAirGroup = getAvailableRandomEscortedAirGroup(airGroup.ArmyIndex);
                    if (escortedAirGroup != null)
                    {
                        List<EMissionType> availableEscortedMissionTypes = new List<EMissionType>();
                        foreach (EMissionType targetMissionType in escortedAirGroup.AircraftInfo.MissionTypes)
                        {
                            if (isMissionTypeAvailable(escortedAirGroup, targetMissionType) && isMissionTypeEscorted(targetMissionType))
                            {
                                availableEscortedMissionTypes.Add(targetMissionType);
                            }
                        }

                        if (availableEscortedMissionTypes.Count > 0)
                        {
                            int escortedMissionTypeIndex = rand.Next(availableEscortedMissionTypes.Count);
                            EMissionType randomEscortedMissionType = availableEscortedMissionTypes[escortedMissionTypeIndex];
                            createAirOperation(sectionFile, briefingFile, escortedAirGroup, randomEscortedMissionType, true, airGroup);

                            airGroup.Escort(missionType, escortedAirGroup);
                        }
                    }
                }
                else if (missionType == EMissionType.INTERCEPT)
                {
                    AirGroup interceptedAirGroup = getAvailableRandomOffensiveAirGroup(airGroup.ArmyIndex);
                    if (interceptedAirGroup != null)
                    {
                        List<EMissionType> availableOffensiveMissionTypes = new List<EMissionType>();
                        foreach (EMissionType targetMissionType in interceptedAirGroup.AircraftInfo.MissionTypes)
                        {
                            if (isMissionTypeAvailable(interceptedAirGroup, targetMissionType) && isMissionTypeOffensive(targetMissionType))
                            {
                                availableOffensiveMissionTypes.Add(targetMissionType);
                            }
                        }

                        if (availableOffensiveMissionTypes.Count > 0)
                        {
                            int offensiveMissionTypeIndex = rand.Next(availableOffensiveMissionTypes.Count);
                            EMissionType randomOffensiveMissionType = availableOffensiveMissionTypes[offensiveMissionTypeIndex];
                            createAirOperation(sectionFile, briefingFile, interceptedAirGroup, randomOffensiveMissionType, false, null);

                            airGroup.Intercept(missionType, interceptedAirGroup);
                        }
                    }
                }
                else if (missionType == EMissionType.MARITIME_RECON)
                {
                    GroundGroup groundGroup = getAvailableRandomEnemyGroundGroup(airGroup.ArmyIndex, new List<EGroundGroupType> { EGroundGroupType.Ship });
                    createRandomGroundOperation(sectionFile, groundGroup);
                    double altitude = getRandomAltitude(randomAircraftParametersInfo);

                    airGroup.Recon(missionType, groundGroup, altitude, escortAirGroup);
                }
                else if (missionType == EMissionType.RECON)
                {
                    GroundGroup groundGroup = getAvailableRandomEnemyGroundGroup(airGroup.ArmyIndex, new List<EGroundGroupType> { EGroundGroupType.Armor, EGroundGroupType.Vehicle });
                    createRandomGroundOperation(sectionFile, groundGroup);
                    double altitude = getRandomAltitude(randomAircraftParametersInfo);

                    airGroup.Recon(missionType, groundGroup, altitude, escortAirGroup);
                }

                getRandomFlightSize(airGroup, missionType);
                createBriefing(briefingFile, airGroup, escortAirGroup);
                airGroup.WriteTo(sectionFile, Config);

                if (forcedEscortAirGroup == null && escortAirGroup != null)
                {
                    availableAirGroups.Remove(escortAirGroup);

                    IList<AircraftParametersInfo> escortAircraftParametersInfos = escortAirGroup.AircraftInfo.GetAircraftParametersInfo(EMissionType.ESCORT);
                    int escortAircraftParametersInfoIndex = rand.Next(escortAircraftParametersInfos.Count);
                    AircraftParametersInfo escortRandomAircraftParametersInfo = escortAircraftParametersInfos[escortAircraftParametersInfoIndex];
                    AircraftLoadoutInfo escortAircraftLoadoutInfo = escortAirGroup.AircraftInfo.GetAircraftLoadoutInfo(escortRandomAircraftParametersInfo.LoadoutId);
                    escortAirGroup.Weapons = escortAircraftLoadoutInfo.Weapons;

                    escortAirGroup.Escort(EMissionType.ESCORT, airGroup);

                    getRandomFlightSize(escortAirGroup, EMissionType.ESCORT);
                    createBriefing(briefingFile, escortAirGroup, null);
                    escortAirGroup.WriteTo(sectionFile, Config);
                }

                if (isMissionTypeOffensive(missionType))
                {
                    if (allowIntercept)
                    {
                        AirGroup interceptAirGroup = getAvailableRandomInterceptAirGroup(airGroup);
                        if (interceptAirGroup != null)
                        {
                            availableAirGroups.Remove(interceptAirGroup);

                            IList<AircraftParametersInfo> interceptAircraftParametersInfos = interceptAirGroup.AircraftInfo.GetAircraftParametersInfo(EMissionType.INTERCEPT);
                            int interceptAircraftParametersInfoIndex = rand.Next(interceptAircraftParametersInfos.Count);
                            AircraftParametersInfo interceptRandomAircraftParametersInfo = interceptAircraftParametersInfos[interceptAircraftParametersInfoIndex];
                            AircraftLoadoutInfo interceptAircraftLoadoutInfo = interceptAirGroup.AircraftInfo.GetAircraftLoadoutInfo(interceptRandomAircraftParametersInfo.LoadoutId);
                            interceptAirGroup.Weapons = interceptAircraftLoadoutInfo.Weapons;

                            interceptAirGroup.Intercept(EMissionType.INTERCEPT, airGroup);

                            getRandomFlightSize(interceptAirGroup, EMissionType.INTERCEPT);
                            createBriefing(briefingFile, interceptAirGroup, null);
                            interceptAirGroup.WriteTo(sectionFile, Config);
                        }
                    }
                }
            }
            else
            {
                throw new ArgumentException(missionType.ToString());
            }
        }

        private void createRandomGroundOperation(ISectionFile missionFile, GroundGroup groundGroup)
        {
            availableGroundGroups.Remove(groundGroup);

            IList<Point3d> friendlyMarkers = World.GetFriendlyMarkers(groundGroup.Army);
            if (friendlyMarkers.Count > 0)
            {
                List<Point3d> availableFriendlyMarkers = new List<Point3d>(friendlyMarkers);

                // Find closest friendly marker
                Point3d? closestMarker = null;
                foreach (Point3d marker in availableFriendlyMarkers)
                {
                    if (closestMarker == null)
                    {
                        closestMarker = marker;
                    }
                    else if (closestMarker.HasValue)
                    {
                        Point3d p1 = new Point3d(marker.x, marker.y, marker.z);
                        Point3d p2 = new Point3d(closestMarker.Value.x, closestMarker.Value.y, closestMarker.Value.z);
                        if (groundGroup.Position.distance(ref p1) < groundGroup.Position.distance(ref p2))
                        {
                            closestMarker = marker;
                        }
                    }
                }

                if (closestMarker != null && closestMarker.HasValue)
                {
                    availableFriendlyMarkers.Remove(closestMarker.Value);

                    if (availableFriendlyMarkers.Count > 0)
                    {
                        int markerIndex = rand.Next(availableFriendlyMarkers.Count);

                        groundGroup.Waypoints.Clear();

                        Point2d start = new Point2d(groundGroup.Position.x, groundGroup.Position.y);
                        Point2d end = new Point2d(availableFriendlyMarkers[markerIndex].x, availableFriendlyMarkers[markerIndex].y);

                        if (groundGroup.Type == EGroundGroupType.Armor || groundGroup.Type == EGroundGroupType.Vehicle)
                        {
                            findPath(groundGroup, new Point2d(closestMarker.Value.x, closestMarker.Value.y), new Point2d(availableFriendlyMarkers[markerIndex].x, availableFriendlyMarkers[markerIndex].y));
                            groundGroup.WriteTo(missionFile);
                        }
                        else
                        {
                            findPath(groundGroup, start, end);

                            groundGroup.WriteTo(missionFile);

                            string groundGroupId = groundGroup.Id;
                            for (int i = 1; i < 3; i++)
                            {
                                double xOffset = -1.0;
                                double yOffset = -1.0;

                                bool subWaypointUsed = false;
                                Point2d p1 = new Point2d(groundGroup.Waypoints[0].X, groundGroup.Waypoints[0].Y);
                                if (groundGroup.Waypoints[0].SubWaypoints.Count > 0)
                                {
                                    foreach (GroundGroupSubWaypoint subWaypoint in groundGroup.Waypoints[0].SubWaypoints)
                                    {
                                        if (subWaypoint.P.HasValue)
                                        {
                                            Point2d p2 = new Point2d(subWaypoint.P.Value.x, subWaypoint.P.Value.y);
                                            double distance = p1.distance(ref p2);
                                            xOffset = 500 * ((p2.x - p1.x) / distance);
                                            yOffset = 500 * ((p2.y - p1.y) / distance);
                                            subWaypointUsed = true;
                                            break;
                                        }
                                    }
                                }
                                if (subWaypointUsed == false)
                                {
                                    Point2d p2 = new Point2d(groundGroup.Waypoints[1].X, groundGroup.Waypoints[1].Y);
                                    double distance = p1.distance(ref p2);
                                    xOffset = 500 * ((p2.x - p1.x) / distance);
                                    yOffset = 500 * ((p2.y - p1.y) / distance);
                                }

                                groundGroup.Waypoints[0].X += xOffset;
                                groundGroup.Waypoints[0].Y += yOffset;

                                subWaypointUsed = false;
                                p1 = new Point2d(groundGroup.Waypoints[groundGroup.Waypoints.Count - 1].X, groundGroup.Waypoints[groundGroup.Waypoints.Count - 1].Y);
                                if (groundGroup.Waypoints[groundGroup.Waypoints.Count - 2].SubWaypoints.Count > 0)
                                {
                                    for (int j = groundGroup.Waypoints[groundGroup.Waypoints.Count - 2].SubWaypoints.Count - 1; j >= 0; j--)
                                    {
                                        GroundGroupSubWaypoint subWaypoint = groundGroup.Waypoints[groundGroup.Waypoints.Count - 2].SubWaypoints[j];
                                        if (subWaypoint.P.HasValue)
                                        {

                                            Point2d p2 = new Point2d(subWaypoint.P.Value.x, subWaypoint.P.Value.y);
                                            double distance = p1.distance(ref p2);
                                            xOffset = 500 * ((p2.x - p1.x) / distance);
                                            yOffset = 500 * ((p2.y - p1.y) / distance);
                                            subWaypointUsed = true;
                                            break;
                                        }
                                    }
                                }
                                if (subWaypointUsed == false)
                                {
                                    Point2d p2 = new Point2d(groundGroup.Waypoints[groundGroup.Waypoints.Count - 2].X, groundGroup.Waypoints[groundGroup.Waypoints.Count - 2].Y);
                                    double distance = p1.distance(ref p2);
                                    xOffset = 500 * ((p2.x - p1.x) / distance);
                                    yOffset = 500 * ((p2.y - p1.y) / distance);
                                }

                                groundGroup.Waypoints[groundGroup.Waypoints.Count - 1].X -= xOffset;
                                groundGroup.Waypoints[groundGroup.Waypoints.Count - 1].Y -= yOffset;

                                groundGroup._id = groundGroupId + "." + i.ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat);

                                groundGroup.WriteTo(missionFile);
                            }
                        }
                    }
                }
            }
        }
        

        
        private List<GroundGroup> getAvailableEnemyGroundGroups(int armyIndex)
        {
            List<GroundGroup> groundGroups = new List<GroundGroup>();
            foreach (GroundGroup groundGroup in availableGroundGroups)
            {
                if (groundGroup.Army != armyIndex)
                {
                    groundGroups.Add(groundGroup);
                }
            }
            return groundGroups;
        }

        private List<GroundGroup> getAvailableFriendlyGroundGroups(int armyIndex)
        {
            List<GroundGroup> groundGroups = new List<GroundGroup>();
            foreach (GroundGroup groundGroup in availableGroundGroups)
            {
                if (groundGroup.Army == armyIndex)
                {
                    groundGroups.Add(groundGroup);
                }
            }
            return groundGroups;
        }

        private List<GroundGroup> getAvailableEnemyGroundGroups(int armyIndex, List<EGroundGroupType> groundGroupTypes)
        {
            List<GroundGroup> groundGroups = new List<GroundGroup>();
            foreach (GroundGroup groundGroup in getAvailableEnemyGroundGroups(armyIndex))
            {
                if (groundGroupTypes.Contains(groundGroup.Type))
                {
                    groundGroups.Add(groundGroup);
                }
            }
            return groundGroups;
        }

        private List<GroundGroup> getAvailableFriendlyGroundGroups(int armyIndex, List<EGroundGroupType> groundGroupTypes)
        {
            List<GroundGroup> groundGroups = new List<GroundGroup>();
            foreach (GroundGroup groundGroup in getAvailableFriendlyGroundGroups(armyIndex))
            {
                if (groundGroupTypes.Contains(groundGroup.Type))
                {
                    groundGroups.Add(groundGroup);
                }
            }
            return groundGroups;
        }

        private List<AirGroup> getAvailableOffensiveAirGroups(int armyIndex)
        {
            List<AirGroup> airGroups = new List<AirGroup>();
            foreach (AirGroup airGroup in availableAirGroups)
            {
                if (airGroup.ArmyIndex != armyIndex)
                {
                    foreach (EMissionType missionType in airGroup.AircraftInfo.MissionTypes)
                    {
                        if (isMissionTypeOffensive(missionType) && isMissionTypeAvailable(airGroup, missionType))
                        {
                            airGroups.Add(airGroup);
                            break;
                        }
                    }
                }
            }
            return airGroups;
        }

        public AirGroup getAvailableRandomOffensiveAirGroup(int armyIndex)
        {
            List<AirGroup> airGroups = getAvailableOffensiveAirGroups(armyIndex);

            if (airGroups.Count > 0)
            {
                int interceptedAirGroupIndex = rand.Next(airGroups.Count);
                AirGroup interceptedAirGroup = airGroups[interceptedAirGroupIndex];

                return interceptedAirGroup;
            }
            else
            {
                return null;
            }
        }

        private List<AirGroup> getAvailableEscortedAirGroups(int armyIndex)
        {
            List<AirGroup> airGroups = new List<AirGroup>();
            foreach (AirGroup airGroup in availableAirGroups)
            {
                if (airGroup.ArmyIndex == armyIndex)
                {
                    foreach (EMissionType missionType in airGroup.AircraftInfo.MissionTypes)
                    {
                        if (isMissionTypeEscorted(missionType) && isMissionTypeAvailable(airGroup, missionType))
                        {
                            airGroups.Add(airGroup);
                            break;
                        }
                    }
                }
            }
            return airGroups;
        }

        private AirGroup getAvailableRandomEscortedAirGroup(int armyIndex)
        {
            List<AirGroup> airGroups = getAvailableEscortedAirGroups(armyIndex);
            if (airGroups.Count > 0)
            {
                int escortedAirGroupIndex = rand.Next(airGroups.Count);
                AirGroup escortedAirGroup = airGroups[escortedAirGroupIndex];

                return escortedAirGroup;
            }
            else
            {
                return null;
            }
        }

        private AirGroup getAvailableRandomEscortAirGroup(AirGroup targetAirUnit)
        {
            List<AirGroup> airGroups = new List<AirGroup>();
            foreach (AirGroup airGroup in availableAirGroups)
            {
                if (airGroup.ArmyIndex == targetAirUnit.ArmyIndex)
                {
                    if (airGroup.AircraftInfo.MissionTypes.Contains(EMissionType.ESCORT))
                    {
                        airGroups.Add(airGroup);
                    }
                }
            }

            if (airGroups.Count > 0)
            {
                int escortAirGroupIndex = rand.Next(airGroups.Count);
                AirGroup escortAirGroup = airGroups[escortAirGroupIndex];

                return escortAirGroup;
            }
            else
            {
                return null;
            }
        }

        private GroundGroup getAvailableRandomEnemyGroundGroup(int armyIndex)
        {
            List<GroundGroup> groundGroups = getAvailableEnemyGroundGroups(armyIndex);
            if (groundGroups.Count > 0)
            {
                int groundGroupIndex = rand.Next(groundGroups.Count);
                GroundGroup targetGroundGroup = groundGroups[groundGroupIndex];

                return targetGroundGroup;
            }
            else
            {
                return null;
            }
        }

        private GroundGroup getAvailableRandomEnemyGroundGroup(int armyIndex, List<EGroundGroupType> groundGroupTypes)
        {
            List<GroundGroup> groundGroups = getAvailableEnemyGroundGroups(armyIndex, groundGroupTypes);
            if (groundGroups.Count > 0)
            {
                int groundGroupIndex = rand.Next(groundGroups.Count);
                GroundGroup targetGroundGroup = groundGroups[groundGroupIndex];

                return targetGroundGroup;
            }
            else
            {
                return null;
            }
        }

        private GroundGroup getAvailableRandomFriendlyGroundGroup(int armyIndex)
        {
            List<GroundGroup> groundGroups = getAvailableFriendlyGroundGroups(armyIndex);
            if (groundGroups.Count > 0)
            {
                int groundGroupIndex = rand.Next(groundGroups.Count);
                GroundGroup targetGroundGroup = groundGroups[groundGroupIndex];

                return targetGroundGroup;
            }
            else
            {
                return null;
            }
        }

        private GroundGroup getAvailableRandomFriendlyGroundGroup(int armyIndex, List<EGroundGroupType> groundGroupTypes)
        {
            List<GroundGroup> groundGroups = getAvailableFriendlyGroundGroups(armyIndex, groundGroupTypes);
            if (groundGroups.Count > 0)
            {
                int groundGroupIndex = rand.Next(groundGroups.Count);
                GroundGroup targetGroundGroup = groundGroups[groundGroupIndex];

                return targetGroundGroup;
            }
            else
            {
                return null;
            }
        }

        private AirGroup getAvailableRandomInterceptAirGroup(AirGroup targetAirUnit)
        {
            List<AirGroup> airGroups = new List<AirGroup>();
            foreach (AirGroup airGroup in availableAirGroups)
            {
                if (airGroup.ArmyIndex != targetAirUnit.ArmyIndex)
                {
                    if (airGroup.AircraftInfo.MissionTypes.Contains(EMissionType.INTERCEPT))
                    {
                        airGroups.Add(airGroup);
                    }
                }
            }

            if (airGroups.Count > 0)
            {
                int interceptAirGroupIndex = rand.Next(airGroups.Count);
                AirGroup interceptAirGroup = airGroups[interceptAirGroupIndex];

                return interceptAirGroup;
            }
            else
            {
                return null;
            }
        }


        
        private bool isMissionTypeEscorted(EMissionType missionType)
        {
            if (missionType == EMissionType.ATTACK_ARMOR
                || missionType == EMissionType.ATTACK_RADAR
                || missionType == EMissionType.ATTACK_SHIP
                || missionType == EMissionType.ATTACK_VEHICLE)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool isMissionTypeOffensive(EMissionType missionType)
        {
            if (missionType == EMissionType.ARMED_MARITIME_RECON
                || missionType == EMissionType.ARMED_RECON
                || missionType == EMissionType.ATTACK_ARMOR
                || missionType == EMissionType.ATTACK_RADAR
                || missionType == EMissionType.ATTACK_SHIP
                || missionType == EMissionType.ATTACK_VEHICLE
                || missionType == EMissionType.MARITIME_RECON
                || missionType == EMissionType.RECON)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool isMissionTypeAvailable(AirGroup airGroup, EMissionType missionType)
        {
            if (missionType == EMissionType.COVER)
            {
                List<AirGroup> airGroups = getAvailableOffensiveAirGroups(airGroup.ArmyIndex);
                if (airGroups.Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (missionType == EMissionType.ARMED_MARITIME_RECON)
            {
                List<GroundGroup> groundGroups = getAvailableEnemyGroundGroups(airGroup.ArmyIndex, new List<EGroundGroupType> { EGroundGroupType.Ship });
                if (groundGroups.Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (missionType == EMissionType.ARMED_RECON)
            {
                List<GroundGroup> groundGroups = getAvailableEnemyGroundGroups(airGroup.ArmyIndex, new List<EGroundGroupType> { EGroundGroupType.Armor, EGroundGroupType.Vehicle });
                if (groundGroups.Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (missionType == EMissionType.ATTACK_ARMOR)
            {
                List<GroundGroup> groundGroups = getAvailableEnemyGroundGroups(airGroup.ArmyIndex, new List<EGroundGroupType> { EGroundGroupType.Armor });
                if (groundGroups.Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (missionType == EMissionType.ATTACK_RADAR)
            {
                IList<Stationary> radars = World.GetEnemyRadars(airGroup.ArmyIndex);
                if (radars.Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (missionType == EMissionType.ATTACK_SHIP)
            {
                List<GroundGroup> groundGroups = getAvailableEnemyGroundGroups(airGroup.ArmyIndex, new List<EGroundGroupType> { EGroundGroupType.Ship });
                if (groundGroups.Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (missionType == EMissionType.ATTACK_VEHICLE)
            {
                List<GroundGroup> groundGroups = getAvailableEnemyGroundGroups(airGroup.ArmyIndex, new List<EGroundGroupType> { EGroundGroupType.Vehicle });
                if (groundGroups.Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (missionType == EMissionType.ESCORT)
            {
                List<AirGroup> airGroups = getAvailableEscortedAirGroups(airGroup.ArmyIndex);
                if (airGroups.Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (missionType == EMissionType.INTERCEPT)
            {
                List<AirGroup> airGroups = getAvailableOffensiveAirGroups(airGroup.ArmyIndex);
                if (airGroups.Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (missionType == EMissionType.MARITIME_RECON)
            {
                List<GroundGroup> groundGroups = getAvailableEnemyGroundGroups(airGroup.ArmyIndex, new List<EGroundGroupType> { EGroundGroupType.Ship });
                if (groundGroups.Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (missionType == EMissionType.RECON)
            {
                List<GroundGroup> groundGroups = getAvailableEnemyGroundGroups(airGroup.ArmyIndex, new List<EGroundGroupType> { EGroundGroupType.Armor, EGroundGroupType.Vehicle });
                if (groundGroups.Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                throw new ArgumentException();
            }
        }


        
        private void createBriefing(BriefingFile briefingFile, AirGroup airGroup, AirGroup escortAirGroup)
        {
            briefingFile.Name[airGroup.Id] = airGroup.Id;
            briefingFile.Description[airGroup.Id] = new BriefingFile.Text();

            briefingFile.Description[airGroup.Id].Sections.Add("descriptionSection", briefingFile.MissionDescription);

            if (airGroup.MissionType != null && airGroup.MissionType.HasValue)
            {
                EMissionType missionType = airGroup.MissionType.Value;
                string mainSection = missionType.ToString();
                briefingFile.Description[airGroup.Id].Sections.Add("mainSection", mainSection);
            }

            if (airGroup.Altitude != null && airGroup.Altitude.HasValue)
            {
                string altitudeSection = "Altitude: " + airGroup.Altitude + "m";

                briefingFile.Description[airGroup.Id].Sections.Add("altitudeSection", altitudeSection);
            }

            if (airGroup.Waypoints.Count > 0)
            {
                string waypointSection = "Waypoints:\n";

                int i = 1;
                foreach (AirGroupWaypoint waypoint in airGroup.Waypoints)
                {
                    if (waypoint.Type == AirGroupWaypoint.AirGroupWaypointTypes.NORMFLY)
                    {
                        waypointSection += i + ". " + GamePlay.gpSectorName(waypoint.X, waypoint.Y) + "\n";
                    }
                    else
                    {
                        waypointSection += i + ". " + GamePlay.gpSectorName(waypoint.X, waypoint.Y) + " " + waypoint.Type.ToString() + "\n";
                    }
                    i++;
                }

                briefingFile.Description[airGroup.Id].Sections.Add("waypointSection", waypointSection);
            }

            if (airGroup.EscortAirGroup != null)
            {
                string escortSection = "Escorted by: " + escortAirGroup.Id;

                briefingFile.Description[airGroup.Id].Sections.Add("escortSection", escortSection);
            }
        }
    }
}
