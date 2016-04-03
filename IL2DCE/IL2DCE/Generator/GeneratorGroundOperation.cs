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

        private Random rand = new Random();

        public IList<GroundGroup> AvailableGroundGroups = new List<GroundGroup>();

        private Core _core;
        
        private MissionFile MissionTemplate
        {
            get
            {
                return _core.MissionTemplate;
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

        public GeneratorGroundOperation(Core core, Generator generator)
        {
            _core = core;
            Generator = generator;
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
        
        
        public void CreateRandomGroundOperation(ISectionFile missionFile, GroundGroup groundGroup)
        {
            AvailableGroundGroups.Remove(groundGroup);

            IList<Point3d> friendlyMarkers = MissionTemplate.GetFriendlyMarkers(groundGroup.Army);
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
                int groundGroupIndex = rand.Next(groundGroups.Count);
                GroundGroup targetGroundGroup = groundGroups[groundGroupIndex];

                return targetGroundGroup;
            }
            else
            {
                return null;
            }
        }

        public GroundGroup getAvailableRandomEnemyGroundGroup(int armyIndex, List<EGroundGroupType> groundGroupTypes)
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

        public GroundGroup getAvailableRandomFriendlyGroundGroup(int armyIndex)
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

        public GroundGroup getAvailableRandomFriendlyGroundGroup(int armyIndex, List<EGroundGroupType> groundGroupTypes)
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
    }
}
