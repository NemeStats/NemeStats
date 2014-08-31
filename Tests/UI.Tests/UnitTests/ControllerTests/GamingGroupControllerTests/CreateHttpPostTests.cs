using BusinessLogic.Models;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace UI.Tests.UnitTests.ControllerTests.GamingGroupControllerTests
{
    [TestFixture]
    public class CreateHttpPostTests : GamingGroupControllerTestBase
    {
        private string gamingGroupName = "name";

        [SetUp]
        public void TestSetUp()
        {
            gamingGroupCreatorMock.Expect(mock => mock.CreateGamingGroupAsync(
                Arg<string>.Is.Anything,
                Arg<ApplicationUser>.Is.Anything))
                    .Return(Task.FromResult(new GamingGroup()));
        }

        [Test]
        public async Task ItRedirectsToTheGamingGroupIndexView()
        {
            RedirectToRouteResult result = await gamingGroupController.Create(gamingGroupName, currentUser) as RedirectToRouteResult;

            Assert.AreEqual(MVC.GamingGroup.ActionNames.Index, result.RouteValues["action"]);
        }

        [Test]
        public async Task ItCreatesANewGamingGroup()
        {
            string gamingGroupName = "name";

            await gamingGroupController.Create(gamingGroupName, currentUser);

            gamingGroupCreatorMock.AssertWasCalled(mock => mock.CreateGamingGroupAsync(gamingGroupName, currentUser));
        }
    }
}
