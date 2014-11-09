using BusinessLogic.DataAccess;
using BusinessLogic.EventTracking;
using BusinessLogic.Logic.GameDefinitions;
using BusinessLogic.Logic.GamingGroups;
using BusinessLogic.Logic.Players;
using BusinessLogic.Logic.Users;
using BusinessLogic.Models.User;
using Microsoft.AspNet.Identity;
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
            applicationUserManagerMock = MockRepository.GenerateMock<ApplicationUserManager>(userStoreMock);
            dataContextMock = MockRepository.GenerateMock<IDataContext>();
            eventTrackerMock = MockRepository.GenerateMock<INemeStatsEventTracker>();
            gamingGroupSaver = new GamingGroupSaver(
                dataContextMock,
                applicationUserManagerMock,
                eventTrackerMock);
        }
    }
}
