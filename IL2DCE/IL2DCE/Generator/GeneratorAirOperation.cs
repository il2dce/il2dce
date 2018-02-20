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

        private CampaignInfo CampaignInfo
        {
            get;
            set;
        }

        private MissionFile MissionTemplate
        {
            get;
            set;
        }

        IGamePlay GamePlay
        {
            get;
            set;
        }

        private Config Config
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

        public IList<AirGroup> AvailableAirGroups = new List<AirGroup>();
        
        public GeneratorAirOperation(Generator generator, CampaignInfo campaignInfo, MissionFile missionTemplate, IGamePlay gamePlay, Config config)
        {
            Generator = generator;
            CampaignInfo = campaignInfo;
            MissionTemplate = missionTemplate;
            GamePlay = gamePlay;
            Config = config;

            AvailableAirGroups.Clear();
            
            foreach (AirGroup airGroup in MissionTemplate.AirGroups)
            {
                AvailableAirGroups.Add(airGroup);
            }
        }

        private double getRandomAltitude(AircraftParametersInfo missionParameters)
        {
            if (missionParameters.MinAltitude != null && missionParameters.MinAltitude.HasValue && missionParameters.MaxAltitude != null && missionParameters.MaxAltitude.HasValue)
            {
                return (double)Random.Next((int)missionParameters.MinAltitude.Value, (int)missionParameters.MaxAltitude.Value);
            }
            else
            {
                GamePlay.gpLogServer(new Player[] { GamePlay.gpPlayer() }, "No altitude defined for: " + missionParameters.LoadoutId + ". Using default altitude.", null);
                // Use some default altitudes
                return (double)Random.Next(300, 7000);
            }
        }

        /// <remarks>
        /// 
        ///     // Tweaked AI settings http://bobgamehub.blogspot.de/2012/03/ai-settings-in-cliffs-of-dover.html
        ///
        ///     // Fighter (the leading space is important)
        ///     string frookie = " 0.30 0.11 0.78 0.40 0.64 0.85 0.85 0.21";
        ///     string faverage = " 0.32 0.12 0.87 0.60 0.74 0.90 0.90 0.31";
        ///     string fexperienced = " 0.52 0.13 0.89 0.70 0.74 0.95 0.92 0.31";
        ///     string fveteran = " 0.73 0.14 0.92 0.80 0.74 1 0.95 0.41";
        ///     string face = " 0.93 0.15 0.96 0.92 0.84 1 1 0.51";
        ///
        ///     // Fighter Bomber (the leading space is important)
        ///     string xrookie = " 0.30 0.11 0.78 0.30 0.74 0.85 0.90 0.40";
        ///     string xaverage = " 0.32 0.12 0.87 0.35 0.74 0.90 0.95 0.52";
        ///     string xexperienced = " 0.52 0.13 0.89 0.38 0.74 0.92 0.95 0.52";
        ///     string xveteran = " 0.73 0.14 0.92 0.40 0.74 0.95 0.95 0.55";
        ///     string xace = " 0.93 0.15 0.96 0.45 0.74 1 1 0.65";
        ///
        ///     // Bomber (the leading space is important)
        ///     string brookie = " 0.30 0.11 0.78 0.20 0.74 0.85 0.90 0.88";
        ///     string baverage = " 0.32 0.12 0.87 0.25 0.74 0.90 0.95 0.91";
        ///     string bexperienced = " 0.52 0.13 0.89 0.28 0.74 0.92 0.95 0.91";
        ///     string bveteran = " 0.73 0.14 0.92 0.30 0.74 0.95 0.95 0.95";
        ///     string bace = " 0.93 0.15 0.96 0.35 0.74 1 1 0.97";
        /// 
        /// </remarks>
        private string getTweakedSkill(EMissionType missionType, int level)
        {
            if(missionType == EMissionType.COVER || missionType == EMissionType.ESCORT
                || missionType == EMissionType.INTERCEPT)
            {
                // Fighter
                string[] skills = new string[] {
                    "0.30 0.11 0.78 0.40 0.64 0.85 0.85 0.21",
                    "0.32 0.12 0.87 0.60 0.74 0.90 0.90 0.31",
                    "0.52 0.13 0.89 0.70 0.74 0.95 0.92 0.31",
                    "0.73 0.14 0.92 0.80 0.74 1 0.95 0.41",
                    "0.93 0.15 0.96 0.92 0.84 1 1 0.51",
                };

                return skills[level];
            }
            // TODO: Find a way to identify that aircraft is fighter bomber.
            //else if()
            //{
            //    // FighterBomber
            //    string[] skills = new string[] {
            //        "0.30 0.11 0.78 0.30 0.74 0.85 0.90 0.40",
            //        "0.32 0.12 0.87 0.35 0.74 0.90 0.95 0.52",
            //        "0.52 0.13 0.89 0.38 0.74 0.92 0.95 0.52",
            //        "0.73 0.14 0.92 0.40 0.74 0.95 0.95 0.55",
            //        "0.93 0.15 0.96 0.45 0.74 1 1 0.6",
            //    };

            //    return skills[level];
            //}
            else
            {
                // Bomber
                string[] skills = new string[] {
                    "0.30 0.11 0.78 0.20 0.74 0.85 0.90 0.88",
                    "0.32 0.12 0.87 0.25 0.74 0.90 0.95 0.91",
                    "0.52 0.13 0.89 0.28 0.74 0.92 0.95 0.91",
                    "0.73 0.14 0.92 0.30 0.74 0.95 0.95 0.95",
                    "0.93 0.15 0.96 0.35 0.74 1 1 0.97",
                };

                return skills[level];
            }
        }

        private string getRandomSkill(EMissionType missionType)
        {
            int randomLevel = Random.Next(0, 5);

            return getTweakedSkill(missionType, randomLevel);
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
            IList<EMissionType> missionTypes = CampaignInfo.GetAircraftInfo(airGroup.Class).MissionTypes;
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
                    int randomMissionTypeIndex = Random.Next(availableMissionTypes.Count);
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

                IList<AircraftParametersInfo> aircraftParametersInfos = CampaignInfo.GetAircraftInfo(airGroup.Class).GetAircraftParametersInfo(missionType);
                int aircraftParametersInfoIndex = Random.Next(aircraftParametersInfos.Count);
                AircraftParametersInfo randomAircraftParametersInfo = aircraftParametersInfos[aircraftParametersInfoIndex];
                AircraftLoadoutInfo aircraftLoadoutInfo = CampaignInfo.GetAircraftInfo(airGroup.Class).GetAircraftLoadoutInfo(randomAircraftParametersInfo.LoadoutId);
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
                            int offensiveMissionTypeIndex = Random.Next(availableOffensiveMissionTypes.Count);
                            EMissionType randomOffensiveMissionType = availableOffensiveMissionTypes[offensiveMissionTypeIndex];
                            CreateAirOperation(sectionFile, briefingFile, offensiveAirGroup, randomOffensiveMissionType, false, null);

                            if (offensiveAirGroup.Altitude != null && offensiveAirGroup.Altitude.HasValue && offensiveAirGroup.TargetGroundGroup != null)
                            {
                                airGroup.Cover(offensiveAirGroup.TargetGroundGroup, offensiveAirGroup.Altitude.Value);
                            }
                            else if (offensiveAirGroup.Altitude != null && offensiveAirGroup.Altitude.HasValue && offensiveAirGroup.TargetStationary != null)
                            {
                                airGroup.Cover(offensiveAirGroup.TargetStationary, offensiveAirGroup.Altitude.Value);
                            }
                            //else if (offensiveAirGroup.Altitude != null && offensiveAirGroup.Altitude.HasValue && offensiveAirGroup.TargetArea != null && offensiveAirGroup.TargetArea.HasValue)
                            //{
                            //    airGroup.Cover(offensiveAirGroup.TargetArea.Value, offensiveAirGroup.Altitude.Value);
                            //}
                        }
                    }
                }
                else if (missionType == EMissionType.ARMED_MARITIME_RECON)
                {
                    GroundGroup groundGroup = Generator.GeneratorGroundOperation.getAvailableRandomEnemyGroundGroup(airGroup.ArmyIndex, new List<EGroundGroupType> { EGroundGroupType.Ship });
                    Generator.GeneratorGroundOperation.CreateRandomGroundOperation(sectionFile, groundGroup);
                    double altitude = getRandomAltitude(randomAircraftParametersInfo);

                    airGroup.GroundAttack(groundGroup, altitude, escortAirGroup);
                }
                else if (missionType == EMissionType.ARMED_RECON)
                {
                    GroundGroup groundGroup = Generator.GeneratorGroundOperation.getAvailableRandomEnemyGroundGroup(airGroup.ArmyIndex, new List<EGroundGroupType> { EGroundGroupType.Armor, EGroundGroupType.Vehicle });
                    Generator.GeneratorGroundOperation.CreateRandomGroundOperation(sectionFile, groundGroup);
                    double altitude = getRandomAltitude(randomAircraftParametersInfo);

                    airGroup.GroundAttack(groundGroup, altitude, escortAirGroup);
                }
                else if (missionType == EMissionType.ATTACK_ARMOR)
                {
                    GroundGroup groundGroup = Generator.GeneratorGroundOperation.getAvailableRandomEnemyGroundGroup(airGroup.ArmyIndex, new List<EGroundGroupType> { EGroundGroupType.Armor });
                    Generator.GeneratorGroundOperation.CreateRandomGroundOperation(sectionFile, groundGroup);
                    double altitude = getRandomAltitude(randomAircraftParametersInfo);

                    airGroup.GroundAttack(groundGroup, altitude, escortAirGroup);
                }
                else if (missionType == EMissionType.ATTACK_RADAR)
                {
                    IList<Stationary> radars = MissionTemplate.GetEnemyRadars(airGroup.ArmyIndex);
                    if (radars.Count > 0)
                    {
                        int radarIndex = Random.Next(radars.Count);
                        Stationary radar = radars[radarIndex];
                        double altitude = getRandomAltitude(randomAircraftParametersInfo);

                        airGroup.GroundAttack(radar, altitude, escortAirGroup);
                    }
                }
                else if (missionType == EMissionType.ATTACK_SHIP)
                {
                    GroundGroup groundGroup = Generator.GeneratorGroundOperation.getAvailableRandomEnemyGroundGroup(airGroup.ArmyIndex, new List<EGroundGroupType> { EGroundGroupType.Ship });
                    Generator.GeneratorGroundOperation.CreateRandomGroundOperation(sectionFile, groundGroup);
                    double altitude = getRandomAltitude(randomAircraftParametersInfo);

                    airGroup.GroundAttack(groundGroup, altitude, escortAirGroup);
                }
                else if (missionType == EMissionType.ATTACK_VEHICLE)
                {
                    GroundGroup groundGroup = Generator.GeneratorGroundOperation.getAvailableRandomEnemyGroundGroup(airGroup.ArmyIndex, new List<EGroundGroupType> { EGroundGroupType.Vehicle });
                    Generator.GeneratorGroundOperation.CreateRandomGroundOperation(sectionFile, groundGroup);
                    double altitude = getRandomAltitude(randomAircraftParametersInfo);

                    airGroup.GroundAttack(groundGroup, altitude, escortAirGroup);
                }
                else if (missionType == EMissionType.ATTACK_TRAIN)
                {
                    GroundGroup groundGroup = Generator.GeneratorGroundOperation.getAvailableRandomEnemyGroundGroup(airGroup.ArmyIndex, new List<EGroundGroupType> { EGroundGroupType.Train });
                    Generator.GeneratorGroundOperation.CreateRandomGroundOperation(sectionFile, groundGroup);
                    double altitude = getRandomAltitude(randomAircraftParametersInfo);

                    airGroup.GroundAttack(groundGroup, altitude, escortAirGroup);
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
                            int escortedMissionTypeIndex = Random.Next(availableEscortedMissionTypes.Count);
                            EMissionType randomEscortedMissionType = availableEscortedMissionTypes[escortedMissionTypeIndex];
                            CreateAirOperation(sectionFile, briefingFile, escortedAirGroup, randomEscortedMissionType, true, airGroup);

                            airGroup.Escort(escortedAirGroup);
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
                            int offensiveMissionTypeIndex = Random.Next(availableOffensiveMissionTypes.Count);
                            EMissionType randomOffensiveMissionType = availableOffensiveMissionTypes[offensiveMissionTypeIndex];
                            CreateAirOperation(sectionFile, briefingFile, interceptedAirGroup, randomOffensiveMissionType, false, null);

                            airGroup.Intercept(interceptedAirGroup);
                        }
                    }
                }
                else if (missionType == EMissionType.MARITIME_RECON)
                {
                    GroundGroup groundGroup = Generator.GeneratorGroundOperation.getAvailableRandomEnemyGroundGroup(airGroup.ArmyIndex, new List<EGroundGroupType> { EGroundGroupType.Ship });
                    Generator.GeneratorGroundOperation.CreateRandomGroundOperation(sectionFile, groundGroup);
                    double altitude = getRandomAltitude(randomAircraftParametersInfo);

                    airGroup.Recon(groundGroup, altitude, escortAirGroup);
                }
                else if (missionType == EMissionType.RECON)
                {
                    GroundGroup groundGroup = Generator.GeneratorGroundOperation.getAvailableRandomEnemyGroundGroup(airGroup.ArmyIndex, new List<EGroundGroupType> { EGroundGroupType.Armor, EGroundGroupType.Vehicle });
                    Generator.GeneratorGroundOperation.CreateRandomGroundOperation(sectionFile, groundGroup);
                    double altitude = getRandomAltitude(randomAircraftParametersInfo);

                    airGroup.Recon(groundGroup, altitude, escortAirGroup);
                }

                getRandomFlightSize(airGroup, missionType);
                airGroup.Skill = getRandomSkill(missionType);
                Generator.GeneratorBriefing.CreateBriefing(briefingFile, airGroup, missionType, escortAirGroup);
                airGroup.WriteTo(sectionFile, Config);

                if (forcedEscortAirGroup == null && escortAirGroup != null)
                {
                    AvailableAirGroups.Remove(escortAirGroup);

                    IList<AircraftParametersInfo> escortAircraftParametersInfos = CampaignInfo.GetAircraftInfo(escortAirGroup.Class).GetAircraftParametersInfo(EMissionType.ESCORT);
                    int escortAircraftParametersInfoIndex = Random.Next(escortAircraftParametersInfos.Count);
                    AircraftParametersInfo escortRandomAircraftParametersInfo = escortAircraftParametersInfos[escortAircraftParametersInfoIndex];
                    AircraftLoadoutInfo escortAircraftLoadoutInfo = CampaignInfo.GetAircraftInfo(escortAirGroup.Class).GetAircraftLoadoutInfo(escortRandomAircraftParametersInfo.LoadoutId);
                    escortAirGroup.Weapons = escortAircraftLoadoutInfo.Weapons;

                    escortAirGroup.Escort(airGroup);

                    getRandomFlightSize(escortAirGroup, EMissionType.ESCORT);
                    escortAirGroup.Skill = getRandomSkill(EMissionType.ESCORT);
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
                            int interceptAircraftParametersInfoIndex = Random.Next(interceptAircraftParametersInfos.Count);
                            AircraftParametersInfo interceptRandomAircraftParametersInfo = interceptAircraftParametersInfos[interceptAircraftParametersInfoIndex];
                            AircraftLoadoutInfo interceptAircraftLoadoutInfo = CampaignInfo.GetAircraftInfo(interceptAirGroup.Class).GetAircraftLoadoutInfo(interceptRandomAircraftParametersInfo.LoadoutId);
                            interceptAirGroup.Weapons = interceptAircraftLoadoutInfo.Weapons;

                            interceptAirGroup.Intercept(airGroup);

                            getRandomFlightSize(interceptAirGroup, EMissionType.INTERCEPT);
                            interceptAirGroup.Skill = getRandomSkill(EMissionType.INTERCEPT);
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
                int interceptedAirGroupIndex = Random.Next(airGroups.Count);
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
                int escortedAirGroupIndex = Random.Next(airGroups.Count);
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
                int escortAirGroupIndex = Random.Next(airGroups.Count);
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
                int interceptAirGroupIndex = Random.Next(airGroups.Count);
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
                || missionType == EMissionType.ATTACK_VEHICLE
                || missionType == EMissionType.ATTACK_TRAIN)
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
                || missionType == EMissionType.ATTACK_TRAIN
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
            else if (missionType == EMissionType.ATTACK_TRAIN)
            {
                List<GroundGroup> groundGroups = Generator.GeneratorGroundOperation.getAvailableEnemyGroundGroups(airGroup.ArmyIndex, new List<EGroundGroupType> { EGroundGroupType.Train });
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
