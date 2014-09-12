using BusinessLogic.DataAccess;
using BusinessLogic.DataAccess.Repositories;
using BusinessLogic.EventTracking;
using BusinessLogic.Logic.GamingGroups;
using BusinessLogic.Logic.Players;
using BusinessLogic.Models;
using BusinessLogic.Models.GamingGroups;
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
        private GamingGroupQuickStart gamingGroupQuickStart;
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
            gamingGroupQuickStart = new GamingGroupQuickStart()
            {
                GamingGroupName = "gaming group name",
                NewPlayerNames = new List<string>(),
                NewGameDefinitionNames = new List<string>()
            };
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
            await gamingGroupCreator.CreateGamingGroupAsync(gamingGroupQuickStart, currentUser);

            dataContextMock.AssertWasCalled(mock =>
                mock.Save(Arg<GamingGroup>.Matches(group => group.OwningUserId == currentUser.Id),
                Arg<ApplicationUser>.Is.Anything));
        }

        [Test]
        public async Task ItSetsTheGamingGroupName()
        {       
            await gamingGroupCreator.CreateGamingGroupAsync(gamingGroupQuickStart, currentUser);

            dataContextMock.AssertWasCalled(mock =>
                mock.Save(Arg<GamingGroup>.Matches(group => group.Name == gamingGroupQuickStart.GamingGroupName),
                Arg<ApplicationUser>.Is.Anything));
        }

        [Test]
        public async Task ItThrowsAnArgumentNullExceptionIfGamingGroupQuickStartIsNull()
        {
            gamingGroupQuickStart = null;
            ArgumentNullException expectedException = new ArgumentNullException("gamingGroupQuickStart");
            try
            {
                await gamingGroupCreator.CreateGamingGroupAsync(gamingGroupQuickStart, currentUser);
            }
            catch (ArgumentNullException exception)
            {
                Assert.AreEqual(expectedException.Message, exception.Message);
            }
        }

        [Test]
        public async Task ItThrowsAnArgumentExceptionIfTheGamingGroupQuickStartIsNull()
        {
            gamingGroupQuickStart.GamingGroupName = null;
            try
            {
                await gamingGroupCreator.CreateGamingGroupAsync(gamingGroupQuickStart, currentUser);
            }catch(ArgumentException exception)
            {
                Assert.AreEqual(GamingGroupCreator.EXCEPTION_MESSAGE_GAMING_GROUP_NAME_CANNOT_BE_NULL_OR_BLANK, exception.Message);
            }
        }

        [Test]
        public async Task ItThrowsAnArgumentExceptionIfTheGamingGroupQuickStartIsWhitespace()
        {
            gamingGroupQuickStart.GamingGroupName = "   ";
            try
            {
                await gamingGroupCreator.CreateGamingGroupAsync(gamingGroupQuickStart, currentUser);
            }
            catch (ArgumentException exception)
            {
                Assert.AreEqual(GamingGroupCreator.EXCEPTION_MESSAGE_GAMING_GROUP_NAME_CANNOT_BE_NULL_OR_BLANK, exception.Message);
            }
        }

        [Test]
        public async Task ItThrowsAnArgumentExceptionIfThePlayerNamesAreNull()
        {
            gamingGroupQuickStart.NewPlayerNames = null;
            try
            {
                await gamingGroupCreator.CreateGamingGroupAsync(gamingGroupQuickStart, currentUser);
            }
            catch (ArgumentException exception)
            {
                Assert.AreEqual(GamingGroupCreator.EXCEPTION_MESSAGE_PLAYER_NAMES_CANNOT_BE_NULL, exception.Message);
            }
        }

        [Test]
        public async Task ItThrowsAnArgumentExceptionIfTheGameDefinitionNamesAreNull()
        {
            gamingGroupQuickStart.NewGameDefinitionNames = null;
            try
            {
                await gamingGroupCreator.CreateGamingGroupAsync(gamingGroupQuickStart, currentUser);
            }
            catch (ArgumentException exception)
            {
                Assert.AreEqual(GamingGroupCreator.EXCEPTION_MESSAGE_GAME_DEFINITION_NAMES_CANNOT_BE_NULL, exception.Message);
            }
        }

        [Test]
        public async Task ItReturnsTheSavedGamingGroup()
        {
            GamingGroup returnedGamingGroup = await gamingGroupCreator.CreateGamingGroupAsync(gamingGroupQuickStart, currentUser);

            IList<object[]> objectsPassedToSaveMethod = dataContextMock.GetArgumentsForCallsMadeOn(
                mock => mock.Save(Arg<GamingGroup>.Is.Anything, Arg<ApplicationUser>.Is.Anything));

            Assert.AreSame(expectedGamingGroup, returnedGamingGroup);
        }

        
        public async Task ItUpdatesTheCurrentUsersGamingGroup()
        {
            GamingGroup returnedGamingGroup = await gamingGroupCreator.CreateGamingGroupAsync(gamingGroupQuickStart, currentUser);

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

            await gamingGroupCreator.CreateGamingGroupAsync(gamingGroupQuickStart, currentUser);

            eventTrackerMock.AssertWasCalled(mock => mock.TrackGamingGroupCreation());
        }

        [Test]
        public async Task ItSavesAnyNewPlayers()
        {
            gamingGroupQuickStart.NewPlayerNames = new List<string>()
            {
                "player 1 name",
                "player 2 name"
            };

            await gamingGroupCreator.CreateGamingGroupAsync(gamingGroupQuickStart, currentUser);

            foreach(string playerName in gamingGroupQuickStart.NewPlayerNames)
            {
                playerSaverMock.AssertWasCalled(mock => mock.Save(
                    Arg<Player>.Matches(player => player.Name == playerName), 
                    Arg<ApplicationUser>.Is.Same(currentUser)));
            }
        }

        [Test]
        public async Task ItSkipsOverAnyPlayersWithNullOrWhitespaceNames()
        {
            gamingGroupQuickStart.NewPlayerNames = new List<string>()
            {
                "   ",
                string.Empty,
                null
            };

            await gamingGroupCreator.CreateGamingGroupAsync(gamingGroupQuickStart, currentUser);

            playerSaverMock.AssertWasNotCalled(mock => mock.Save(Arg<Player>.Is.Anything, Arg<ApplicationUser>.Is.Anything));
        }

        [Test]
        public async Task ItSavesAnyNewGameDefinitions()
        {
            gamingGroupQuickStart.NewGameDefinitionNames = new List<string>()
            {
                "game definition 1 name",
                "game definition 2 name"
            };

            await gamingGroupCreator.CreateGamingGroupAsync(gamingGroupQuickStart, currentUser);

            foreach (string gameDefinitionName in gamingGroupQuickStart.NewGameDefinitionNames)
            {
                gameDefinitionCreator.AssertWasCalled(mock => mock.Save(
                    Arg<GameDefinition>.Matches(gameDefinition => gameDefinition.Name == gameDefinitionName),
                    Arg<ApplicationUser>.Is.Same(currentUser)));
            }
        }

        [Test]
        public async Task ItSkipsOverAnyGameDefinitionsWithNullOrWhitespaceNames()
        {
            gamingGroupQuickStart.NewPlayerNames = new List<string>()
            {
                "   ",
                string.Empty,
                null
            };

            await gamingGroupCreator.CreateGamingGroupAsync(gamingGroupQuickStart, currentUser);

            gameDefinitionCreator.AssertWasNotCalled(mock => mock.Save(
                Arg<GameDefinition>.Is.Anything, Arg<ApplicationUser>.Is.Anything));
        }
    }
}
