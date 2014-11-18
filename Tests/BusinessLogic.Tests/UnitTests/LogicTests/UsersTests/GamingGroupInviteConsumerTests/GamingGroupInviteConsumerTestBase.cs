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
        protected List<GamingGroupInvitation> gamingGroupInvitations;
        protected ApplicationUser currentUser;

        [SetUp]
        public virtual void SetUp()
        {
            pendingGamingGroupInvitationRetriever = MockRepository.GenerateMock<IPendingGamingGroupInvitationRetriever>();
            userStoreMock = MockRepository.GenerateMock<IUserStore<ApplicationUser>>();
            applicationUserManagerMock = MockRepository.GenerateMock<ApplicationUserManager>(userStoreMock);
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
