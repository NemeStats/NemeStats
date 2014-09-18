using BusinessLogic.DataAccess;
using BusinessLogic.EventTracking;
using BusinessLogic.Logic.GameDefinitions;
using BusinessLogic.Logic.GamingGroups;
using BusinessLogic.Logic.Players;
using BusinessLogic.Models;
using BusinessLogic.Models.User;
using Microsoft.AspNet.Identity;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Tests.UnitTests.LogicTests.GameDefinitionsTests.GamingGroupsTests.GamingGroupSaverTests
{
    public class GamingGroupSaverTestBase
    {
        protected GamingGroupSaver gamingGroupSaver;
        protected IUserStore<ApplicationUser> userStoreMock;
        protected UserManager<ApplicationUser> userManager;
        protected IDataContext dataContextMock;
        protected NemeStatsEventTracker eventTrackerMock;
        protected IPlayerSaver playerSaverMock;
        protected IGameDefinitionSaver gameDefinitionCreator;
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
            userManager = new UserManager<ApplicationUser>(userStoreMock);
            dataContextMock = MockRepository.GenerateMock<IDataContext>();
            eventTrackerMock = MockRepository.GenerateMock<NemeStatsEventTracker>();
            playerSaverMock = MockRepository.GenerateMock<IPlayerSaver>();
            gameDefinitionCreator = MockRepository.GenerateMock<IGameDefinitionSaver>();
            gamingGroupSaver = new GamingGroupSaver(
                dataContextMock,
                userManager,
                eventTrackerMock,
                playerSaverMock,
                gameDefinitionCreator);
        }
    }
}
