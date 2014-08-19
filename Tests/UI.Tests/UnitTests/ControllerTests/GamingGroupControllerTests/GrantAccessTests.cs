using System.Web.Routing;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using UI.Models.GamingGroup;
using ModelBindingContext = System.Web.ModelBinding.ModelBindingContext;

namespace UI.Tests.UnitTests.ControllerTests.GamingGroupControllerTests
{
    [TestFixture]
    public class GrantAccessTests : GamingGroupControllerTestBase
    {
        [Test]
        public void ItRedirectsToTheIndexAction()
        {
            RedirectToRouteResult redirectResult = gamingGroupController.GrantAccess(new GamingGroupViewModel(), currentUser) as RedirectToRouteResult;

            Assert.AreEqual(MVC.GamingGroup.ActionNames.Index, redirectResult.RouteValues["action"]);
        }

        [Test]
        public void ItDoesCreateInvitationIfTheEmailIsEmpty()
        {
            var model = new GamingGroupViewModel 
            {
                InviteeEmail = string.Empty
            };

            gamingGroupController.ViewData.ModelState.AddModelError("EmptyEmail", new Exception());
            gamingGroupController.GrantAccess(model, currentUser);

            Assert.IsFalse(gamingGroupController.ModelState.IsValid); 
            gamingGroupAccessGranterMock.AssertWasNotCalled(mock => mock.CreateInvitation(model.InviteeEmail, currentUser));
        }

        [Test]
        public void ItGrantsAccessToTheSpecifiedEmailAddress()
        {
            var model = new GamingGroupViewModel 
            {
                InviteeEmail = "abc@xyz.com"
            };

            gamingGroupAccessGranterMock.Expect(mock => mock.CreateInvitation(model.InviteeEmail, currentUser))
                .Repeat.Once();

            gamingGroupController.GrantAccess(model, currentUser);

            gamingGroupAccessGranterMock.VerifyAllExpectations();
        }
    }
}
