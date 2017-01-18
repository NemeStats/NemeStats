using BusinessLogic.DataAccess;
using BusinessLogic.DataAccess.Security;
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
        protected RhinoAutoMocker<PlayedGameSaver> AutoMocker;
        protected ApplicationUser CurrentUser;
        protected GameDefinition GameDefinition;
        protected Player ExistingPlayerWithMatchingGamingGroup;
        protected const int GAMING_GROUP_ID = 9;
        protected const int EXPECTED_PLAYED_GAME_ID = 50;

        [SetUp]
        public void TestSetUp()
        {
            AutoMocker = new RhinoAutoMocker<PlayedGameSaver>();
            AutoMocker.PartialMockTheClassUnderTest();

            CurrentUser = new ApplicationUser
            {
                Id = "user id",
                CurrentGamingGroupId = GAMING_GROUP_ID,
                AnonymousClientId = "anonymous client id"
            };
            GameDefinition = new GameDefinition
            {
                Name = "game definition name",
                GamingGroupId = GAMING_GROUP_ID,
                Id = 9598
            };

            AutoMocker.Get<ISecuredEntityValidator>().Expect(mock => mock.RetrieveAndValidateAccess<GameDefinition>(Arg<int>.Is.Anything, Arg<ApplicationUser>.Is.Anything)).Return(GameDefinition);

            ExistingPlayerWithMatchingGamingGroup = new Player
            {
                Id = 1,
                GamingGroupId = GAMING_GROUP_ID
            };
            AutoMocker.Get<IDataContext>().Expect(mock => mock.FindById<Player>(Arg<int>.Is.Anything)).Return(ExistingPlayerWithMatchingGamingGroup);
        }
    }
}