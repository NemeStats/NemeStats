using BusinessLogic.Models;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using UI.Models.GamingGroup;

namespace UI.Tests.UnitTests.ControllerTests.GamingGroupControllerTests
{
    [TestFixture]
    public class IndexTests : GamingGroupControllerTestBase
    {
        [Test]
        public void ItReturnsTheIndexView()
        {
            ViewResult viewResult = gamingGroupController.Index(userContext) as ViewResult;

            Assert.AreEqual(MVC.GamingGroup.Views.Index, viewResult.ViewName);
        }

        [Test]
        public void ItAddsAGamingGroupViewModelToTheView()
        {
            GamingGroupViewModel gamingGroupViewModel = new GamingGroupViewModel();
            GamingGroup gamingGroup = new GamingGroup();

            gamingGroupRepositoryMock.Expect(mock => mock.GetGamingGroupDetails(userContext.GamingGroupId.Value, userContext))
                .Repeat.Once()
                .Return(gamingGroup);

            gamingGroupToGamingGroupViewModelTransformationMock.Expect(mock => mock.Build(gamingGroup))
                .Repeat.Once()
                .Return(gamingGroupViewModel);
            
            ViewResult viewResult = gamingGroupController.Index(userContext) as ViewResult;

            Assert.AreSame(gamingGroupViewModel, viewResult.Model);
        }
    }
}
