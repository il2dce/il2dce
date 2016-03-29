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
using System.Collections;
using System.Collections.Generic;

using maddox.game;
using maddox.game.play;
using maddox.game.page;

namespace IL2DCE
{
    namespace Pages
    {
        public class SelectCareerPage : PageDefImpl
        {
            public SelectCareerPage()
                : base("Select Career", new SelectCareer())
            {
                FrameworkElement.Back.Click += new System.Windows.RoutedEventHandler(bBack_Click);
                FrameworkElement.New.Click += new System.Windows.RoutedEventHandler(bNew_Click);
                FrameworkElement.Delete.Click += new System.Windows.RoutedEventHandler(Delete_Click);
                FrameworkElement.Continue.Click += new System.Windows.RoutedEventHandler(bContinue_Click);

                FrameworkElement.ListCareer.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(listCampaign_SelectionChanged);

                FrameworkElement.Continue.IsEnabled = false;
                FrameworkElement.Delete.IsEnabled = false;
            }

            public override void _enter(maddox.game.IGame play, object arg)
            {
                base._enter(play, arg);

                _game = play as IGame;

                FrameworkElement.ListCareer.ItemsSource = Game.Core.AvailableCareers;

                if (FrameworkElement.ListCareer.Items.Count > 0)
                {
                    FrameworkElement.ListCareer.SelectedIndex = 0;
                }
                else
                {
                    FrameworkElement.ListCareer.SelectedIndex = -1;
                }

                FrameworkElement.ListCareer.Items.Refresh();
            }

            public override void _leave(maddox.game.IGame play, object arg)
            {
                base._leave(play, arg);

                _game = null;
            }

            private SelectCareer FrameworkElement
            {
                get
                {
                    return FE as SelectCareer;
                }
            }

            private IGame Game
            {
                get
                {
                    return _game;
                }
            }
            private IGame _game;

            private void bBack_Click(object sender, System.Windows.RoutedEventArgs e)
            {
                Game.gameInterface.PagePop(null);
            }

            private void bNew_Click(object sender, System.Windows.RoutedEventArgs e)
            {
                Game.gameInterface.PageChange(new CareerIntroPage(), null);
            }

            private void bContinue_Click(object sender, System.Windows.RoutedEventArgs e)
            {
                if (Game.Core.CurrentCareer.CampaignInfo != null)
                {
                    Game.Core.InitCampaign();
                    Game.gameInterface.PageChange(new BattleIntroPage(), null);
                }
                else
                {
                    Game.gameInterface.PageChange(new SelectCampaignPage(), null);
                }
            }

            void Delete_Click(object sender, System.Windows.RoutedEventArgs e)
            {
                Game.Core.DeleteCareer(Game.Core.CurrentCareer);
                FrameworkElement.ListCareer.Items.RemoveAt(FrameworkElement.ListCareer.SelectedIndex);

                FrameworkElement.ListCareer.Items.Refresh();
            }

            private void listCampaign_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
            {
                if (e.AddedItems.Count > 0)
                {
                    Career careerSelected = e.AddedItems[0] as Career;
                    Game.Core.CurrentCareer = careerSelected;
                }

                if (Game.Core.CurrentCareer != null)
                {
                    FrameworkElement.Continue.IsEnabled = true;
                    FrameworkElement.Delete.IsEnabled = true;
                }
                else
                {
                    FrameworkElement.Continue.IsEnabled = false;
                    FrameworkElement.Delete.IsEnabled = false;
                }
            }
        }
    }
}
