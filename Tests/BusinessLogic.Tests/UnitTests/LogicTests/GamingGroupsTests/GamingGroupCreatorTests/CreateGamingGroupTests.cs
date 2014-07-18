using BusinessLogic.DataAccess.Repositories;
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

namespace BusinessLogic.Tests.UnitTests.LogicTests.GamingGroupsTests.GamingGroupCreatorTests
{
    [TestFixture, Ignore("Freezing up on async calls. Need to figure this out. and re-enable tests.")]
    public class CreateGamingGroupTests
    {
        private GamingGroupRepository gamingGroupRepositoryMock;
        private GamingGroupCreatorImpl gamingGroupCreator;
        private IUserStore<ApplicationUser> userStoreMock;
        private UserManager<ApplicationUser> userManager;
        private UserContext userContext;
        ApplicationUser appUser;

        [SetUp]
        public void SetUp()
        {
            gamingGroupRepositoryMock = MockRepository.GenerateMock<GamingGroupRepository>();
            userStoreMock = MockRepository.GenerateMock<IUserStore<ApplicationUser>>();
            userManager = new UserManager<ApplicationUser>(userStoreMock);
            gamingGroupCreator = new GamingGroupCreatorImpl(gamingGroupRepositoryMock, userManager);
            userContext = new UserContext()
            {
                ApplicationUserId = "application user id"
            };

            appUser = new ApplicationUser()
            {
                Id = userContext.ApplicationUserId
            };
            Task<ApplicationUser> task = new Task<ApplicationUser>(() => appUser);
            userStoreMock.Expect(mock => mock.FindByIdAsync(userContext.ApplicationUserId))
                .Repeat.Once()
                .Return(task);
        }

        [Test]
        public async Task ItSetsTheOwnerToTheCurrentUser()
        {
            await gamingGroupCreator.CreateGamingGroupAsync("a", userContext);

            gamingGroupRepositoryMock.AssertWasCalled(mock =>
                mock.Save(Arg<GamingGroup>.Matches(group => group.OwningUserId == userContext.ApplicationUserId),
                Arg<UserContext>.Is.Anything));
        }

        [Test]
        public async Task ItSetsTheGamingGroupName()
        {
            string gamingGroupName = "name";

            await gamingGroupCreator.CreateGamingGroupAsync(gamingGroupName, userContext);

            gamingGroupRepositoryMock.AssertWasCalled(mock =>
                mock.Save(Arg<GamingGroup>.Matches(group => group.Name == gamingGroupName),
                Arg<UserContext>.Is.Anything));
        }

        [Test]
        public void ItThrowsAnArgumentExceptionIfTheGamingGroupNameIsEmpty()
        {
            Exception exception = Assert.Throws<ArgumentException>(() => gamingGroupCreator.CreateGamingGroupAsync(string.Empty, userContext).RunSynchronously());

            Assert.AreEqual(GamingGroupCreatorImpl.EXCEPTION_MESSAGE_GAMING_GROUP_NAME_CANNOT_BE_NULL_OR_BLANK, exception.Message);
        }

        [Test]
        public async Task ItReturnsTheSavedGamingGroup()
        {
            GamingGroup expectedGamingGroup = new GamingGroup();
            gamingGroupRepositoryMock.Expect(mock => mock.Save(Arg<GamingGroup>.Is.Anything, Arg<UserContext>.Is.Anything))
                .Repeat.Once()
                .Return(expectedGamingGroup);

            GamingGroup returnedGamingGroup = await gamingGroupCreator.CreateGamingGroupAsync("a", userContext);

            IList<object[]> objectsPassedToSaveMethod = gamingGroupRepositoryMock.GetArgumentsForCallsMadeOn(
                mock => mock.Save(Arg<GamingGroup>.Is.Anything, Arg<UserContext>.Is.Anything));

            Assert.AreSame(expectedGamingGroup, returnedGamingGroup);
        }

        [Test]
        public async Task ItUpdatesTheCurrentUsersGamingGroup()
        {
            GamingGroup expectedGamingGroup = new GamingGroup() { Id = 123 };

            gamingGroupRepositoryMock.Expect(mock => mock.Save(Arg<GamingGroup>.Is.Anything, Arg<UserContext>.Is.Anything))
                .Repeat.Once()
                .Return(expectedGamingGroup);
            await gamingGroupCreator.CreateGamingGroupAsync("a", userContext);

            userStoreMock.AssertWasCalled(mock => mock.UpdateAsync(Arg<ApplicationUser>.Matches(user => user.CurrentGamingGroupId == expectedGamingGroup.Id && user == appUser)));
        }
    }
}
