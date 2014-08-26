using BusinessLogic.DataAccess;
using BusinessLogic.DataAccess.Repositories;
using BusinessLogic.Logic;
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

namespace BusinessLogic.Tests.UnitTests.DataAccessTests.RepositoriesTests.EntityFrameworkPlayerRepositoryTests
{
    [TestFixture]
    public class GetPlayerDetailsTests
    {
        private DataContext dataContextMock;
        private EntityFrameworkPlayerRepository playerRepositoryPartialMock;
        private Player player;
        private int numberOfRecentGames = 1;
        private Nemesis nemesis;
        private ApplicationUser currentUser;

        [SetUp]
        public void SetUp()
        {
            currentUser = new ApplicationUser()
            {
                Id = "123",
                CurrentGamingGroupId = 15151
            };
            dataContextMock = MockRepository.GenerateMock<DataContext>();
            playerRepositoryPartialMock = MockRepository.GeneratePartialMock<EntityFrameworkPlayerRepository>(dataContextMock);
            player = new Player()
            {
                Id = 1351,
                Name = "the player",
                PlayerGameResults = new List<PlayerGameResult>(),
                Active = true
            };

            dataContextMock.Expect(mock => mock.FindById<Player>(player.Id))
                .Return(player);

            PlayerStatistics playerStatistics = new PlayerStatistics();
            playerRepositoryPartialMock.Expect(repo => repo.GetPlayerStatistics(player.Id))
                .Repeat.Once()
                .Return(playerStatistics);
            
            nemesis = new Nemesis()
            {
                NemesisPlayerId = 151541
            };
            playerRepositoryPartialMock.Expect(mock => mock.GetNemesis(player.Id))
                .Repeat.Once()
                .Return(nemesis);

            playerRepositoryPartialMock.Expect(mock => mock.GetPlayerGameResultsWithPlayedGameAndGameDefinition(player.Id, numberOfRecentGames))
                            .Repeat.Once()
                            .Return(player.PlayerGameResults.ToList());
        }

        //TODO need tests for the transformation... which should probably be refactored into a different class

        [Test]
        public void ItGetsThePlayersNemesis()
        {
            PlayerDetails playerDetails = playerRepositoryPartialMock.GetPlayerDetails(player.Id, numberOfRecentGames);

            Assert.AreEqual(nemesis, playerDetails.Nemesis);
        }
    }
}
