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
    public class ConsumeGamingGroupInvitationTests
    {
        private NemeStatsDbContext dbContextMock;
        private IPendingGamingGroupInvitationRetriever pendingGamingGroupInvitationRetriever;
        private IUserStore<ApplicationUser> userStoreMock;
        private ApplicationUserManager applicationUserManagerMock;
        private GamingGroupInviteConsumer inviteConsumer;
        private IGamingGroupAccessGranter gamingGroupAccessGranter;
        private List<GamingGroupInvitation> gamingGroupInvitations;
        private ApplicationUser currentUser;
        private ApplicationUser applicationUser;

        [SetUp]
        public void SetUp()
        {
            dbContextMock = MockRepository.GenerateMock<NemeStatsDbContext>();
            pendingGamingGroupInvitationRetriever = MockRepository.GenerateMock<IPendingGamingGroupInvitationRetriever>();
            userStoreMock = MockRepository.GenerateMock<IUserStore<ApplicationUser>>();
            applicationUserManagerMock = MockRepository.GenerateMock<ApplicationUserManager>(userStoreMock);
            gamingGroupAccessGranter = MockRepository.GenerateMock<IGamingGroupAccessGranter>();
            inviteConsumer = new GamingGroupInviteConsumer(pendingGamingGroupInvitationRetriever, applicationUserManagerMock, gamingGroupAccessGranter);
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
            pendingGamingGroupInvitationRetriever.Expect(mock => mock.GetPendingGamingGroupInvitations(currentUser))
                .Repeat.Once()
                .Return(gamingGroupInvitations);
            int? gamingGroupId = await inviteConsumer.ConsumeGamingGroupInvitation(currentUser);

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
