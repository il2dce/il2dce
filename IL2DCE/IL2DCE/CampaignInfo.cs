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

using maddox.game;

namespace IL2DCE
{
    public class CampaignInfo
    {
        ISectionFile _globalAircraftInfoFile;
        ISectionFile _localAircraftInfoFile;

        public CampaignInfo(string id, string campaignFolderPath, ISectionFile campaignFile, ISectionFile glocalAircraftInfoFile, ISectionFile localAircraftInfoFile = null)
        {
            _id = id;
            _globalAircraftInfoFile = glocalAircraftInfoFile;
            _localAircraftInfoFile = localAircraftInfoFile;

            if (campaignFile.exist("Main", "name"))
            {
                name = campaignFile.get("Main", "name");
            }

            if (campaignFile.exist("Main", "staticTemplateFile"))
            {
                staticTemplateFilePath = campaignFolderPath + campaignFile.get("Main", "staticTemplateFile");
            }

            if (campaignFile.exist("Main", "templateFile"))
            {
                templateFilePath = campaignFolderPath + campaignFile.get("Main", "templateFile");
            }

            if (campaignFile.exist("Main", "scriptFile"))
            {
                _scriptFileName = campaignFile.get("Main", "scriptFile");
            }

            if (campaignFile.exist("Main", "startDate"))
            {
                string startDateString = campaignFile.get("Main", "startDate");
                _startDate = DateTime.Parse(startDateString);
            }

            if (campaignFile.exist("Main", "endDate"))
            {
                string endDateString = campaignFile.get("Main", "endDate");
                _endDate = DateTime.Parse(endDateString);
            }
        }

        public override string ToString()
        {
            return Name;
        }

        public string Id
        {
            get
            {
                return _id;
            }
        }
        string _id;

        public string Name
        {
            get
            {
                return name;
            }
        }
        string name;

        public string StaticTemplateFilePath
        {
            get
            {
                return staticTemplateFilePath;
            }
        }
        private string staticTemplateFilePath;

        public string TemplateFilePath
        {
            get
            {
                return templateFilePath;
            }
        }
        private string templateFilePath;

        public string ScriptFileName
        {
            get
            {
                return _scriptFileName;
            }
        }
        private string _scriptFileName;

        public DateTime StartDate
        {
            get
            {
                return _startDate;
            }
        }
        private DateTime _startDate;

        public DateTime EndDate
        {
            get
            {
                return _endDate;
            }
        }
        private DateTime _endDate;

        public AircraftInfo GetAircraftInfo(string aircraft)
        {
            if(_localAircraftInfoFile != null && _localAircraftInfoFile.exist("Main", aircraft))
            {
                return new AircraftInfo(_localAircraftInfoFile, aircraft);
            }
            else if(_globalAircraftInfoFile.exist("Main", aircraft))
            {
                return new AircraftInfo(_globalAircraftInfoFile, aircraft);
            }
            else
            {
                throw new ArgumentException();
            }
        }
    }
}