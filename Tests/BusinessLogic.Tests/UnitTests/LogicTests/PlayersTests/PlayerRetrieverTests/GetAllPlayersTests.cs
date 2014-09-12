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
    public class GetAllPlayersTests
    {
        private PlayerRetriever playerRetriever;
        private IDataContext dataContextMock;
        private IQueryable<Player> playerQueryable;
        private int gamingGroupId = 558585;

        [SetUp]
        public void SetUp()
        {
            dataContextMock = MockRepository.GenerateMock<IDataContext>();
            playerRetriever = new PlayerRetriever(dataContextMock);

            List<Player> players = new List<Player>()
            {
                new Player(){ GamingGroupId = gamingGroupId, Name = "2" },
                new Player(){ GamingGroupId = gamingGroupId, Name = "3" },
                new Player(){ GamingGroupId = -1, Name = "1" },
                new Player(){ GamingGroupId = gamingGroupId, Name = "1" },
            };
            playerQueryable = players.AsQueryable<Player>();

            dataContextMock.Expect(mock => mock.GetQueryable<Player>())
                .Return(playerQueryable);
        }

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
