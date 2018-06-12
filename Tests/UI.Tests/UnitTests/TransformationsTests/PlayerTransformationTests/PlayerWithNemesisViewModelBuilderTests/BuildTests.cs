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
using BusinessLogic.Models.User;
using NUnit.Framework;
using System;
using System.Linq;
using BusinessLogic.Models.Points;
using UI.Models.Players;
using UI.Models.Points;
using UI.Transformations.PlayerTransformations;

namespace UI.Tests.UnitTests.TransformationsTests.PlayerTransformationTests.PlayerWithNemesisViewModelBuilderTests
{
    [TestFixture]
    public class BuildTests
    {
        private PlayerWithNemesisViewModelBuilder builder;
        private PlayerWithNemesis playerWithNemesis;
        private readonly int gamingGroupId = 1;
        private ApplicationUser currentUser;

        [SetUp]
        public void SetUp()
        {
            builder = new PlayerWithNemesisViewModelBuilder();
            playerWithNemesis = new PlayerWithNemesis
            {
                PlayerId = 100,
                PlayerRegistered = true,
                PlayerName = "player name",
                PlayerActive = true,
                NemesisPlayerId = 300,
                NemesisPlayerName = "nemesis player name",
                PreviousNemesisPlayerId = 400,
                PreviousNemesisPlayerName = "previous nemesis player name",
                GamingGroupId = gamingGroupId,
                GamesLost = 1,
                GamesWon = 1,
                NemePointsSummary = new NemePointsSummary(1, 4, 5),
                TotalChampionedGames = 1
            };

            currentUser = new ApplicationUser
            {
                CurrentGamingGroupId = gamingGroupId
            };
        }

        [Test]
        public void ItThrowsAnArgumentNullExceptionIfThePlayerIsNull()
        {
            Exception actualException = Assert.Throws<ArgumentNullException>(() => builder.Build(null, null, currentUser));

            Assert.AreEqual(new ArgumentNullException("player").Message, actualException.Message);
        }

        [Test]
        public void ItCopiesThePlayerId()
        {
            PlayerWithNemesisViewModel actualViewModel = builder.Build(playerWithNemesis, null, currentUser);

            Assert.AreEqual(playerWithNemesis.PlayerId, actualViewModel.PlayerId);
        }

        [Test]
        public void ItCopiesThePlayerNameForActivePlayers()
        {
            PlayerWithNemesisViewModel actualViewModel = builder.Build(playerWithNemesis, null, currentUser);

            Assert.AreEqual(playerWithNemesis.PlayerName, actualViewModel.PlayerName);
        }

        [Test]
        public void ItAddsAnInactivePlayerSuffixToInactivePlayers()
        {
            playerWithNemesis.PlayerActive = false;
            string expectedPlayerName = playerWithNemesis.PlayerName + " (INACTIVE)";

            PlayerWithNemesisViewModel actualViewModel = builder.Build(playerWithNemesis, null, currentUser);

            Assert.AreEqual(expectedPlayerName, actualViewModel.PlayerName);
        }

        [Test]
        public void ItSetsThePlayerRegisteredFlagToTrueIfThePlayerHasAnApplicationUserId()
        {
            PlayerWithNemesisViewModel actualViewModel = builder.Build(playerWithNemesis, null, currentUser);

            Assert.AreEqual(true, actualViewModel.PlayerRegistered);
        }

        [Test]
        public void ItCopiesThePlayerRegisteredFlag()
        {
            PlayerWithNemesisViewModel actualViewModel = builder.Build(playerWithNemesis, null, currentUser);

            Assert.AreEqual(playerWithNemesis.PlayerRegistered, actualViewModel.PlayerRegistered);
        }

        [Test]
        public void ItCopiesTheNemesisPlayerId()
        {
            PlayerWithNemesisViewModel actualViewModel = builder.Build(playerWithNemesis, null, currentUser);

            Assert.AreEqual(playerWithNemesis.NemesisPlayerId, actualViewModel.NemesisPlayerId);
        }

        [Test]
        public void ItCopiesTheNemesisPlayerName()
        {
            PlayerWithNemesisViewModel actualViewModel = builder.Build(playerWithNemesis, null, currentUser);

            Assert.AreEqual(playerWithNemesis.NemesisPlayerName, actualViewModel.NemesisPlayerName);
        }

        [Test]
        public void ItCopiesThePlayerActiveFlag()
        {
            PlayerWithNemesisViewModel actualViewModel = builder.Build(playerWithNemesis, null, currentUser);

            Assert.AreEqual(playerWithNemesis.PlayerActive, actualViewModel.PlayerActive);
        }

        [Test]
        public void ItCopiesThePreviousNemesisPlayerId()
        {
            PlayerWithNemesisViewModel actualViewModel = builder.Build(playerWithNemesis, null, currentUser);

            Assert.AreEqual(playerWithNemesis.PreviousNemesisPlayerId, actualViewModel.PreviousNemesisPlayerId);
        }

        [Test]
        public void ItCopiesThePreviousNemesisPlayerName()
        {
            PlayerWithNemesisViewModel actualViewModel = builder.Build(playerWithNemesis, null, currentUser);

            Assert.AreEqual(playerWithNemesis.PreviousNemesisPlayerName, actualViewModel.PreviousNemesisPlayerName);
        }

        [Test]
        public void ItCopiesTheNumberOfPlayedGames()
        {
            PlayerWithNemesisViewModel actualViewModel = builder.Build(playerWithNemesis, null, currentUser);

            Assert.AreEqual(playerWithNemesis.GamesWon + playerWithNemesis.GamesLost, actualViewModel.NumberOfPlayedGames);
        }

        [Test]
        public void ItSetsTheOverallWinPercentage()
        {
            PlayerWithNemesisViewModel actualViewModel = builder.Build(playerWithNemesis, null, currentUser);

            Assert.AreEqual(50, actualViewModel.OverallWinPercentage);
        }

        [Test]
        public void ItCopiesTheNemePointsSummary()
        {
            PlayerWithNemesisViewModel actualViewModel = builder.Build(playerWithNemesis, null, currentUser);

            var expected = new NemePointsSummaryViewModel(playerWithNemesis.NemePointsSummary);
            Assert.AreEqual(expected, actualViewModel.NemePointsSummary);
        }

        [Test]
        public void ItSetsTheRegisteredUserEmailAddress()
        {
            //--arrange
            string email = "someemail@email.com";

            //--act
            var actualViewModel = builder.Build(playerWithNemesis, email, currentUser);

            //--assert
            Assert.AreEqual(email, actualViewModel.RegisteredUserEmailAddress);
        }

        [Test]
        public void ItCalculatesAveragePoints()
        {
            PlayerWithNemesisViewModel actualViewModel = builder.Build(playerWithNemesis, null, currentUser);

            var avgPoints = (float) playerWithNemesis.NemePointsSummary.TotalPoints /
                            (float) (playerWithNemesis.GamesWon + playerWithNemesis.GamesLost);
            Assert.AreEqual(avgPoints, actualViewModel.AveragePointsPerGame);
        }

        [Test]
        public void TheUserHasChampionBadges()
        {
            PlayerWithNemesisViewModel actualViewModel = builder.Build(playerWithNemesis, null, currentUser);

            Assert.AreEqual(playerWithNemesis.TotalChampionedGames, actualViewModel.TotalChampionedGames);
        }

        [Test]
        public void TheUserCanEditViewModelIfTheyShareGamingGroups()
        {
            PlayerWithNemesisViewModel actualViewModel = builder.Build(playerWithNemesis, null, currentUser);

            Assert.True(actualViewModel.UserCanEdit);
        }

        [Test]
        public void TheUserCanNotEditViewModelIfTheyDoNotShareGamingGroups()
        {
            currentUser.CurrentGamingGroupId = -1;
            PlayerWithNemesisViewModel actualViewModel = builder.Build(playerWithNemesis, null, currentUser);

            Assert.False(actualViewModel.UserCanEdit);
        }

        [Test]
        public void TheUserCanNotEditViewModelIfTheUserIsUnknown()
        {
            PlayerWithNemesisViewModel actualViewModel = builder.Build(playerWithNemesis, null, null);

            Assert.False(actualViewModel.UserCanEdit);
        }
    }
}
