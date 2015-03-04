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
using BusinessLogic.Models.Players;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace BusinessLogic.Tests.UnitTests.LogicTests.PlayersTests.PlayerRetrieverTests
{
    [TestFixture]
    public class GetAllPlayersWithNemesisInfoTests : PlayerRetrieverTestBase
    {
        [Test]
        public void ItOnlyReturnsPlayersForTheGivenGamingGroup()
        {
            List<PlayerWithNemesis> players = playerRetriever.GetAllPlayersWithNemesisInfo(gamingGroupId);

            Assert.True(players.All(player => player.GamingGroupId == gamingGroupId));
        }

        [Test]
        public void ItReturnsPlayersOrderedByTheirNameAscending()
        {
            List<PlayerWithNemesis> players = playerRetriever.GetAllPlayersWithNemesisInfo(gamingGroupId);

            string lastPlayerName = "0";
            foreach (PlayerWithNemesis player in players)
            {
                Assert.LessOrEqual(lastPlayerName, player.PlayerName);
                lastPlayerName = player.PlayerName;
            }
        }

        [Test]
        public void ItReturnsTheNumberOfGamesPlayed()
        {
            int expectedNumberOfGamesPlayed = playerGameResultsForFirstPlayer.Count();

            List<PlayerWithNemesis> players = playerRetriever.GetAllPlayersWithNemesisInfo(gamingGroupId);

            Assert.That(players[0].NumberOfPlayedGames, Is.EqualTo(expectedNumberOfGamesPlayed));
        }
    }
}
