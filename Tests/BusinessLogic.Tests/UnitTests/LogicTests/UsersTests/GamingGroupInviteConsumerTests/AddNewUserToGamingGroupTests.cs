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
            var expectedException = new EntityDoesNotExistException<GamingGroupInvitation>(invitationId);

            Exception actualException = Assert.Throws<EntityDoesNotExistException<GamingGroupInvitation>>(
                () => gamingGroupInviteConsumer.AddNewUserToGamingGroup(currentUser.Id, invitationId));

            Assert.That(expectedException.Message, Is.EqualTo(actualException.Message));
        }

        [Test]
        public void ItThrowsAnEntityDoesNotExistExceptionIfThePassedInUserDoesntExistYet()
        {
            BuildDataContextMock(false, true, true);
            var expectedException = new EntityDoesNotExistException<ApplicationUser>(currentUser.Id);

            Exception actualException = Assert.Throws<EntityDoesNotExistException<ApplicationUser>>(
                () => gamingGroupInviteConsumer.AddNewUserToGamingGroup(currentUser.Id, invitationId));

            Assert.That(expectedException.Message, Is.EqualTo(actualException.Message));
        }

        [Test]
        public void ItThrowsAnEntityDoesNotExistExceptionIfThePlayerOnTheInviteDoesNotExist()
        {
            BuildDataContextMock(true, true, false);

            var expectedException = new EntityDoesNotExistException<Player>(expectedInvitation.PlayerId);

            Exception actualException = Assert.Throws<EntityDoesNotExistException<Player>>(
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
        public void ItUpdatesTheInvitation()
        {
            BuildDataContextMock(true, true, true);

            gamingGroupInviteConsumer.AddNewUserToGamingGroup(currentUser.Id, invitationId);

            dataContextMock.AssertWasCalled(mock => mock.Save(Arg<GamingGroupInvitation>.Matches(
                ggi => ggi.RegisteredUserId == currentUser.Id
                && ggi.DateRegistered.Value.Date == DateTime.UtcNow.Date),
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
