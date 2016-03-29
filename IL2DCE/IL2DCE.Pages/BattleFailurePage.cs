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
        public class BattleFailurePage : PageDefImpl
        {
            public BattleFailurePage()
                : base("Battle Failure", new CampaignBattleFailure())
            {
                FrameworkElement.Fly.Click += new System.Windows.RoutedEventHandler(Fly_Click);
                FrameworkElement.ReFly.Click += new System.Windows.RoutedEventHandler(ReFly_Click);
                FrameworkElement.Back.Click += new System.Windows.RoutedEventHandler(Back_Click);

                FrameworkElement.Fly.IsEnabled = false;
                FrameworkElement.Fly.Visibility = System.Windows.Visibility.Hidden;
            }

            void ReFly_Click(object sender, System.Windows.RoutedEventArgs e)
            {
                Game.gameInterface.PageChange(new BattleIntroPage(), null);
            }

            void Back_Click(object sender, System.Windows.RoutedEventArgs e)
            {
                Game.gameInterface.PagePop(null);
            }

            void Fly_Click(object sender, System.Windows.RoutedEventArgs e)
            {
                Game.Core.AdvanceCampaign(Game);

                Game.gameInterface.PageChange(new BattleIntroPage(), null);
            }

            public override void _enter(maddox.game.IGame play, object arg)
            {
                base._enter(play, arg);

                _game = play as IGame;

                if (Game is IGameSingle)
                {
                    FrameworkElement.textBoxDescription.Text = (Game as IGameSingle).BattleSuccess.ToString();
                }
            }

            public override void _leave(maddox.game.IGame play, object arg)
            {
                base._leave(play, arg);

                _game = null;
            }

            private CampaignBattleFailure FrameworkElement
            {
                get
                {
                    return FE as CampaignBattleFailure;
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
        }
    }
}