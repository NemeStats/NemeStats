using BusinessLogic.DataAccess;
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
        private List<GamingGroupInvitation> gamingGroupInvitations;
        private UserContext userContext;

        [SetUp]
        public void SetUp()
        {
            dbContextMock = MockRepository.GenerateMock<NemeStatsDbContext>();
            gamingGroupRepositoryMock = MockRepository.GenerateMock<GamingGroupRepository>();
            userStoreMock = MockRepository.GenerateMock<IUserStore<ApplicationUser>>();
            userManager = new UserManager<ApplicationUser>(userStoreMock);
            inviteConsumer = new GamingGroupInviteConsumerImpl(gamingGroupRepositoryMock, userManager);
            userContext = new UserContext()
            {
                ApplicationUserId = "user context"
            };
            gamingGroupInvitations = new List<GamingGroupInvitation>();
        }

        [Test]
        public async Task ItReturnsNullIfThereAreNoInvitesForTheGivenUser()
        {
            gamingGroupRepositoryMock.Expect(mock => mock.GetPendingGamingGroupInvitations(userContext))
                .Repeat.Once()
                .Return(gamingGroupInvitations);
            int? gamingGroupId = await inviteConsumer.AddUserToInvitedGroupAsync(userContext);

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
            gamingGroupRepositoryMock.Expect(mock => mock.GetPendingGamingGroupInvitations(userContext))
                .Repeat.Once()
                .Return(gamingGroupInvitations);

            int? gamingGroupId = await inviteConsumer.AddUserToInvitedGroupAsync(userContext);

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
            gamingGroupRepositoryMock.Expect(mock => mock.GetPendingGamingGroupInvitations(userContext))
                .Repeat.Once()
                .Return(gamingGroupInvitations);

            int? gamingGroupId = await inviteConsumer.AddUserToInvitedGroupAsync(userContext);

            Assert.AreEqual(invite.GamingGroupId, gamingGroupId);
        }
         * */
    }
}
