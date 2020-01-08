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
    public enum EStationaryType
    {
        Radar,
        Aircraft,
        Artillery,
        Depot,
        Unknown,
    }

    public class Stationary
    {
        private List<string> _depots = new List<string>
        {
            "Stationary.Morris_CS8_tank",
            "Stationary.Opel_Blitz_fuel",
        };

        private List<string> _aircrafts = new List<string>
        {
            "Stationary.AnsonMkI",
            "Stationary.BeaufighterMkIF",
            "Stationary.BeaufighterMkINF",
            "Stationary.Bf-108B-2",
            "Stationary.Bf-109E-1",
            "Stationary.Bf-109E-1B",
            "Stationary.Bf-109E-3",
            "Stationary.Bf-109E-3B",
            "Stationary.Bf-109E-4",
            "Stationary.Bf-109E-4_Late",
            "Stationary.Bf-109E-4N",
            "Stationary.Bf-109E-4N_Late",
            "Stationary.Bf-109E-4B",
            "Stationary.Bf-109E-4B_Late",
            "Stationary.Bf-110C-2",
            "Stationary.Bf-110C-4",
            "Stationary.Bf-110C-4Late",
            "Stationary.Bf-110C-4B",
            "Stationary.Bf-110C-4N",
            "Stationary.Bf-110C-4N_Late",
            "Stationary.Bf-110C-4-NJG",
            "Stationary.Bf-110C-6",
            "Stationary.Bf-110C-7",
            "Stationary.BlenheimMkI",
            "Stationary.BlenheimMkIF",
            "Stationary.BlenheimMkINF",
            "Stationary.BlenheimMkIV",
            "Stationary.BlenheimMkIVF",
            "Stationary.BlenheimMkIVNF",
            "Stationary.BlenheimMkIV_Late",
            "Stationary.BlenheimMkIVF_Late",
            "Stationary.BlenheimMkIVNF_Late",
            "Stationary.BR-20M",
            "Stationary.CR42",
            "Stationary.DefiantMkI",
            "Stationary.Do-17Z-1",
            "Stationary.Do-17Z-2",
            "Stationary.Do-215B-1",
            "Stationary.FW-200C-1",
            "Stationary.G50",
            "Stationary.GladiatorMkII",
            "Stationary.He-111H-2",
            "Stationary.He-111P-2",
            "Stationary.He-115B-2",
            "Stationary.SpitfireMkI_Heartbreaker",
            "Stationary.HurricaneMkI_dH5-20",
            "Stationary.HurricaneMkI_dH5-20_100oct",
            "Stationary.HurricaneMkI",
            "Stationary.HurricaneMkI_100oct",
            "Stationary.HurricaneMkI_100oct-NF",
            "Stationary.HurricaneMkI_FB",
            "Stationary.Ju-87B-2",
            "Stationary.Ju-88A-1",
            "Stationary.SpitfireMkI",
            "Stationary.SpitfireMkI_100oct",
            "Stationary.SpitfireMkIa",
            "Stationary.SpitfireMkIa_100oct",
            "Stationary.SpitfireMkIIa",
            "Stationary.SunderlandMkI",
            "Stationary.DH82A",
            "Stationary.WalrusMkI",
            "Stationary.WellingtonMkIc",
            "Stationary.Su-26M",
        };


        public Stationary(ISectionFile sectionFile, string id)
        {
            _id = id;

            string value = sectionFile.get("Stationary", id);

            string[] valueParts = value.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (valueParts.Length > 4)
            {
                Class = valueParts[0];
                Country = (ECountry)Enum.Parse(typeof(ECountry), valueParts[1]);
                double.TryParse(valueParts[2], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture.NumberFormat, out X);
                double.TryParse(valueParts[3], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture.NumberFormat, out Y);
                double.TryParse(valueParts[4], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture.NumberFormat, out Direction);

                if (valueParts.Length > 5)
                {
                    for (int i = 5; i < valueParts.Length; i++)
                    {
                        Options += valueParts[i] + " ";
                    }
                    Options = Options.Trim();
                }
            }
        }

        public Stationary(string id, string @class, ECountry country, double x, double y, double direction, string options = null)
        {
            _id = id;
            X = x;
            Y = y;
            Direction = direction;
            Class = @class;
            Country = country;
            Options = options;
        }

        public string Id
        {
            get
            {
                return _id;
            }
        }
        private string _id;

        public double X;

        public double Y;

        public double Direction;

        public string Class
        {
            get;
            set;
        }

        public ECountry Country
        {
            get;
            set;
        }

        public Point2d Position
        {
            get
            {
                return new Point2d(this.X, this.Y);
            }
        }


        public EStationaryType Type
        {
            get
            {
                // Type
                if (Class.StartsWith("Stationary.Radar"))
                {
                    return EStationaryType.Radar;
                }
                else if (Class.StartsWith("Artillery"))
                {
                    return EStationaryType.Artillery;
                }
                else if(_aircrafts.Contains(Class))
                {
                    return EStationaryType.Aircraft;
                }
                else if(_depots.Contains(Class))
                {
                    return EStationaryType.Depot;
                }             
                else
                {
                    return EStationaryType.Unknown;
                }
            }
        }


        public int Army
        {
            get
            {
                if (Country == ECountry.gb)
                {
                    return 1;
                }
                else if (Country == ECountry.de)
                {
                    return 2;
                }
                else
                {
                    return 0;
                }
            }
        }

        public string Options
        {
            get;
            set;
        }


        public void WriteTo(ISectionFile sectionFile)
        {
            string value = Class + " " + Country.ToString() + " " + X.ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat) + " " + Y.ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat) + " " + Direction.ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
            if (Options != null)
            {
                value += Options;
            }
            sectionFile.add("Stationary", Id, value);
        }
    }
}