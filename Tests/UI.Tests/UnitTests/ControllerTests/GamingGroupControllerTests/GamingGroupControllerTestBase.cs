using BusinessLogic.DataAccess;
using BusinessLogic.DataAccess.Repositories;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UI.Controllers;
using UI.Transformations;

namespace UI.Tests.UnitTests.ControllerTests.GamingGroupControllerTests
{
    [TestFixture]
    public class GamingGroupControllerTestBase
    {
        protected NemeStatsDbContext dbContextMock;
        protected GamingGroupRepository gamingGroupRepositoryMock;
        protected GamingGroupToGamingGroupViewModelTransformation gamingGroupToGamingGroupViewModelTransformationMock;
        protected GamingGroupController gamingGroupController;
        protected UserContext userContext;

        [SetUp]
        public void SetUp()
        {
            dbContextMock = MockRepository.GenerateMock<NemeStatsDbContext>();
            gamingGroupRepositoryMock = MockRepository.GenerateMock<GamingGroupRepository>();
            gamingGroupToGamingGroupViewModelTransformationMock = MockRepository.GenerateMock<GamingGroupToGamingGroupViewModelTransformation>();
            gamingGroupController = new GamingGroupController(
                dbContextMock, 
                gamingGroupToGamingGroupViewModelTransformationMock, 
                gamingGroupRepositoryMock);
            userContext = new UserContext()
            {
                ApplicationUserId = "user  id",
                GamingGroupId = 1315
            };
        }
    }
}
