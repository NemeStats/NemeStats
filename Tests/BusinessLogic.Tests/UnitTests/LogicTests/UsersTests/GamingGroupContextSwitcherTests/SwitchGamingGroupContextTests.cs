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
using BusinessLogic.DataAccess;
using BusinessLogic.Logic.Users;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;

namespace BusinessLogic.Tests.UnitTests.LogicTests.UsersTests.GamingGroupContextSwitcherTests
{
    public class SwitchGamingGroupContextTests
    {
        protected IDataContext dataContextMock;
        protected GamingGroupContextSwitcher contextSwitcher;
        protected ApplicationUser currentUser;
        protected ApplicationUser retrievedUser;
        protected List<UserGamingGroup> userGamingGroups;
        protected int gamingGroupIdUserCanSee = 1;
        protected int gamingGroupidUserCannotSee = 2;
            
        [SetUp]
        public void SetUp()
        {
            dataContextMock = MockRepository.GenerateMock<IDataContext>();
            contextSwitcher = new GamingGroupContextSwitcher(dataContextMock);

            currentUser = new ApplicationUser
            {
                Id = "user id",
                CurrentGamingGroupId = 777
            };
            retrievedUser = new ApplicationUser
            {
                Id = "user id"
            };

            userGamingGroups = new List<UserGamingGroup>
            {
                new UserGamingGroup
                {
                    Id = 1,
                    ApplicationUserId = currentUser.Id,
                    GamingGroupId = gamingGroupIdUserCanSee
                },
                new UserGamingGroup
                {
                    Id = 2,
                    ApplicationUserId = "some other id the user cant see",
                    GamingGroupId = gamingGroupIdUserCanSee
                }
            };

            dataContextMock.Expect(mock => mock.GetQueryable<UserGamingGroup>())
                           .Return(userGamingGroups.AsQueryable());
            dataContextMock.Expect(mock => mock.FindById<ApplicationUser>(currentUser.Id))
                           .Return(retrievedUser);
        }

        [Test]
        public void ItValidatesThatTheUserHasAccessToThisGamingGroup()
        {
            int gamingGroupTheUserCantSee = -999;
            string expectedMessage = string.Format(GamingGroupContextSwitcher.EXCEPTION_MESSAGE_NO_ACCESS, currentUser.Id, gamingGroupTheUserCantSee);

            var exception = Assert.Throws<UnauthorizedAccessException>(() => contextSwitcher.SwitchGamingGroupContext(gamingGroupTheUserCantSee, currentUser));

            Assert.That(exception.Message, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void ItDoesNotSaveIfTheUserIsAlreadySetToThatGamingGroup()
        {
            contextSwitcher.SwitchGamingGroupContext(currentUser.CurrentGamingGroupId.Value, currentUser);

            dataContextMock.AssertWasNotCalled(mock => mock.Save(Arg<ApplicationUser>.Is.Anything, Arg<ApplicationUser>.Is.Anything));
        }

        [Test]
        public void ItUpdatesTheUsersGamingGroup()
        {
            contextSwitcher.SwitchGamingGroupContext(gamingGroupIdUserCanSee, currentUser);

            dataContextMock.AssertWasCalled(mock => mock.Save(
                Arg<ApplicationUser>.Matches(user => user.CurrentGamingGroupId == gamingGroupIdUserCanSee && user.Id == currentUser.Id), 
                Arg<ApplicationUser>.Is.Same(currentUser)));
        }
    }
}
