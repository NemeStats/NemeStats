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
using BusinessLogic.Logic.Users;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using StructureMap.AutoMocking;

namespace BusinessLogic.Tests.UnitTests.LogicTests.UsersTests.GamingGroupContextSwitcherTests
{
    public class SwitchGamingGroupContextTests
    {
        protected RhinoAutoMocker<GamingGroupContextSwitcher> AutoMocker;
        protected ApplicationUser CurrentUser;
        protected ApplicationUser RetrievedUser;
        protected List<UserGamingGroup> UserGamingGroups;
        protected int GamingGroupIdUserCanSee = 1;
        protected int GamingGroupidUserCannotSee = 2;
            
        [SetUp]
        public void SetUp()
        {
            AutoMocker = new RhinoAutoMocker<GamingGroupContextSwitcher>();

            CurrentUser = new ApplicationUser
            {
                Id = "user id",
                CurrentGamingGroupId = 777
            };
            RetrievedUser = new ApplicationUser
            {
                Id = "user id"
            };

            UserGamingGroups = new List<UserGamingGroup>
            {
                new UserGamingGroup
                {
                    Id = 1,
                    ApplicationUserId = CurrentUser.Id,
                    GamingGroupId = GamingGroupIdUserCanSee
                },
                new UserGamingGroup
                {
                    Id = 2,
                    ApplicationUserId = "some other id the user cant see",
                    GamingGroupId = GamingGroupIdUserCanSee
                }
            };

            AutoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<UserGamingGroup>())
                           .Return(UserGamingGroups.AsQueryable());
            AutoMocker.Get<IDataContext>().Expect(mock => mock.FindById<ApplicationUser>(CurrentUser.Id))
                           .Return(RetrievedUser);
        }

        [Test]
        public void ItValidatesThatTheUserHasAccessToThisGamingGroup()
        {
            int gamingGroupTheUserCantSee = -999;
            string expectedMessage = string.Format(GamingGroupContextSwitcher.EXCEPTION_MESSAGE_NO_ACCESS, CurrentUser.Id, gamingGroupTheUserCantSee);

            var exception = Assert.Throws<UnauthorizedAccessException>(() => AutoMocker.ClassUnderTest.SwitchGamingGroupContext(gamingGroupTheUserCantSee, CurrentUser));

            Assert.That(exception.Message, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void ItDoesNotSaveIfTheUserIsAlreadySetToThatGamingGroup()
        {
            AutoMocker.ClassUnderTest.SwitchGamingGroupContext(CurrentUser.CurrentGamingGroupId, CurrentUser);

            AutoMocker.Get<IDataContext>().AssertWasNotCalled(mock => mock.Save(Arg<ApplicationUser>.Is.Anything, Arg<ApplicationUser>.Is.Anything));
        }

        [Test]
        public void ItUpdatesTheUsersGamingGroup()
        {
            AutoMocker.ClassUnderTest.SwitchGamingGroupContext(GamingGroupIdUserCanSee, CurrentUser);

            AutoMocker.Get<IDataContext>().AssertWasCalled(mock => mock.Save(
                Arg<ApplicationUser>.Matches(user => user.CurrentGamingGroupId == GamingGroupIdUserCanSee && user.Id == CurrentUser.Id), 
                Arg<ApplicationUser>.Is.Same(CurrentUser)));
        }
    }
}
