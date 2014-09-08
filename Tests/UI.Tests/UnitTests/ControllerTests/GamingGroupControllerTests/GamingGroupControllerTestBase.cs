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
        protected DataContext dataContext;
        protected GamingGroupViewModelBuilder gamingGroupViewModelBuilderMock;
        protected GamingGroupController gamingGroupController;
        protected GamingGroupAccessGranter gamingGroupAccessGranterMock;
        protected GamingGroupCreator gamingGroupCreatorMock;
        protected GamingGroupRetriever gamingGroupRetrieverMock;
        protected ShowingXResultsMessageBuilder showingXResultsMessageBuilderMock;
        protected ApplicationUser currentUser;

        [SetUp]
        public virtual void SetUp()
        {
            dataContext = MockRepository.GenerateMock<DataContext>();
            gamingGroupViewModelBuilderMock = MockRepository.GenerateMock<GamingGroupViewModelBuilder>();
            gamingGroupAccessGranterMock = MockRepository.GenerateMock<GamingGroupAccessGranter>();
            gamingGroupCreatorMock = MockRepository.GenerateMock<GamingGroupCreator>();
            gamingGroupRetrieverMock = MockRepository.GenerateMock<GamingGroupRetriever>();
            showingXResultsMessageBuilderMock = MockRepository.GenerateMock<ShowingXResultsMessageBuilder>();
            gamingGroupController = new GamingGroupController(
                dataContext, 
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
