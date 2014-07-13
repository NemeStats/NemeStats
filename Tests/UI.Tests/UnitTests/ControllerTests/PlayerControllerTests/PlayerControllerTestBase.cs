using BusinessLogic.DataAccess;
using BusinessLogic.DataAccess.Repositories;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UI.Controllers;
using UI.Transformations;
using UI.Transformations.Player;

namespace UI.Tests.UnitTests.ControllerTests.PlayerControllerTests
{
    [TestFixture]
    public class PlayerControllerTestBase
    {
        protected NemeStatsDbContext dbContextMock;
        protected PlayerRepository playerRepositoryMock;
        protected GameResultViewModelBuilder playerGameResultDetailsBuilderMock;
        protected PlayerDetailsViewModelBuilder playerDetailsViewModelBuilderMock;
        protected PlayerController playerController;
        protected UserContext userContext;

        [SetUp]
        public void SetUp()
        {
            userContext = new UserContext()
            {
                GamingGroupId = 123,
                ApplicationUserId = "app user id"
            };
            dbContextMock = MockRepository.GenerateMock<NemeStatsDbContext>();
            playerRepositoryMock = MockRepository.GenerateMock<PlayerRepository>();
            playerGameResultDetailsBuilderMock = MockRepository.GenerateMock<GameResultViewModelBuilder>();
            playerDetailsViewModelBuilderMock = MockRepository.GenerateMock<PlayerDetailsViewModelBuilder>();
            playerController = new PlayerController(
                                dbContextMock,
                                playerRepositoryMock,
                                playerGameResultDetailsBuilderMock,
                                playerDetailsViewModelBuilderMock);
        }
    }
}
