using BusinessLogic.DataAccess;
using BusinessLogic.DataAccess.GamingGroups;
using BusinessLogic.DataAccess.Repositories;
using BusinessLogic.Logic.Users;
using BusinessLogic.Models;
using BusinessLogic.Models.User;
using Microsoft.AspNet.Identity;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Tests.UnitTests.LogicTests.UsersTests.GamingGroupInviteConsumerImplTests
{
    [TestFixture]
    public class AddUserToInvitedGroupTests
    {
        private NemeStatsDbContext dbContextMock;
        private GamingGroupRepository gamingGroupRepositoryMock;
        private IUserStore<ApplicationUser> userStoreMock;
        private UserManager<ApplicationUser> userManager;
        private GamingGroupInviteConsumerImpl inviteConsumer;
        private GamingGroupAccessGranter gamingGroupAccessGranter;
        private List<GamingGroupInvitation> gamingGroupInvitations;
        private ApplicationUser currentUser;
        private ApplicationUser applicationUser;

        [SetUp]
        public void SetUp()
        {
            dbContextMock = MockRepository.GenerateMock<NemeStatsDbContext>();
            gamingGroupRepositoryMock = MockRepository.GenerateMock<GamingGroupRepository>();
            userStoreMock = MockRepository.GenerateMock<IUserStore<ApplicationUser>>();
            userManager = new UserManager<ApplicationUser>(userStoreMock);
            gamingGroupAccessGranter = MockRepository.GenerateMock<GamingGroupAccessGranter>();
            inviteConsumer = new GamingGroupInviteConsumerImpl(gamingGroupRepositoryMock, userManager, gamingGroupAccessGranter);
            currentUser = new ApplicationUser()
            {
                Id = "user id"
            };
            applicationUser = new ApplicationUser() { Id = currentUser.Id };
            gamingGroupInvitations = new List<GamingGroupInvitation>();
        }

        [Test]
        public async Task ItReturnsNullIfThereAreNoInvitesForTheGivenUser()
        {
            gamingGroupRepositoryMock.Expect(mock => mock.GetPendingGamingGroupInvitations(currentUser))
                .Repeat.Once()
                .Return(gamingGroupInvitations);
            int? gamingGroupId = await inviteConsumer.AddUserToInvitedGroupAsync(currentUser);

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
