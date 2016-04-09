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
using maddox.game.world;
using maddox.GP;

namespace IL2DCE
{
    public class AirGroup
    {
        #region Public constructors

        public AirGroup(ISectionFile sectionFile, string id)
        {
            // airGroupId = <airGroupKey>.<squadronIndex><flightMask>

            // AirGroupKey
            AirGroupKey = id.Substring(0, id.IndexOf("."));

            // SquadronIndex
            int.TryParse(id.Substring(id.LastIndexOf(".") + 1, 1), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture.NumberFormat, out _squadronIndex);

            // Flight
            for (int i = 0; i < 4; i++)
            {
                if (sectionFile.exist(id, "Flight" + i.ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat)))
                {
                    string acNumberLine = sectionFile.get(id, "Flight" + i.ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat));
                    string[] acNumberList = acNumberLine.Split(new char[] { ' ' });
                    if (acNumberList != null && acNumberList.Length > 0)
                    {
                        List<string> acNumbers = new List<string>();
                        Flights.Add(i, acNumbers);
                        for (int j = 0; j < acNumberList.Length; j++)
                        {
                            acNumbers.Add(acNumberList[j]);
                        }
                    }
                }
            }

            // Class
            Class = sectionFile.get(id, "Class");

            // Formation
            Formation = sectionFile.get(id, "Formation");

            // CallSign
            int.TryParse(sectionFile.get(id, "CallSign"), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture.NumberFormat, out _callSign);

            // Fuel
            int.TryParse(sectionFile.get(id, "Fuel"), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture.NumberFormat, out _fuel);

            // Weapons
            string weaponsLine = sectionFile.get(id, "Weapons");
            string[] weaponsList = weaponsLine.Split(new char[] { ' ' });
            if (weaponsList != null && weaponsList.Length > 0)
            {
                Weapons = new int[weaponsList.Length];
                for (int i = 0; i < weaponsList.Length; i++)
                {
                    int.TryParse(weaponsList[i], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture.NumberFormat, out Weapons[i]);
                }
            }

            for (int i = 0; i < sectionFile.lines(id); i++)
            {
                string key;
                string value;
                sectionFile.get(id, i, out key, out value);
                if (key == "Detonator")
                {
                    this._detonator.Add(value);
                }
            }

            // Belt
            // TODO: Parse belt

            // Skill
            // TODO: Parse skill

            // Waypoints            
            for (int i = 0; i < sectionFile.lines(Id + "_Way"); i++)
            {
                AirGroupWaypoint waypoint = new AirGroupWaypoint(sectionFile, Id, i);
                Waypoints.Add(waypoint);
            }

            if (Waypoints.Count > 0)
            {
                Position = new Point3d(Waypoints[0].X, Waypoints[0].Y, Waypoints[0].Z);
                if (Waypoints[0].Type == AirGroupWaypoint.AirGroupWaypointTypes.TAKEOFF)
                {
                    Airstart = false;
                }
                else if (Waypoints[0].Type == AirGroupWaypoint.AirGroupWaypointTypes.NORMFLY)
                {
                    Airstart = true;
                }
            }
        }

        #endregion

        #region Public properties

        public string AirGroupKey
        {
            get;
            set;
        }

        public int SquadronIndex
        {
            get
            {
                return _squadronIndex;
            }
            set
            {
                _squadronIndex = value;
            }
        }
        private int _squadronIndex;

        public System.Collections.Generic.IDictionary<int, System.Collections.Generic.IList<string>> Flights
        {
            get
            {
                return _flights;
            }
        }
        System.Collections.Generic.IDictionary<int, System.Collections.Generic.IList<string>> _flights = new Dictionary<int, IList<string>>();

        public string Class
        {
            get;
            set;
        }

        public string Formation
        {
            get;
            set;
        }

        public int CallSign
        {
            get
            {
                return _callSign;
            }
            set
            {
                _callSign = value;
            }
        }
        private int _callSign;

        public int Fuel
        {
            get
            {
                return this._fuel;
            }
            set
            {
                this._fuel = value;
            }
        }
        private int _fuel;

        public int[] Weapons
        {
            get;
            set;
        }

        public IList<string> Detonator
        {
            get
            {
                return this._detonator;
            }
            set
            {
                this._detonator = value;
            }
        }
        private IList<string> _detonator = new List<string>();

        public Point3d Position
        {
            get
            {
                return _position;
            }
            set
            {
                _position = value;
            }
        }
        private Point3d _position;

        public double Speed
        {
            get
            {
                // TODO: Use aicraft info to determine speed
                return 300.0;
            }
        }

        public bool Airstart
        {
            get;
            set;
        }

        public bool SetOnParked
        {
            get;
            set;
        }

        public string Briefing
        {
            get;
            set;
        }

        public string Id
        {
            get
            {
                int flightMask = 0x0;

                foreach (int flightIndex in Flights.Keys)
                {
                    if (Flights[flightIndex].Count > 0)
                    {
                        int bit = (0x1 << flightIndex);
                        flightMask = (flightMask | bit);
                    }
                }

                return AirGroupKey + "." + SquadronIndex.ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat) + flightMask.ToString("X");
            }
        }

        public int ArmyIndex
        {
            get
            {
                if (IL2DCE.AirGroupInfo.GetAirGroupInfo(1, AirGroupKey) != null)
                {
                    return 1;
                }
                else if (IL2DCE.AirGroupInfo.GetAirGroupInfo(2, AirGroupKey) != null)
                {
                    return 2;
                }
                else
                {
                    return 0;
                }
            }
        }
        
        public AirGroupInfo AirGroupInfo
        {
            get
            {
                return AirGroupInfo.GetAirGroupInfo(AirGroupKey);
            }
        }

        public IList<AirGroupWaypoint> Waypoints
        {
            get
            {
                return _waypoints;
            }
        }
        IList<AirGroupWaypoint> _waypoints = new List<AirGroupWaypoint>();
        
        public double? Altitude
        {
            get
            {
                return this._altitude;
            }
            set
            {
                this._altitude = value;
            }
        }

        public AirGroup EscortAirGroup
        {
            get
            {
                return _escortAirGroup;
            }
            set
            {
                _escortAirGroup = value;
            }
        }

        public Stationary TargetStationary
        {
            get
            {
                return _targetStationary;
            }
            set
            {
                _targetStationary = value;
            }
        }

        public GroundGroup TargetGroundGroup
        {
            get
            {
                return _targetGroundGroup;
            }
            set
            {
                _targetGroundGroup = value;
            }
        }

        public AirGroup TargetAirGroup
        {
            get
            {
                return _targetAirGroup;
            }
            set
            {
                _targetAirGroup = value;
            }
        }

        //public Point2d? TargetArea
        //{
        //    get
        //    {
        //        return _targetArea;
        //    }
        //    set
        //    {
        //        _targetArea = value;
        //    }
        //}

        #endregion

        #region Private methods

        private double distanceBetween(AirGroupWaypoint start, AirGroupWaypoint end)
        {
            double distanceStart = distanceTo(start);
            double distanceEnd = distanceTo(end);
            return distanceEnd - distanceStart;
        }

        private double distanceTo(AirGroupWaypoint waypoint)
        {
            double distance = 0.0;
            AirGroupWaypoint previousWaypoint = null;
            if (Waypoints.Contains(waypoint))
            {
                foreach (AirGroupWaypoint wp in Waypoints)
                {
                    if (previousWaypoint == null)
                    {
                        distance = 0.0;
                    }
                    else
                    {
                        Point3d p = wp.Position;
                        distance += previousWaypoint.Position.distance(ref p);
                    }
                    previousWaypoint = wp;

                    if (wp == waypoint)
                    {
                        break;
                    }
                }
            }

            return distance;
        }

        private void createStartWaypoints()
        {
            if (!Airstart)
            {
                Waypoints.Add(new AirGroupWaypoint(AirGroupWaypoint.AirGroupWaypointTypes.TAKEOFF, Position, 0.0));
            }
            else
            {
                Waypoints.Add(new AirGroupWaypoint(AirGroupWaypoint.AirGroupWaypointTypes.NORMFLY, Position, Speed));
            }
        }

        private void createEndWaypoints(AiAirport landingAirport = null)
        {
            if (landingAirport != null)
            {
                Waypoints.Add(new AirGroupWaypoint(AirGroupWaypoint.AirGroupWaypointTypes.LANDING, landingAirport.Pos(), 0.0));
            }
            else
            {
                if (!Airstart)
                {
                    Waypoints.Add(new AirGroupWaypoint(AirGroupWaypoint.AirGroupWaypointTypes.LANDING, Position, 0.0));
                }
                else
                {
                    Waypoints.Add(new AirGroupWaypoint(AirGroupWaypoint.AirGroupWaypointTypes.NORMFLY, Position, Speed));
                }
            }
        }

        private void createInbetweenWaypoints(Point3d a, Point3d b)
        {
            Point3d p1 = new Point3d(a.x + 0.25 * (b.x - a.x), a.y + 0.25 * (b.y - a.y), a.z + 1.00 * (b.z - a.z));
            Point3d p2 = new Point3d(a.x + 0.50 * (b.x - a.x), a.y + 0.50 * (b.y - a.y), a.z + 1.00 * (b.z - a.z));
            Point3d p3 = new Point3d(a.x + 0.75 * (b.x - a.x), a.y + 0.75 * (b.y - a.y), a.z + 1.00 * (b.z - a.z));

            Waypoints.Add(new AirGroupWaypoint(AirGroupWaypoint.AirGroupWaypointTypes.NORMFLY, p1, 300.0));
            Waypoints.Add(new AirGroupWaypoint(AirGroupWaypoint.AirGroupWaypointTypes.NORMFLY, p2, 300.0));
            Waypoints.Add(new AirGroupWaypoint(AirGroupWaypoint.AirGroupWaypointTypes.NORMFLY, p3, 300.0));
        }

        private void createStartInbetweenPoints(Point3d target)
        {
            createInbetweenWaypoints(Position, target);
        }

        private void createEndInbetweenPoints(Point3d target, AiAirport landingAirport = null)
        {
            if (landingAirport != null)
            {
                Point3d point = new Point3d(landingAirport.Pos().x, landingAirport.Pos().y, target.z);
                createInbetweenWaypoints(target, point);
            }
            else
            {
                Point3d point = new Point3d(Position.x, Position.y, target.z);
                createInbetweenWaypoints(target, point);
            }
        }

        private void reset()
        {
            Waypoints.Clear();

            this._altitude = null;
            this.EscortAirGroup = null;
            this.TargetAirGroup = null;
            this.TargetGroundGroup = null;
            this.TargetStationary = null;
            //this.TargetArea = null;
        }

        #endregion

        #region Public methods

        public void WriteTo(ISectionFile sectionFile, Config config)
        {
            if (Waypoints.Count > 0)
            {
                sectionFile.add("AirGroups", Id, "");

                foreach (int flightIndex in Flights.Keys)
                {
                    if (Flights[flightIndex].Count > 0)
                    {
                        string acNumberLine = "";
                        foreach (string acNumber in Flights[flightIndex])
                        {
                            acNumberLine += acNumber + " ";
                        }
                        sectionFile.add(Id, "Flight" + flightIndex, acNumberLine.TrimEnd());
                    }
                }

                sectionFile.add(Id, "Class", Class);
                sectionFile.add(Id, "Formation", Formation);
                sectionFile.add(Id, "CallSign", CallSign.ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat));
                sectionFile.add(Id, "Fuel", Fuel.ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat));

                if (Weapons != null && Weapons.Length > 0)
                {
                    string weaponsLine = "";
                    foreach (int weapon in Weapons)
                    {
                        weaponsLine += weapon.ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat) + " ";
                    }
                    sectionFile.add(Id, "Weapons", weaponsLine.TrimEnd());
                }

                if (Detonator != null && Detonator.Count > 0)
                {
                    foreach (string detonator in Detonator)
                    {
                        sectionFile.add(Id, "Detonator", detonator);
                    }
                }

                if (config.SpawnParked == true)
                {
                    sectionFile.add(Id, "SetOnPark", "1");
                }
                else
                {
                    sectionFile.add(Id, "SetOnPark", "0");
                }


                // TODO: Use tweaked AI settings http://bobgamehub.blogspot.de/2012/03/ai-settings-in-cliffs-of-dover.html
                //
                //      // Fighter (the leading space is important)
                //      string frookie = " 0.30 0.11 0.78 0.40 0.64 0.85 0.85 0.21";
                //      string faverage = " 0.32 0.12 0.87 0.60 0.74 0.90 0.90 0.31";
                //      string fexperienced = " 0.52 0.13 0.89 0.70 0.74 0.95 0.92 0.31";
                //      string fveteran = " 0.73 0.14 0.92 0.80 0.74 1 0.95 0.41";
                //      string face = " 0.93 0.15 0.96 0.92 0.84 1 1 0.51";

                //      // Fighter Bomber (the leading space is important)
                //      string xrookie = " 0.30 0.11 0.78 0.30 0.74 0.85 0.90 0.40";
                //      string xaverage = " 0.32 0.12 0.87 0.35 0.74 0.90 0.95 0.52";
                //      string xexperienced = " 0.52 0.13 0.89 0.38 0.74 0.92 0.95 0.52";
                //      string xveteran = " 0.73 0.14 0.92 0.40 0.74 0.95 0.95 0.55";
                //      string xace = " 0.93 0.15 0.96 0.45 0.74 1 1 0.65";

                //      // Bomber (the leading space is important)
                //      string brookie = " 0.30 0.11 0.78 0.20 0.74 0.85 0.90 0.88";
                //      string baverage = " 0.32 0.12 0.87 0.25 0.74 0.90 0.95 0.91";
                //      string bexperienced = " 0.52 0.13 0.89 0.28 0.74 0.92 0.95 0.91";
                //      string bveteran = " 0.73 0.14 0.92 0.30 0.74 0.95 0.95 0.95";
                //      string bace = " 0.93 0.15 0.96 0.35 0.74 1 1 0.97";

                sectionFile.add(Id, "Skill", "0.3 0.3 0.3 0.3 0.3 0.3 0.3 0.3");

                foreach (AirGroupWaypoint waypoint in Waypoints)
                {
                    if (waypoint.Target == null)
                    {
                        sectionFile.add(Id + "_Way", waypoint.Type.ToString(), waypoint.X.ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat) + " " + waypoint.Y.ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat) + " " + waypoint.Z.ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat) + " " + waypoint.V.ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat));
                    }
                    else
                    {
                        sectionFile.add(Id + "_Way", waypoint.Type.ToString(), waypoint.X.ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat) + " " + waypoint.Y.ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat) + " " + waypoint.Z.ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat) + " " + waypoint.V.ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat) + " " + waypoint.Target);
                    }
                }

                sectionFile.add(Id, "Briefing", this.Id);
            }
        }

        public override string ToString()
        {
            return AirGroupKey + "." + SquadronIndex;
        }

        //public void Transfer(double altitude, AiAirport landingAirport = null)
        //{
        //    this.reset();
        //    this.Altitude = altitude;

        //    createStartWaypoints();

        //    Point3d target = new Point3d(Position.x, Position.y, altitude);
        //    createEndInbetweenPoints(target, landingAirport);

        //    createEndWaypoints(landingAirport);
        //}

        public void Cover(GroundGroup targetGroundGroup, double altitude, AiAirport landingAirport = null)
        {
            this.reset();
            this.Altitude = altitude;
            this.TargetGroundGroup = targetGroundGroup;

            if (targetGroundGroup.Waypoints.Count > 0)
            {
                createStartWaypoints();

                Point3d p = new Point3d(targetGroundGroup.Waypoints[0].Position.x, targetGroundGroup.Waypoints[0].Position.y, altitude);

                createStartInbetweenPoints(p);

                Waypoints.Add(new AirGroupWaypoint(AirGroupWaypoint.AirGroupWaypointTypes.COVER, p.x, p.y, altitude, 300.0));
                AirGroupWaypoint start = Waypoints[Waypoints.Count - 1];

                while (distanceBetween(start, Waypoints[Waypoints.Count - 1]) < 200000.0)
                {
                    Waypoints.Add(new AirGroupWaypoint(AirGroupWaypoint.AirGroupWaypointTypes.COVER, p.x + 5000.0, p.y + 5000.0, altitude, 300.0));
                    Waypoints.Add(new AirGroupWaypoint(AirGroupWaypoint.AirGroupWaypointTypes.COVER, p.x + 5000.0, p.y - 5000.0, altitude, 300.0));
                    Waypoints.Add(new AirGroupWaypoint(AirGroupWaypoint.AirGroupWaypointTypes.COVER, p.x - 5000.0, p.y - 5000.0, altitude, 300.0));
                    Waypoints.Add(new AirGroupWaypoint(AirGroupWaypoint.AirGroupWaypointTypes.COVER, p.x - 5000.0, p.y + 5000.0, altitude, 300.0));
                }

                createEndInbetweenPoints(p, landingAirport);

                createEndWaypoints(landingAirport);
            }
        }

        public void Cover(Stationary targetStationary, double altitude, AiAirport landingAirport = null)
        {
            this.reset();
            this.Altitude = altitude;
            this.TargetStationary = targetStationary;

            createStartWaypoints();

            Point3d p = new Point3d(targetStationary.Position.x, targetStationary.Position.y, altitude);
            createStartInbetweenPoints(p);

            Waypoints.Add(new AirGroupWaypoint(AirGroupWaypoint.AirGroupWaypointTypes.COVER, targetStationary.Position.x, targetStationary.Position.y, altitude, 300.0));
            AirGroupWaypoint start = Waypoints[Waypoints.Count - 1];

            while (distanceBetween(start, Waypoints[Waypoints.Count - 1]) < 200000.0)
            {
                Waypoints.Add(new AirGroupWaypoint(AirGroupWaypoint.AirGroupWaypointTypes.COVER, targetStationary.Position.x + 5000.0, targetStationary.Position.y + 5000.0, altitude, 300.0));
                Waypoints.Add(new AirGroupWaypoint(AirGroupWaypoint.AirGroupWaypointTypes.COVER, targetStationary.Position.x + 5000.0, targetStationary.Position.y - 5000.0, altitude, 300.0));
                Waypoints.Add(new AirGroupWaypoint(AirGroupWaypoint.AirGroupWaypointTypes.COVER, targetStationary.Position.x - 5000.0, targetStationary.Position.y - 5000.0, altitude, 300.0));
                Waypoints.Add(new AirGroupWaypoint(AirGroupWaypoint.AirGroupWaypointTypes.COVER, targetStationary.Position.x - 5000.0, targetStationary.Position.y + 5000.0, altitude, 300.0));
            }

            createEndInbetweenPoints(p, landingAirport);

            createEndWaypoints(landingAirport);
        }

        //public void Cover(Point2d targetArea, double altitude, AiAirport landingAirport = null)
        //{
        //    this.reset();
        //    this.Altitude = altitude;
        //    this.TargetArea = targetArea;

        //    createStartWaypoints();

        //    Point3d p = new Point3d(targetArea.x, targetArea.y, altitude);
        //    createStartInbetweenPoints(p);

        //    Waypoints.Add(new AirGroupWaypoint(AirGroupWaypoint.AirGroupWaypointTypes.COVER, targetArea.x, targetArea.y, altitude, 300.0));
        //    AirGroupWaypoint start = Waypoints[Waypoints.Count - 1];

        //    while (distanceBetween(start, Waypoints[Waypoints.Count - 1]) < 200000.0)
        //    {
        //        Waypoints.Add(new AirGroupWaypoint(AirGroupWaypoint.AirGroupWaypointTypes.COVER, targetArea.x + 5000.0, targetArea.y + 5000.0, altitude, 300.0));
        //        Waypoints.Add(new AirGroupWaypoint(AirGroupWaypoint.AirGroupWaypointTypes.COVER, targetArea.x + 5000.0, targetArea.y - 5000.0, altitude, 300.0));
        //        Waypoints.Add(new AirGroupWaypoint(AirGroupWaypoint.AirGroupWaypointTypes.COVER, targetArea.x - 5000.0, targetArea.y - 5000.0, altitude, 300.0));
        //        Waypoints.Add(new AirGroupWaypoint(AirGroupWaypoint.AirGroupWaypointTypes.COVER, targetArea.x - 5000.0, targetArea.y + 5000.0, altitude, 300.0));
        //    }

        //    createEndInbetweenPoints(p, landingAirport);

        //    createEndWaypoints(landingAirport);
        //}

        //public void Hunting(Point2d targetArea, double altitude, AiAirport landingAirport = null)
        //{
        //    this.reset();
        //    this.Altitude = altitude;
        //    this.TargetArea = targetArea;

        //    createStartWaypoints();

        //    Point3d p = new Point3d(targetArea.x, targetArea.y, altitude);
        //    createStartInbetweenPoints(p);

        //    Waypoints.Add(new AirGroupWaypoint(AirGroupWaypoint.AirGroupWaypointTypes.HUNTING, targetArea.x, targetArea.y, altitude, 300.0));

        //    createEndInbetweenPoints(p, landingAirport);
        //    createEndWaypoints(landingAirport);
        //}

        //public void GroundAttack(Point2d targetArea, double altitude, AirGroup escortAirGroup = null, AiAirport landingAirport = null)
        //{
        //    this.reset();
        //    this.Altitude = altitude;
        //    this.TargetArea = targetArea;
        //    this.EscortAirGroup = escortAirGroup;

        //    Point3d? rendevouzPosition = null;
        //    if (EscortAirGroup != null)
        //    {
        //        rendevouzPosition = new Point3d(Position.x + 0.50 * (EscortAirGroup.Position.x - Position.x), Position.y + 0.50 * (EscortAirGroup.Position.y - Position.y), altitude);
        //    }

        //    Waypoints.Clear();

        //    createStartWaypoints();

        //    Point3d p = new Point3d(targetArea.x, targetArea.y, altitude);
        //    if (rendevouzPosition != null && rendevouzPosition.HasValue)
        //    {
        //        Waypoints.Add(new AirGroupWaypoint(AirGroupWaypoint.AirGroupWaypointTypes.NORMFLY, rendevouzPosition.Value, 300.0));
                
        //        createInbetweenWaypoints(rendevouzPosition.Value, p);
        //    }
        //    else
        //    {
        //        createStartInbetweenPoints(p);
        //    }

        //    Waypoints.Add(new AirGroupWaypoint(AirGroupWaypoint.AirGroupWaypointTypes.GATTACK_POINT, targetArea.x, targetArea.y, altitude, 300.0));

        //    createEndInbetweenPoints(p, landingAirport);
        //    createEndWaypoints(landingAirport);
        //}

        public void GroundAttack(Stationary targetStationary, double altitude, AirGroup escortAirGroup = null, AiAirport landingAirport = null)
        {
            this.reset();
            this.Altitude = altitude;
            this.TargetStationary = targetStationary;
            this.EscortAirGroup = escortAirGroup;

            Point3d? rendevouzPosition = null;
            if (EscortAirGroup != null)
            {
                rendevouzPosition = new Point3d(Position.x + 0.50 * (EscortAirGroup.Position.x - Position.x), Position.y + 0.50 * (EscortAirGroup.Position.y - Position.y), altitude);
            }
            
            createStartWaypoints();

            if (rendevouzPosition != null && rendevouzPosition.HasValue)
            {
                Waypoints.Add(new AirGroupWaypoint(AirGroupWaypoint.AirGroupWaypointTypes.NORMFLY, rendevouzPosition.Value, 300.0));
                Point3d pStart = new Point3d(targetStationary.X, targetStationary.Y, altitude);
                createInbetweenWaypoints(rendevouzPosition.Value, pStart);
            }
            else
            {
                Point3d pStart = new Point3d(targetStationary.X, targetStationary.Y, altitude);
                createStartInbetweenPoints(pStart);
            }

            Waypoints.Add(new AirGroupWaypoint(AirGroupWaypoint.AirGroupWaypointTypes.GATTACK_TARG, targetStationary.X, targetStationary.Y, altitude, 300.0, targetStationary.Id));
            
            Point3d pEnd = new Point3d(targetStationary.X, targetStationary.Y, altitude);
            createEndInbetweenPoints(pEnd, landingAirport);
            
            createEndWaypoints(landingAirport);
        }

        public void GroundAttack(GroundGroup targetGroundGroup, double altitude, AirGroup escortAirGroup = null, AiAirport landingAirport = null)
        {
            this.reset();
            this.Altitude = altitude;
            this.TargetGroundGroup= targetGroundGroup;
            this.EscortAirGroup = escortAirGroup;

            Point3d? rendevouzPosition = null;
            if (EscortAirGroup != null)
            {
                rendevouzPosition = new Point3d(Position.x + 0.50 * (EscortAirGroup.Position.x - Position.x), Position.y + 0.50 * (EscortAirGroup.Position.y - Position.y), altitude);
            }

            if (targetGroundGroup.Waypoints.Count > 0)
            {
                createStartWaypoints();

                if (rendevouzPosition != null && rendevouzPosition.HasValue)
                {
                    Waypoints.Add(new AirGroupWaypoint(AirGroupWaypoint.AirGroupWaypointTypes.NORMFLY, rendevouzPosition.Value, 300.0));
                    Point3d pStart = new Point3d(targetGroundGroup.Waypoints[0].Position.x, targetGroundGroup.Waypoints[0].Position.y, altitude);
                    createInbetweenWaypoints(rendevouzPosition.Value, pStart);
                }
                else
                {
                    Point3d pStart = new Point3d(targetGroundGroup.Waypoints[0].Position.x, targetGroundGroup.Waypoints[0].Position.y, altitude);
                    createStartInbetweenPoints(pStart);
                }

                GroundGroupWaypoint lastGroundGroupWaypoint = null;
                AirGroupWaypoint start = null;
                foreach (GroundGroupWaypoint groundGroupWaypoint in targetGroundGroup.Waypoints)
                {
                    lastGroundGroupWaypoint = groundGroupWaypoint;
                    Waypoints.Add(new AirGroupWaypoint(AirGroupWaypoint.AirGroupWaypointTypes.GATTACK_TARG, groundGroupWaypoint.Position.x, groundGroupWaypoint.Position.y, altitude, 300.0, targetGroundGroup.Id + " " + targetGroundGroup.Waypoints.IndexOf(groundGroupWaypoint)));
                    if (start == null)
                    {
                        start = Waypoints[Waypoints.Count - 1];
                    }
                    else
                    {
                        if (distanceBetween(start, Waypoints[Waypoints.Count - 1]) > 20000.0)
                        {
                            break;
                        }
                    }
                }

                if (lastGroundGroupWaypoint != null)
                {
                    Point3d pEnd = new Point3d(lastGroundGroupWaypoint.Position.x, lastGroundGroupWaypoint.Position.y, altitude);
                    createEndInbetweenPoints(pEnd, landingAirport);
                }

                createEndWaypoints(landingAirport);
            }
        }

        //public void Recon(Point2d targetArea, double altitude, AiAirport landingAirport = null)
        //{
        //    this.reset();
        //    this.Altitude = altitude;
        //    this.TargetArea = targetArea;

        //    Point3d p = new Point3d(targetArea.x, targetArea.y, altitude);

        //    createStartWaypoints();
        //    createStartInbetweenPoints(p);

        //    Waypoints.Add(new AirGroupWaypoint(AirGroupWaypoint.AirGroupWaypointTypes.RECON, targetArea.x, targetArea.y, altitude, 300.0));

        //    createEndInbetweenPoints(p, landingAirport);
        //    createEndWaypoints(landingAirport);
        //}

        public void Recon(GroundGroup targetGroundGroup, double altitude, AirGroup escortAirGroup = null, AiAirport landingAirport = null)
        {
            this.reset();
            this.Altitude = altitude;
            this.TargetGroundGroup = targetGroundGroup;
            this.EscortAirGroup = escortAirGroup;

            Point3d? rendevouzPosition = null;
            if (EscortAirGroup != null)
            {
                rendevouzPosition = new Point3d(Position.x + 0.50 * (EscortAirGroup.Position.x - Position.x), Position.y + 0.50 * (EscortAirGroup.Position.y - Position.y), altitude);
            }

            Waypoints.Clear();

            if (targetGroundGroup.Waypoints.Count > 0)
            {
                createStartWaypoints();

                if (rendevouzPosition != null && rendevouzPosition.HasValue)
                {
                    Waypoints.Add(new AirGroupWaypoint(AirGroupWaypoint.AirGroupWaypointTypes.NORMFLY, rendevouzPosition.Value, 300.0));
                    Point3d pStart = new Point3d(targetGroundGroup.Waypoints[0].Position.x, targetGroundGroup.Waypoints[0].Position.y, altitude);
                    createInbetweenWaypoints(rendevouzPosition.Value, pStart);                    
                }
                else
                {
                    Point3d pStart = new Point3d(targetGroundGroup.Waypoints[0].Position.x, targetGroundGroup.Waypoints[0].Position.y, altitude);
                    createStartInbetweenPoints(pStart);
                }

                GroundGroupWaypoint lastGroundGroupWaypoint = null;
                AirGroupWaypoint start = null;
                foreach (GroundGroupWaypoint groundGroupWaypoint in targetGroundGroup.Waypoints)
                {
                    lastGroundGroupWaypoint = groundGroupWaypoint;
                    Waypoints.Add(new AirGroupWaypoint(AirGroupWaypoint.AirGroupWaypointTypes.RECON, groundGroupWaypoint.Position.x, groundGroupWaypoint.Position.y, altitude, 300.0, targetGroundGroup.Id + " " + targetGroundGroup.Waypoints.IndexOf(groundGroupWaypoint)));
                    if (start == null)
                    {
                        start = Waypoints[Waypoints.Count - 1];
                    }
                    else
                    {
                        if (distanceBetween(start, Waypoints[Waypoints.Count - 1]) > 20000.0)
                        {
                            break;
                        }
                    }
                }

                if (lastGroundGroupWaypoint != null)
                {
                    Point3d pEnd = new Point3d(lastGroundGroupWaypoint.Position.x, lastGroundGroupWaypoint.Position.y, altitude);
                    createEndInbetweenPoints(pEnd, landingAirport);
                }

                createEndWaypoints(landingAirport);
            }
        }

        public void Escort(AirGroup targetAirGroup, AiAirport landingAirport = null)
        {
            this.reset();
            this.Altitude = targetAirGroup.Altitude;
            this.TargetAirGroup = targetAirGroup;

            createStartWaypoints();

            foreach (AirGroupWaypoint waypoint in targetAirGroup.Waypoints)
            {
                if (waypoint.Type != AirGroupWaypoint.AirGroupWaypointTypes.TAKEOFF && waypoint.Type != AirGroupWaypoint.AirGroupWaypointTypes.LANDING)
                {
                    Waypoints.Add(new AirGroupWaypoint(AirGroupWaypoint.AirGroupWaypointTypes.ESCORT, waypoint.X, waypoint.Y, waypoint.Z, 300.0, targetAirGroup.Id + " " + targetAirGroup.Waypoints.IndexOf(waypoint)));
                }
            }

            createEndWaypoints(landingAirport);
        }

        public void Intercept(AirGroup targetAirGroup, AiAirport landingAirport = null)
        {
            this.reset();
            this.Altitude = targetAirGroup.Altitude;
            this.TargetAirGroup = targetAirGroup;

            createStartWaypoints();

            AirGroupWaypoint interceptWaypoint = null;
            AirGroupWaypoint closestInterceptWaypoint = null;
            foreach (AirGroupWaypoint waypoint in targetAirGroup.Waypoints)
            {
                Point3d p = Position;
                if (targetAirGroup.distanceTo(waypoint) > waypoint.Position.distance(ref p))
                {
                    interceptWaypoint = waypoint;
                    break;
                }
                else
                {
                    if (closestInterceptWaypoint == null)
                    {
                        closestInterceptWaypoint = waypoint;
                    }
                    else
                    {
                        if (targetAirGroup.distanceTo(waypoint) < closestInterceptWaypoint.Position.distance(ref p))
                        {
                            closestInterceptWaypoint = waypoint;
                        }
                    }
                }
            }

            if (interceptWaypoint != null)
            {
                createStartInbetweenPoints(interceptWaypoint.Position);
                Waypoints.Add(new AirGroupWaypoint(AirGroupWaypoint.AirGroupWaypointTypes.AATTACK_BOMBERS, interceptWaypoint.X, interceptWaypoint.Y, interceptWaypoint.Z, 300.0, targetAirGroup.Id + " " + targetAirGroup.Waypoints.IndexOf(interceptWaypoint)));


                if (targetAirGroup.Waypoints.IndexOf(interceptWaypoint) - 1 >= 0)
                {
                    AirGroupWaypoint nextInterceptWaypoint = targetAirGroup.Waypoints[targetAirGroup.Waypoints.IndexOf(interceptWaypoint) - 1];
                    Waypoints.Add(new AirGroupWaypoint(AirGroupWaypoint.AirGroupWaypointTypes.AATTACK_BOMBERS, nextInterceptWaypoint.X, nextInterceptWaypoint.Y, nextInterceptWaypoint.Z, 300.0, targetAirGroup.Id + " " + targetAirGroup.Waypoints.IndexOf(nextInterceptWaypoint)));

                    createEndInbetweenPoints(nextInterceptWaypoint.Position, landingAirport);
                }
                else
                {
                    createEndInbetweenPoints(interceptWaypoint.Position, landingAirport);
                }
            }
            else if (closestInterceptWaypoint != null)
            {
                createStartInbetweenPoints(closestInterceptWaypoint.Position);
                Waypoints.Add(new AirGroupWaypoint(AirGroupWaypoint.AirGroupWaypointTypes.AATTACK_BOMBERS, closestInterceptWaypoint.X, closestInterceptWaypoint.Y, closestInterceptWaypoint.Z, 300.0, targetAirGroup.Id + " " + targetAirGroup.Waypoints.IndexOf(closestInterceptWaypoint)));


                if (targetAirGroup.Waypoints.IndexOf(closestInterceptWaypoint) + 1 < targetAirGroup.Waypoints.Count)
                {
                    AirGroupWaypoint nextInterceptWaypoint = targetAirGroup.Waypoints[targetAirGroup.Waypoints.IndexOf(interceptWaypoint) + 1];
                    Waypoints.Add(new AirGroupWaypoint(AirGroupWaypoint.AirGroupWaypointTypes.AATTACK_BOMBERS, nextInterceptWaypoint.X, nextInterceptWaypoint.Y, nextInterceptWaypoint.Z, 300.0, targetAirGroup.Id + " " + targetAirGroup.Waypoints.IndexOf(nextInterceptWaypoint)));

                    createEndInbetweenPoints(nextInterceptWaypoint.Position, landingAirport);
                }
                else
                {
                    createEndInbetweenPoints(closestInterceptWaypoint.Position, landingAirport);
                }
            }

            createEndWaypoints(landingAirport);
        }
        
        #endregion        
        
        #region Fields

        private double? _altitude = null;
        private AirGroup _escortAirGroup = null;
        private Stationary _targetStationary = null;
        private GroundGroup _targetGroundGroup = null;
        private AirGroup _targetAirGroup = null;
        //private Point2d? _targetArea = null;

        #endregion
    }
}