using BusinessLogic.DataAccess;
using BusinessLogic.DataAccess.Repositories;
using BusinessLogic.Logic;
using BusinessLogic.Logic.Players;
using BusinessLogic.Models;
using BusinessLogic.Models.Players;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Tests.UnitTests.LogicTests.PlayersTests.PlayerRetrieverTests
{
    [TestFixture]
    public class GetPlayerDetailsTests
    {
        private IDataContext dataContextMock;
        private PlayerRetriever playerRetrieverPartialMock;
        private Player player;
        private int numberOfRecentGames = 1;
        private Nemesis nemesis;
        private List<Player> minions;
        private ApplicationUser currentUser;

        [SetUp]
        public void SetUp()
        {
            currentUser = new ApplicationUser()
            {
                Id = "123",
                CurrentGamingGroupId = 15151
            };
            dataContextMock = MockRepository.GenerateMock<IDataContext>();
            playerRetrieverPartialMock = MockRepository.GeneratePartialMock<PlayerRetriever>(dataContextMock);
            player = new Player()
            {
                Id = 1351,
                Name = "the player",
                PlayerGameResults = new List<PlayerGameResult>(),
                Active = true,
                NemesisId = 566677
            };

            dataContextMock.Expect(mock => mock.FindById<Player>(player.Id))
                .Return(player);

            PlayerStatistics playerStatistics = new PlayerStatistics();
            playerRetrieverPartialMock.Expect(repo => repo.GetPlayerStatistics(player.Id))
                .Repeat.Once()
                .Return(playerStatistics);

            playerRetrieverPartialMock.Expect(mock => mock.GetPlayerGameResultsWithPlayedGameAndGameDefinition(player.Id, numberOfRecentGames))
                            .Repeat.Once()
                            .Return(player.PlayerGameResults.ToList());

            minions = new List<Player>();
            playerRetrieverPartialMock.Expect(mock => mock.GetMinions(player.Id))
                .Return(minions);

            nemesis = new Nemesis()
            {
                NemesisPlayerId = 151541
            };
            dataContextMock.Expect(mock => mock.FindById<Nemesis>(player.NemesisId))
                .Return(nemesis);
        }

        //TODO need tests for the transformation... which should probably be refactored into a different class

        [Test]
        public void ItSetsThePlayersNemesis()
        {
            PlayerDetails playerDetails = playerRetrieverPartialMock.GetPlayerDetails(player.Id, numberOfRecentGames);

            Assert.AreEqual(nemesis, playerDetails.PlayerNemesis);
        }

        [Test]
        public void ItSetsThePlayersMinions()
        {
            PlayerDetails playerDetails = playerRetrieverPartialMock.GetPlayerDetails(player.Id, numberOfRecentGames);

            Assert.AreSame(minions, playerDetails.Minions);
        }
    }
}
