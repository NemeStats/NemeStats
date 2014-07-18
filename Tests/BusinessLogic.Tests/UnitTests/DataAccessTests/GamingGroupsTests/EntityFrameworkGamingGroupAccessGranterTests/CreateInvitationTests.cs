using BusinessLogic.DataAccess;
using BusinessLogic.DataAccess.GamingGroups;
using BusinessLogic.Models;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Data.Entity;

namespace BusinessLogic.Tests.UnitTests.DataAccessTests.GamingGroupsTests.EntityFrameworkGamingGroupAccessGranterTests
{
    [TestFixture]
    public class CreateInvitationTests : EntityFrameworkGamingGroupAccessGranterTestBase
    {
        protected string inviteeEmail = "email@email.com";

        [Test]
        public void ItSetsTheInviteeEmail()
        {
            gamingGroupAccessGranter.CreateInvitation(inviteeEmail, userContext);

            gamingGroupInvitationRepositoryMock.AssertWasCalled(mock => mock.Save(Arg<GamingGroupInvitation>.Matches(invitation => invitation.InviteeEmail == inviteeEmail), 
                Arg<UserContext>.Is.Same(userContext)));
        }

        [Test]
        public void ItSetsTheInviterUserId()
        {
            gamingGroupAccessGranter.CreateInvitation(inviteeEmail, userContext);

            gamingGroupInvitationRepositoryMock.AssertWasCalled(
                mock => mock.Save(Arg<GamingGroupInvitation>.Matches(invitation => invitation.InvitingUserId == userContext.ApplicationUserId), 
                    Arg<UserContext>.Is.Same(userContext)));
        }

        [Test]
        public void ItSetsTheGamingGroupId()
        {
            gamingGroupAccessGranter.CreateInvitation(inviteeEmail, userContext);

            gamingGroupInvitationRepositoryMock.AssertWasCalled(
                mock => mock.Save(Arg<GamingGroupInvitation>.Matches(invitation => invitation.GamingGroupId == userContext.GamingGroupId), 
                    Arg<UserContext>.Is.Same(userContext)));
        }

        public void ItSetsTheDateInvitationWasSent()
        {
            gamingGroupAccessGranter.CreateInvitation(inviteeEmail, userContext);

            gamingGroupInvitationRepositoryMock.AssertWasCalled(
                mock => mock.Save(Arg<GamingGroupInvitation>.Matches(invitation => invitation.DateSent.Date == DateTime.UtcNow.Date), userContext));
        }

        [Test]
        public void ItSavesToTheDatabase()
        {
            gamingGroupAccessGranter.CreateInvitation(inviteeEmail, userContext);

            gamingGroupInvitationRepositoryMock.AssertWasCalled(mock => mock.Save(Arg<GamingGroupInvitation>.Is.Anything, 
                Arg<UserContext>.Is.Same(userContext)));
        }

        [Test]
        public void ItReturnsTheGamingGroupInvitation()
        {
            GamingGroupInvitation returnedInvitation = gamingGroupAccessGranter.CreateInvitation(inviteeEmail, userContext);
            IList<object[]> objectsPassedToAddMethod = gamingGroupInvitationRepositoryMock.GetArgumentsForCallsMadeOn(
                mock => mock.Save(Arg<GamingGroupInvitation>.Is.Anything, Arg<UserContext>.Is.Anything));
            GamingGroupInvitation savedInvitation = (GamingGroupInvitation)objectsPassedToAddMethod[0][0];

            Assert.AreSame(savedInvitation, returnedInvitation);
        }
    }
}
