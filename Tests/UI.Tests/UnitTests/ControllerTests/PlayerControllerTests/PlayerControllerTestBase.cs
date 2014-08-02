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
        protected DataContext dataContextMock;
        protected PlayerRepository playerRepositoryMock;
        protected GameResultViewModelBuilder playerGameResultDetailsBuilderMock;
        protected PlayerDetailsViewModelBuilder playerDetailsViewModelBuilderMock;
        protected PlayerController playerController;
        protected ApplicationUser currentUser;

        [SetUp]
        public void SetUp()
        {
            currentUser = new ApplicationUser()
            {
                CurrentGamingGroupId = 123,
                Id = "app user id"
            };
            dataContextMock = MockRepository.GenerateMock<DataContext>();
            playerRepositoryMock = MockRepository.GenerateMock<PlayerRepository>();
            playerGameResultDetailsBuilderMock = MockRepository.GenerateMock<GameResultViewModelBuilder>();
            playerDetailsViewModelBuilderMock = MockRepository.GenerateMock<PlayerDetailsViewModelBuilder>();
            playerController = new PlayerController(
                                dataContextMock,
                                playerRepositoryMock,
                                playerGameResultDetailsBuilderMock,
                                playerDetailsViewModelBuilderMock);
        }
    }
}
