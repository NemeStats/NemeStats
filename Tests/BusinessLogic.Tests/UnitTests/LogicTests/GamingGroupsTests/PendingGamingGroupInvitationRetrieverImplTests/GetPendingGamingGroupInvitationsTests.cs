using BusinessLogic.DataAccess;
using BusinessLogic.Logic.GamingGroups;
using BusinessLogic.Models;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Tests.UnitTests.LogicTests.GamingGroupsTests.PendingGamingGroupInvitationRetrieverImplTests
{
    [TestFixture]
    public class GetPendingGamingGroupInvitationsTests
    {
        protected PendingGamingGroupInvitationRetrieverImpl retriever;
        protected NemeStatsDataContext dataContextMock;
        protected ApplicationUser currentUser;
        protected ApplicationUser expectedApplicationUser;
        protected List<GamingGroupInvitation> expectedGamingGroupInvitations;
        protected GamingGroupInvitation expectedGamingGroupInvitation;

        [SetUp]
        public void SetUp()
        {
            dataContextMock = MockRepository.GenerateMock<NemeStatsDataContext>();
            retriever = new PendingGamingGroupInvitationRetrieverImpl(dataContextMock);

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
