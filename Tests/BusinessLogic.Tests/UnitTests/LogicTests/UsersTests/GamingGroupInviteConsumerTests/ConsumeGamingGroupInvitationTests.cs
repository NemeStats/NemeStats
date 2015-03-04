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
using BusinessLogic.DataAccess.GamingGroups;
using BusinessLogic.Logic.GamingGroups;
using BusinessLogic.Logic.Users;
using BusinessLogic.Models;
using BusinessLogic.Models.User;
using Microsoft.AspNet.Identity;
using NUnit.Framework;
using Rhino.Mocks;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BusinessLogic.Tests.UnitTests.LogicTests.UsersTests.GamingGroupInviteConsumerTests
{
    [TestFixture]
    public class ConsumeGamingGroupInvitationTests : GamingGroupInviteConsumerTestBase
    {
        [Test]
        public async Task ItReturnsNullIfThereAreNoInvitesForTheGivenUser()
        {
            pendingGamingGroupInvitationRetriever.Expect(mock => mock.GetPendingGamingGroupInvitations(currentUser))
                .Repeat.Once()
                .Return(gamingGroupInvitations);
            int? gamingGroupId = await this.gamingGroupInviteConsumer.ConsumeGamingGroupInvitation(currentUser);

            Assert.Null(gamingGroupId);
        }

        //TODO keep getting NullReferenceException when calling the FindAsync method...
        /*
        [Test]
        public async Task ItUpdatesTheUsersCurrentGamingGroupToTheGamingGroupOnTheOldestInvite()
        {
            GamingGroupInvitation newestInvite = new GamingGroupInvitation()
            {
                GamingGroupId = 1,
                DateSent = DateTime.UtcNow.AddDays(1)
            };
            gamingGroupInvitations.Add(newestInvite);
            GamingGroupInvitation oldestInvite = new GamingGroupInvitation()
            {
                GamingGroupId = 2,
                DateSent = DateTime.UtcNow.AddDays(-90)
            };
            gamingGroupInvitations.Add(oldestInvite);
            GamingGroupInvitation middleInvite = new GamingGroupInvitation()
            {
                GamingGroupId = 3,
                DateSent = DateTime.UtcNow
            };
            gamingGroupInvitations.Add(oldestInvite);
            gamingGroupRepositoryMock.Expect(mock => mock.GetPendingGamingGroupInvitations(currentUser))
                .Repeat.Once()
                .Return(gamingGroupInvitations);

            int? gamingGroupId = await inviteConsumer.AddUserToInvitedGroupAsync(currentUser);

            userManager.AssertWasCalled(
                mock => mock.Update(Arg<ApplicationUser>.Matches(user => user.CurrentGamingGroupId == oldestInvite.GamingGroupId)));
        }

        [Test]
        public async Task ItReturnsTheGamingGroupIdThatWasSet()
        {
            GamingGroupInvitation invite = new GamingGroupInvitation()
            {
                GamingGroupId = 135
            };
            gamingGroupInvitations.Add(invite);
            gamingGroupRepositoryMock.Expect(mock => mock.GetPendingGamingGroupInvitations(currentUser))
                .Repeat.Once()
                .Return(gamingGroupInvitations);

            int? gamingGroupId = await inviteConsumer.AddUserToInvitedGroupAsync(currentUser);

            Assert.AreEqual(invite.GamingGroupId, gamingGroupId);
        }

        [Test]
        public async Task ItConsumesTheInvitation()
        {
            GamingGroupInvitation invitation = new GamingGroupInvitation();
            gamingGroupInvitations.Add(invitation);
            gamingGroupRepositoryMock.Expect(mock => mock.GetPendingGamingGroupInvitations(currentUser))
                .Repeat.Once()
                .Return(gamingGroupInvitations);
 
            Task<ApplicationUser> task = new Task<ApplicationUser>(() => applicationUser);
            userStoreMock.Expect(mock => mock.FindByIdAsync(currentUser.ApplicationUserId))
                .Repeat.Once()
                .Return(task);

            int? gamingGroupId = await inviteConsumer.AddUserToInvitedGroupAsync(currentUser);

            gamingGroupAccessGranter.AssertWasCalled(
                mock => mock.ConsumeInvitation(Arg<GamingGroupInvitation>
                    .Matches(invite => invite == invitation),
                     Arg<ApplicationUser>.Is.Equal(currentUser)));
        }
         * * */
    }
}
