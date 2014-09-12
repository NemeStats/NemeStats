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
using UI.Controllers.Helpers;
using BusinessLogic.Logic.Players;
using System.Web.Mvc;

namespace UI.Tests.UnitTests.ControllerTests.PlayerControllerTests
{
    [TestFixture]
    public class PlayerControllerTestBase
    {
        protected IDataContext dataContextMock;
        protected IPlayerRepository playerRepositoryMock;
        protected IPlayerRetriever playerRetriever;
        protected GameResultViewModelBuilder playerGameResultDetailsBuilderMock;
        protected PlayerDetailsViewModelBuilder playerDetailsViewModelBuilderMock;
        protected ShowingXResultsMessageBuilder showingXResultsMessageBuilderMock;
        protected IPlayerSaver playerCreatorMock;
        protected UrlHelper urlHelperMock;
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
            dataContextMock = MockRepository.GenerateMock<IDataContext>();
            playerRepositoryMock = MockRepository.GenerateMock<IPlayerRepository>();
            playerRetriever = MockRepository.GenerateMock<IPlayerRetriever>();
            playerGameResultDetailsBuilderMock = MockRepository.GenerateMock<GameResultViewModelBuilder>();
            playerDetailsViewModelBuilderMock = MockRepository.GenerateMock<PlayerDetailsViewModelBuilder>();
            showingXResultsMessageBuilderMock = MockRepository.GenerateMock<ShowingXResultsMessageBuilder>();
            playerCreatorMock = MockRepository.GenerateMock<IPlayerSaver>();
            urlHelperMock = MockRepository.GenerateMock<UrlHelper>();
            playerController = new PlayerController(
                                dataContextMock,
                                playerRepositoryMock,
                                playerGameResultDetailsBuilderMock,
                                playerDetailsViewModelBuilderMock,
                                showingXResultsMessageBuilderMock,
                                playerCreatorMock,
                                playerRetriever);
            playerController.Url = urlHelperMock;
        }
    }
}
