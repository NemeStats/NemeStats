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
            gamingGroupAccessGranter.CreateInvitation(inviteeEmail, currentUser);

            gamingGroupInvitationRepositoryMock.AssertWasCalled(mock => mock.Save(Arg<GamingGroupInvitation>.Matches(invitation => invitation.InviteeEmail == inviteeEmail), 
                Arg<ApplicationUser>.Is.Same(currentUser)));
        }

        [Test]
        public void ItSetsTheInviterUserId()
        {
            gamingGroupAccessGranter.CreateInvitation(inviteeEmail, currentUser);

            gamingGroupInvitationRepositoryMock.AssertWasCalled(
                mock => mock.Save(Arg<GamingGroupInvitation>.Matches(invitation => invitation.InvitingUserId == currentUser.Id), 
                    Arg<ApplicationUser>.Is.Same(currentUser)));
        }

        [Test]
        public void ItSetsTheGamingGroupId()
        {
            gamingGroupAccessGranter.CreateInvitation(inviteeEmail, currentUser);

            gamingGroupInvitationRepositoryMock.AssertWasCalled(
                mock => mock.Save(Arg<GamingGroupInvitation>.Matches(invitation => invitation.GamingGroupId == currentUser.CurrentGamingGroupId), 
                    Arg<ApplicationUser>.Is.Same(currentUser)));
        }

        public void ItSetsTheDateInvitationWasSent()
        {
            gamingGroupAccessGranter.CreateInvitation(inviteeEmail, currentUser);

            gamingGroupInvitationRepositoryMock.AssertWasCalled(
                mock => mock.Save(Arg<GamingGroupInvitation>.Matches(invitation => invitation.DateSent.Date == DateTime.UtcNow.Date), currentUser));
        }

        [Test]
        public void ItSavesToTheDatabase()
        {
            gamingGroupAccessGranter.CreateInvitation(inviteeEmail, currentUser);

            gamingGroupInvitationRepositoryMock.AssertWasCalled(mock => mock.Save(Arg<GamingGroupInvitation>.Is.Anything, 
                Arg<ApplicationUser>.Is.Same(currentUser)));
        }

        [Test]
        public void ItReturnsTheGamingGroupInvitation()
        {
            GamingGroupInvitation returnedInvitation = gamingGroupAccessGranter.CreateInvitation(inviteeEmail, currentUser);
            IList<object[]> objectsPassedToAddMethod = gamingGroupInvitationRepositoryMock.GetArgumentsForCallsMadeOn(
                mock => mock.Save(Arg<GamingGroupInvitation>.Is.Anything, Arg<ApplicationUser>.Is.Anything));
            GamingGroupInvitation savedInvitation = (GamingGroupInvitation)objectsPassedToAddMethod[0][0];

            Assert.AreSame(savedInvitation, returnedInvitation);
        }
    }
}
