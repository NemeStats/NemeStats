using BusinessLogic.DataAccess;
using BusinessLogic.DataAccess.GamingGroups;
using BusinessLogic.DataAccess.Repositories;
using BusinessLogic.Logic.GamingGroups;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using UI.Controllers;
using UI.Controllers.Helpers;
using UI.Transformations;

namespace UI.Tests.UnitTests.ControllerTests.GamingGroupControllerTests
{
    [TestFixture]
    public class GamingGroupControllerTestBase
    {
        protected IDataContext dataContext;
        protected IGamingGroupViewModelBuilder gamingGroupViewModelBuilderMock;
        protected GamingGroupController gamingGroupController;
        protected IGamingGroupAccessGranter gamingGroupAccessGranterMock;
        protected IGamingGroupCreator gamingGroupCreatorMock;
        protected IGamingGroupRetriever gamingGroupRetrieverMock;
        protected IShowingXResultsMessageBuilder showingXResultsMessageBuilderMock;
        protected ApplicationUser currentUser;

        [SetUp]
        public virtual void SetUp()
        {
            dataContext = MockRepository.GenerateMock<IDataContext>();
            gamingGroupViewModelBuilderMock = MockRepository.GenerateMock<IGamingGroupViewModelBuilder>();
            gamingGroupAccessGranterMock = MockRepository.GenerateMock<IGamingGroupAccessGranter>();
            gamingGroupCreatorMock = MockRepository.GenerateMock<IGamingGroupCreator>();
            gamingGroupRetrieverMock = MockRepository.GenerateMock<IGamingGroupRetriever>();
            showingXResultsMessageBuilderMock = MockRepository.GenerateMock<IShowingXResultsMessageBuilder>();
            gamingGroupController = new GamingGroupController(
                gamingGroupViewModelBuilderMock, 
                gamingGroupAccessGranterMock,
                gamingGroupCreatorMock,
                gamingGroupRetrieverMock,
                showingXResultsMessageBuilderMock);
            currentUser = new ApplicationUser()
            {
                Id = "user  id",
                CurrentGamingGroupId = 1315
            };
        }
    }
}
