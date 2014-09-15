using BusinessLogic.DataAccess;
using BusinessLogic.DataAccess.Repositories;
using BusinessLogic.EventTracking;
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
using UniversalAnalyticsHttpWrapper;
using BusinessLogic.Logic.GameDefinitions;

namespace BusinessLogic.Tests.UnitTests.LogicTests.GamingGroupsTests.GamingGroupCreatorTests
{
    [TestFixture]
    public class CreateGamingGroupAsyncTests
    {
        private GamingGroupCreator gamingGroupCreator;
        private IUserStore<ApplicationUser> userStoreMock;
        private UserManager<ApplicationUser> userManager;
        private IDataContext dataContextMock;
        private NemeStatsEventTracker eventTrackerMock;
        private IPlayerSaver playerSaverMock;
        private IGameDefinitionSaver gameDefinitionCreator;
        private ApplicationUser currentUser = new ApplicationUser()
        {
            Id = "application user id"
        };
        private string gamingGroupName = "gaming group name";
        private GamingGroup expectedGamingGroup;
        private ApplicationUser appUserRetrievedFromFindMethod;

        [SetUp]
        public void SetUp()
        {
            userStoreMock = MockRepository.GenerateMock<IUserStore<ApplicationUser>>();
            userManager = new UserManager<ApplicationUser>(userStoreMock);
            dataContextMock = MockRepository.GenerateMock<IDataContext>();
            eventTrackerMock = MockRepository.GenerateMock<NemeStatsEventTracker>();
            playerSaverMock = MockRepository.GenerateMock<IPlayerSaver>();
            gameDefinitionCreator = MockRepository.GenerateMock<IGameDefinitionSaver>();
            gamingGroupCreator = new GamingGroupCreator(
                dataContextMock, 
                userManager, 
                eventTrackerMock, 
                playerSaverMock,
                gameDefinitionCreator);

            expectedGamingGroup = new GamingGroup() { Id = 123 };
            dataContextMock.Expect(mock => mock.Save(Arg<GamingGroup>.Is.Anything, Arg<ApplicationUser>.Is.Anything))
                .Repeat.Once()
                .Return(expectedGamingGroup);

            appUserRetrievedFromFindMethod = new ApplicationUser()
            {
                Id = currentUser.Id
            };
            userStoreMock.Expect(mock => mock.FindByIdAsync(currentUser.Id))
                .Repeat.Once()
                .Return(Task.FromResult(appUserRetrievedFromFindMethod));
            userStoreMock.Expect(mock => mock.UpdateAsync(appUserRetrievedFromFindMethod))
                 .Return(Task.FromResult(new IdentityResult()));
        }

        [Test]
        public async Task ItThrowsAnArgumentNullExceptionIfGamingGroupNameIsNull()
        {
            ArgumentNullException expectedException = new ArgumentNullException("gamingGroupName");
            try
            {
                await gamingGroupCreator.CreateNewGamingGroup(null, currentUser);
            }
            catch (ArgumentNullException exception)
            {
                Assert.AreEqual(expectedException.Message, exception.Message);
            }
        }

        [Test]
        public async Task ItThrowsAnArgumentNullExceptionIfGamingGroupNameIsWhiteSpace()
        {
            ArgumentNullException expectedException = new ArgumentNullException("gamingGroupName");
            try
            {
                await gamingGroupCreator.CreateNewGamingGroup("   ", currentUser);
            }
            catch (ArgumentNullException exception)
            {
                Assert.AreEqual(expectedException.Message, exception.Message);
            }
        }

        [Test]
        public async Task ItSetsTheOwnerToTheCurrentUser()
        {
            await gamingGroupCreator.CreateNewGamingGroup(gamingGroupName, currentUser);

            dataContextMock.AssertWasCalled(mock =>
                mock.Save(Arg<GamingGroup>.Matches(group => group.OwningUserId == currentUser.Id),
                Arg<ApplicationUser>.Is.Anything));
        }

        [Test]
        public async Task ItSetsTheGamingGroupName()
        {
            await gamingGroupCreator.CreateNewGamingGroup(gamingGroupName, currentUser);

            dataContextMock.AssertWasCalled(mock =>
                mock.Save(Arg<GamingGroup>.Matches(group => group.Name == gamingGroupName),
                Arg<ApplicationUser>.Is.Anything));
        }

        [Test]
        public async Task ItReturnsTheSavedGamingGroup()
        {
            GamingGroup returnedGamingGroup = await gamingGroupCreator.CreateNewGamingGroup(gamingGroupName, currentUser);

            IList<object[]> objectsPassedToSaveMethod = dataContextMock.GetArgumentsForCallsMadeOn(
                mock => mock.Save(Arg<GamingGroup>.Is.Anything, Arg<ApplicationUser>.Is.Anything));

            Assert.AreSame(expectedGamingGroup, returnedGamingGroup);
        }

        
        public async Task ItUpdatesTheCurrentUsersGamingGroup()
        {
            GamingGroup returnedGamingGroup = await gamingGroupCreator.CreateNewGamingGroup(gamingGroupName, currentUser);

            userStoreMock.AssertWasCalled(mock => mock.UpdateAsync(Arg<ApplicationUser>.Matches(
                user => user.CurrentGamingGroupId == expectedGamingGroup.Id 
                    && user.Id == appUserRetrievedFromFindMethod.Id)));
        }

        [Test]
        public async Task ItTracksTheGamingGroupCreation()
        {
            GamingGroup expectedGamingGroup = new GamingGroup() { Id = 123 };
            dataContextMock.Expect(mock => mock.Save(Arg<GamingGroup>.Is.Anything, Arg<ApplicationUser>.Is.Anything))
                .Repeat.Once()
                .Return(expectedGamingGroup);

            await gamingGroupCreator.CreateNewGamingGroup(gamingGroupName, currentUser);
            eventTrackerMock.AssertWasCalled(mock => mock.TrackGamingGroupCreation());
        }
    }
}
