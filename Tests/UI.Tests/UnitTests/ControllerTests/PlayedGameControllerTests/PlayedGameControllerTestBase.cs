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
using UI.Controllers.Helpers;
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
        protected ShowingXResultsMessageBuilder showingXResultsMessageBuilderMock;
        protected string testUserName = "the test user name";
        protected ApplicationUser currentUser;
        protected List<GameDefinition> gameDefinitions;

        [SetUp]
        public virtual void TestSetUp()
        {
            dataContext = MockRepository.GenerateMock<NemeStatsDataContext>();
            playedGameLogicMock = MockRepository.GenerateMock<PlayedGameRepository>();
            playerLogicMock = MockRepository.GenerateMock<PlayerRepository>();
            playedGameDetailsBuilderMock = MockRepository.GenerateMock<PlayedGameDetailsViewModelBuilder>();
            gameDefinitionRetrieverMock = MockRepository.GenerateMock<GameDefinitionRetriever>();
            showingXResultsMessageBuilderMock = MockRepository.GenerateMock<ShowingXResultsMessageBuilder>();
            playedGameController = new Controllers.PlayedGameController(
                dataContext,
                playedGameLogicMock, 
                playerLogicMock, 
                playedGameDetailsBuilderMock,
                gameDefinitionRetrieverMock,
                showingXResultsMessageBuilderMock);

            playedGameControllerPartialMock = MockRepository.GeneratePartialMock<PlayedGameController>(
                dataContext,
                playedGameLogicMock,
                playerLogicMock,
                playedGameDetailsBuilderMock,
                gameDefinitionRetrieverMock,
                showingXResultsMessageBuilderMock);

            gameDefinitions = new List<GameDefinition>();
            gameDefinitionRetrieverMock.Expect(mock => mock.GetAllGameDefinitions(currentUser))
                .Repeat.Once()
                .Return(gameDefinitions);
        }
    }
}
