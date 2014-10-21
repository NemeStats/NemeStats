using BusinessLogic.DataAccess;
using NUnit.Framework;
using Rhino.Mocks;
using System.Collections.Generic;
using System.Linq;
using BusinessLogic.Logic.PlayedGames;
using BusinessLogic.Models.User;
using BusinessLogic.Models;
using BusinessLogic.Logic.Nemeses;

namespace BusinessLogic.Tests.UnitTests.LogicTests.PlayedGamesTests.PlayedGameDeleterTests
{
    [TestFixture]
    public class DeletePlayedGameTests
    {
        private IDataContext dataContextMock;
        private INemesisRecalculator nemesisRecalculatorMock;
        private PlayedGameDeleter playedGameDeleter;
        private ApplicationUser currentUser;
        private List<PlayerGameResult> playerGameResults;
        private int playedGameId = 1;
        private int playerInGame1Id = 1;
        private int playerInGame2Id = 2;
        private int playerNotInGame = 9999;

        [SetUp]
        public void SetUp()
        {
            dataContextMock = MockRepository.GenerateMock<IDataContext>();
            nemesisRecalculatorMock = MockRepository.GenerateMock<INemesisRecalculator>();
            playedGameDeleter = new PlayedGameDeleter(dataContextMock, nemesisRecalculatorMock);

            currentUser = new ApplicationUser();

            playerGameResults = new List<PlayerGameResult>()
            {
                new PlayerGameResult(){ PlayerId = playerInGame1Id, PlayedGameId = playedGameId },
                new PlayerGameResult(){ PlayerId = playerInGame2Id, PlayedGameId = playedGameId },
                new PlayerGameResult(){ PlayerId = 3, PlayedGameId = playedGameId + 9999 }
            };

            dataContextMock.Expect(mock => mock.GetQueryable<PlayerGameResult>())
                .Return(playerGameResults.AsQueryable());
        }

        [Test]
        public void ItDeletesTheSpecifiedPlayedGame()
        {
            playedGameDeleter.DeletePlayedGame(playedGameId, currentUser);

            dataContextMock.AssertWasCalled(mock => mock.DeleteById<PlayedGame>(playedGameId, currentUser));
        }

        [Test]
        public void ItRecalculatesTheNemesesOfAllPlayersInTheDeletedGame()
        {
            int playedGameIdId = 1;
            playedGameDeleter.DeletePlayedGame(playedGameIdId, currentUser);

            nemesisRecalculatorMock.AssertWasCalled(mock => mock.RecalculateNemesis(playerInGame1Id, currentUser));
            nemesisRecalculatorMock.AssertWasCalled(mock => mock.RecalculateNemesis(playerInGame2Id, currentUser));
            nemesisRecalculatorMock.AssertWasNotCalled(mock => mock.RecalculateNemesis(playerNotInGame, currentUser));
        }
    }
}
