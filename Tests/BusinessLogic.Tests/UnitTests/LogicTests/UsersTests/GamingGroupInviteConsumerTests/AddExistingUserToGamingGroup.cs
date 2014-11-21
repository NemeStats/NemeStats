using System;
using System.Linq;
using BusinessLogic.Exceptions;
using BusinessLogic.Models;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;

namespace BusinessLogic.Tests.UnitTests.LogicTests.UsersTests.GamingGroupInviteConsumerTests
{
    [TestFixture]
    public class AddExistingUserToGamingGroup : GamingGroupInviteConsumerTestBase
    {
        private readonly string gamingGroupInvitationId = Guid.NewGuid().ToString();
        private string email;

        [Test]
        public void ItThrowsAnEntityNotFoundIfTheGamingGroupInvitationDoesNotExist()
        {
            dataContextMock.Expect(mock => mock.FindById<GamingGroupInvitation>(new Guid(gamingGroupInvitationId)))
                           .Return(null);
            var expectedException = new EntityDoesNotExistException(gamingGroupInvitationId);

            Exception actualException = Assert.Throws<EntityDoesNotExistException>(
                () => gamingGroupInviteConsumer.AddExistingUserToGamingGroup(this.gamingGroupInvitationId));

            Assert.That(actualException.Message.Equals(expectedException.Message));
        }

        [Test]
        public void ItThrowsAnEntityNotFoundExceptionIfTheInvitedPlayerDoesNotExist()
        {
            var invitation = new GamingGroupInvitation
            {
                RegisteredUserId = "user id that doesn't have a corresponding player"
            };
            dataContextMock.Expect(mock => mock.FindById<GamingGroupInvitation>(new Guid(gamingGroupInvitationId)))
                           .Return(invitation);
            dataContextMock.Expect(mock => mock.FindById<ApplicationUser>(invitation.RegisteredUserId))
                           .Return(null);
            var expectedException = new EntityDoesNotExistException(invitation.RegisteredUserId);

            Exception actualException = Assert.Throws<EntityDoesNotExistException>(
                () => gamingGroupInviteConsumer.AddExistingUserToGamingGroup(this.gamingGroupInvitationId));

            Assert.That(actualException.Message, Is.EqualTo(expectedException.Message));
        }

        [Test]
        public void TheUserAddedToExistingGameGroupFlagIsFalseIfTheInvitationIsNotForAPlayerWithAnExistingRegisteredUserId()
        {
            var invitation = new GamingGroupInvitation();
            dataContextMock.Expect(mock => mock.FindById<GamingGroupInvitation>(new Guid(gamingGroupInvitationId)))
                           .Return(invitation);

            var actualResult = gamingGroupInviteConsumer.AddExistingUserToGamingGroup(this.gamingGroupInvitationId);

            Assert.False(actualResult.UserAddedToExistingGamingGroup);
        }

        [Test]
        public void TheUserAddedToExistingGameGroupFlagIsTrueIfTheInvitationIsForAPlayerWithAnExistingRegisteredUserId()
        {
            var invitation = new GamingGroupInvitation
            {
                RegisteredUserId = "registered user id"
            };
            dataContextMock.Expect(mock => mock.FindById<GamingGroupInvitation>(new Guid(gamingGroupInvitationId)))
                           .Return(invitation);
            dataContextMock.Expect(mock => mock.FindById<ApplicationUser>(invitation.RegisteredUserId))
                            .Return(new ApplicationUser());
            var actualResult = gamingGroupInviteConsumer.AddExistingUserToGamingGroup(this.gamingGroupInvitationId);

            Assert.True(actualResult.UserAddedToExistingGamingGroup);
        }

        [Test]
        public void ItAddsAUserGamingGroupRecordIfTheInvitationIsForAnExistingPlayer()
        {
            var invitation = new GamingGroupInvitation
            {
                GamingGroupId = 123,
                RegisteredUserId = "registered user id"
            };
            dataContextMock.Expect(mock => mock.FindById<GamingGroupInvitation>(new Guid(gamingGroupInvitationId)))
                           .Return(invitation);
            dataContextMock.Expect(mock => mock.FindById<ApplicationUser>(invitation.RegisteredUserId))
                            .Return(new ApplicationUser());
            gamingGroupInviteConsumer.AddExistingUserToGamingGroup(this.gamingGroupInvitationId);

            dataContextMock.AssertWasCalled(mock => mock.Save(Arg<UserGamingGroup>
                                                                  .Matches(ugg => ugg.ApplicationUserId == invitation.RegisteredUserId
                                                                                  && ugg.GamingGroupId == invitation.GamingGroupId),
                                                              Arg<ApplicationUser>.Is.Anything));
        }

        [Test]
        public void ItSwitchesTheUsersContextToTheNewGamingGroup()
        {
            var invitation = new GamingGroupInvitation
            {
                GamingGroupId = 123,
                RegisteredUserId = "registered user id"
            };
            dataContextMock.Expect(mock => mock.FindById<GamingGroupInvitation>(new Guid(gamingGroupInvitationId)))
                           .Return(invitation);
            dataContextMock.Expect(mock => mock.FindById<ApplicationUser>(invitation.RegisteredUserId))
                .Return(new ApplicationUser());

            gamingGroupInviteConsumer.AddExistingUserToGamingGroup(this.gamingGroupInvitationId);

            dataContextMock.AssertWasCalled(mock => mock.Save(Arg<ApplicationUser>
                                                                  .Matches(appUser => appUser.CurrentGamingGroupId == invitation.GamingGroupId),
                                                              Arg<ApplicationUser>.Is.Anything));
        }
    }
}