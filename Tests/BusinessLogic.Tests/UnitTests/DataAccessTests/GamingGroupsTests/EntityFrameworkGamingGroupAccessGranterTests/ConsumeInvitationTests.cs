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
using System.Linq;

namespace BusinessLogic.Tests.UnitTests.DataAccessTests.GamingGroupsTests.EntityFrameworkGamingGroupAccessGranterTests
{
    [TestFixture]
    public class ConsumeInvitationTests : EntityFrameworkGamingGroupAccessGranterTestBase
    {
        [Test]
        public void ItSetsTheDateRegistered()
        {
            GamingGroupInvitation invitation = new GamingGroupInvitation();

            gamingGroupAccessGranter.ConsumeInvitation(invitation, currentUser);

            dataContextMock.AssertWasCalled(mock => mock.Save(
                Arg<GamingGroupInvitation>.Matches(invite => invite.DateRegistered.Value.Date == DateTime.UtcNow.Date),
                Arg<ApplicationUser>.Is.Same(currentUser)));
        }

        [Test]
        public void ItSetsTheRegisteredUserId()
        {
            GamingGroupInvitation invitation = new GamingGroupInvitation();

            gamingGroupAccessGranter.ConsumeInvitation(invitation, currentUser);

            dataContextMock.AssertWasCalled(mock => mock.Save(
                Arg<GamingGroupInvitation>.Matches(invite => invite.RegisteredUserId == currentUser.Id),
                Arg<ApplicationUser>.Is.Same(currentUser)));
        }
    }
}
