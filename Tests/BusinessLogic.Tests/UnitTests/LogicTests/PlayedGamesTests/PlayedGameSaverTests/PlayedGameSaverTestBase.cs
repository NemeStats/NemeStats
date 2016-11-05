using System.Linq;
using BusinessLogic.DataAccess;
using BusinessLogic.Logic.PlayedGames;
using BusinessLogic.Models;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using StructureMap.AutoMocking;

namespace BusinessLogic.Tests.UnitTests.LogicTests.PlayedGamesTests.PlayedGameSaverTests
{
    public abstract class PlayedGameSaverTestBase
    {
        protected RhinoAutoMocker<PlayedGameSaver> autoMocker;
        protected ApplicationUser currentUser;
        protected GameDefinition gameDefinition;
        protected Player existingPlayerWithMatchingGamingGroup;
        protected const int GAMING_GROUP_ID = 9;
        protected const int EXPECTED_PLAYED_GAME_ID = 50;

        [SetUp]
        public void TestSetUp()
        {
            autoMocker = new RhinoAutoMocker<PlayedGameSaver>();
            autoMocker.PartialMockTheClassUnderTest();

            currentUser = new ApplicationUser
            {
                Id = "user id",
                CurrentGamingGroupId = GAMING_GROUP_ID,
                AnonymousClientId = "anonymous client id"
            };
            gameDefinition = new GameDefinition
            {
                Name = "game definition name",
                GamingGroupId = GAMING_GROUP_ID,
                Id = 9598
            };
            autoMocker.Get<IDataContext>().Expect(mock => mock.FindById<GameDefinition>(gameDefinition.Id))
                .Return(gameDefinition);

            existingPlayerWithMatchingGamingGroup = new Player
            {
                Id = 1,
                GamingGroupId = GAMING_GROUP_ID
            };
            autoMocker.Get<IDataContext>().Expect(mock => mock.FindById<Player>(Arg<int>.Is.Anything)).Return(existingPlayerWithMatchingGamingGroup);
        }
    }
}