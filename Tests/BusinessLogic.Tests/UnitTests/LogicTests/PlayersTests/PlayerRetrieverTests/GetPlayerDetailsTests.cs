using BusinessLogic.DataAccess;
using BusinessLogic.DataAccess.Repositories;
using BusinessLogic.Logic;
using BusinessLogic.Logic.Players;
using BusinessLogic.Models;
using BusinessLogic.Models.Nemeses;
using BusinessLogic.Models.Players;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.Logic.Nemeses;

namespace BusinessLogic.Tests.UnitTests.LogicTests.PlayersTests.PlayerRetrieverTests
{
    [TestFixture]
    public class GetPlayerDetailsTests
    {
        private IDataContext dataContextMock;
        private INemesisHistoryRetriever nemesisHistoryRetrieverMock;
        private PlayerRetriever playerRetrieverPartialMock;
        private Player player;
        private int numberOfRecentGames = 1;
        private Nemesis expectedNemesis;
        private Nemesis expectedPriorNemesis;
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
            nemesisHistoryRetrieverMock = MockRepository.GenerateMock<INemesisHistoryRetriever>();
            playerRetrieverPartialMock = MockRepository.GeneratePartialMock<PlayerRetriever>(dataContextMock, nemesisHistoryRetrieverMock);
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

            expectedNemesis = new Nemesis()
            {
                NemesisPlayerId = 8888
            };
            expectedNemesis = new Nemesis()
            {
                NemesisPlayerId = 9999
            };

            NemesisHistoryData nemesisHistoryData = new NemesisHistoryData
            {
                CurrentNemesis = expectedNemesis,
                PreviousNemeses = new List<Nemesis>() { expectedPriorNemesis }
            };

            nemesisHistoryRetrieverMock.Expect(mock => mock.GetNemesisHistory(player.Id, PlayerRetriever.NUMBER_OF_PREVIOUS_NEMESES_TO_RETURN))
                                       .Return(nemesisHistoryData);
        }

        //TODO need tests for the transformation... which should probably be refactored into a different class

        [Test]
        public void ItSetsTheCurrentNemesis()
        {
            PlayerDetails playerDetails = playerRetrieverPartialMock.GetPlayerDetails(player.Id, numberOfRecentGames);

            Assert.AreSame(expectedNemesis, playerDetails.CurrentNemesis);
        }

        [Test]
        public void ItSetsThePreviousNemesis()
        {
            PlayerDetails playerDetails = playerRetrieverPartialMock.GetPlayerDetails(player.Id, numberOfRecentGames);

            Assert.AreSame(expectedPriorNemesis, playerDetails.PreviousNemesis);
        }

        [Test]
        public void ItSetsThePlayersMinions()
        {
            PlayerDetails playerDetails = playerRetrieverPartialMock.GetPlayerDetails(player.Id, numberOfRecentGames);

            Assert.AreSame(minions, playerDetails.Minions);
        }
    }
}
