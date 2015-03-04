#region LICENSE
// NemeStats is a free website for tracking the results of board games.
//     Copyright (C) 2015 Jacob Gordon
// 
//     This program is free software: you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     This program is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU General Public License for more details.
// 
//     You should have received a copy of the GNU General Public License
//     along with this program.  If not, see <http://www.gnu.org/licenses/>
#endregion
using BusinessLogic.Models;
using NUnit.Framework;
using System;
using System.Linq;
using UI.Models.Players;
using UI.Transformations.PlayerTransformations;

namespace UI.Tests.UnitTests.TransformationsTests.PlayerTransformationTests
{
    [TestFixture]
    public class MinionViewModelBuilderTests
    {
        private MinionViewModelBuilder builder;
        private Player player;
        private MinionViewModel viewModelResult;

        [SetUp]
        public void SetUp()
        {
            builder = new MinionViewModelBuilder();
            player = new Player()
            {
                Id = 999,
                Name = "minion player name",
                Nemesis = new Nemesis()
                {
                    LossPercentage = 15,
                    NumberOfGamesLost = 150
                }
            };

            viewModelResult = builder.Build(player);
        }

        [Test]
        public void ItRequiresThatThePlayerHasNemesisDataPopulated()
        {
            Player player = new Player();
            MinionViewModelBuilder builder = new MinionViewModelBuilder();

            var exception = Assert.Throws<ArgumentException>(() =>
                    builder.Build(player));

            Assert.AreEqual(MinionViewModelBuilder.EXCEPTION_NEMESIS_CANNOT_BE_NULL, exception.Message);
        }

        [Test]
        public void ItSetsTheMinionPlayerId()
        {
            Assert.AreEqual(player.Id, viewModelResult.MinionPlayerId);
        }

        [Test]
        public void ItSetsTheMinionPlayerName()
        {
            Assert.AreEqual(player.Name, viewModelResult.MinionName);
        }

        [Test]
        public void ItSetsTheWinPercentageVsThisMinion()
        {
            Assert.AreEqual(player.Nemesis.LossPercentage, viewModelResult.WinPercentageVersusMinion);
        }

        [Test]
        public void ItSetsTheNumberOfGamesWonVsThisMinion()
        {
            Assert.AreEqual(player.Nemesis.NumberOfGamesLost, viewModelResult.NumberOfGamesWonVersusMinion);
        }
    }
}
