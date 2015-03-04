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
using BusinessLogic.Models.Players;
using NUnit.Framework;
using System.Linq;
using UI.Models.Players;
using UI.Transformations.PlayerTransformations;

namespace UI.Tests.UnitTests.TransformationsTests.PlayerTransformationTests.TopPlayerViewModelBuilderTests
{
    [TestFixture]
    public class BuildTests
    {
        private TopPlayerViewModelBuilder builder;
        private TopPlayer topPlayer;
        private TopPlayerViewModel topPlayerViewModel;

        [SetUp]
        public void SetUp()
        {
            builder = new TopPlayerViewModelBuilder();
            topPlayer = new TopPlayer()
            {
                PlayerId = 1,
                PlayerName = "player name",
                TotalNumberOfGamesPlayed = 100,
                TotalPoints = 1234,
                WinPercentage = 75
            };

            topPlayerViewModel = builder.Build(topPlayer);
        }

        [Test]
        public void ItCopiesThePlayerId()
        {
            Assert.AreEqual(topPlayer.PlayerId, topPlayerViewModel.PlayerId);
        }

        [Test]
        public void ItCopiesThePlayerName()
        {
            Assert.AreEqual(topPlayer.PlayerName, topPlayerViewModel.PlayerName);
        }

        [Test]
        public void ItCopiesTheTotalNumberOfGamesPlayed()
        {
            Assert.AreEqual(topPlayer.TotalNumberOfGamesPlayed, topPlayerViewModel.TotalNumberOfGamesPlayed);
        }

        [Test]
        public void ItCopiesTheTotalPoints()
        {
            Assert.AreEqual(topPlayer.TotalPoints, topPlayerViewModel.TotalPoints);
        }

        [Test]
        public void ItCopiesTheWinPercentage()
        {
            Assert.AreEqual(topPlayer.WinPercentage, topPlayerViewModel.WinPercentage);
        }
    }
}
