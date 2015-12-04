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

using System.Web.Mvc;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;

namespace UI.Tests.UnitTests.ControllerTests.AccountControllerTests
{
    [TestFixture]
    public class AddExistingUserToGamingGroup : AccountControllerTestBase
    {
        private string gamingGroupInvitationId = "invitation id";

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
        } 

        [Test]
        public void ItConsumesTheInvitation()
        {
            gamingGroupInviteConsumerMock.Expect(mock => mock.AddExistingUserToGamingGroup(Arg<string>.Is.Anything))
                              .Return(new AddUserToGamingGroupResult());

            accountControllerPartialMock.ConsumeInvitation(gamingGroupInvitationId, currentUser);

            gamingGroupInviteConsumerMock.AssertWasCalled(mock => mock.AddExistingUserToGamingGroup(this.gamingGroupInvitationId));
        }

        [Test]
        public void ItRedirectsToTheGamingGroupPageIfThePlayerWasAddedDirectlyToAGamingGroupWithoutHavingToEnterInformation()
        {
            gamingGroupInviteConsumerMock.Expect(mock => mock.AddExistingUserToGamingGroup(Arg<string>.Is.Anything))
                                         .Return(new AddUserToGamingGroupResult{ UserAddedToExistingGamingGroup = true });

            RedirectToRouteResult redirectResult = accountControllerPartialMock.ConsumeInvitation(gamingGroupInvitationId, currentUser) as RedirectToRouteResult;

            Assert.That(redirectResult.RouteValues["action"], Is.EqualTo(MVC.GamingGroup.ActionNames.Index));
        }

        [Test]
        public void ItShowsTheRegisterAgainstExistingGamingGroupViewIfTheUserDoesntAlreadyExist()
        {
            gamingGroupInviteConsumerMock.Expect(mock => mock.AddExistingUserToGamingGroup(Arg<string>.Is.Anything))
                                         .Return(new AddUserToGamingGroupResult { UserAddedToExistingGamingGroup = false });

            ViewResult viewResult = accountControllerPartialMock.ConsumeInvitation(gamingGroupInvitationId, currentUser) as ViewResult;

            Assert.That(MVC.Account.Views.RegisterAgainstExistingGamingGroup, Is.EqualTo(viewResult.ViewName));
        }
    }
}
