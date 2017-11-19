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
using System.Configuration.Abstractions;
using System.Linq;
using System.Threading.Tasks;
using BusinessLogic.DataAccess;
using BusinessLogic.Exceptions;
using BusinessLogic.Logic.Players;
using BusinessLogic.Models;
using BusinessLogic.Models.Players;
using BusinessLogic.Models.User;
using Microsoft.AspNet.Identity;
using NUnit.Framework;
using Rhino.Mocks;
using Shouldly;

namespace BusinessLogic.Tests.UnitTests.LogicTests.PlayersTests.PlayerInviterTests
{
    [TestFixture]
    public class InvitePlayerTests
    {
        private PlayerInviter _playerInviter;
        private IDataContext _dataContextMock;
        private IIdentityMessageService _emailServiceMock;
        private IConfigurationManager _configurationManagerMock;
        private PlayerInvitation _playerInvitation;
        private ApplicationUser _currentUser;
        private int _expectedGamingGroupId = 15;
        private GamingGroup _gamingGroup;
        private GamingGroupInvitation _gamingGroupInvitation;
        private string _rootUrl = "http://nemestats.com";
        private string _existingUserId = "existing user id";

        [SetUp]
        public void SetUp()
        {
            _dataContextMock = MockRepository.GenerateMock<IDataContext>();
            _emailServiceMock = MockRepository.GenerateMock<IIdentityMessageService>();
            _configurationManagerMock = MockRepository.GenerateMock<IConfigurationManager>();
            _playerInvitation = new PlayerInvitation
            {
                CustomEmailMessage = "custom message",
                EmailSubject = "email subject",
                InvitedPlayerEmail = "player email",
                InvitedPlayerId = 1
            };
            _currentUser = new ApplicationUser
            {
                CurrentGamingGroupId = _expectedGamingGroupId,
                UserName = "Fergie Ferg"
            };
            _gamingGroup = new GamingGroup
            {
                Id = _expectedGamingGroupId,
                Name = "jake's Gaming Group"
            };
            _gamingGroupInvitation = new GamingGroupInvitation
            {
                Id = Guid.NewGuid()
            };

            _dataContextMock.Expect(mock => mock.FindById<GamingGroup>(_currentUser.CurrentGamingGroupId))
                           .Return(_gamingGroup);

            List<ApplicationUser> applicationUsers = new List<ApplicationUser>
            {
                new ApplicationUser
                {
                    Email = _playerInvitation.InvitedPlayerEmail,
                    Id = _existingUserId
                }
            };
            _dataContextMock.Expect(mock => mock.GetQueryable<ApplicationUser>())
                           .Return(applicationUsers.AsQueryable());

            _dataContextMock.Expect(mock => mock.Save<GamingGroupInvitation>(Arg<GamingGroupInvitation>.Is.Anything, Arg<ApplicationUser>.Is.Anything))
                           .Return(_gamingGroupInvitation);

            _configurationManagerMock.Expect(mock => mock.AppSettings[PlayerInviter.APP_SETTING_URL_ROOT])
                                    .Return(_rootUrl);

            _emailServiceMock.Expect(mock => mock.SendAsync(Arg<IdentityMessage>.Is.Anything))
                            .Return(Task.FromResult<object>(null));

            _playerInviter = new PlayerInviter(_dataContextMock, _emailServiceMock, _configurationManagerMock);
        }

        [Test]
        public void It_Throws_A_UserHasNoGamingGroupException_If_The_Inviting_User_Has_No_Gaming_Group()
        {
            //--arrange
            _currentUser.CurrentGamingGroupId = null;
            var expectedException = new UserHasNoGamingGroupException(_currentUser.Id);

            //--act
            var actualException = Assert.Throws<UserHasNoGamingGroupException>(() => _playerInviter.InvitePlayer(_playerInvitation, _currentUser));

            //--assert
            actualException.Message.ShouldBe(expectedException.Message);
        }


        [Test]
        public void It_Saves_A_Gaming_Group_Invitation()
        {
            _playerInviter.InvitePlayer(_playerInvitation, _currentUser);

            _dataContextMock.AssertWasCalled(mock => mock.Save<GamingGroupInvitation>(Arg<GamingGroupInvitation>.Matches(
                invite => invite.PlayerId == _playerInvitation.InvitedPlayerId
                && invite.DateSent.Date == DateTime.UtcNow.Date
                && invite.GamingGroupId == _currentUser.CurrentGamingGroupId
                && invite.InviteeEmail == _playerInvitation.InvitedPlayerEmail
                && invite.InvitingUserId == _currentUser.Id),
                Arg<ApplicationUser>.Is.Same(_currentUser)));
        }

        [Test]
        public void It_Sets_The_Registered_User_Id_On_The_Gaming_Group_Invitation_If_The_User_Already_Has_An_Existing_Account()
        {
            _playerInviter.InvitePlayer(_playerInvitation, _currentUser);

            _dataContextMock.AssertWasCalled(mock => mock.Save<GamingGroupInvitation>(Arg<GamingGroupInvitation>.Matches(
                invite => invite.RegisteredUserId == _existingUserId),
                Arg<ApplicationUser>.Is.Anything));
        }

        [Test]
        public void It_Emails_The_User()
        {
            string expectedBody = string.Format(PlayerInviter.EMAIL_MESSAGE_INVITE_PLAYER,
                                                _currentUser.UserName,
                                                _gamingGroup.Name,
                                                _rootUrl,
                                                _playerInvitation.CustomEmailMessage,
                                                _gamingGroupInvitation.Id,
                                                "<br/><br/>");

            _playerInviter.InvitePlayer(_playerInvitation, _currentUser);

            _emailServiceMock.AssertWasCalled(mock => mock.SendAsync(Arg<IdentityMessage>.Matches(
                message => message.Subject == _playerInvitation.EmailSubject
                && message.Body == expectedBody)));
        }
    }
}
