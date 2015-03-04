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
using BusinessLogic.DataAccess.GamingGroups;
using BusinessLogic.Logic.GamingGroups;
using BusinessLogic.Logic.Users;
using BusinessLogic.Models;
using BusinessLogic.Models.User;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security.DataProtection;
using NUnit.Framework;
using Rhino.Mocks;

namespace BusinessLogic.Tests.UnitTests.LogicTests.UsersTests.GamingGroupInviteConsumerTests
{
    public class GamingGroupInviteConsumerTestBase
    {
        protected IPendingGamingGroupInvitationRetriever pendingGamingGroupInvitationRetriever;
        protected IUserStore<ApplicationUser> userStoreMock;
        protected IDataContext dataContextMock;
        protected ApplicationUserManager applicationUserManagerMock;
        protected GamingGroupInviteConsumer gamingGroupInviteConsumer;
        protected IGamingGroupAccessGranter gamingGroupAccessGranter;
        protected IDataProtectionProvider dataProtectionProviderMock;
        protected List<GamingGroupInvitation> gamingGroupInvitations;
        protected ApplicationUser currentUser;

        [SetUp]
        public virtual void SetUp()
        {
            pendingGamingGroupInvitationRetriever = MockRepository.GenerateMock<IPendingGamingGroupInvitationRetriever>();
            var dataProtector = MockRepository.GenerateMock<IDataProtector>();
            dataProtectionProviderMock = MockRepository.GenerateMock<IDataProtectionProvider>();
            dataProtectionProviderMock.Expect(mock => mock.Create(Arg<string>.Is.Anything)).Return(dataProtector);
            userStoreMock = MockRepository.GenerateMock<IUserStore<ApplicationUser>>();
            applicationUserManagerMock = MockRepository.GenerateMock<ApplicationUserManager>(userStoreMock, dataProtectionProviderMock);
            gamingGroupAccessGranter = MockRepository.GenerateMock<IGamingGroupAccessGranter>();
            dataContextMock = MockRepository.GenerateMock<IDataContext>();
            this.gamingGroupInviteConsumer = new GamingGroupInviteConsumer(
                pendingGamingGroupInvitationRetriever, 
                applicationUserManagerMock, 
                gamingGroupAccessGranter,
                dataContextMock);
            currentUser = new ApplicationUser()
            {
                Id = "user id"
            };
            gamingGroupInvitations = new List<GamingGroupInvitation>();
        }
    }
}
