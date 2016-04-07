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
using System.Collections.Generic;

using maddox.game;

namespace IL2DCE
{
    public class Career
    {
        public static List<string> RafRanks = new List<string> {
            "Pilot Officer",
            "Flying Officer",
            "Flight Lieutenant",
            "Squadron Leader",
            "Wing Commander",
            "Group Captain",
        };

        public static List<string> LwRanks = new List<string> {
            "Leutnant",
            "Oberleutnant",
            "Hauptmann",
            "Major",
            "Oberstleutnant",
            "Oberst",
        };

        public static List<string> RaRanks = new List<string> {
            "Sottotenente",
            "Tenente",
            "Capitano",
            "Maggiore",
            "Tenente Colonnello",
            "Colonnello",
        };

        public Career(string pilotName, int armyIndex, int airForceIndex, int rankIndex)
        {
            _pilotName = pilotName;
            _armyIndex = armyIndex;
            _airForceIndex = airForceIndex;
            _rankIndex = rankIndex;
            _experience = 0;

            _campaignInfo = null;
            _date = null;
            _airGroup = null;
            _missionFileName = null;

        }

        public Career(string pilotName, IList<CampaignInfo> campaignInfos, ISectionFile careerFile)
        {
            _pilotName = pilotName;

            if (careerFile.exist("Main", "armyIndex")
                && careerFile.exist("Main", "rankIndex")
                && careerFile.exist("Main", "experience"))
            {
                _armyIndex = int.Parse(careerFile.get("Main", "armyIndex"));
                _airForceIndex = int.Parse(careerFile.get("Main", "airForceIndex"));
                _rankIndex = int.Parse(careerFile.get("Main", "rankIndex"));
                _experience = int.Parse(careerFile.get("Main", "experience"));
            }
            else
            {
                throw new FormatException();
            }

            if (careerFile.exist("Campaign", "date")
                && careerFile.exist("Campaign", "id")
                && careerFile.exist("Campaign", "airGroup")
                && careerFile.exist("Campaign", "missionFile"))
            {
                string id = careerFile.get("Campaign", "id");
                foreach (CampaignInfo campaignInfo in campaignInfos)
                {
                    if (campaignInfo.Id == id)
                    {
                        CampaignInfo = campaignInfo;
                        break;
                    }
                }
                _date = DateTime.Parse(careerFile.get("Campaign", "date"));
                _airGroup = careerFile.get("Campaign", "airGroup");
                _missionFileName = careerFile.get("Campaign", "missionFile");
            }
            else
            {
                throw new FormatException();
            }
        }

        public override string ToString()
        {
            if(ArmyIndex == 1 && AirForceIndex == 1)
            {
                return RafRanks[RankIndex] + " " + PilotName;
            }
            else if (ArmyIndex == 2 && AirForceIndex == 1)
            {
                return LwRanks[RankIndex] + " " + PilotName;
            }
            else if (ArmyIndex == 2 && AirForceIndex == 2)
            {
                return RaRanks[RankIndex] + " " + PilotName;
            }
            else
            {
                return PilotName;
            }
        }

        public string PilotName
        {
            get
            {
                return _pilotName;
            }
            set
            {
                _pilotName = value;
            }
        }
        private string _pilotName;

        public int ArmyIndex
        {
            get
            {
                return _armyIndex;
            }
            set
            {
                _armyIndex = value;
            }
        }
        private int _armyIndex;

        public int AirForceIndex
        {
            get
            {
                return _airForceIndex;
            }
            set
            {
                _airForceIndex = value;
            }
        }
        private int _airForceIndex;

        public int RankIndex
        {
            get
            {
                return _rankIndex;
            }
            set
            {
                if (value < 6)
                {
                    _rankIndex = value;
                }
            }
        }
        private int _rankIndex;

        public int Experience
        {
            get
            {
                return _experience;
            }
            set
            {
                _experience = value;
            }
        }
        private int _experience;

        public CampaignInfo CampaignInfo
        {
            get
            {
                return _campaignInfo;
            }
            set
            {
                _campaignInfo = value;
            }
        }
        private CampaignInfo _campaignInfo;

        public DateTime? Date
        {
            get
            {
                return _date;
            }
            set
            {
                _date = value;
            }
        }
        DateTime? _date;

        public string MissionFileName
        {
            get
            {
                return _missionFileName;
            }
            set
            {
                _missionFileName = value;
            }
        }
        private string _missionFileName;

        public string AirGroup
        {
            get
            {
                return _airGroup;
            }
            set
            {
                _airGroup = value;
            }
        }
        private string _airGroup;

        public void WriteTo(ISectionFile careerFile)
        {
            careerFile.add("Main", "armyIndex", ArmyIndex.ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat));
            careerFile.add("Main", "airForceIndex", AirForceIndex.ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat));
            careerFile.add("Main", "rankIndex", RankIndex.ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat));
            careerFile.add("Main", "experience", Experience.ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat));

            careerFile.add("Campaign", "date", Date.Value.Year.ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat) + "-" + Date.Value.Month.ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat) + "-" + Date.Value.Day.ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat));
            careerFile.add("Campaign", "airGroup", AirGroup);
            careerFile.add("Campaign", "missionFile", MissionFileName);
            careerFile.add("Campaign", "id", CampaignInfo.Id);
        }
    }
}