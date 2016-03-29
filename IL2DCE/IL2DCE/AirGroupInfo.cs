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

namespace IL2DCE
{
    public abstract class AirGroupInfo
    {
        #region Public properties

        public abstract List<string> Aircrafts
        {
            get;
        }

        public abstract List<string> AirGroupKeys
        {
            get;
        }

        public abstract int SquadronCount
        {
            get;
        }

        public abstract int FlightCount
        {
            get;
        }

        public abstract int FlightSize
        {
            get;
        }

        public int AircraftMaxCount
        {
            get { return FlightCount * FlightSize; }
        }

        #endregion

        static public AirGroupInfo[] GetAirGroupInfos(int armyIndex)
        {
            if (armyIndex == 1)
            {
                return RedAirGroupInfos;
            }
            else if (armyIndex == 2)
            {
                return BlueAirGroupInfos;
            }
            else
            {
                return new AirGroupInfo[] { };
            }
        }

        static public AirGroupInfo GetAirGroupInfo(int armyIndex, string airGroupKey)
        {
            foreach (AirGroupInfo airGroupInfo in GetAirGroupInfos(armyIndex))
            {
                List<string> airGroupKeys = new List<string>(airGroupInfo.AirGroupKeys);
                if (airGroupKeys.Contains(airGroupKey))
                {
                    return airGroupInfo;
                }
            }

            return null;
        }

        static public AirGroupInfo GetAirGroupInfo(string airGroupKey)
        {
            foreach (AirGroupInfo airGroupInfo in RedAirGroupInfos)
            {
                List<string> airGroupKeys = new List<string>(airGroupInfo.AirGroupKeys);
                if (airGroupKeys.Contains(airGroupKey))
                {
                    return airGroupInfo;
                }
            }
            foreach (AirGroupInfo airGroupInfo in BlueAirGroupInfos)
            {
                List<string> airGroupKeys = new List<string>(airGroupInfo.AirGroupKeys);
                if (airGroupKeys.Contains(airGroupKey))
                {
                    return airGroupInfo;
                }
            }

            return null;
        }

        static public AirGroupInfo[] RedAirGroupInfos = new AirGroupInfo[]
    {
        new RafFighterCommandEarlyAirGroupInfo(),
        new RafFighterCommandLateAirGroupInfo(),
        new RafBomberCommandAirGroupInfo(),
        new RafFlyingTrainingSchoolEarlyAirGroupInfo(),
        new RafFlyingTrainingSchoolLateAirGroupInfo(),
    };

        static public AirGroupInfo[] BlueAirGroupInfos = new AirGroupInfo[]
    {
        new LwFighterStabAirGroupInfo(),
        new LwFighterAirGroupInfo(),
        new LwZerstoererStabAirGroupInfo(),
        new LwZerstoererAirGroupInfo(),
        new LwStukaStabAirGroupInfo(),
        new LwStukaAirGroupInfo(),
        new LwBomberStabAirGroupInfo(),
        new LwBomberAirGroupInfo(),
        new LwTransportAirGroupInfo(),
        new LwReconAirGroupInfo(),
        new RaFighterAirGroupInfo(),
        new RaBomberAirGroupInfo(),
    };
    }

    public class RafFighterCommandEarlyAirGroupInfo : AirGroupInfo
    {
        #region Private members

        public List<string> aircrafts = new List<string>
        {
            "Aircraft.BeaufighterMkIF",
            "Aircraft.DefiantMkI",
            "Aircraft.GladiatorMkII",
            "Aircraft.HurricaneMkI_dH5-20",
            "Aircraft.HurricaneMkI",
            "Aircraft.SpitfireMkI",
            //"Aircraft.SpitfireMkI_Heartbreaker",
            "Aircraft.SpitfireMkIa",
            "Aircraft.SpitfireMkIIa",
        };

        private List<string> airGroupKeys = new List<string>
        {
            //"gb01", /* Generic Fighter Command Early */
            "BoB_RAF_F_1Sqn_Early",
            "BoB_RAF_F_1_RCAF_Early",
            "BoB_RAF_F_111Sqn_Early",
            "BoB_RAF_F_141Sqn_Early",
            "BoB_RAF_F_145Sqn_Early",
            "BoB_RAF_F_151Sqn_Early",
            "BoB_RAF_F_152Sqn_Early",
            "BoB_RAF_F_17Sqn_Early",
            "BoB_RAF_F_19Sqn_Early",
            "BoB_RAF_F_213Sqn_Early",
            "BoB_RAF_F_219Sqn_Early",
            "BoB_RAF_F_222Sqn_Early",
            "BoB_RAF_F_229Sqn_Early",
            "BoB_RAF_F_23Sqn_Early",
            "BoB_RAF_F_232Sqn_Early",
            "BoB_RAF_F_234Sqn_Early",
            "BoB_RAF_F_235Sqn_Early",
            "BoB_RAF_F_236Sqn_Early",
            "BoB_RAF_F_238Sqn_Early",
            "BoB_RAF_F_242Sqn_Early",
            "BoB_RAF_F_245Sqn_Early",
            "BoB_RAF_F_247Sqn_Early",
            "BoB_RAF_F_248Sqn_Early",
            "BoB_RAF_F_249Sqn_Early",
            "BoB_RAF_F_25Sqn_Early",
            "BoB_RAF_F_253Sqn_Early",
            "BoB_RAF_F_257Sqn_Early",
            "BoB_RAF_F_263Sqn_Early",
            "BoB_RAF_F_264Sqn_Early",
            "BoB_RAF_F_266Sqn_Early",
            "BoB_RAF_F_29Sqn_Early",
            "BoB_RAF_F_3Sqn_Early",
            "BoB_RAF_F_302_PL_Early",
            "BoB_RAF_F_303_PL_Early",
            "BoB_RAF_F_310_CZ_Early",
            "BoB_RAF_F_312_CZ_Early",
            "BoB_RAF_F_32Sqn_Early",
            "BoB_RAF_F_41Sqn_Early",
            "BoB_RAF_F_43Sqn_Early",
            "BoB_RAF_F_46Sqn_Early",
            "BoB_RAF_F_501Sqn_Early",
            "BoB_RAF_F_504Sqn_Early",
            "BoB_RAF_F_54Sqn_Early",
            "BoB_RAF_F_56Sqn_Early",
            "BoB_RAF_F_600Sqn_Early",
            "BoB_RAF_F_601Sqn_Early",
            "BoB_RAF_F_602Sqn_Early",
            "BoB_RAF_F_603Sqn_Early",
            "BoB_RAF_F_604Sqn_Early",
            "BoB_RAF_F_605Sqn_Early",
            "BoB_RAF_F_607Sqn_Early",
            "BoB_RAF_F_609Sqn_Early",
            "BoB_RAF_F_610Sqn_Early",
            "BoB_RAF_F_611Sqn_Early",
            "BoB_RAF_F_615Sqn_Early",
            "BoB_RAF_F_616Sqn_Early",
            "BoB_RAF_F_64Sqn_Early",
            "BoB_RAF_F_65Sqn_Early",
            "BoB_RAF_F_66Sqn_Early",
            "BoB_RAF_F_72Sqn_Early",
            "BoB_RAF_F_73Sqn_Early",
            "BoB_RAF_F_74Sqn_Early",
            "BoB_RAF_F_79Sqn_Early",
            "BoB_RAF_F_85Sqn_Early",
            "BoB_RAF_F_87Sqn_Early",
            "BoB_RAF_F_92Sqn_Early",
            //"BoB_RAF_F_FatCat_Early", /* Fiction Early */
        };

        #endregion

        #region Public properties

        public override List<string> Aircrafts
        {
            get
            {
                return aircrafts;
            }
        }

        public override List<string> AirGroupKeys
        {
            get
            {
                return airGroupKeys;
            }
        }

        public override int SquadronCount
        {
            get { return 1; }
        }

        public override int FlightCount
        {
            get { return 2; }
        }

        public override int FlightSize
        {
            get { return 6; }
        }

        #endregion
    }

    public class RafFighterCommandLateAirGroupInfo : AirGroupInfo
    {
        #region Private members

        private List<string> aircrafts = new List<string>
        {
            "Aircraft.BeaufighterMkIF",
            "Aircraft.DefiantMkI",
            "Aircraft.GladiatorMkII",
            "Aircraft.HurricaneMkI_dH5-20",
            "Aircraft.HurricaneMkI",
            "Aircraft.SpitfireMkI",
            //"Aircraft.SpitfireMkI_Heartbreaker",
            "Aircraft.SpitfireMkIa",
            "Aircraft.SpitfireMkIIa",
        };

        private List<string> airGroupKeys = new List<string>
        {
            //"gb01_Late", /* Generic Fighter Command Late */
            "BoB_RAF_F_1Sqn_Late",
            "BoB_RAF_F_1_RCAF_Late",
            "BoB_RAF_F_111Sqn_Late",
            "BoB_RAF_F_141Sqn_Late",
            "BoB_RAF_F_145Sqn_Late",
            "BoB_RAF_F_151Sqn_Late",
            "BoB_RAF_F_152Sqn_Late",
            "BoB_RAF_F_17Sqn_Late",
            "BoB_RAF_F_19Sqn_Late",
            "BoB_RAF_F_213Sqn_Late",
            "BoB_RAF_F_219Sqn_Late",
            "BoB_RAF_F_222Sqn_Late",
            "BoB_RAF_F_229Sqn_Late",
            "BoB_RAF_F_23Sqn_Late",
            "BoB_RAF_F_232Sqn_Late",
            "BoB_RAF_F_234Sqn_Late",
            "BoB_RAF_F_235Sqn_Late",
            "BoB_RAF_F_236Sqn_Late",
            "BoB_RAF_F_238Sqn_Late",
            "BoB_RAF_F_242Sqn_Late",
            "BoB_RAF_F_245Sqn_Late",
            "BoB_RAF_F_247Sqn_Late",
            "BoB_RAF_F_248Sqn_Late",
            "BoB_RAF_F_249Sqn_Late",
            "BoB_RAF_F_25Sqn_Late",
            "BoB_RAF_F_253Sqn_Late",
            "BoB_RAF_F_257Sqn_Late",
            "BoB_RAF_F_263Sqn_Late",
            "BoB_RAF_F_264Sqn_Late",
            "BoB_RAF_F_266Sqn_Late",
            "BoB_RAF_F_29Sqn_Late",
            "BoB_RAF_F_3Sqn_Late",
            "BoB_RAF_F_302_PL_Late",
            "BoB_RAF_F_303_PL_Late",
            "BoB_RAF_F_310_CZ_Late",
            "BoB_RAF_F_312_CZ_Late",
            "BoB_RAF_F_32Sqn_Late",
            "BoB_RAF_F_41Sqn_Late",
            "BoB_RAF_F_43Sqn_Late",
            "BoB_RAF_F_46Sqn_Late",
            "BoB_RAF_F_501Sqn_Late",
            "BoB_RAF_F_504Sqn_Late",
            "BoB_RAF_F_54Sqn_Late",
            "BoB_RAF_F_56Sqn_Late",
            "BoB_RAF_F_600Sqn_Late",
            "BoB_RAF_F_601Sqn_Late",
            "BoB_RAF_F_602Sqn_Late",
            "BoB_RAF_F_603Sqn_Late",
            "BoB_RAF_F_604Sqn_Late",
            "BoB_RAF_F_605Sqn_Late",
            "BoB_RAF_F_607Sqn_Late",
            "BoB_RAF_F_609Sqn_Late",
            "BoB_RAF_F_610Sqn_Late",
            "BoB_RAF_F_611Sqn_Late",
            "BoB_RAF_F_615Sqn_Late",
            "BoB_RAF_F_616Sqn_Late",
            "BoB_RAF_F_64Sqn_Late",
            "BoB_RAF_F_65Sqn_Late",
            "BoB_RAF_F_66Sqn_Late",
            "BoB_RAF_F_72Sqn_Late",
            "BoB_RAF_F_73Sqn_Late",
            "BoB_RAF_F_74Sqn_Late",
            "BoB_RAF_F_79Sqn_Late",
            "BoB_RAF_F_85Sqn_Late",
            "BoB_RAF_F_87Sqn_Late",
            "BoB_RAF_F_92Sqn_Late",
            //"BoB_RAF_F_FatCat_Late", /* Fiction Early */
        };

        #endregion

        #region Public properties

        public override List<string> Aircrafts
        {
            get
            {
                return aircrafts;
            }
        }

        public override List<string> AirGroupKeys
        {
            get
            {
                return airGroupKeys;
            }
        }

        public override int SquadronCount
        {
            get { return 1; }
        }

        public override int FlightCount
        {
            get { return 3; }
        }

        public override int FlightSize
        {
            get { return 4; }
        }

        #endregion
    }

    public class RafBomberCommandAirGroupInfo : AirGroupInfo
    {
        #region Private members

        private List<string> aircrafts = new List<string>
        {
            "Aircraft.AnsonMkI",
            "Aircraft.BlenheimMkI",
            "Aircraft.BlenheimMkIV",
            //"Aircraft.SunderlandMkI",
            //"Aircraft.WalrusMkI",
            "Aircraft.WellingtonMkIc",
        };

        private List<string> airGroupKeys = new List<string>
        {
            //"gb02", /* Generic Command Bomber */
            "BoB_RAF_B_10Sqn",
            "BoB_RAF_B_101Sqn",
            "BoB_RAF_B_102Sqn",
            "BoB_RAF_B_103Sqn",
            "BoB_RAF_B_104Sqn",
            "BoB_RAF_B_105Sqn",
            "BoB_RAF_B_106Sqn",
            "BoB_RAF_B_107Sqn",
            "BoB_RAF_B_110Sqn",
            "BoB_RAF_B_114Sqn",
            "BoB_RAF_B_115Sqn",
            "BoB_RAF_B_12Sqn",
            "BoB_RAF_B_139Sqn",
            "BoB_RAF_B_142Sqn",
            "BoB_RAF_B_144Sqn",
            "BoB_RAF_B_148Sqn",
            "BoB_RAF_B_149Sqn",
            "BoB_RAF_B_15Sqn",
            "BoB_RAF_B_150Sqn",
            "BoB_RAF_B_166Sqn",
            "BoB_RAF_B_207Sqn",
            "BoB_RAF_B_21Sqn",
            "BoB_RAF_B_214Sqn",
            "BoB_RAF_B_215Sqn",
            "BoB_RAF_B_218Sqn",
            "BoB_RAF_B_35Sqn",
            "BoB_RAF_B_37Sqn",
            "BoB_RAF_B_38Sqn",
            "BoB_RAF_B_40Sqn",
            "BoB_RAF_B_44Sqn",
            "BoB_RAF_B_48Sqn",
            "BoB_RAF_B_49Sqn",
            "BoB_RAF_B_50Sqn",
            "BoB_RAF_B_51Sqn",
            "BoB_RAF_B_52Sqn",
            "BoB_RAF_B_58Sqn",
            "BoB_RAF_B_61Sqn",
            "BoB_RAF_B_7Sqn",
            "BoB_RAF_B_75Sqn",
            "BoB_RAF_B_76Sqn",
            "BoB_RAF_B_77Sqn",
            "BoB_RAF_B_78Sqn",
            "BoB_RAF_B_82Sqn",
            "BoB_RAF_B_83Sqn",
            "BoB_RAF_B_88Sqn",
            "BoB_RAF_B_9Sqn",
            "BoB_RAF_B_90Sqn",
            "BoB_RAF_B_97Sqn",
            "BoB_RAF_B_98Sqn",
            "BoB_RAF_B_99Sqn",
        };

        #endregion

        #region Public properties

        public override List<string> Aircrafts
        {
            get
            {
                return aircrafts;
            }
        }

        public override List<string> AirGroupKeys
        {
            get
            {
                return airGroupKeys;
            }
        }

        public override int SquadronCount
        {
            get { return 1; }
        }

        public override int FlightCount
        {
            get { return 2; }
        }

        public override int FlightSize
        {
            get { return 6; }
        }

        #endregion
    }

    public class RafFlyingTrainingSchoolEarlyAirGroupInfo : AirGroupInfo
    {
        #region Private members

        private List<string> aircrafts = new List<string>
        {
            "Aircraft.AnsonMkI",
            "Aircraft.DH82A",
        };

        private List<string> airGroupKeys = new List<string>
        {
            "LONDON",
        };

        #endregion

        #region Public properties

        public override List<string> Aircrafts
        {
            get
            {
                return aircrafts;
            }
        }

        public override List<string> AirGroupKeys
        {
            get
            {
                return airGroupKeys;
            }
        }

        public override int SquadronCount
        {
            get { return 1; }
        }

        public override int FlightCount
        {
            get { return 2; }
        }

        public override int FlightSize
        {
            get { return 6; }
        }

        #endregion
    }

    public class RafFlyingTrainingSchoolLateAirGroupInfo : AirGroupInfo
    {
        #region Private members

        private List<string> aircrafts = new List<string>
        {
            "Aircraft.AnsonMkI",
            "Aircraft.DH82A",
        };

        private List<string> airGroupKeys = new List<string>
        {
            "LONDON_Late",
        };

        #endregion

        #region Public properties

        public override List<string> Aircrafts
        {
            get
            {
                return aircrafts;
            }
        }

        public override List<string> AirGroupKeys
        {
            get
            {
                return airGroupKeys;
            }
        }

        public override int SquadronCount
        {
            get { return 1; }
        }

        public override int FlightCount
        {
            get { return 3; }
        }

        public override int FlightSize
        {
            get { return 4; }
        }

        #endregion
    }

    public class LwFighterStabAirGroupInfo : AirGroupInfo
    {
        #region Private members

        private List<string> aircrafts = new List<string>
        {
            "Aircraft.Bf-109E-1",
            "Aircraft.Bf-109E-3",
            "Aircraft.Bf-109E-3B",
        };

        private List<string> airGroupKeys = new List<string>
        {
            "BoB_LW_JG2_Stab",
            "BoB_LW_JG27_Stab",
            "BoB_LW_JG3_Stab",
            "BoB_LW_JG51_Stab",
            "BoB_LW_JG52_Stab",
            "BoB_LW_JG53_Stab",
            "BoB_LW_JG54_Stab",
        };

        #endregion

        #region Public properties

        public override List<string> Aircrafts
        {
            get
            {
                return aircrafts;
            }
        }

        public override List<string> AirGroupKeys
        {
            get
            {
                return airGroupKeys;
            }
        }

        public override int SquadronCount
        {
            get { return 1; }
        }

        public override int FlightCount
        {
            get { return 3; }
        }

        public override int FlightSize
        {
            get { return 4; }
        }

        #endregion
    }

    public class LwFighterAirGroupInfo : AirGroupInfo
    {
        #region Private members

        private List<string> fighterAircrafts = new List<string>
        {
            "Aircraft.Bf-109E-1",
            "Aircraft.Bf-109E-3",
            "Aircraft.Bf-109E-3B",
        };

        private List<string> fighterAirGroupKeys = new List<string>
        {
            //"g01", /* Generic Fighter */
            //"g04", /* Generic Fighter Bomber */
            "BoB_LW_ErprGr210F",
            "BoB_LW_LG2_I",
            "BoB_LW_JG2_I",
            "BoB_LW_JG26_I",
            "BoB_LW_JG27_I",
            "BoB_LW_JG3_I",
            "BoB_LW_JG51_I",
            "BoB_LW_JG52_I",
            "BoB_LW_JG53_I",
            "BoB_LW_JG54_I",
            "BoB_LW_JG77_I",
            "BoB_LW_JG2_II",
            "BoB_LW_JG26_II",
            "BoB_LW_JG27_II",
            "BoB_LW_JG3_II",
            "BoB_LW_JG51_II",
            "BoB_LW_JG52_II",
            "BoB_LW_JG53_II",
            "BoB_LW_JG54_II",
            "BoB_LW_JG2_III",
            "BoB_LW_JG26_III",
            "BoB_LW_JG27_III",
            "BoB_LW_JG3_III",
            "BoB_LW_JG51_III",
            "BoB_LW_JG52_III",
            "BoB_LW_JG53_III",
            "BoB_LW_JG54_III",
            "BoB_LW_LG1_V"
        };

        #endregion

        #region Public properties

        public override List<string> Aircrafts
        {
            get
            {
                return fighterAircrafts;
            }
        }

        public override List<string> AirGroupKeys
        {
            get
            {
                return fighterAirGroupKeys;
            }
        }

        public override int SquadronCount
        {
            get { return 4; }
        }

        public override int FlightCount
        {
            get { return 3; }
        }

        public override int FlightSize
        {
            get { return 4; }
        }

        #endregion
    }

    public class LwZerstoererStabAirGroupInfo : AirGroupInfo
    {
        #region Private members

        private List<string> aircrafts = new List<string>
        {
            "Aircraft.Bf-110C-4",
            "Aircraft.Bf-110C-7",
        };

        private List<string> airGroupKeys = new List<string>
        {
            "BoB_LW_ZG2_Stab",
            "BoB_LW_ZG26_Stab",
        };

        #endregion

        #region Public properties

        public override List<string> Aircrafts
        {
            get
            {
                return aircrafts;
            }
        }

        public override List<string> AirGroupKeys
        {
            get
            {
                return airGroupKeys;
            }
        }

        public override int SquadronCount
        {
            get { return 1; }
        }

        public override int FlightCount
        {
            get { return 3; }
        }

        public override int FlightSize
        {
            get { return 4; }
        }

        #endregion
    }

    public class LwZerstoererAirGroupInfo : AirGroupInfo
    {
        #region Private members

        private List<string> aircrafts = new List<string>
        {
            "Aircraft.Bf-110C-4",
            "Aircraft.Bf-110C-7",
        };

        private List<string> airGroupKeys = new List<string>
        {
            "BoB_LW_ErprGr210",
            "BoB_LW_ZG2_I",
            "BoB_LW_ZG26_I",
            "BoB_LW_ZG2_II",
            "BoB_LW_ZG26_II",
            "BoB_LW_ZG76_II",
            "BoB_LW_ZG26_III",
            "BoB_LW_ZG76_III",
        };

        #endregion

        #region Public properties

        public override List<string> Aircrafts
        {
            get
            {
                return aircrafts;
            }
        }

        public override List<string> AirGroupKeys
        {
            get
            {
                return airGroupKeys;
            }
        }

        public override int SquadronCount
        {
            get { return 4; }
        }

        public override int FlightCount
        {
            get { return 3; }
        }

        public override int FlightSize
        {
            get { return 4; }
        }

        #endregion
    }

    public class LwStukaStabAirGroupInfo : AirGroupInfo
    {
        #region Private members

        private List<string> aircrafts = new List<string>
        {
            "Aircraft.Ju-87B-2",
        };

        private List<string> airGroupKeys = new List<string>
        {
            "BoB_LW_StG1_Stab",
            "BoB_LW_StG2_Stab",
            "BoB_LW_StG3_Stab",
            "BoB_LW_StG77_Stab",
        };

        #endregion

        #region Public properties

        public override List<string> Aircrafts
        {
            get
            {
                return aircrafts;
            }
        }

        public override List<string> AirGroupKeys
        {
            get
            {
                return airGroupKeys;
            }
        }

        public override int SquadronCount
        {
            get { return 1; }
        }

        public override int FlightCount
        {
            get { return 3; }
        }

        public override int FlightSize
        {
            get { return 3; }
        }

        #endregion
    }

    public class LwStukaAirGroupInfo : AirGroupInfo
    {
        #region Private members

        private List<string> aircrafts = new List<string>
        {
            "Aircraft.Ju-87B-2",
        };

        private List<string> airGroupKeys = new List<string>
        {
            //"g03", /* Generic Stuka */
            "BoB_LW_StG1_I",
            "BoB_LW_StG2_I",
            "BoB_LW_StG3_I",
            "BoB_LW_StG77_I",
            "BoB_LW_StG1_II",
            "BoB_LW_StG2_II",
            "BoB_LW_StG3_II",
            "BoB_LW_StG77_II",
            "BoB_LW_StG1_III",
            "BoB_LW_StG3_III",
            "BoB_LW_StG77_III",
            "BoB_LW_LG1_IV",
        };

        #endregion

        #region Public properties

        public override List<string> Aircrafts
        {
            get
            {
                return aircrafts;
            }
        }

        public override List<string> AirGroupKeys
        {
            get
            {
                return airGroupKeys;
            }
        }

        public override int SquadronCount
        {
            get { return 4; }
        }

        public override int FlightCount
        {
            get { return 3; }
        }

        public override int FlightSize
        {
            get { return 3; }
        }

        #endregion
    }

    public class LwBomberStabAirGroupInfo : AirGroupInfo
    {
        #region Private members

        private List<string> aircrafts = new List<string>
        {
            "Aircraft.Do-17Z-1",
            "Aircraft.Do-17Z-2",
            "Aircraft.Do-215B-1",
            "Aircraft.FW-200C-1",
            "Aircraft.He-111H-2",
            "Aircraft.He-111P-2",
            "Aircraft.Ju-88A-1"
        };

        private List<string> airGroupKeys = new List<string>
        {
            "BoB_LW_KGr_100",
            "BoB_LW_KGr_806",
            "BoB_LW_KuFlGr_106",
            "BoB_LW_KuFlGr_406",
            "BoB_LW_KuFlGr_506",
            "BoB_LW_KuFlGr_606",
            "BoB_LW_KuFlGr_706",
            "BoB_LW_KG26_Stab",
            "BoB_LW_KG27_Stab",
            "BoB_LW_KG3_Stab",
            "BoB_LW_KG30_Stab",
            "BoB_LW_KG40_Stab",
            "BoB_LW_KG51_Stab",
            "BoB_LW_KG54_Stab",
            "BoB_LW_KGzbV1_Stab",
            "BoB_LW_ZG76_Stab",
            "BoB_LW_KG76_Stab",
        };

        #endregion

        #region Public properties

        public override List<string> Aircrafts
        {
            get
            {
                return aircrafts;
            }
        }

        public override List<string> AirGroupKeys
        {
            get
            {
                return airGroupKeys;
            }
        }

        public override int SquadronCount
        {
            get { return 1; }
        }

        public override int FlightCount
        {
            get { return 3; }
        }

        public override int FlightSize
        {
            get { return 3; }
        }

        #endregion
    }

    public class LwBomberAirGroupInfo : AirGroupInfo
    {
        #region Private members

        private List<string> aircrafts = new List<string>
        {
            "Aircraft.Do-17Z-1",
            "Aircraft.Do-17Z-2",
            "Aircraft.Do-215B-1",
            "Aircraft.FW-200C-1",
            "Aircraft.He-111H-2",
            "Aircraft.He-111P-2",
            "Aircraft.Ju-88A-1"
        };

        private List<string> airGroupKeys = new List<string>
        {
            //"g02", /* Generic Bomber */
            //"g05", /* Generic Training */
            //"g06", /* Generic Transport */
            "BoB_LW_KG1_I",
            "BoB_LW_KG2_I",
            "BoB_LW_KG26_I",
            "BoB_LW_KG27_I",
            "BoB_LW_KG3_I",
            "BoB_LW_KG30_I",
            "BoB_LW_KG4_I",
            "BoB_LW_KG40_I",
            "BoB_LW_KG51_I",
            "BoB_LW_KG53_I",
            "BoB_LW_KG54_I",
            "BoB_LW_KG55_I",
            "BoB_LW_KG76_I",
            "BoB_LW_LG1_I",
            "BoB_LW_LG2_II",
            "BoB_LW_KG1_II",
            "BoB_LW_KG2_II",
            "BoB_LW_KG26_II",
            "BoB_LW_KG27_II",
            "BoB_LW_KG3_II",
            "BoB_LW_KG30_II",
            "BoB_LW_KG4_II",
            "BoB_LW_KG51_II",
            "BoB_LW_KG53_II",
            "BoB_LW_KG54_II",
            "BoB_LW_KG55_II",
            "BoB_LW_KG76_II",
            "BoB_LW_LG1_II",
            "BoB_LW_KG1_III",
            "BoB_LW_KG2_III",
            "BoB_LW_KG26_III",
            "BoB_LW_KG27_III",
            "BoB_LW_KG3_III",
            "BoB_LW_KG30_III",
            "BoB_LW_KG4_III",
            "BoB_LW_KG51_III",
            "BoB_LW_KG53_III",
            "BoB_LW_KG55_III",
            "BoB_LW_KG76_III",
            "BoB_LW_LG1_III",
            "BoB_LW_KG1_IV",
            "BoB_LW_KG27_IV",
            "BoB_LW_KG2_IV",
            "BoB_LW_KG3_IV",
        };

        #endregion

        #region Public properties

        public override List<string> Aircrafts
        {
            get
            {
                return aircrafts;
            }
        }

        public override List<string> AirGroupKeys
        {
            get
            {
                return airGroupKeys;
            }
        }

        public override int SquadronCount
        {
            get { return 4; }
        }

        public override int FlightCount
        {
            get { return 3; }
        }

        public override int FlightSize
        {
            get { return 3; }
        }

        #endregion
    }

    public class LwTransportAirGroupInfo : AirGroupInfo
    {
        #region Private members

        private List<string> aircrafts = new List<string>
        {
            "Aircraft.FW-200C-1",
        };

        private List<string> airGroupKeys = new List<string>
        {
            "BoB_LW_KGzbV1_I",
            "BoB_LW_KGzbV1_II",
            "BoB_LW_KGzbV1_III",
            "BoB_LW_KGzbV1_IV",
        };

        #endregion

        #region Public properties

        public override List<string> Aircrafts
        {
            get
            {
                return aircrafts;
            }
        }

        public override List<string> AirGroupKeys
        {
            get
            {
                return airGroupKeys;
            }
        }

        public override int SquadronCount
        {
            get { return 5; }
        }

        public override int FlightCount
        {
            get { return 3; }
        }

        public override int FlightSize
        {
            get { return 3; }
        }

        #endregion
    }

    public class LwReconAirGroupInfo : AirGroupInfo
    {
        #region Private members

        private List<string> aircrafts = new List<string>
        {
            "Aircraft.Do-17Z-1",
            "Aircraft.Do-17Z-2",
            "Aircraft.Do-215B-1",
            "Aircraft.FW-200C-1",
            "Aircraft.He-111H-2",
            "Aircraft.He-111P-2",
            "Aircraft.Ju-88A-1",
        };

        private List<string> airGroupKeys = new List<string>
        {
            "BoB_LW_AufklGr_120",
            "BoB_LW_AufklGr_121",
            "BoB_LW_AufklGr_122",
            "BoB_LW_AufklGr_123",
            "BoB_LW_AufklGr_22",
            "BoB_LW_AufklGr_11",
            "BoB_LW_AufklGr_14",
            "BoB_LW_AufklGr_31",
            "BoB_LW_AufklGr_ObdL",
            "BoB_LW_AufklGr10",
            "BoB_LW_Wekusta_51",
            "BoB_LW_Wekusta_ObdL",
        };

        #endregion

        #region Public properties

        public override List<string> Aircrafts
        {
            get
            {
                return aircrafts;
            }
        }

        public override List<string> AirGroupKeys
        {
            get
            {
                return airGroupKeys;
            }
        }

        public override int SquadronCount
        {
            get { return 1; }
        }

        public override int FlightCount
        {
            get { return 4; }
        }

        public override int FlightSize
        {
            get { return 1; }
        }

        #endregion
    }

    public class RaFighterAirGroupInfo : AirGroupInfo
    {
        #region Private members

        private List<string> aircrafts = new List<string>
        {
            "Aircraft.CR42",
            "Aircraft.G50",
        };

        private List<string> airGroupKeys = new List<string>
        {
            //"it02", /* Generic Fighter */
            "BoB_RA_56St_18Gruppo_83Sq",
            "BoB_RA_56St_18Gruppo_85Sq",
            "BoB_RA_56St_18Gruppo_95Sq",
            "BoB_RA_56St_20Gruppo_351Sq",
            "BoB_RA_56St_20Gruppo_352Sq",
            "BoB_RA_56St_20Gruppo_353Sq",
        };

        #endregion

        #region Public properties

        public override List<string> Aircrafts
        {
            get
            {
                return aircrafts;
            }
        }

        public override List<string> AirGroupKeys
        {
            get
            {
                return airGroupKeys;
            }
        }

        public override int SquadronCount
        {
            get { return 1; }
        }

        public override int FlightCount
        {
            get { return 3; }
        }

        public override int FlightSize
        {
            get { return 3; }
        }

        #endregion
    }

    public class RaBomberAirGroupInfo : AirGroupInfo
    {
        #region Private members

        private List<string> aircrafts = new List<string>
        {
            "Aircraft.BR-20M",
        };

        private List<string> airGroupKeys = new List<string>
        {
            //"it01", /* Generic Bomber */
            "BoB_RA_13St_11Gruppo_1Sq",
            "BoB_RA_13St_11Gruppo_4Sq",
            "BoB_RA_13St_43Gruppo_3Sq",
            "BoB_RA_13St_43Gruppo_5Sq",
            "BoB_RA_43St_98Gruppo_240Sq",
            "BoB_RA_43St_98Gruppo_241Sq",
            "BoB_RA_43St_99Gruppo_242Sq",
            "BoB_RA_43St_99Gruppo_243Sq",
        };

        #endregion

        #region Public properties

        public override List<string> Aircrafts
        {
            get
            {
                return aircrafts;
            }
        }

        public override List<string> AirGroupKeys
        {
            get
            {
                return airGroupKeys;
            }
        }

        public override int SquadronCount
        {
            get { return 1; }
        }

        public override int FlightCount
        {
            get { return 3; }
        }

        public override int FlightSize
        {
            get { return 3; }
        }

        #endregion
    }
}