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
using BusinessLogic.DataAccess;
using BusinessLogic.Logic.GamingGroups;
using BusinessLogic.Models;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using System.Collections.Generic;
using System.Linq;

namespace BusinessLogic.Tests.UnitTests.LogicTests.GamingGroupsTests.PendingGamingGroupInvitationRetrieverTests
{
    [TestFixture]
    public class GetPendingGamingGroupInvitationsTests
    {
        protected PendingGamingGroupInvitationRetriever retriever;
        protected NemeStatsDataContext dataContextMock;
        protected ApplicationUser currentUser;
        protected ApplicationUser expectedApplicationUser;
        protected List<GamingGroupInvitation> expectedGamingGroupInvitations;
        protected GamingGroupInvitation expectedGamingGroupInvitation;

        [SetUp]
        public void SetUp()
        {
            dataContextMock = MockRepository.GenerateMock<NemeStatsDataContext>();
            retriever = new PendingGamingGroupInvitationRetriever(dataContextMock);

            currentUser = new ApplicationUser()
            {
                Id = "application user id"
            };
            expectedApplicationUser = new ApplicationUser()
            {
                Id = currentUser.Id,
                Email = "email@email.com"
            };

            dataContextMock.Expect(mock => mock.FindById<ApplicationUser>(currentUser.Id))
                .Return(expectedApplicationUser);

            expectedGamingGroupInvitations = new List<GamingGroupInvitation>();
            expectedGamingGroupInvitation = new GamingGroupInvitation() { InviteeEmail = expectedApplicationUser.Email };
            expectedGamingGroupInvitations.Add(expectedGamingGroupInvitation);
            expectedGamingGroupInvitations.Add(new GamingGroupInvitation() { InviteeEmail = "some other email that shouldnt be included" });

            dataContextMock.Expect(mock => mock.GetQueryable<GamingGroupInvitation>())
                .Return(expectedGamingGroupInvitations.AsQueryable());
        }

        [Test]
        public void ItGetsAllPendingInvitesForTheCurrentUsersEmailAddress()
        {
            List<GamingGroupInvitation> actualGamingGroupInvitations = retriever.GetPendingGamingGroupInvitations(currentUser);

            Assert.True(actualGamingGroupInvitations.Count == 1);
            Assert.AreSame(expectedGamingGroupInvitation, actualGamingGroupInvitations[0]);
        }
    }
}
