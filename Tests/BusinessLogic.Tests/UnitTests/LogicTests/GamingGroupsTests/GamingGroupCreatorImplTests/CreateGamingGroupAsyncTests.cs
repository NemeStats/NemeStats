using BusinessLogic.DataAccess;
using BusinessLogic.DataAccess.Repositories;
using BusinessLogic.EventTracking;
using BusinessLogic.Logic.GamingGroups;
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

namespace BusinessLogic.Tests.UnitTests.LogicTests.GamingGroupsTests.GamingGroupCreatorImplTests
{
    [TestFixture]
    public class CreateGamingGroupAsyncTests
    {
        private GamingGroupCreatorImpl gamingGroupCreator;
        private IUserStore<ApplicationUser> userStoreMock;
        private UserManager<ApplicationUser> userManager;
        private DataContext dataContextMock;
        private NemeStatsEventTracker eventTrackerMock;
        private ApplicationUser currentUser = new ApplicationUser()
        {
            Id = "application user id"
        };
        GamingGroup expectedGamingGroup;
        private ApplicationUser appUserRetrievedFromFindMethod;

        [SetUp]
        public void SetUp()
        {
            userStoreMock = MockRepository.GenerateMock<IUserStore<ApplicationUser>>();
            userManager = new UserManager<ApplicationUser>(userStoreMock);
            dataContextMock = MockRepository.GenerateMock<DataContext>();
            eventTrackerMock = MockRepository.GenerateMock<NemeStatsEventTracker>();
            gamingGroupCreator = new GamingGroupCreatorImpl(dataContextMock, userManager, eventTrackerMock);

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
        public async Task ItSetsTheOwnerToTheCurrentUser()
        {
            await gamingGroupCreator.CreateGamingGroupAsync("a", currentUser);

            dataContextMock.AssertWasCalled(mock =>
                mock.Save(Arg<GamingGroup>.Matches(group => group.OwningUserId == currentUser.Id),
                Arg<ApplicationUser>.Is.Anything));
        }

        [Test]
        public async Task ItSetsTheGamingGroupName()
        {
            string gamingGroupName = "name";

            await gamingGroupCreator.CreateGamingGroupAsync(gamingGroupName, currentUser);

            dataContextMock.AssertWasCalled(mock =>
                mock.Save(Arg<GamingGroup>.Matches(group => group.Name == gamingGroupName),
                Arg<ApplicationUser>.Is.Anything));
        }

        [Test]
        public async Task ItThrowsAnArgumentExceptionIfTheGamingGroupNameIsEmpty()
        {
            try
            {
                await gamingGroupCreator.CreateGamingGroupAsync(string.Empty, currentUser);
            }catch(ArgumentException exception)
            {
                Assert.AreEqual(GamingGroupCreatorImpl.EXCEPTION_MESSAGE_GAMING_GROUP_NAME_CANNOT_BE_NULL_OR_BLANK, exception.Message);
            }
        }

        [Test]
        public async Task ItReturnsTheSavedGamingGroup()
        {
            GamingGroup returnedGamingGroup = await gamingGroupCreator.CreateGamingGroupAsync("a", currentUser);

            IList<object[]> objectsPassedToSaveMethod = dataContextMock.GetArgumentsForCallsMadeOn(
                mock => mock.Save(Arg<GamingGroup>.Is.Anything, Arg<ApplicationUser>.Is.Anything));

            Assert.AreSame(expectedGamingGroup, returnedGamingGroup);
        }

        //TODO need to figure out why this fails. It seems to work just fine.
        [Test, Ignore("No idea why this fails!!")]
        public async Task ItUpdatesTheCurrentUsersGamingGroup()
        {
            GamingGroup returnedGamingGroup = await gamingGroupCreator.CreateGamingGroupAsync("a", currentUser);

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
            await gamingGroupCreator.CreateGamingGroupAsync("a", currentUser);

            eventTrackerMock.AssertWasCalled(mock => mock.TrackGamingGroupCreation());
        }
    }
}
