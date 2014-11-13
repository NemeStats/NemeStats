using BusinessLogic.DataAccess;
using BusinessLogic.DataAccess.GamingGroups;
using BusinessLogic.Logic.GamingGroups;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using UI.Controllers;
using UI.Controllers.Helpers;
using UI.Transformations;
using UI.Transformations.PlayerTransformations;

namespace UI.Tests.UnitTests.ControllerTests.GamingGroupControllerTests
{
    [TestFixture]
    public class GamingGroupControllerTestBase
    {
        protected IDataContext dataContext;
        protected IGamingGroupViewModelBuilder gamingGroupViewModelBuilderMock;
        protected GamingGroupController gamingGroupController;
        protected IGamingGroupAccessGranter gamingGroupAccessGranterMock;
        protected IGamingGroupSaver gamingGroupSaverMock;
        protected IGamingGroupRetriever gamingGroupRetrieverMock;
        protected IShowingXResultsMessageBuilder showingXResultsMessageBuilderMock;
        protected IPlayerWithNemesisViewModelBuilder playerWithNemesisViewModelBuilderMock;
        protected IPlayedGameDetailsViewModelBuilder playedGameDetailsViewModelBuilderMock;
        protected IGameDefinitionSummaryViewModelBuilder gameDefinitionSummaryViewModelBuilderMock;
        protected ApplicationUser currentUser;

        [SetUp]
        public virtual void SetUp()
        {
            dataContext = MockRepository.GenerateMock<IDataContext>();
            gamingGroupViewModelBuilderMock = MockRepository.GenerateMock<IGamingGroupViewModelBuilder>();
            gamingGroupAccessGranterMock = MockRepository.GenerateMock<IGamingGroupAccessGranter>();
            gamingGroupSaverMock = MockRepository.GenerateMock<IGamingGroupSaver>();
            gamingGroupRetrieverMock = MockRepository.GenerateMock<IGamingGroupRetriever>();
            showingXResultsMessageBuilderMock = MockRepository.GenerateMock<IShowingXResultsMessageBuilder>();
            playerWithNemesisViewModelBuilderMock = MockRepository.GenerateMock<IPlayerWithNemesisViewModelBuilder>();
            playedGameDetailsViewModelBuilderMock = MockRepository.GenerateMock<IPlayedGameDetailsViewModelBuilder>();
            gameDefinitionSummaryViewModelBuilderMock = MockRepository.GenerateMock<IGameDefinitionSummaryViewModelBuilder>();
            gamingGroupController = new GamingGroupController(
                gamingGroupViewModelBuilderMock, 
                gamingGroupAccessGranterMock,
                gamingGroupSaverMock,
                gamingGroupRetrieverMock,
                showingXResultsMessageBuilderMock,
                playerWithNemesisViewModelBuilderMock,
                playedGameDetailsViewModelBuilderMock,
                gameDefinitionSummaryViewModelBuilderMock);
            currentUser = new ApplicationUser()
            {
                Id = "user  id",
                CurrentGamingGroupId = 1315
            };
        }
    }
}
