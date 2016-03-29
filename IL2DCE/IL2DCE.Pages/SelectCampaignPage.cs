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
        public class SelectCampaignPage : PageDefImpl
        {
            public SelectCampaignPage()
                : base("Select Campaign", new SelectCampaign())
            {
                FrameworkElement.bBack.Click += new System.Windows.RoutedEventHandler(bBack_Click);
                FrameworkElement.bNew.Click += new System.Windows.RoutedEventHandler(bNew_Click);
                FrameworkElement.bContinue.Click += new System.Windows.RoutedEventHandler(bContinue_Click);

                FrameworkElement.ListCampaign.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(listCampaign_SelectionChanged);

                // TODO: Make button visible when it is possible to continue a campaign.
                FrameworkElement.bNew.IsEnabled = false;
                FrameworkElement.bContinue.Visibility = System.Windows.Visibility.Hidden;                
            }

            public override void _enter(maddox.game.IGame play, object arg)
            {
                base._enter(play, arg);

                _game = play as IGame;

                FrameworkElement.ListCampaign.ItemsSource = Game.Core.CampaignInfos;

                if (FrameworkElement.ListCampaign.Items.Count > 0)
                {
                    FrameworkElement.ListCampaign.SelectedIndex = 0;
                }
                else
                {
                    FrameworkElement.ListCampaign.SelectedIndex = -1;
                }
            }

            public override void _leave(maddox.game.IGame play, object arg)
            {
                base._leave(play, arg);

                _game = null;
            }

            private SelectCampaign FrameworkElement
            {
                get
                {
                    return FE as SelectCampaign;
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
                Game.Core.InitCampaign();

                Game.gameInterface.PageChange(new CampaignIntoPage(), null);
            }

            private void bContinue_Click(object sender, System.Windows.RoutedEventArgs e)
            {
                Game.gameInterface.PagePop(null);
            }

            private void listCampaign_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
            {
                if (e.AddedItems.Count > 0)
                {
                    CampaignInfo campaignSelected = e.AddedItems[0] as CampaignInfo;
                    Game.Core.CurrentCareer.CampaignInfo = campaignSelected;
                }

                if (Game.Core.CurrentCareer.CampaignInfo != null)
                {
                    FrameworkElement.bNew.IsEnabled = true;
                }
                else
                {
                    FrameworkElement.bNew.IsEnabled = false;
                }
            }
        }
    }
}
