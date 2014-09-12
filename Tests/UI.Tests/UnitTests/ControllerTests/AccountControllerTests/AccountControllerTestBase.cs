using BusinessLogic.EventTracking;
using BusinessLogic.Logic.Users;
using BusinessLogic.Models.User;
using Microsoft.AspNet.Identity;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using UI.Controllers;
using UI.Models;

namespace UI.Tests.UnitTests.ControllerTests.AccountControllerTests
{
    [TestFixture]
    public class AccountControllerTestBase
    {
        protected UserManager<ApplicationUser> userManager;
        protected IUserStore<ApplicationUser> userStoreMock;
        protected IGamingGroupInviteConsumer gamingGroupInviteConsumerMock;
        protected NemeStatsEventTracker eventTrackerMock;
        protected AccountController accountControllerPartialMock;
        //protected ModelStateDictionary modelStateDictionaryMock;
        protected RegisterViewModel registerViewModel;
        protected ApplicationUser newApplicationUser;

        [SetUp]
        public void SetUp()
        {
            userStoreMock = MockRepository.GenerateMock<IUserStore<ApplicationUser>>();
            gamingGroupInviteConsumerMock = MockRepository.GenerateMock<IGamingGroupInviteConsumer>();
            eventTrackerMock = MockRepository.GenerateMock<NemeStatsEventTracker>();
            userManager = new UserManager<ApplicationUser>(userStoreMock);
            accountControllerPartialMock = MockRepository.GeneratePartialMock<AccountController>(userManager, gamingGroupInviteConsumerMock, eventTrackerMock);
           // modelStateDictionaryMock = MockRepository.GenerateMock<ModelStateDictionary>();
            newApplicationUser = new ApplicationUser()
            {
                Id = "new application user"
            };
            registerViewModel = new RegisterViewModel()
            {
                ConfirmPassword = "confirm password",
                Password = "password",
                UserName = "user name",
                EmailAddress = "email@email.com"
            };
        }
    }
}
