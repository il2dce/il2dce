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
    class GeneratorGroundOperation
    {
        private Generator Generator
        {
            get;
            set;
        }

        private IRandom Random
        {
            get
            {
                return Generator.Random;
            }
        }

        public IList<GroundGroup> AvailableGroundGroups = new List<GroundGroup>();

        public IList<Stationary> AvailableStationaries = new List<Stationary>();

        private MissionFile MissionTemplate
        {
            get;
            set;
        }

        private IGamePlay GamePlay
        {
            get;
            set;
        }

        private Config Config
        {
            get;
            set;

        }
        
        public GroundGroup getRandomTargetBasedOnRange(List<GroundGroup> availableGroundGroups, AirGroup offensiveAirGroup)
        {
            GroundGroup selectedGroundGroup = null;

            if (availableGroundGroups.Count > 1)
            {
                availableGroundGroups.Sort(new DistanceComparer(offensiveAirGroup.Position));

                Point2d position = new Point2d(offensiveAirGroup.Position.x, offensiveAirGroup.Position.y);

                // TODO: Use range of the aircraft instead of the maxDistance.
                // Problem is that range depends on loadout, so depending on loadout different targets would be available.

                Point2d last = availableGroundGroups[availableGroundGroups.Count - 1].Position;
                double maxDistance = last.distance(ref position);

                List<KeyValuePair<GroundGroup, int>> elements = new List<KeyValuePair<GroundGroup, int>>();

                int previousWeight = 0;

                foreach (GroundGroup groundGroup in availableGroundGroups)
                {
                    double distance = groundGroup.Position.distance(ref position);
                    int weight = Convert.ToInt32(Math.Ceiling(maxDistance - distance));
                    int cumulativeWeight = previousWeight + weight;
                    elements.Add(new KeyValuePair<GroundGroup, int>(groundGroup, cumulativeWeight));

                    previousWeight = cumulativeWeight;
                }

                int diceRoll = Random.Next(0, previousWeight);
                int cumulative = 0;
                for (int i = 0; i < elements.Count; i++)
                {
                    cumulative += elements[i].Value;
                    if (diceRoll <= cumulative)
                    {
                        selectedGroundGroup = elements[i].Key;
                        break;
                    }
                }
            }
            else if(availableGroundGroups.Count == 1)
            {
                selectedGroundGroup = availableGroundGroups[0];
            }           
                
            return selectedGroundGroup;
        }


        public Stationary getRandomTargetBasedOnRange(List<Stationary> availableStationaries, AirGroup offensiveAirGroup)
        {
            Stationary selectedStationary = null;

            if (availableStationaries.Count > 1)
            {
                availableStationaries.Sort(new DistanceComparer(offensiveAirGroup.Position));

                Point2d position = new Point2d(offensiveAirGroup.Position.x, offensiveAirGroup.Position.y);

                // TODO: Use range of the aircraft instead of the maxDistance.
                // Problem is that range depends on loadout, so depending on loadout different targets would be available.

                Point2d last = availableStationaries[availableStationaries.Count - 1].Position;
                double maxDistance = last.distance(ref position);

                List<KeyValuePair<Stationary, int>> elements = new List<KeyValuePair<Stationary, int>>();

                int previousWeight = 0;

                foreach (Stationary stationary in availableStationaries)
                {
                    double distance = stationary.Position.distance(ref position);
                    int weight = Convert.ToInt32(Math.Ceiling(maxDistance - distance));
                    int cumulativeWeight = previousWeight + weight;
                    elements.Add(new KeyValuePair<Stationary, int>(stationary, cumulativeWeight));

                    previousWeight = cumulativeWeight;
                }

                int diceRoll = Random.Next(0, previousWeight);
                int cumulative = 0;
                for (int i = 0; i < elements.Count; i++)
                {
                    cumulative += elements[i].Value;
                    if (diceRoll <= cumulative)
                    {
                        selectedStationary = elements[i].Key;
                        break;
                    }
                }
            }
            else if(availableStationaries.Count == 1)
            {
                selectedStationary = availableStationaries[0];
            }

            return selectedStationary;
        }



        public GeneratorGroundOperation(Generator generator, CampaignInfo campaignInfo, MissionFile missionTemplate, IGamePlay gamePlay, Config config)
        {
            Generator = generator;
            GamePlay = gamePlay;
            MissionTemplate = missionTemplate;
            Config = config;

            AvailableGroundGroups.Clear();
            AvailableStationaries.Clear();

            foreach (GroundGroup groundGroup in MissionTemplate.GroundGroups)
            {
                AvailableGroundGroups.Add(groundGroup);
            }

            foreach (Stationary stationary in MissionTemplate.Stationaries)
            {
                AvailableStationaries.Add(stationary);
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
                                GroundGroupWaypoint groundGroupWaypoint = new GroundGroupWaypointLine(aiGroundWayPoint.P.x, aiGroundWayPoint.P.y, aiGroundWayPoint.roadWidth, aiGroundWayPoint.Speed);
                                lastGroundGroupWaypoint = groundGroupWaypoint;
                                groundGroup.Waypoints.Add(groundGroupWaypoint);
                            }
                            else if (lastGroundGroupWaypoint != null)
                            {
                                // TODO: Fix calculated param

                                //string s = aiGroundWayPoint.P.x.ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat) + " " + aiGroundWayPoint.P.y.ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat) + " " + aiGroundWayPoint.P.z.ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat) + " " + aiGroundWayPoint.roadWidth.ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
                                //GroundGroupSubWaypoint groundGroupSubWaypoint = new GroundGroupSubWaypoint(s, null);
                                //lastGroundGroupWaypoint.SubWaypoints.Add(groundGroupSubWaypoint);
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
        
        
        public void CreateRandomGroundOperation(ISectionFile missionFile, GroundGroup groundGroup)
        {
            AvailableGroundGroups.Remove(groundGroup);

            if (groundGroup.Type == EGroundGroupType.Ship)
            {
                // Ships already have the correct waypoint from the mission template. Only remove some waypoints to make the position more random, but leave at least 2 waypoints.
                groundGroup.Waypoints.RemoveRange(0, Random.Next(0, groundGroup.Waypoints.Count-1));
                
                groundGroup.WriteTo(missionFile);

                generateColumnFormation(missionFile, groundGroup, 3);
            }
            else if(groundGroup.Type == EGroundGroupType.Train)
            {
                groundGroup.Waypoints.RemoveRange(0, Random.Next(0, groundGroup.Waypoints.Count-1));

                groundGroup.WriteTo(missionFile);
            }
            else
            {
                //IList<Point3d> friendlyMarkers = MissionTemplate.GetFriendlyMarkers(groundGroup.Army);
                //if (friendlyMarkers.Count > 0)
                //{
                //    List<Point3d> availableFriendlyMarkers = new List<Point3d>(friendlyMarkers);

                //    // Find closest friendly marker
                //    Point3d? closestMarker = null;
                //    foreach (Point3d marker in availableFriendlyMarkers)
                //    {
                //        if (closestMarker == null)
                //        {
                //            closestMarker = marker;
                //        }
                //        else if (closestMarker.HasValue)
                //        {
                //            Point3d p1 = new Point3d(marker.x, marker.y, marker.z);
                //            Point3d p2 = new Point3d(closestMarker.Value.x, closestMarker.Value.y, closestMarker.Value.z);
                //            if (groundGroup.Position.distance(ref p1) < groundGroup.Position.distance(ref p2))
                //            {
                //                closestMarker = marker;
                //            }
                //        }
                //    }

                //    if (closestMarker != null && closestMarker.HasValue)
                //    {
                //        availableFriendlyMarkers.Remove(closestMarker.Value);

                //        if (availableFriendlyMarkers.Count > 0)
                //        {
                //            int markerIndex = rand.Next(availableFriendlyMarkers.Count);

                //            groundGroup.Waypoints.Clear();

                //            if (groundGroup.Type == EGroundGroupType.Armor || groundGroup.Type == EGroundGroupType.Vehicle)
                //            {
                //                findPath(groundGroup, new Point2d(closestMarker.Value.x, closestMarker.Value.y), new Point2d(availableFriendlyMarkers[markerIndex].x, availableFriendlyMarkers[markerIndex].y));
                //                groundGroup.WriteTo(missionFile);
                //            }
                //            else
                //            {
                //                Point2d start = new Point2d(groundGroup.Position.x, groundGroup.Position.y);
                //                Point2d end = new Point2d(availableFriendlyMarkers[markerIndex].x, availableFriendlyMarkers[markerIndex].y);


                //                findPath(groundGroup, start, end);

                //                groundGroup.WriteTo(missionFile);
                                                                
                //                generateColumnFormation(missionFile, groundGroup, 3);
                //            }
                //        }
                //    }
                //}
            }
        }

        private static void generateColumnFormation(ISectionFile missionFile, GroundGroup groundGroup, int columnSize)
        {
            string groundGroupId = groundGroup.Id;

            for (int i = 1; i < columnSize; i++)
            {
                double xOffset = -1.0;
                double yOffset = -1.0;

                bool subWaypointUsed = false;
                Point2d p1 = groundGroup.Waypoints[0].Position;
                if (groundGroup.Waypoints[0].SubWaypoints.Count > 0)
                {
                    foreach (GroundGroupWaypoint subWaypoint in groundGroup.Waypoints[0].SubWaypoints)
                    {
                        Point2d p2 = subWaypoint.Position;
                        double distance = p1.distance(ref p2);
                        xOffset = 500 * ((p2.x - p1.x) / distance);
                        yOffset = 500 * ((p2.y - p1.y) / distance);
                        subWaypointUsed = true;
                        break;
                    }
                }
                if (subWaypointUsed == false)
                {
                    Point2d p2 = groundGroup.Waypoints[1].Position;
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
                        GroundGroupWaypoint subWaypoint = groundGroup.Waypoints[groundGroup.Waypoints.Count - 2].SubWaypoints[j];
                        
                        Point2d p2 = subWaypoint.Position;
                        double distance = p1.distance(ref p2);
                        xOffset = 500 * ((p2.x - p1.x) / distance);
                        yOffset = 500 * ((p2.y - p1.y) / distance);
                        subWaypointUsed = true;
                        break;
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

                groundGroup.Id = groundGroupId + "." + i.ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat);

                groundGroup.WriteTo(missionFile);
            }
        }


        #region GroundGroup

        public List<GroundGroup> getAvailableEnemyGroundGroups(int armyIndex)
        {
            List<GroundGroup> groundGroups = new List<GroundGroup>();
            foreach (GroundGroup groundGroup in AvailableGroundGroups)
            {
                if (groundGroup.Army != armyIndex)
                {
                    groundGroups.Add(groundGroup);
                }
            }
            return groundGroups;
        }

        public List<GroundGroup> getAvailableFriendlyGroundGroups(int armyIndex)
        {
            List<GroundGroup> groundGroups = new List<GroundGroup>();
            foreach (GroundGroup groundGroup in AvailableGroundGroups)
            {
                if (groundGroup.Army == armyIndex)
                {
                    groundGroups.Add(groundGroup);
                }
            }
            return groundGroups;
        }

        public List<GroundGroup> getAvailableEnemyGroundGroups(int armyIndex, List<EGroundGroupType> groundGroupTypes)
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

        public List<GroundGroup> getAvailableFriendlyGroundGroups(int armyIndex, List<EGroundGroupType> groundGroupTypes)
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

        public GroundGroup getAvailableRandomEnemyGroundGroup(int armyIndex)
        {
            List<GroundGroup> groundGroups = getAvailableEnemyGroundGroups(armyIndex);
            if (groundGroups.Count > 0)
            {
                int groundGroupIndex = Random.Next(groundGroups.Count);
                GroundGroup targetGroundGroup = groundGroups[groundGroupIndex];

                return targetGroundGroup;
            }
            else
            {
                return null;
            }
        }

        public GroundGroup getAvailableRandomEnemyGroundGroup(AirGroup airGroup, EMissionType missionType)
        {
            if(missionType == EMissionType.ARMED_MARITIME_RECON || missionType == EMissionType.MARITIME_RECON
                || missionType == EMissionType.ATTACK_SHIP)
            {
                return getAvailableRandomEnemyGroundGroup(airGroup, new List<EGroundGroupType> { EGroundGroupType.Ship });
            }
            else if(missionType == EMissionType.ARMED_RECON || missionType == EMissionType.RECON)
            {
                return getAvailableRandomEnemyGroundGroup(airGroup, new List<EGroundGroupType> { EGroundGroupType.Armor, EGroundGroupType.Train, EGroundGroupType.Vehicle });
            }
            else if(missionType == EMissionType.ATTACK_ARMOR)
            {
                return getAvailableRandomEnemyGroundGroup(airGroup, new List<EGroundGroupType> { EGroundGroupType.Armor });
            }
            else if (missionType == EMissionType.ATTACK_VEHICLE)
            {
                return getAvailableRandomEnemyGroundGroup(airGroup, new List<EGroundGroupType> { EGroundGroupType.Vehicle });
            }
            else if (missionType == EMissionType.ATTACK_TRAIN)
            {
                return getAvailableRandomEnemyGroundGroup(airGroup, new List<EGroundGroupType> { EGroundGroupType.Train });
            }
            else
            {
                return null;
            }
        }


        public GroundGroup getAvailableRandomEnemyGroundGroup(AirGroup airGroup, List<EGroundGroupType> groundGroupTypes)
        {
            List<GroundGroup> groundGroups = getAvailableEnemyGroundGroups(airGroup.ArmyIndex, groundGroupTypes);
            if (groundGroups.Count > 0)
            {
                //int groundGroupIndex = Random.Next(groundGroups.Count);
                //GroundGroup targetGroundGroup = groundGroups[groundGroupIndex];

                GroundGroup targetGroundGroup = getRandomTargetBasedOnRange(groundGroups, airGroup);

                return targetGroundGroup;
            }
            else
            {
                return null;
            }
        }

        public GroundGroup getAvailableRandomFriendlyGroundGroup(AirGroup airGroup)
        {
            List<GroundGroup> groundGroups = getAvailableFriendlyGroundGroups(airGroup.ArmyIndex);
            if (groundGroups.Count > 0)
            {
                //int groundGroupIndex = Random.Next(groundGroups.Count);
                //GroundGroup targetGroundGroup = groundGroups[groundGroupIndex];

                GroundGroup targetGroundGroup = getRandomTargetBasedOnRange(groundGroups, airGroup);

                return targetGroundGroup;
            }
            else
            {
                return null;
            }
        }

        public GroundGroup getAvailableRandomFriendlyGroundGroup(AirGroup airGroup, List<EGroundGroupType> groundGroupTypes)
        {
            List<GroundGroup> groundGroups = getAvailableFriendlyGroundGroups(airGroup.ArmyIndex, groundGroupTypes);
            if (groundGroups.Count > 0)
            {
                //int groundGroupIndex = Random.Next(groundGroups.Count);
                //GroundGroup targetGroundGroup = groundGroups[groundGroupIndex];

                GroundGroup targetGroundGroup = getRandomTargetBasedOnRange(groundGroups, airGroup);

                return targetGroundGroup;
            }
            else
            {
                return null;
            }
        }

#endregion

        #region Stationary


        public List<Stationary> getAvailableEnemyStationaries(int armyIndex)
        {
            List<Stationary> stationaries = new List<Stationary>();
            foreach (Stationary stationary in AvailableStationaries)
            {
                if (stationary.Army != armyIndex)
                {
                    stationaries.Add(stationary);
                }
            }
            return stationaries;
        }

        public List<Stationary> getAvailableFriendlyStationaries(int armyIndex)
        {
            List<Stationary> stationaries = new List<Stationary>();
            foreach (Stationary stationary in AvailableStationaries)
            {
                if (stationary.Army == armyIndex)
                {
                    stationaries.Add(stationary);
                }
            }
            return stationaries;
        }

        public List<Stationary> getAvailableEnemyStationaries(int armyIndex, List<EStationaryType> stationaryTypes)
        {
            List<Stationary> stationaries = new List<Stationary>();
            foreach (Stationary stationary in getAvailableEnemyStationaries(armyIndex))
            {
                if (stationaryTypes.Contains(stationary.Type))
                {
                    stationaries.Add(stationary);
                }
            }
            return stationaries;
        }

        public List<Stationary> getAvailableFriendlyStationaries(int armyIndex, List<EStationaryType> stationaryTypes)
        {
            List<Stationary> stationaries = new List<Stationary>();
            foreach (Stationary stationary in getAvailableFriendlyStationaries(armyIndex))
            {
                if (stationaryTypes.Contains(stationary.Type))
                {
                    stationaries.Add(stationary);
                }
            }
            return stationaries;
        }

        public Stationary getAvailableRandomEnemyStationary(int armyIndex)
        {
            List<Stationary> stationaries = getAvailableEnemyStationaries(armyIndex);
            if (stationaries.Count > 0)
            {
                int stationaryIndex = Random.Next(stationaries.Count);
                Stationary targetStationary = stationaries[stationaryIndex];

                return targetStationary;
            }
            else
            {
                return null;
            }
        }

        public Stationary getAvailableRandomEnemyStationary(AirGroup airGroup, EMissionType missionType)
        {
            if (missionType == EMissionType.ATTACK_AIRCRAFT)
            {
                return getAvailableRandomEnemyStationary(airGroup, new List<EStationaryType> { EStationaryType.Aircraft });
            }
            else if (missionType == EMissionType.ATTACK_ARTILLERY)
            {
                return getAvailableRandomEnemyStationary(airGroup, new List<EStationaryType> { EStationaryType.Artillery });
            }
            else if (missionType == EMissionType.ATTACK_RADAR)
            {
                return getAvailableRandomEnemyStationary(airGroup, new List<EStationaryType> { EStationaryType.Radar });
            }
            else if (missionType == EMissionType.ATTACK_DEPOT)
            {
                return getAvailableRandomEnemyStationary(airGroup, new List<EStationaryType> { EStationaryType.Depot });
            }
            else
            {
                return null;
            }
        }


        public Stationary getAvailableRandomEnemyStationary(AirGroup airGroup, List<EStationaryType> stationaryTypes)
        {
            List<Stationary> stationaries = getAvailableEnemyStationaries(airGroup.ArmyIndex, stationaryTypes);
            if (stationaries.Count > 0)
            {
                //int groundGroupIndex = Random.Next(groundGroups.Count);
                //GroundGroup targetGroundGroup = groundGroups[groundGroupIndex];

                Stationary targetStationary = getRandomTargetBasedOnRange(stationaries, airGroup);

                return targetStationary;
            }
            else
            {
                return null;
            }
        }

        public Stationary getAvailableRandomFriendlyStationary(AirGroup airGroup)
        {
            List<Stationary> stationaries = getAvailableFriendlyStationaries(airGroup.ArmyIndex);
            if (stationaries.Count > 0)
            {
                //int groundGroupIndex = Random.Next(groundGroups.Count);
                //GroundGroup targetGroundGroup = groundGroups[groundGroupIndex];

                Stationary targetStationary = getRandomTargetBasedOnRange(stationaries, airGroup);

                return targetStationary;
            }
            else
            {
                return null;
            }
        }

        public Stationary getAvailableRandomFriendlyStationary(AirGroup airGroup, List<EStationaryType> stationaryTypes)
        {
            List<Stationary> stationaries = getAvailableFriendlyStationaries(airGroup.ArmyIndex, stationaryTypes);
            if (stationaries.Count > 0)
            {
                //int groundGroupIndex = Random.Next(groundGroups.Count);
                //GroundGroup targetGroundGroup = groundGroups[groundGroupIndex];

                Stationary targetStationary = getRandomTargetBasedOnRange(stationaries, airGroup);

                return targetStationary;
            }
            else
            {
                return null;
            }
        }


        #endregion
    }
}
