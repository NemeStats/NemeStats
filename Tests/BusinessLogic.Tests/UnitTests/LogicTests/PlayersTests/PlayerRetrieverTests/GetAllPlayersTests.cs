using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.Logic.Players;
using BusinessLogic.DataAccess;
using Rhino.Mocks;
using BusinessLogic.Models;
using BusinessLogic.Models.User;

namespace BusinessLogic.Tests.UnitTests.LogicTests.PlayersTests.PlayerRetrieverTests
{
    [TestFixture]
    public class GetAllPlayersTests : PlayerRetrieverTestBase
    {
        [Test]
        public void ItOnlyReturnsPlayersForTheGivenGamingGroup()
        {
            List<Player> players = playerRetriever.GetAllPlayers(gamingGroupId);

            Assert.True(players.All(player => player.GamingGroupId == gamingGroupId));
        }

        [Test]
        public void ItReturnsPlayersOrderedByTheirNameAscending()
        {
            List<Player> players = playerRetriever.GetAllPlayers(gamingGroupId);
            string lastPlayerName = "0";
            foreach(Player player in players)
            {
                Assert.LessOrEqual(lastPlayerName, player.Name);
                lastPlayerName = player.Name;
            }
        }
    }
}
