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
using BusinessLogic.Models;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;

namespace BusinessLogic.Tests.UnitTests.DataAccessTests.GamingGroupsTests.EntityFrameworkGamingGroupAccessGranterTests
{
    [TestFixture]
    public class CreateInvitationTests : EntityFrameworkGamingGroupAccessGranterTestBase
    {
        protected string inviteeEmail = "email@email.com";

        [Test]
        public void ItSetsTheInviteeEmail()
        {
            gamingGroupAccessGranter.CreateInvitation(inviteeEmail, currentUser);

            dataContextMock.AssertWasCalled(
                mock => mock.Save<GamingGroupInvitation>(Arg<GamingGroupInvitation>.Matches(invitation => invitation.InviteeEmail == inviteeEmail), 
                Arg<ApplicationUser>.Is.Same(currentUser)));
        }

        [Test]
        public void ItSetsTheInviterUserId()
        {
            gamingGroupAccessGranter.CreateInvitation(inviteeEmail, currentUser);

            dataContextMock.AssertWasCalled(
                mock => mock.Save<GamingGroupInvitation>(Arg<GamingGroupInvitation>
                    .Matches(invitation => invitation.InvitingUserId == currentUser.Id), 
                    Arg<ApplicationUser>.Is.Same(currentUser)));
        }

        [Test]
        public void ItSetsTheGamingGroupId()
        {
            gamingGroupAccessGranter.CreateInvitation(inviteeEmail, currentUser);

            dataContextMock.AssertWasCalled(
                mock => mock.Save<GamingGroupInvitation>(Arg<GamingGroupInvitation>
                    .Matches(invitation => invitation.GamingGroupId == currentUser.CurrentGamingGroupId), 
                    Arg<ApplicationUser>.Is.Same(currentUser)));
        }

        public void ItSetsTheDateInvitationWasSent()
        {
            gamingGroupAccessGranter.CreateInvitation(inviteeEmail, currentUser);

            dataContextMock.AssertWasCalled(
                mock => mock.Save<GamingGroupInvitation>(Arg<GamingGroupInvitation>
                    .Matches(invitation => invitation.DateSent.Date == DateTime.UtcNow.Date), currentUser));
        }

        [Test]
        public void ItSavesToTheDatabase()
        {
            gamingGroupAccessGranter.CreateInvitation(inviteeEmail, currentUser);

            dataContextMock.AssertWasCalled(
                mock => mock.Save<GamingGroupInvitation>(Arg<GamingGroupInvitation>.Is.Anything, 
                Arg<ApplicationUser>.Is.Same(currentUser)));
        }

        [Test]
        public void ItReturnsTheGamingGroupInvitation()
        {
            GamingGroupInvitation returnedInvitation = gamingGroupAccessGranter.CreateInvitation(inviteeEmail, currentUser);
            IList<object[]> objectsPassedToAddMethod = dataContextMock.GetArgumentsForCallsMadeOn(
                mock => mock.Save<GamingGroupInvitation>(Arg<GamingGroupInvitation>.Is.Anything, Arg<ApplicationUser>.Is.Anything));
            GamingGroupInvitation savedInvitation = (GamingGroupInvitation)objectsPassedToAddMethod[0][0];

            Assert.AreSame(savedInvitation, returnedInvitation);
        }
    }
}
