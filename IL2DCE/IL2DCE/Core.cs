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
    public class Core
    {
        public Core(IGame game)
            : this(game, new Random())
        {

        }

        public Core(IGame game, IRandom random)
        {
            _gamePlay = game;
            _random = random;

            
            ISectionFile confFile = game.gameInterface.SectionFileLoad("$home/parts/IL2DCE/conf.ini");
            _config = new Config(confFile);

            string campaignsFolderPath = Config.CampaignsFolder;

            this._campaignsFolderSystemPath = game.gameInterface.ToFileSystemPath(Config.CampaignsFolder);
            System.IO.DirectoryInfo campaignsFolder = new System.IO.DirectoryInfo(_campaignsFolderSystemPath);
            if (campaignsFolder.Exists && campaignsFolder.GetDirectories() != null && campaignsFolder.GetDirectories().Length > 0)
            {
                ISectionFile globalAircraftInfoFile = game.gameInterface.SectionFileLoad(campaignsFolderPath + "/" + "AircraftInfo.ini");
                foreach (System.IO.DirectoryInfo campaignFolder in campaignsFolder.GetDirectories())
                {
                    if (campaignFolder.GetFiles("CampaignInfo.ini") != null && campaignFolder.GetFiles("CampaignInfo.ini").Length == 1)
                    {
                        ISectionFile campaignInfoFile = game.gameInterface.SectionFileLoad(campaignsFolderPath + "/" + campaignFolder.Name + "/CampaignInfo.ini");

                        ISectionFile localAircraftInfoFile = null;
                        System.IO.FileInfo localAircraftInfoFileInfo = new System.IO.FileInfo(game.gameInterface.ToFileSystemPath(campaignsFolderPath + "/" + campaignFolder.Name + "/AircraftInfo.ini"));
                        if (localAircraftInfoFileInfo.Exists)
                        {
                            localAircraftInfoFile = game.gameInterface.SectionFileLoad(campaignsFolderPath + "/" + campaignFolder.Name + "/AircraftInfo.ini");
                        }

                        CampaignInfo campaignInfo = new CampaignInfo(campaignFolder.Name, campaignsFolderPath + "/" + campaignFolder.Name + "/", campaignInfoFile, globalAircraftInfoFile, localAircraftInfoFile);
                        CampaignInfos.Add(campaignInfo);
                    }
                }
            }

            this._careersFolderSystemPath = game.gameInterface.ToFileSystemPath("$user/mission/IL2DCE");
            System.IO.DirectoryInfo careersFolder = new System.IO.DirectoryInfo(_careersFolderSystemPath);
            if (careersFolder.Exists && careersFolder.GetDirectories() != null && careersFolder.GetDirectories().Length > 0)
            {
                foreach (System.IO.DirectoryInfo careerFolder in careersFolder.GetDirectories())
                {
                    if (careerFolder.GetFiles("Career.ini") != null && careerFolder.GetFiles("Career.ini").Length == 1)
                    {
                        ISectionFile careerFile = game.gameInterface.SectionFileLoad("$user/mission/IL2DCE" + "/" + careerFolder.Name + "/Career.ini");

                        Career career = new Career(careerFolder.Name, CampaignInfos, careerFile);
                        AvailableCareers.Add(career);
                    }
                }
            }

            this._debugFolderSystemPath = game.gameInterface.ToFileSystemPath("$user/missions/IL2DCE/Debug");
        }

        public void ResetCampaign(IGame game)
        {
            // Reset campaign state
            CurrentCareer.Date = null;

            AdvanceCampaign(game);
        }

        public void AdvanceCampaign(IGame game)
        {
            Generator generator = new Generator(this);
            
            ISectionFile previousMissionTemplateFile = null;

            if (!CurrentCareer.Date.HasValue)
            {
                // It is the first mission.
                CurrentCareer.Date = CurrentCareer.CampaignInfo.StartDate;
                CurrentCareer.Experience = CurrentCareer.RankIndex * 1000;
                                
                // Generate the initial mission tempalte
                generator.GenerateInitialMissionTempalte(CurrentCareer.CampaignInfo.InitialMissionTemplateFiles, out previousMissionTemplateFile);
            }
            else
            {
                CurrentCareer.Date = CurrentCareer.Date.Value.Add(new TimeSpan(1, 0, 0, 0));

                if (game is IGameSingle)
                {
                    if ((game as IGameSingle).BattleSuccess == EBattleResult.SUCCESS)
                    {
                        CurrentCareer.Experience += 200;
                    }
                    else if ((game as IGameSingle).BattleSuccess == EBattleResult.DRAW)
                    {
                        CurrentCareer.Experience += 100;
                    }
                }

                if (CurrentCareer.Experience >= (CurrentCareer.RankIndex + 1) * 1000)
                {
                    CurrentCareer.RankIndex += 1;
                }

                previousMissionTemplateFile = game.gpLoadSectionFile(CurrentCareer.MissionTemplateFileName);
            }

            string missionFolderSystemPath = this._careersFolderSystemPath + "\\" + CurrentCareer.PilotName;
            if (!System.IO.Directory.Exists(missionFolderSystemPath))
            {
                System.IO.Directory.CreateDirectory(missionFolderSystemPath);
            }

            if (game.gameInterface.BattleIsRun())
            {
                // Stop the currntly running battle.
                game.gameInterface.BattleStop();
            }

            // Preload mission file for path calculation.
            game.gameInterface.MissionLoad(CurrentCareer.CampaignInfo.StaticTemplateFiles[0]);

            ISectionFile careerFile = GamePlay.gpCreateSectionFile();
            string careerFileName = "$user/mission/IL2DCE/" + CurrentCareer.PilotName + "/Career.ini";
            string missionId = CurrentCareer.CampaignInfo.Id + "_" + CurrentCareer.Date.Value.Date.Year.ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat) + "-" + CurrentCareer.Date.Value.Date.Month.ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat) + "-" + CurrentCareer.Date.Value.Date.Day.ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat);

            string missionFileName = string.Format("$user/mission/IL2DCE/" + CurrentCareer.PilotName + "/{0}.mis", missionId);
            CurrentCareer.MissionFileName = missionFileName;

            // Generate the template for the next mission
            ISectionFile missionTemplateFile = null;
            generator.GenerateMissionTemplate(CurrentCareer.CampaignInfo.StaticTemplateFiles, previousMissionTemplateFile, out missionTemplateFile);
            missionTemplateFile.save(CurrentCareer.MissionTemplateFileName);
            
            // Generate the next mission based on the new template.

            ISectionFile missionFile = null;
            BriefingFile briefingFile = null;
            generator.GenerateMission(CurrentCareer.CampaignInfo.EnvironmentTemplateFile, CurrentCareer.MissionTemplateFileName, missionId, out missionFile, out briefingFile);

            string briefingFileSystemPath = string.Format(this._careersFolderSystemPath + "\\" + CurrentCareer.PilotName + "\\{0}.briefing", missionId);
            string scriptSourceFileSystemPath = this._campaignsFolderSystemPath + "\\" + CurrentCareer.CampaignInfo.Id + "\\" + CurrentCareer.CampaignInfo.ScriptFileName;
            string scriptDestinationFileSystemPath = this._careersFolderSystemPath + "\\" + CurrentCareer.PilotName + "\\" + missionId + ".cs";
            System.IO.File.Copy(scriptSourceFileSystemPath, scriptDestinationFileSystemPath, true);

            missionFile.save(missionFileName);
            briefingFile.SaveTo(briefingFileSystemPath);

            // Stop the preloaded battle to prevent a postload.
            game.gameInterface.BattleStop();


#if DEBUG
            Config.Debug = 1;
#endif
            if (Config.Debug == 1)
            {
                if (!System.IO.Directory.Exists(this._debugFolderSystemPath))
                {
                    System.IO.Directory.CreateDirectory(this._debugFolderSystemPath);
                }
                missionTemplateFile.save("$user/missions/IL2DCE/Debug/IL2DCEDebugTemplate.mis");
                missionFile.save("$user/missions/IL2DCE/Debug/IL2DCEDebug.mis");
                briefingFile.SaveTo(this._debugFolderSystemPath + "\\IL2DCEDebug.briefing");
                System.IO.File.Copy(scriptSourceFileSystemPath, this._debugFolderSystemPath + "\\IL2DCEDebug.cs", true);
            }

            CurrentCareer.WriteTo(careerFile);
            careerFile.save(careerFileName);
        }
        
        public void InitCampaign()
        {
            
        }
        
        public void DeleteCareer(Career career)
        {
            AvailableCareers.Remove(career);
            if (CurrentCareer == career)
            {
                CurrentCareer = null;
            }

            List<System.IO.DirectoryInfo> deleteFolders = new List<System.IO.DirectoryInfo>();
            System.IO.DirectoryInfo careersFolder = new System.IO.DirectoryInfo(this._careersFolderSystemPath);
            if (careersFolder.Exists && careersFolder.GetDirectories() != null && careersFolder.GetDirectories().Length > 0)
            {
                foreach (System.IO.DirectoryInfo careerFolder in careersFolder.GetDirectories())
                {
                    if (career.PilotName == careerFolder.Name)
                    {
                        deleteFolders.Add(careerFolder);
                    }
                }
            }

            for (int i = 0; i < deleteFolders.Count; i++)
            {
                deleteFolders[i].Delete(true);
            }
        }
        
        public Config Config
        {
            get
            {
                return _config;
            }
        }

        public Career CurrentCareer
        {
            get
            {
                return _currentCareer;
            }
            set
            {
                if (_currentCareer != value)
                {
                    _currentCareer = value;
                }
            }
        }

        public IList<Career> AvailableCareers
        {
            get
            {
                return _availableCareers;
            }
        }

        public IList<CampaignInfo> CampaignInfos
        {
            get
            {
                return _campaigns;
            }
        }

        public IGamePlay GamePlay
        {
            get
            {
                return _gamePlay;
            }
        }

        public IRandom Random
        {
            get
            {
                return _random;
            }
        }

        private string _careersFolderSystemPath;
        private string _campaignsFolderSystemPath;
        private string _debugFolderSystemPath;
        private Config _config;
        private Career _currentCareer;
        private IList<Career> _availableCareers = new List<Career>();
        private IList<CampaignInfo> _campaigns = new List<CampaignInfo>();
        private IGamePlay _gamePlay;
        private IRandom _random;
    }
}