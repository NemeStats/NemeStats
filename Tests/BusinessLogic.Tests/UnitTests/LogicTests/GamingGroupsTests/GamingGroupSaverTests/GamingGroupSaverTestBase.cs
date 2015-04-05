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
using BusinessLogic.DataAccess;
using BusinessLogic.EventTracking;
using BusinessLogic.Logic.GameDefinitions;
using BusinessLogic.Logic.GamingGroups;
using BusinessLogic.Logic.Players;
using BusinessLogic.Logic.Users;
using BusinessLogic.Models.User;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security.DataProtection;
using NUnit.Framework;
using Rhino.Mocks;
using System.Linq;

namespace BusinessLogic.Tests.UnitTests.LogicTests.GamingGroupsTests.GamingGroupSaverTests
{
    public class GamingGroupSaverTestBase
    {
        protected GamingGroupSaver gamingGroupSaver;
        protected IUserStore<ApplicationUser> userStoreMock;
        protected ApplicationUserManager applicationUserManagerMock;
        protected IDataContext dataContextMock;
        protected INemeStatsEventTracker eventTrackerMock;
        protected IPlayerSaver playerSaverMock;
        protected IDataProtectionProvider dataProtectionProviderMock;
        protected ApplicationUser currentUser = new ApplicationUser()
        {
            Id = "application user id",
            CurrentGamingGroupId = 1235
        };
        protected string gamingGroupName = "gaming group name";

        [SetUp]
        public virtual void SetUp()
        {
            userStoreMock = MockRepository.GenerateMock<IUserStore<ApplicationUser>>();
            var dataProtector = MockRepository.GenerateMock<IDataProtector>();
            dataProtectionProviderMock = MockRepository.GenerateMock<IDataProtectionProvider>();
            dataProtectionProviderMock.Expect(mock => mock.Create(Arg<string>.Is.Anything)).Return(dataProtector);
            applicationUserManagerMock = MockRepository.GenerateMock<ApplicationUserManager>(userStoreMock, dataProtectionProviderMock);
            dataContextMock = MockRepository.GenerateMock<IDataContext>();
            playerSaverMock = MockRepository.GenerateMock<IPlayerSaver>();
            eventTrackerMock = MockRepository.GenerateMock<INemeStatsEventTracker>();
            gamingGroupSaver = new GamingGroupSaver(
                dataContextMock,
                eventTrackerMock,
                playerSaverMock);
        }
    }
}
