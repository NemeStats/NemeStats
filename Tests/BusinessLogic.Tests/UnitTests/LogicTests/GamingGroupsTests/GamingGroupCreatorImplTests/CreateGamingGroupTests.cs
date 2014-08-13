using BusinessLogic.DataAccess;
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

namespace BusinessLogic.Tests.UnitTests.LogicTests.GamingGroupsTests.GamingGroupCreatorImplTests
{
    [TestFixture, Ignore("Freezing up on async calls. Need to figure this out. and re-enable tests.")]
    public class CreateGamingGroupTests
    {
        private GamingGroupCreatorImpl gamingGroupCreator;
        private IUserStore<ApplicationUser> userStoreMock;
        private UserManager<ApplicationUser> userManager;
        private DataContext dataContext;
        private ApplicationUser currentUser;
        ApplicationUser appUser;

        [SetUp]
        public void SetUp()
        {
            userStoreMock = MockRepository.GenerateMock<IUserStore<ApplicationUser>>();
            userManager = new UserManager<ApplicationUser>(userStoreMock);
            dataContext = MockRepository.GenerateMock<DataContext>();
            gamingGroupCreator = new GamingGroupCreatorImpl(dataContext, userManager);
            currentUser = new ApplicationUser()
            {
                Id = "application user id"
            };

            appUser = new ApplicationUser()
            {
                Id = currentUser.Id
            };
            Task<ApplicationUser> task = new Task<ApplicationUser>(() => appUser);
            userStoreMock.Expect(mock => mock.FindByIdAsync(currentUser.Id))
                .Repeat.Once()
                .Return(task);
        }

        [Test]
        public async Task ItSetsTheOwnerToTheCurrentUser()
        {
            await gamingGroupCreator.CreateGamingGroupAsync("a", currentUser);

            dataContext.AssertWasCalled(mock =>
                mock.Save(Arg<GamingGroup>.Matches(group => group.OwningUserId == currentUser.Id),
                Arg<ApplicationUser>.Is.Anything));
        }

        [Test]
        public async Task ItSetsTheGamingGroupName()
        {
            string gamingGroupName = "name";

            await gamingGroupCreator.CreateGamingGroupAsync(gamingGroupName, currentUser);

            dataContext.AssertWasCalled(mock =>
                mock.Save(Arg<GamingGroup>.Matches(group => group.Name == gamingGroupName),
                Arg<ApplicationUser>.Is.Anything));
        }

        [Test]
        public void ItThrowsAnArgumentExceptionIfTheGamingGroupNameIsEmpty()
        {
            Exception exception = Assert.Throws<ArgumentException>(() => gamingGroupCreator.CreateGamingGroupAsync(string.Empty, currentUser).RunSynchronously());

            Assert.AreEqual(GamingGroupCreatorImpl.EXCEPTION_MESSAGE_GAMING_GROUP_NAME_CANNOT_BE_NULL_OR_BLANK, exception.Message);
        }

        [Test]
        public async Task ItReturnsTheSavedGamingGroup()
        {
            GamingGroup expectedGamingGroup = new GamingGroup();
            dataContext.Expect(mock => mock.Save(Arg<GamingGroup>.Is.Anything, Arg<ApplicationUser>.Is.Anything))
                .Repeat.Once()
                .Return(expectedGamingGroup);

            GamingGroup returnedGamingGroup = await gamingGroupCreator.CreateGamingGroupAsync("a", currentUser);

            IList<object[]> objectsPassedToSaveMethod = dataContext.GetArgumentsForCallsMadeOn(
                mock => mock.Save(Arg<GamingGroup>.Is.Anything, Arg<ApplicationUser>.Is.Anything));

            Assert.AreSame(expectedGamingGroup, returnedGamingGroup);
        }

        [Test]
        public async Task ItUpdatesTheCurrentUsersGamingGroup()
        {
            GamingGroup expectedGamingGroup = new GamingGroup() { Id = 123 };

            dataContext.Expect(mock => mock.Save(Arg<GamingGroup>.Is.Anything, Arg<ApplicationUser>.Is.Anything))
                .Repeat.Once()
                .Return(expectedGamingGroup);
            await gamingGroupCreator.CreateGamingGroupAsync("a", currentUser);

            userStoreMock.AssertWasCalled(mock => mock.UpdateAsync(Arg<ApplicationUser>.Matches(user => user.CurrentGamingGroupId == expectedGamingGroup.Id && user == appUser)));
        }
    }
}
