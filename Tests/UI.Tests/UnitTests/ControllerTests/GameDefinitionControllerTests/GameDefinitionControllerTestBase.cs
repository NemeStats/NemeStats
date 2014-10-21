using BusinessLogic.DataAccess;
using BusinessLogic.Logic.GameDefinitions;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using System.Linq;
using System.Web.Mvc;
using UI.Controllers;
using UI.Controllers.Helpers;
using UI.Transformations;

namespace UI.Tests.UnitTests.ControllerTests.GameDefinitionControllerTests
{
    [TestFixture]
    public class GameDefinitionControllerTestBase
    {
        protected GameDefinitionController gameDefinitionControllerPartialMock;
        protected IGameDefinitionRetriever gameDefinitionRetrieverMock;
        protected IGameDefinitionViewModelBuilder gameDefinitionTransformationMock;
        protected IShowingXResultsMessageBuilder showingXResultsMessageBuilderMock;
        protected IGameDefinitionSaver gameDefinitionCreatorMock;
        protected NemeStatsDataContext dataContextMock;
        protected UrlHelper urlHelperMock;
        protected ApplicationUser currentUser;

        [SetUp]
        public virtual void SetUp()
        {
            dataContextMock = MockRepository.GenerateMock<NemeStatsDataContext>();
            gameDefinitionRetrieverMock = MockRepository.GenerateMock<IGameDefinitionRetriever>();
            gameDefinitionTransformationMock = MockRepository.GenerateMock<IGameDefinitionViewModelBuilder>();
            showingXResultsMessageBuilderMock = MockRepository.GenerateMock<IShowingXResultsMessageBuilder>();
            gameDefinitionCreatorMock = MockRepository.GenerateMock<IGameDefinitionSaver>();
            urlHelperMock = MockRepository.GenerateMock<UrlHelper>();
            gameDefinitionControllerPartialMock = MockRepository.GeneratePartialMock<GameDefinitionController>(
                dataContextMock, 
                gameDefinitionRetrieverMock,
                gameDefinitionTransformationMock,
                showingXResultsMessageBuilderMock,
                gameDefinitionCreatorMock);
            gameDefinitionControllerPartialMock.Url = urlHelperMock;
            currentUser = new ApplicationUser()
            {
                Id = "user id",
                CurrentGamingGroupId = 15151
            };
        }
    }
}
