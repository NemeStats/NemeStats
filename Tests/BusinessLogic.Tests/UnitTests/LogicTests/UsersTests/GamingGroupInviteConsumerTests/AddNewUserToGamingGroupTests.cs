using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.Exceptions;
using BusinessLogic.Logic.Users;
using BusinessLogic.Models;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;

namespace BusinessLogic.Tests.UnitTests.LogicTests.UsersTests.GamingGroupInviteConsumerTests
{
    [TestFixture]
    public class AddNewUserToGamingGroupTests : GamingGroupInviteConsumerTestBase
    {
        private readonly Guid invitationId = Guid.NewGuid();
        private GamingGroupInvitation expectedInvitation;
        private Player expectedPlayer;

        [SetUp]
        public void SetUp()
        {
            expectedInvitation = new GamingGroupInvitation
            {
                GamingGroupId = 1,
                PlayerId = 2
            };
            expectedPlayer = new Player();
        }

        private void BuildDataContextMock(bool userExists, bool gamingGroupInvitationExists, bool playerExists)
        {
            dataContextMock.Expect(mock => mock.FindById<ApplicationUser>(currentUser.Id))
                .Return(userExists ? currentUser : null);
            dataContextMock.Expect(mock => mock.FindById<GamingGroupInvitation>(invitationId))
                .Return(gamingGroupInvitationExists ? expectedInvitation : null);
            dataContextMock.Expect(mock => mock.FindById<Player>(expectedInvitation.PlayerId))
                .Return(playerExists ? expectedPlayer : null);
        }

        [Test]
        public void ItThrowsAnEntityDoesNotExistExceptionIfTheInvitationDoesntExist()
        {
            BuildDataContextMock(true, false, true);
            EntityDoesNotExistException expectedException = new EntityDoesNotExistException(invitationId);

            Exception actualException = Assert.Throws<EntityDoesNotExistException>(
                () => gamingGroupInviteConsumer.AddNewUserToGamingGroup(currentUser.Id, invitationId));

            Assert.That(expectedException.Message, Is.EqualTo(actualException.Message));
        }

        [Test]
        public void ItThrowsAnEntityDoesNotExistExceptionIfThePassedInUserDoesntExistYet()
        {
            BuildDataContextMock(false, true, true);
            EntityDoesNotExistException expectedException = new EntityDoesNotExistException(currentUser.Id);

            Exception actualException = Assert.Throws<EntityDoesNotExistException>(
                () => gamingGroupInviteConsumer.AddNewUserToGamingGroup(currentUser.Id, invitationId));

            Assert.That(expectedException.Message, Is.EqualTo(actualException.Message));
        }

        [Test]
        public void ItThrowsAnEntityDoesNotExistExceptionIfThePlayerOnTheInviteDoesNotExist()
        {
            BuildDataContextMock(true, true, false);

            EntityDoesNotExistException expectedException = new EntityDoesNotExistException(expectedInvitation.PlayerId);

            Exception actualException = Assert.Throws<EntityDoesNotExistException>(
                () => gamingGroupInviteConsumer.AddNewUserToGamingGroup(currentUser.Id, invitationId));

            Assert.That(expectedException.Message, Is.EqualTo(actualException.Message));
        }

        [Test]
        public void ItAddsTheUserToTheGamingGroup()
        {
            BuildDataContextMock(true, true, true);

            gamingGroupInviteConsumer.AddNewUserToGamingGroup(currentUser.Id, invitationId);

            dataContextMock.AssertWasCalled(mock => mock.Save(Arg<UserGamingGroup>.Matches(
                ugg => ugg.ApplicationUserId == currentUser.Id
                && ugg.GamingGroupId == expectedInvitation.GamingGroupId),
                Arg<ApplicationUser>.Is.Anything));
        }

        [Test]
        public void ItUpdatesTheUsersGamingGroup()
        {
            BuildDataContextMock(true, true, true);

            gamingGroupInviteConsumer.AddNewUserToGamingGroup(currentUser.Id, invitationId);

            dataContextMock.AssertWasCalled(mock => mock.Save(Arg<ApplicationUser>.Matches(
                user => user.CurrentGamingGroupId == expectedInvitation.GamingGroupId),
                Arg<ApplicationUser>.Is.Anything));
        }

        [Test]
        public void ItUpdatesThePlayersApplicationUserId()
        {
            BuildDataContextMock(true, true, true);

            gamingGroupInviteConsumer.AddNewUserToGamingGroup(currentUser.Id, invitationId);

            dataContextMock.AssertWasCalled(mock => mock.Save(Arg<Player>.Matches(
                player => player.ApplicationUserId == currentUser.Id),
                Arg<ApplicationUser>.Is.Anything));
        }
    }
}
