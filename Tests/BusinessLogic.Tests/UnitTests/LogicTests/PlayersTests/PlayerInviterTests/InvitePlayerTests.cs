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
using NemeStats.TestingHelpers.NemeStatsTestingExtensions;
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
                InvitedPlayerId = 1,
                GamingGroupId = _expectedGamingGroupId
            };
            _currentUser = new ApplicationUser
            {
                UserName = "Fergie Ferg"
            };
            _gamingGroup = new GamingGroup
            {
                Id = _expectedGamingGroupId,
                Name = "jake's Gaming Group"
            };
            _gamingGroupInvitation = new GamingGroupInvitation
            {
                Id = Guid.NewGuid(),
                GamingGroupId = _expectedGamingGroupId
            };

            _dataContextMock.Expect(mock => mock.FindById<GamingGroup>(_gamingGroupInvitation.GamingGroupId))
                           .Return(_gamingGroup);

            var applicationUsers = new List<ApplicationUser>
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
        public void It_Saves_A_Gaming_Group_Invitation()
        {
            _playerInviter.InvitePlayer(_playerInvitation, _currentUser);

            var args = _dataContextMock.GetArgumentsForCallsMadeOn(mock =>
                mock.Save(Arg<GamingGroupInvitation>.Is.Anything,
                    Arg<ApplicationUser>.Is.Anything));

            var actualInvitation = args.AssertFirstCallIsType<GamingGroupInvitation>();
            actualInvitation.PlayerId.ShouldBe(_playerInvitation.InvitedPlayerId);
            actualInvitation.GamingGroupId.ShouldBe(_playerInvitation.GamingGroupId);
            actualInvitation.DateSent.Date.ShouldBe(DateTime.UtcNow.Date);
            actualInvitation.InviteeEmail.ShouldBe(_playerInvitation.InvitedPlayerEmail);
            actualInvitation.InvitingUserId.ShouldBe(_currentUser.Id);

            var actualUser = args.AssertFirstCallIsType<ApplicationUser>(1);
            actualUser.ShouldBeSameAs(_currentUser);
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
        public void It_Emails_The_User_Including_The_Users_Custom_Comment()
        {
            _playerInviter.InvitePlayer(_playerInvitation, _currentUser);

            var args = _emailServiceMock.GetArgumentsForCallsMadeOn(mock => mock.SendAsync(Arg<IdentityMessage>.Is.Anything));
            var actualIdentityMessage = args.AssertFirstCallIsType<IdentityMessage>();
            actualIdentityMessage.Subject.ShouldBe(_playerInvitation.EmailSubject);
            //--too painful to get it to line up character for character
            actualIdentityMessage.Body.ShouldContain($"Well hello there! You've been invited by '{ _currentUser.UserName}' to join the NemeStats Gaming Group called '{ _gamingGroup.Name}'!");
            actualIdentityMessage.Body.ShouldContain($"To join this Gaming Group, click on the following link: {_rootUrl}/Account/ConsumeInvitation/{_gamingGroupInvitation.Id} <br/><br/>");
            actualIdentityMessage.Body.ShouldContain($"{ _currentUser.UserName} says: { _playerInvitation.CustomEmailMessage} <br/><br/>");
            actualIdentityMessage.Body.ShouldContain("If you believe you've received this in error just disregard the email.");
            actualIdentityMessage.Destination.ShouldBe(_playerInvitation.InvitedPlayerEmail);
        }

        [Test]
        public void It_Emails_The_User_With_A_Different_Message_When_There_Isnt_A_Custom_Message()
        {
            _playerInvitation.CustomEmailMessage = null;

            _playerInviter.InvitePlayer(_playerInvitation, _currentUser);

            var args = _emailServiceMock.GetArgumentsForCallsMadeOn(mock => mock.SendAsync(Arg<IdentityMessage>.Is.Anything));
            var actualIdentityMessage = args.AssertFirstCallIsType<IdentityMessage>();
            actualIdentityMessage.Subject.ShouldBe(_playerInvitation.EmailSubject);
            //--simple way to test, albeit not the most thorough
            actualIdentityMessage.Body.ShouldNotContain("says: ");
        }
    }
}
