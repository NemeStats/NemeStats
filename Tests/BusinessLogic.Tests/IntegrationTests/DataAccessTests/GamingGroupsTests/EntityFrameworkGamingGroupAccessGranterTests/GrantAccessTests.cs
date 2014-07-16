using BusinessLogic.DataAccess;
using BusinessLogic.DataAccess.GamingGroups;
using BusinessLogic.Models;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Tests.IntegrationTests.DataAccessTests.GamingGroups.EntityFrameworkGamingGroupAccessGranterTests
{
    [TestFixture]
    public class GrantAccessTests
    {
        protected NemeStatsDbContext dbContextMock;
        protected DbSet<GamingGroupInvitation> gamingGroupInvitationDbSetMock;
        protected GamingGroupAccessGranter gamingGroupAccessGranter;
        protected UserContext userContext;
        protected string inviteeEmail = "email@email.com";

        [SetUp]
        public void SetUp()
        {
            dbContextMock = MockRepository.GenerateMock<NemeStatsDbContext>();
            gamingGroupInvitationDbSetMock = MockRepository.GenerateMock<DbSet<GamingGroupInvitation>>();
            dbContextMock.Expect(mock => mock.GamingGroupInvitations)
                .Repeat.Once()
                .Return(gamingGroupInvitationDbSetMock);
            userContext = new UserContext()
            {
                ApplicationUserId = "user id",
                GamingGroupId = 777
            };
            gamingGroupAccessGranter = new EntityFrameworkGamingGroupAccessGranter(dbContextMock);
        }

        [Test]
        public void ItSetsTheInviteeEmail()
        {
            gamingGroupAccessGranter.GrantAccess(inviteeEmail, userContext);

            gamingGroupInvitationDbSetMock.AssertWasCalled(mock => mock.Add(Arg<GamingGroupInvitation>.Matches(invitation => invitation.InviteeEmail == inviteeEmail)));
        }

        [Test]
        public void ItSetsTheInviterUserId()
        {
            gamingGroupAccessGranter.GrantAccess(inviteeEmail, userContext);

            gamingGroupInvitationDbSetMock.AssertWasCalled(
                mock => mock.Add(Arg<GamingGroupInvitation>.Matches(invitation => invitation.InvitingUserId == userContext.ApplicationUserId)));
        }

        [Test]
        public void ItSetsTheGamingGroupId()
        {
            gamingGroupAccessGranter.GrantAccess(inviteeEmail, userContext);

            gamingGroupInvitationDbSetMock.AssertWasCalled(
                mock => mock.Add(Arg<GamingGroupInvitation>.Matches(invitation => invitation.GamingGroupId == userContext.GamingGroupId)));
        }

        public void ItSetsTheDateInvitationWasSent()
        {
            gamingGroupAccessGranter.GrantAccess(inviteeEmail, userContext);

            gamingGroupInvitationDbSetMock.AssertWasCalled(
                mock => mock.Add(Arg<GamingGroupInvitation>.Matches(invitation => invitation.DateSent.Date == DateTime.UtcNow.Date)));
        }

        [Test]
        public void ItSavesToTheDatabase()
        {
            gamingGroupAccessGranter.GrantAccess(inviteeEmail, userContext);

            dbContextMock.AssertWasCalled(mock => mock.SaveChanges());
        }

        [Test]
        public void ItReturnsTheGamingGroupInvitation()
        {
            GamingGroupInvitation returnedInvitation = gamingGroupAccessGranter.GrantAccess(inviteeEmail, userContext);
            IList<object[]> objectsPassedToAddMethod = gamingGroupInvitationDbSetMock.GetArgumentsForCallsMadeOn(
                mock => mock.Add(Arg<GamingGroupInvitation>.Is.Anything));
            GamingGroupInvitation savedInvitation = (GamingGroupInvitation)objectsPassedToAddMethod[0][0];

            Assert.AreSame(savedInvitation, returnedInvitation);
        }
    }
}
