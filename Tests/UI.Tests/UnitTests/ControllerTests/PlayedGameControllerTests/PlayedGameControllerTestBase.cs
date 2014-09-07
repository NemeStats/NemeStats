using BusinessLogic.DataAccess;
using BusinessLogic.DataAccess.Repositories;
using BusinessLogic.Logic;
using BusinessLogic.Logic.GameDefinitions;
using BusinessLogic.Logic.PlayedGames;
using BusinessLogic.Logic.Players;
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
        protected PlayerRetriever playerRetrieverMock;
        protected PlayedGameDetailsViewModelBuilder playedGameDetailsBuilderMock;
        protected GameDefinitionRetriever gameDefinitionRetrieverMock;
        protected PlayedGameCreator playedGameCreatorMock;
        protected ShowingXResultsMessageBuilder showingXResultsMessageBuilderMock;
        protected string testUserName = "the test user name";
        protected ApplicationUser currentUser;
        protected List<GameDefinition> gameDefinitions;

        [SetUp]
        public virtual void TestSetUp()
        {
            dataContext = MockRepository.GenerateMock<NemeStatsDataContext>();
            playedGameLogicMock = MockRepository.GenerateMock<PlayedGameRepository>();
            playerRetrieverMock = MockRepository.GenerateMock<PlayerRetriever>();
            playedGameDetailsBuilderMock = MockRepository.GenerateMock<PlayedGameDetailsViewModelBuilder>();
            gameDefinitionRetrieverMock = MockRepository.GenerateMock<GameDefinitionRetriever>();
            playedGameCreatorMock = MockRepository.GenerateMock<PlayedGameCreator>();
            showingXResultsMessageBuilderMock = MockRepository.GenerateMock<ShowingXResultsMessageBuilder>();
            playedGameController = new Controllers.PlayedGameController(
                dataContext,
                playedGameLogicMock,
                playerRetrieverMock, 
                playedGameDetailsBuilderMock,
                gameDefinitionRetrieverMock,
                showingXResultsMessageBuilderMock,
                playedGameCreatorMock);

            playedGameControllerPartialMock = MockRepository.GeneratePartialMock<PlayedGameController>(
                dataContext,
                playedGameLogicMock,
                playerRetrieverMock,
                playedGameDetailsBuilderMock,
                gameDefinitionRetrieverMock,
                showingXResultsMessageBuilderMock,
                playedGameCreatorMock);

            currentUser = new ApplicationUser()
            {
                CurrentGamingGroupId = 1
            };
            gameDefinitions = new List<GameDefinition>();
            gameDefinitionRetrieverMock.Expect(mock => mock.GetAllGameDefinitions(currentUser.CurrentGamingGroupId.Value))
                .Repeat.Once()
                .Return(gameDefinitions);
        }
    }
}
