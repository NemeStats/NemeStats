#region LICENSE
// NemeStats is a free website for tracking the results of board games.
//     Copyright (C) 2015 Jacob Gordon
// 
//     This program is free software: you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     This program is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU General Public License for more details.
// 
//     You should have received a copy of the GNU General Public License
//     along with this program.  If not, see <http://www.gnu.org/licenses/>
#endregion
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Linq;
using System.Web.Mvc;
using UI.Models.GamingGroup;

namespace UI.Tests.UnitTests.ControllerTests.GamingGroupControllerTests
{
    [TestFixture]
    public class GrantAccessTests : GamingGroupControllerTestBase
    {
        [Test]
        public void ItRedirectsToTheIndexAction()
        {
            RedirectToRouteResult redirectResult = gamingGroupControllerPartialMock.GrantAccess(new GamingGroupViewModel(), currentUser) as RedirectToRouteResult;

            Assert.AreEqual(MVC.GamingGroup.ActionNames.Index, redirectResult.RouteValues["action"]);
        }

        [Test]
        public void ItDoesCreateInvitationIfTheEmailIsEmpty()
        {
            var model = new GamingGroupViewModel 
            {
                InviteeEmail = string.Empty
            };

            gamingGroupControllerPartialMock.ViewData.ModelState.AddModelError("EmptyEmail", new Exception());
            gamingGroupControllerPartialMock.GrantAccess(model, currentUser);

            Assert.IsFalse(gamingGroupControllerPartialMock.ModelState.IsValid); 
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

            gamingGroupControllerPartialMock.GrantAccess(model, currentUser);

            gamingGroupAccessGranterMock.VerifyAllExpectations();
        }
    }
}
