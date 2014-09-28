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
using UI.Transformations.PlayerTransformations;
using UI.Controllers.Helpers;
using BusinessLogic.Logic.Players;
using System.Web.Mvc;

namespace UI.Tests.UnitTests.ControllerTests.PlayerControllerTests
{
    [TestFixture]
    public class PlayerControllerTestBase
    {
        protected IDataContext dataContextMock;
        protected IPlayerRetriever playerRetrieverMock;
        protected IGameResultViewModelBuilder playerGameResultDetailsBuilderMock;
        protected IPlayerDetailsViewModelBuilder playerDetailsViewModelBuilderMock;
        protected IShowingXResultsMessageBuilder showingXResultsMessageBuilderMock;
        protected IPlayerSaver playerSaverMock;
        protected UrlHelper urlHelperMock;
        protected PlayerController playerController;
        protected ApplicationUser currentUser;

        [SetUp]
        public virtual void SetUp()
        {
            currentUser = new ApplicationUser()
            {
                CurrentGamingGroupId = 123,
                Id = "app user id"
            };
            dataContextMock = MockRepository.GenerateMock<IDataContext>();
            playerRetrieverMock = MockRepository.GenerateMock<IPlayerRetriever>();
            playerGameResultDetailsBuilderMock = MockRepository.GenerateMock<IGameResultViewModelBuilder>();
            playerDetailsViewModelBuilderMock = MockRepository.GenerateMock<IPlayerDetailsViewModelBuilder>();
            showingXResultsMessageBuilderMock = MockRepository.GenerateMock<IShowingXResultsMessageBuilder>();
            playerSaverMock = MockRepository.GenerateMock<IPlayerSaver>();
            urlHelperMock = MockRepository.GenerateMock<UrlHelper>();
            playerController = MockRepository.GeneratePartialMock<PlayerController>(
                                dataContextMock,
                                playerGameResultDetailsBuilderMock,
                                playerDetailsViewModelBuilderMock,
                                showingXResultsMessageBuilderMock,
                                playerSaverMock,
                                playerRetrieverMock);
            playerController.Url = urlHelperMock;
        }
    }
}
