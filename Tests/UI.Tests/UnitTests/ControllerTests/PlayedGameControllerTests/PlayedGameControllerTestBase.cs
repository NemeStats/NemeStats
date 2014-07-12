using BusinessLogic.DataAccess;
using BusinessLogic.DataAccess.Repositories;
using BusinessLogic.Logic;
using BusinessLogic.Models;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using UI.Controllers;
using UI.Transformations;

namespace UI.Tests.UnitTests.ControllerTests.PlayedGameControllerTests
{
    public class PlayedGameControllerTestBase
    {
        protected NemeStatsDbContext dbContexMock;
        protected PlayedGameController playedGameController;
        protected PlayedGameController playedGameControllerPartialMock;
        protected PlayedGameLogic playedGameLogicMock;
        protected PlayerLogic playerLogicMock;
        protected PlayedGameDetailsViewModelBuilder playedGameDetailsBuilderMock;
        protected string testUserName = "the test user name";
        protected UserContext userContext;

        [SetUp]
        public virtual void TestSetUp()
        {
            dbContexMock = MockRepository.GenerateMock<NemeStatsDbContext>();
            playedGameLogicMock = MockRepository.GenerateMock<PlayedGameLogic>();
            playerLogicMock = MockRepository.GenerateMock<PlayerLogic>();
            playedGameDetailsBuilderMock = MockRepository.GenerateMock<PlayedGameDetailsViewModelBuilder>();
            playedGameController = new Controllers.PlayedGameController(
                dbContexMock,
                playedGameLogicMock, 
                playerLogicMock, 
                playedGameDetailsBuilderMock);

            playedGameControllerPartialMock = MockRepository.GeneratePartialMock<PlayedGameController>(
                dbContexMock,
                playedGameLogicMock,
                playerLogicMock,
                playedGameDetailsBuilderMock);
        }
    }
}
