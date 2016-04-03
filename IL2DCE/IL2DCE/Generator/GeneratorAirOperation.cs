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
    class GeneratorAirOperation
    {
        private Generator Generator
        {
            get;
            set;
        }


        private Random rand = new Random();

        public IList<AirGroup> AvailableAirGroups = new List<AirGroup>();

        private Core _core;

        private Core Core
        {
            get
            {
                return _core;
            }
        }

        private MissionFile MissionTemplate
        {
            get
            {
                return _core.MissionTemplate;
            }
        }

        private Config Config
        {
            get
            {
                return _core.Config;
            }

        }

        private CampaignInfo CampaignInfo
        {
            get
            {
                return _core.CurrentCareer.CampaignInfo;
            }
        }

        public GeneratorAirOperation(Core core, Generator generator)
        {
            _core = core;
            Generator = generator;
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



        public void CreateRandomAirOperation(ISectionFile sectionFile, BriefingFile briefingFile, AirGroup airGroup)
        {
            IList<EMissionType> missionTypes = _core.CurrentCareer.CampaignInfo.GetAircraftInfo(airGroup.Class).MissionTypes;
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

                    CreateAirOperation(sectionFile, briefingFile, airGroup, randomMissionType, true, null);
                }
            }
        }

        public void CreateAirOperation(ISectionFile sectionFile, BriefingFile briefingFile, AirGroup airGroup, EMissionType missionType, bool allowIntercept, AirGroup forcedEscortAirGroup)
        {
            if (isMissionTypeAvailable(airGroup, missionType))
            {
                AvailableAirGroups.Remove(airGroup);

                IList<AircraftParametersInfo> aircraftParametersInfos = Core.CurrentCareer.CampaignInfo.GetAircraftInfo(airGroup.Class).GetAircraftParametersInfo(missionType);
                int aircraftParametersInfoIndex = rand.Next(aircraftParametersInfos.Count);
                AircraftParametersInfo randomAircraftParametersInfo = aircraftParametersInfos[aircraftParametersInfoIndex];
                AircraftLoadoutInfo aircraftLoadoutInfo = Core.CurrentCareer.CampaignInfo.GetAircraftInfo(airGroup.Class).GetAircraftLoadoutInfo(randomAircraftParametersInfo.LoadoutId);
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
                        foreach (EMissionType targetMissionType in CampaignInfo.GetAircraftInfo(offensiveAirGroup.Class).MissionTypes)
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
                            CreateAirOperation(sectionFile, briefingFile, offensiveAirGroup, randomOffensiveMissionType, false, null);

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
                    GroundGroup groundGroup = Generator.GeneratorGroundOperation.getAvailableRandomEnemyGroundGroup(airGroup.ArmyIndex, new List<EGroundGroupType> { EGroundGroupType.Ship });
                    Generator.GeneratorGroundOperation.CreateRandomGroundOperation(sectionFile, groundGroup);
                    double altitude = getRandomAltitude(randomAircraftParametersInfo);

                    airGroup.GroundAttack(missionType, groundGroup, altitude, escortAirGroup);
                }
                else if (missionType == EMissionType.ARMED_RECON)
                {
                    GroundGroup groundGroup = Generator.GeneratorGroundOperation.getAvailableRandomEnemyGroundGroup(airGroup.ArmyIndex, new List<EGroundGroupType> { EGroundGroupType.Armor, EGroundGroupType.Vehicle });
                    Generator.GeneratorGroundOperation.CreateRandomGroundOperation(sectionFile, groundGroup);
                    double altitude = getRandomAltitude(randomAircraftParametersInfo);

                    airGroup.GroundAttack(missionType, groundGroup, altitude, escortAirGroup);
                }
                else if (missionType == EMissionType.ATTACK_ARMOR)
                {
                    GroundGroup groundGroup = Generator.GeneratorGroundOperation.getAvailableRandomEnemyGroundGroup(airGroup.ArmyIndex, new List<EGroundGroupType> { EGroundGroupType.Armor });
                    Generator.GeneratorGroundOperation.CreateRandomGroundOperation(sectionFile, groundGroup);
                    double altitude = getRandomAltitude(randomAircraftParametersInfo);

                    airGroup.GroundAttack(missionType, groundGroup, altitude, escortAirGroup);
                }
                else if (missionType == EMissionType.ATTACK_RADAR)
                {
                    IList<Stationary> radars = MissionTemplate.GetEnemyRadars(airGroup.ArmyIndex);
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
                    GroundGroup groundGroup = Generator.GeneratorGroundOperation.getAvailableRandomEnemyGroundGroup(airGroup.ArmyIndex, new List<EGroundGroupType> { EGroundGroupType.Ship });
                    Generator.GeneratorGroundOperation.CreateRandomGroundOperation(sectionFile, groundGroup);
                    double altitude = getRandomAltitude(randomAircraftParametersInfo);

                    airGroup.GroundAttack(missionType, groundGroup, altitude, escortAirGroup);
                }
                else if (missionType == EMissionType.ATTACK_VEHICLE)
                {
                    GroundGroup groundGroup = Generator.GeneratorGroundOperation.getAvailableRandomEnemyGroundGroup(airGroup.ArmyIndex, new List<EGroundGroupType> { EGroundGroupType.Vehicle });
                    Generator.GeneratorGroundOperation.CreateRandomGroundOperation(sectionFile, groundGroup);
                    double altitude = getRandomAltitude(randomAircraftParametersInfo);

                    airGroup.GroundAttack(missionType, groundGroup, altitude, escortAirGroup);
                }
                else if (missionType == EMissionType.ESCORT)
                {
                    AirGroup escortedAirGroup = getAvailableRandomEscortedAirGroup(airGroup.ArmyIndex);
                    if (escortedAirGroup != null)
                    {
                        List<EMissionType> availableEscortedMissionTypes = new List<EMissionType>();
                        foreach (EMissionType targetMissionType in CampaignInfo.GetAircraftInfo(escortedAirGroup.Class).MissionTypes)
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
                            CreateAirOperation(sectionFile, briefingFile, escortedAirGroup, randomEscortedMissionType, true, airGroup);

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
                        foreach (EMissionType targetMissionType in CampaignInfo.GetAircraftInfo(interceptedAirGroup.Class).MissionTypes)
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
                            CreateAirOperation(sectionFile, briefingFile, interceptedAirGroup, randomOffensiveMissionType, false, null);

                            airGroup.Intercept(missionType, interceptedAirGroup);
                        }
                    }
                }
                else if (missionType == EMissionType.MARITIME_RECON)
                {
                    GroundGroup groundGroup = Generator.GeneratorGroundOperation.getAvailableRandomEnemyGroundGroup(airGroup.ArmyIndex, new List<EGroundGroupType> { EGroundGroupType.Ship });
                    Generator.GeneratorGroundOperation.CreateRandomGroundOperation(sectionFile, groundGroup);
                    double altitude = getRandomAltitude(randomAircraftParametersInfo);

                    airGroup.Recon(missionType, groundGroup, altitude, escortAirGroup);
                }
                else if (missionType == EMissionType.RECON)
                {
                    GroundGroup groundGroup = Generator.GeneratorGroundOperation.getAvailableRandomEnemyGroundGroup(airGroup.ArmyIndex, new List<EGroundGroupType> { EGroundGroupType.Armor, EGroundGroupType.Vehicle });
                    Generator.GeneratorGroundOperation.CreateRandomGroundOperation(sectionFile, groundGroup);
                    double altitude = getRandomAltitude(randomAircraftParametersInfo);

                    airGroup.Recon(missionType, groundGroup, altitude, escortAirGroup);
                }

                getRandomFlightSize(airGroup, missionType);
                Generator.GeneratorBriefing.CreateBriefing(briefingFile, airGroup, missionType, escortAirGroup);
                airGroup.WriteTo(sectionFile, Config);

                if (forcedEscortAirGroup == null && escortAirGroup != null)
                {
                    AvailableAirGroups.Remove(escortAirGroup);

                    IList<AircraftParametersInfo> escortAircraftParametersInfos = CampaignInfo.GetAircraftInfo(escortAirGroup.Class).GetAircraftParametersInfo(EMissionType.ESCORT);
                    int escortAircraftParametersInfoIndex = rand.Next(escortAircraftParametersInfos.Count);
                    AircraftParametersInfo escortRandomAircraftParametersInfo = escortAircraftParametersInfos[escortAircraftParametersInfoIndex];
                    AircraftLoadoutInfo escortAircraftLoadoutInfo = CampaignInfo.GetAircraftInfo(escortAirGroup.Class).GetAircraftLoadoutInfo(escortRandomAircraftParametersInfo.LoadoutId);
                    escortAirGroup.Weapons = escortAircraftLoadoutInfo.Weapons;

                    escortAirGroup.Escort(EMissionType.ESCORT, airGroup);

                    getRandomFlightSize(escortAirGroup, EMissionType.ESCORT);
                    Generator.GeneratorBriefing.CreateBriefing(briefingFile, escortAirGroup, EMissionType.ESCORT, null);
                    escortAirGroup.WriteTo(sectionFile, Config);
                }

                if (isMissionTypeOffensive(missionType))
                {
                    if (allowIntercept)
                    {
                        AirGroup interceptAirGroup = getAvailableRandomInterceptAirGroup(airGroup);
                        if (interceptAirGroup != null)
                        {
                            AvailableAirGroups.Remove(interceptAirGroup);

                            IList<AircraftParametersInfo> interceptAircraftParametersInfos = CampaignInfo.GetAircraftInfo(interceptAirGroup.Class).GetAircraftParametersInfo(EMissionType.INTERCEPT);
                            int interceptAircraftParametersInfoIndex = rand.Next(interceptAircraftParametersInfos.Count);
                            AircraftParametersInfo interceptRandomAircraftParametersInfo = interceptAircraftParametersInfos[interceptAircraftParametersInfoIndex];
                            AircraftLoadoutInfo interceptAircraftLoadoutInfo = CampaignInfo.GetAircraftInfo(interceptAirGroup.Class).GetAircraftLoadoutInfo(interceptRandomAircraftParametersInfo.LoadoutId);
                            interceptAirGroup.Weapons = interceptAircraftLoadoutInfo.Weapons;

                            interceptAirGroup.Intercept(EMissionType.INTERCEPT, airGroup);

                            getRandomFlightSize(interceptAirGroup, EMissionType.INTERCEPT);
                            Generator.GeneratorBriefing.CreateBriefing(briefingFile, interceptAirGroup, EMissionType.INTERCEPT, null);
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

        private List<AirGroup> getAvailableOffensiveAirGroups(int armyIndex)
        {
            List<AirGroup> airGroups = new List<AirGroup>();
            foreach (AirGroup airGroup in AvailableAirGroups)
            {
                if (airGroup.ArmyIndex != armyIndex)
                {
                    foreach (EMissionType missionType in CampaignInfo.GetAircraftInfo(airGroup.Class).MissionTypes)
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
            foreach (AirGroup airGroup in AvailableAirGroups)
            {
                if (airGroup.ArmyIndex == armyIndex)
                {
                    foreach (EMissionType missionType in CampaignInfo.GetAircraftInfo(airGroup.Class).MissionTypes)
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
            foreach (AirGroup airGroup in AvailableAirGroups)
            {
                if (airGroup.ArmyIndex == targetAirUnit.ArmyIndex)
                {
                    if (CampaignInfo.GetAircraftInfo(airGroup.Class).MissionTypes.Contains(EMissionType.ESCORT))
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

        private AirGroup getAvailableRandomInterceptAirGroup(AirGroup targetAirUnit)
        {
            List<AirGroup> airGroups = new List<AirGroup>();
            foreach (AirGroup airGroup in AvailableAirGroups)
            {
                if (airGroup.ArmyIndex != targetAirUnit.ArmyIndex)
                {
                    if (CampaignInfo.GetAircraftInfo(airGroup.Class).MissionTypes.Contains(EMissionType.INTERCEPT))
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
                List<GroundGroup> groundGroups = Generator.GeneratorGroundOperation.getAvailableEnemyGroundGroups(airGroup.ArmyIndex, new List<EGroundGroupType> { EGroundGroupType.Ship });
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
                List<GroundGroup> groundGroups = Generator.GeneratorGroundOperation.getAvailableEnemyGroundGroups(airGroup.ArmyIndex, new List<EGroundGroupType> { EGroundGroupType.Armor, EGroundGroupType.Vehicle });
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
                List<GroundGroup> groundGroups = Generator.GeneratorGroundOperation.getAvailableEnemyGroundGroups(airGroup.ArmyIndex, new List<EGroundGroupType> { EGroundGroupType.Armor });
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
                IList<Stationary> radars = MissionTemplate.GetEnemyRadars(airGroup.ArmyIndex);
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
                List<GroundGroup> groundGroups = Generator.GeneratorGroundOperation.getAvailableEnemyGroundGroups(airGroup.ArmyIndex, new List<EGroundGroupType> { EGroundGroupType.Ship });
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
                List<GroundGroup> groundGroups = Generator.GeneratorGroundOperation.getAvailableEnemyGroundGroups(airGroup.ArmyIndex, new List<EGroundGroupType> { EGroundGroupType.Vehicle });
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
                List<GroundGroup> groundGroups = Generator.GeneratorGroundOperation.getAvailableEnemyGroundGroups(airGroup.ArmyIndex, new List<EGroundGroupType> { EGroundGroupType.Ship });
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
                List<GroundGroup> groundGroups = Generator.GeneratorGroundOperation.getAvailableEnemyGroundGroups(airGroup.ArmyIndex, new List<EGroundGroupType> { EGroundGroupType.Armor, EGroundGroupType.Vehicle });
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

    }
}
