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
using System;
using System.Collections.Generic;
using System.Linq;
using BusinessLogic.DataAccess;
using BusinessLogic.Exceptions;
using BusinessLogic.Models;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;

namespace BusinessLogic.Tests.UnitTests.LogicTests.UsersTests.GamingGroupInviteConsumerTests
{
    [TestFixture]
    public class AddExistingUserToGamingGroupTests : GamingGroupInviteConsumerTestBase
    {
        private readonly Guid gamingGroupInvitationId = Guid.NewGuid();
        private GamingGroupInvitation invitation;
        private string email;
        private int gamingGroupId = 123;
        private List<UserGamingGroup> userGamingGroups;
        private Player player;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            invitation = new GamingGroupInvitation
            {
                Id = gamingGroupInvitationId
            };

            userGamingGroups = new List<UserGamingGroup>
            {
                new UserGamingGroup
                {
                    ApplicationUserId = currentUser.Id,
                    GamingGroupId = gamingGroupId
                }
            };

            player = new Player();

            dataContextMock.Expect(mock => mock.FindById<Player>(invitation.PlayerId))
                           .Return(player);
        }

        private void SetupMocks(
            bool hasGamingGroupInvitation, 
            bool hasInvitedPlayer, 
            bool hasExistingRegisteredUser, 
            bool hasExistingUserGamingGroupRecord)
        {
            invitation.RegisteredUserId = hasExistingRegisteredUser ? currentUser.Id : null;
            invitation.GamingGroupId = gamingGroupId;

            dataContextMock.Expect(mock => mock.FindById<GamingGroupInvitation>(gamingGroupInvitationId))
                .Return(hasGamingGroupInvitation ? invitation : null);

            dataContextMock.Expect(mock => mock.FindById<ApplicationUser>(invitation.RegisteredUserId))
                .Return(hasInvitedPlayer ? new ApplicationUser{ Id = currentUser.Id } : null);

            dataContextMock.Expect(mock => mock.GetQueryable<UserGamingGroup>())
                .Return(hasExistingUserGamingGroupRecord ? userGamingGroups.AsQueryable() : new List<UserGamingGroup>().AsQueryable());
        }

        [Test]
        public void ItThrowsAnEntityNotFoundIfTheGamingGroupInvitationDoesNotExist()
        {
            this.SetupMocks(false, true, true, true);
            var expectedException = new EntityDoesNotExistException(typeof(GamingGroupInvitation), gamingGroupInvitationId);

            Exception actualException = Assert.Throws<EntityDoesNotExistException>(
                () => gamingGroupInviteConsumer.AddExistingUserToGamingGroup(this.gamingGroupInvitationId.ToString()));

            Assert.That(actualException.Message.Equals(expectedException.Message));
        }

        [Test]
        public void ItThrowsAnEntityNotFoundExceptionIfTheInvitationDoesNotExist()
        {
            this.SetupMocks(true, false, true, true);

            var expectedException = new EntityDoesNotExistException(typeof(GamingGroupInvitation), invitation.RegisteredUserId);

            Exception actualException = Assert.Throws<EntityDoesNotExistException>(
                () => gamingGroupInviteConsumer.AddExistingUserToGamingGroup(this.gamingGroupInvitationId.ToString()));

            Assert.That(actualException.Message, Is.EqualTo(expectedException.Message));
        }

        [Test]
        public void TheUserAddedToExistingGameGroupFlagIsFalseIfTheInvitationIsNotForAPlayerWithAnExistingRegisteredUserId()
        {
            this.SetupMocks(true, true, false, true);

            var actualResult = gamingGroupInviteConsumer.AddExistingUserToGamingGroup(this.gamingGroupInvitationId.ToString());

            Assert.False(actualResult.UserAddedToExistingGamingGroup);
        }

        [Test]
        public void TheUserAddedToExistingGameGroupFlagIsTrueIfTheInvitationIsForAPlayerWithAnExistingRegisteredUserId()
        {
            this.SetupMocks(true, true, true, true);

            var actualResult = gamingGroupInviteConsumer.AddExistingUserToGamingGroup(this.gamingGroupInvitationId.ToString());

            Assert.True(actualResult.UserAddedToExistingGamingGroup);
        }

        [Test]
        public void ItAddsAUserGamingGroupRecordIfTheInvitationIsForAnExistingPlayer()
        {
            this.SetupMocks(true, true, true, false);
            
            gamingGroupInviteConsumer.AddExistingUserToGamingGroup(this.gamingGroupInvitationId.ToString());

            dataContextMock.AssertWasCalled(mock => mock.Save(Arg<UserGamingGroup>
                                                                  .Matches(ugg => ugg.ApplicationUserId == invitation.RegisteredUserId
                                                                                  && ugg.GamingGroupId == invitation.GamingGroupId),
                                                              Arg<ApplicationUser>.Is.Anything));
        }

        [Test]
        public void ItDoesntAddAUserGamingGroupRecordIfTheAssociationAlreadyExists()
        {
            this.SetupMocks(true, true, true, true);

            gamingGroupInviteConsumer.AddExistingUserToGamingGroup(this.gamingGroupInvitationId.ToString());

            dataContextMock.AssertWasNotCalled(mock => mock.Save(Arg<UserGamingGroup>
                                                                  .Matches(ugg => ugg.ApplicationUserId == invitation.RegisteredUserId
                                                                                  && ugg.GamingGroupId == invitation.GamingGroupId),
                                                              Arg<ApplicationUser>.Is.Anything));
        }

        [Test]
        public void ItSwitchesTheUsersContextToTheNewGamingGroup()
        {
            this.SetupMocks(true, true, true, true);

            gamingGroupInviteConsumer.AddExistingUserToGamingGroup(this.gamingGroupInvitationId.ToString());

            dataContextMock.AssertWasCalled(mock => mock.Save(Arg<ApplicationUser>
                                                                  .Matches(appUser => appUser.CurrentGamingGroupId == invitation.GamingGroupId),
                                                              Arg<ApplicationUser>.Is.Anything));
        }

        [Test]
        public void ItUpdatesTheGamingGroupInvitationIfTheUserAlreadyExists()
        {
            this.SetupMocks(true, true, true, true);

            gamingGroupInviteConsumer.AddExistingUserToGamingGroup(this.gamingGroupInvitationId.ToString());

            dataContextMock.AssertWasCalled(mock => mock.Save(Arg<GamingGroupInvitation>
                                                                  .Matches(ggi => ggi.RegisteredUserId == invitation.RegisteredUserId
                                                                  && ggi.DateRegistered.Value.Date == DateTime.UtcNow.Date),
                                                              Arg<ApplicationUser>.Is.Anything));
        }

        [Test]
        public void ItUpdatesTheExistingPlayer()
        {
            this.SetupMocks(true, true, true, true);

            gamingGroupInviteConsumer.AddExistingUserToGamingGroup(this.gamingGroupInvitationId.ToString());

            dataContextMock.AssertWasCalled(mock => mock.Save(Arg<Player>
                                                                  .Matches(p => p.ApplicationUserId == currentUser.Id),
                                                              Arg<ApplicationUser>.Is.Anything));
        }
    }
}