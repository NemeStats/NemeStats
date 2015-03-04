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
