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
        public class BattleIntroPage : PageDefImpl
        {
            public BattleIntroPage()
                : base("Battle Intro", new CampaignBattleIntro())
            {
                FrameworkElement.Fly.Click += new System.Windows.RoutedEventHandler(Fly_Click);
                FrameworkElement.Back.Click += new System.Windows.RoutedEventHandler(Back_Click);
            }

            void Back_Click(object sender, System.Windows.RoutedEventArgs e)
            {
                Game.gameInterface.PagePop(null);
            }

            void Fly_Click(object sender, System.Windows.RoutedEventArgs e)
            {
                if (Game is IGameSingle)
                {
                    IGameSingle gameSingle = Game as IGameSingle;
                    gameSingle.BattleSuccess = EBattleResult.DRAW;
                }

                Game.gameInterface.PageChange(new BattlePage(), null);
            }

            public override void _enter(maddox.game.IGame play, object arg)
            {
                base._enter(play, arg);

                _game = play as IGame;

                // TODO:                
                // Skip this page
                if (Game is IGameSingle)
                {
                    IGameSingle gameSingle = Game as IGameSingle;
                    gameSingle.BattleSuccess = EBattleResult.NONE;
                }

                Game.gameInterface.PageChange(new BattlePage(), null);
            }

            public override void _leave(maddox.game.IGame play, object arg)
            {
                base._leave(play, arg);

                _game = null;
            }

            private CampaignBattleIntro FrameworkElement
            {
                get
                {
                    return FE as CampaignBattleIntro;
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