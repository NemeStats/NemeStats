using BusinessLogic.DataAccess;
using BusinessLogic.DataAccess.Repositories;
using BusinessLogic.Logic;
using BusinessLogic.Logic.GameDefinitions;
using BusinessLogic.Models;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using System.Collections.Generic;
using UI.Controllers;
using UI.Transformations;

namespace UI.Tests.UnitTests.ControllerTests.PlayedGameControllerTests
{
    public class PlayedGameControllerTestBase
    {
        protected NemeStatsDataContext dataContext;
        protected PlayedGameController playedGameController;
        protected PlayedGameController playedGameControllerPartialMock;
        protected PlayedGameRepository playedGameLogicMock;
        protected PlayerRepository playerLogicMock;
        protected PlayedGameDetailsViewModelBuilder playedGameDetailsBuilderMock;
        protected GameDefinitionRetriever gameDefinitionRetrieverMock;
        protected string testUserName = "the test user name";
        protected ApplicationUser currentUser;

        [SetUp]
        public virtual void TestSetUp()
        {
            dataContext = MockRepository.GenerateMock<NemeStatsDataContext>();
            playedGameLogicMock = MockRepository.GenerateMock<PlayedGameRepository>();
            playerLogicMock = MockRepository.GenerateMock<PlayerRepository>();
            playedGameDetailsBuilderMock = MockRepository.GenerateMock<PlayedGameDetailsViewModelBuilder>();
            gameDefinitionRetrieverMock = MockRepository.GenerateMock<GameDefinitionRetriever>();
            playedGameController = new Controllers.PlayedGameController(
                dataContext,
                playedGameLogicMock, 
                playerLogicMock, 
                playedGameDetailsBuilderMock,
                gameDefinitionRetrieverMock);

            playedGameControllerPartialMock = MockRepository.GeneratePartialMock<PlayedGameController>(
                dataContext,
                playedGameLogicMock,
                playerLogicMock,
                playedGameDetailsBuilderMock,
                gameDefinitionRetrieverMock);

            gameDefinitionRetrieverMock.Expect(mock => mock.GetAllGameDefinitions(currentUser))
                .Repeat.Once()
                .Return(new List<GameDefinition>());
        }
    }
}
