using BusinessLogic.Models;
using BusinessLogic.Models.User;
using Microsoft.AspNet.Identity;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BusinessLogic.Tests.UnitTests.LogicTests.GamingGroupsTests.GamingGroupSaverTests
{
    [TestFixture]
    public class CreateGamingGroupAsyncTests : GamingGroupSaverTestBase
    {
        protected GamingGroup expectedGamingGroup;
        protected UserGamingGroup expectedUserGamingGroup;
        protected ApplicationUser appUserRetrievedFromFindMethod;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            expectedGamingGroup = new GamingGroup() { Id = currentUser.CurrentGamingGroupId.Value };
            dataContextMock.Expect(mock => mock.Save(Arg<GamingGroup>.Is.Anything, Arg<ApplicationUser>.Is.Anything))
                .Return(expectedGamingGroup);
            expectedUserGamingGroup = new UserGamingGroup
            {
                ApplicationUserId = currentUser.Id,
                GamingGroupId = expectedGamingGroup.Id
            };
            dataContextMock.Expect(mock => mock.Save<UserGamingGroup>(Arg<UserGamingGroup>.Is.Anything, Arg<ApplicationUser>.Is.Anything))
                           .Return(expectedUserGamingGroup);
            appUserRetrievedFromFindMethod = new ApplicationUser()
            {
                Id = currentUser.Id
            };
            applicationUserManagerMock.Expect(mock => mock.FindByIdAsync(currentUser.Id))
                .Repeat.Once()
                .Return(Task.FromResult(appUserRetrievedFromFindMethod));
            applicationUserManagerMock.Expect(mock => mock.UpdateAsync(appUserRetrievedFromFindMethod))
                 .Return(Task.FromResult(new IdentityResult()));
        }

        [Test]
        public async Task ItThrowsAnArgumentNullExceptionIfGamingGroupNameIsNull()
        {
            ArgumentNullException expectedException = new ArgumentNullException("gamingGroupName");
            try
            {
                await gamingGroupSaver.CreateNewGamingGroup(null, currentUser);
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
                await gamingGroupSaver.CreateNewGamingGroup("   ", currentUser);
            }
            catch (ArgumentNullException exception)
            {
                Assert.AreEqual(expectedException.Message, exception.Message);
            }
        }

        [Test]
        public async Task ItSetsTheOwnerToTheCurrentUser()
        {
            await gamingGroupSaver.CreateNewGamingGroup(gamingGroupName, currentUser);

            dataContextMock.AssertWasCalled(mock =>
                mock.Save(Arg<GamingGroup>.Matches(group => group.OwningUserId == currentUser.Id),
                Arg<ApplicationUser>.Is.Anything));
        }

        [Test]
        public async Task ItSetsTheGamingGroupName()
        {
            await gamingGroupSaver.CreateNewGamingGroup(gamingGroupName, currentUser);

            dataContextMock.AssertWasCalled(mock =>
                mock.Save(Arg<GamingGroup>.Matches(group => group.Name == gamingGroupName),
                Arg<ApplicationUser>.Is.Anything));
        }

        [Test]
        public async Task ItReturnsTheSavedGamingGroup()
        {
            GamingGroup returnedGamingGroup = await gamingGroupSaver.CreateNewGamingGroup(gamingGroupName, currentUser);

            IList<object[]> objectsPassedToSaveMethod = dataContextMock.GetArgumentsForCallsMadeOn(
                mock => mock.Save(Arg<GamingGroup>.Is.Anything, Arg<ApplicationUser>.Is.Anything));

            Assert.AreSame(expectedGamingGroup, returnedGamingGroup);
        }

        public async Task ItAssociatesTheUserWithTheGamingGroup()
        {
            await gamingGroupSaver.CreateNewGamingGroup(gamingGroupName, currentUser);

            dataContextMock.AssertWasCalled(mock => mock.Save<UserGamingGroup>(
                Arg<UserGamingGroup>.Matches(ugg => ugg.ApplicationUserId == currentUser.Id
                    && ugg.GamingGroupId == expectedGamingGroup.Id),
                Arg<ApplicationUser>.Is.Anything));
        }

        [Test]
        public async Task ItUpdatesTheCurrentUsersGamingGroup()
        {
            GamingGroup returnedGamingGroup = await gamingGroupSaver.CreateNewGamingGroup(gamingGroupName, currentUser);

            applicationUserManagerMock.AssertWasCalled(mock => mock.UpdateAsync(Arg<ApplicationUser>.Matches(
                user => user.CurrentGamingGroupId == expectedGamingGroup.Id 
                    && user.Id == appUserRetrievedFromFindMethod.Id)));
        }

        [Test]
        public async Task ItCreatesANewPlayerNamedAfterTheUserName()
        {
            GamingGroup returnedGamingGroup = await gamingGroupSaver.CreateNewGamingGroup(gamingGroupName, currentUser);

            playerSaverMock.AssertWasCalled(mock => mock.Save(Arg<Player>.Matches(
                                        player => player.Name == currentUser.UserName
                                            && player.ApplicationUserId == currentUser.Id
                                            && player.Active), 
                                            Arg<ApplicationUser>.Is.Same(currentUser)));
        }

        [Test]
        public async Task ItTracksTheGamingGroupCreation()
        {
            GamingGroup expectedGamingGroup = new GamingGroup() { Id = 123 };
            dataContextMock.Expect(mock => mock.Save(Arg<GamingGroup>.Is.Anything, Arg<ApplicationUser>.Is.Anything))
                .Repeat.Once()
                .Return(expectedGamingGroup);

            await gamingGroupSaver.CreateNewGamingGroup(gamingGroupName, currentUser);
            eventTrackerMock.AssertWasCalled(mock => mock.TrackGamingGroupCreation());
        }
    }
}
