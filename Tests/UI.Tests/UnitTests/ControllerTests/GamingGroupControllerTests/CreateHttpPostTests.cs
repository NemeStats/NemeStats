using BusinessLogic.Models;
using BusinessLogic.Models.GamingGroups;
using BusinessLogic.Models.User;
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
    public class CreateHttpPostTests : GamingGroupControllerTestBase
    {
        private GamingGroupQuickStart gamingGroupQuickStart;

        [SetUp]
        public void TestSetUp()
        {
            gamingGroupQuickStart = new GamingGroupQuickStart();

            gamingGroupCreatorMock.Expect(mock => mock.CreateGamingGroupAsync(
                Arg<GamingGroupQuickStart>.Is.Anything,
                Arg<ApplicationUser>.Is.Anything))
                    .Return(Task.FromResult(new GamingGroup()));
        }

        [Test]
        public async Task ItRedirectsToTheGamingGroupIndexView()
        {
            RedirectToRouteResult result = await gamingGroupController.Create(gamingGroupQuickStart, currentUser) as RedirectToRouteResult;

            Assert.AreEqual(MVC.GamingGroup.ActionNames.Index, result.RouteValues["action"]);
        }

        [Test]
        public async Task ItCreatesANewGamingGroupWithAllTheFixins()
        {
            GamingGroupQuickStart gamingGroupQuickStart = new GamingGroupQuickStart();

            await gamingGroupController.Create(gamingGroupQuickStart, currentUser);

            gamingGroupCreatorMock.AssertWasCalled(mock => mock.CreateGamingGroupAsync(gamingGroupQuickStart, currentUser));
        }
    }
}
