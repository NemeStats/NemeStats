using System.Collections.Generic;
using System.Linq;
using BusinessLogic.DataAccess;
using BusinessLogic.Exceptions;
using BusinessLogic.Logic.Security;
using BusinessLogic.Models;
using BusinessLogic.Models.Games;
using NUnit.Framework;
using Rhino.Mocks;
using Shouldly;
using StructureMap.AutoMocking;

namespace BusinessLogic.Tests.UnitTests.LogicTests.SecurityTests.SynchedPlayedGameValidatorTests
{
    [TestFixture]
    public class ValidateTests
    {
        private RhinoAutoMocker<SynchedPlayedGameValidator> _autoMocker;
        private string _expectedSourceName = "some source name";
        private string _expectedSourceId = "some id";

        [SetUp]
        public void SetUp()
        {
            _autoMocker = new RhinoAutoMocker<SynchedPlayedGameValidator>();

            var newlyCompletedGame = CreateNewlyCompletedGame();

            var playedGameQueryable = new List<PlayedGame>
            {
                new PlayedGame
                {
                    ExternalSourceApplicationName = newlyCompletedGame.ExternalSourceApplicationName,
                    ExternalSourceEntityId = newlyCompletedGame.ExternalSourceEntityId,
                    GamingGroupId = newlyCompletedGame.GamingGroupId.Value
                },
                new PlayedGame
                {
                    ExternalSourceApplicationName = string.Empty,
                    ExternalSourceEntityId = newlyCompletedGame.ExternalSourceEntityId,
                    GamingGroupId = newlyCompletedGame.GamingGroupId.Value
                },
                new PlayedGame
                {
                    ExternalSourceApplicationName = null,
                    ExternalSourceEntityId = newlyCompletedGame.ExternalSourceEntityId,
                    GamingGroupId = newlyCompletedGame.GamingGroupId.Value
                },
                new PlayedGame
                {
                    ExternalSourceApplicationName = newlyCompletedGame.ExternalSourceApplicationName,
                    ExternalSourceEntityId = string.Empty,
                    GamingGroupId = newlyCompletedGame.GamingGroupId.Value
                },
                new PlayedGame
                {
                    ExternalSourceApplicationName = newlyCompletedGame.ExternalSourceApplicationName,
                    ExternalSourceEntityId = null,
                    GamingGroupId = newlyCompletedGame.GamingGroupId.Value
                }
                ,new PlayedGame
                {
                    ExternalSourceApplicationName = newlyCompletedGame.ExternalSourceApplicationName,
                    ExternalSourceEntityId = newlyCompletedGame.ExternalSourceEntityId,
                    GamingGroupId = 50
                }
            }.AsQueryable();

            _autoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<PlayedGame>()).Return(playedGameQueryable);
        }

        [Test]
        public void It_Throws_An_EntityAlreadySynchedException_If_An_Entity_With_This_Source_And_Id_Already_Exists_In_This_Gaming_Group()
        {
            //--arrange
            var newlyCompletedGame = CreateNewlyCompletedGame();
            var expectedException = new EntityAlreadySynchedException(newlyCompletedGame, newlyCompletedGame.GamingGroupId.Value);

            //--act
            var exception = Assert.Throws<EntityAlreadySynchedException>(() => _autoMocker.ClassUnderTest.Validate(newlyCompletedGame));

            //--assert
            exception.Message.ShouldBe(expectedException.Message);
        }

        [Test]
        public void It_Doesnt_Throw_An_EntityAlreadySynchedException_If_An_Entity_With_This_ExternalSourceApplicationName_Doesnt_Exist()
        {
            //--arrange
            var newlyCompletedGame = CreateNewlyCompletedGame();
            newlyCompletedGame.ExternalSourceApplicationName = "some name that doesn't exist";

            //--act
            _autoMocker.ClassUnderTest.Validate(newlyCompletedGame);
        }

        [Test]
        public void It_Doesnt_Throw_An_EntityAlreadySynchedException_If_The_ExternalSourceApplicationName_Isnt_Set()
        {
            //--arrange
            var newlyCompletedGame = CreateNewlyCompletedGame();
            newlyCompletedGame.ExternalSourceApplicationName = null;

            //--act
            _autoMocker.ClassUnderTest.Validate(newlyCompletedGame);
        }

        [Test]
        public void It_Doesnt_Throw_An_EntityAlreadySynchedException_If_The_ExternalSourceApplicationName_Is_Empty()
        {
            //--arrange
            var newlyCompletedGame = CreateNewlyCompletedGame();
            newlyCompletedGame.ExternalSourceApplicationName = string.Empty;

            //--act
            _autoMocker.ClassUnderTest.Validate(newlyCompletedGame);
        }

        [Test]
        public void It_Doesnt_Throw_An_EntityAlreadySynchedException_If_An_Entity_With_This_ExternalSourceEntityId_Doesnt_Exist()
        {
            //--arrange
            var newlyCompletedGame = CreateNewlyCompletedGame();
            newlyCompletedGame.ExternalSourceEntityId = "some id that doesn't exist";

            //--act
            _autoMocker.ClassUnderTest.Validate(newlyCompletedGame);
        }

        [Test]
        public void It_Doesnt_Throw_An_EntityAlreadySynchedException_If_The_ExternalSourceEntityId_Isnt_Set()
        {
            //--arrange
            var newlyCompletedGame = CreateNewlyCompletedGame();
            newlyCompletedGame.ExternalSourceEntityId = null;

            //--act
            _autoMocker.ClassUnderTest.Validate(newlyCompletedGame);
        }

        [Test]
        public void It_Doesnt_Throw_An_EntityAlreadySynchedException_If_The_ExternalSourceEntityId_Is_Empty()
        {
            //--arrange
            var newlyCompletedGame = CreateNewlyCompletedGame();
            newlyCompletedGame.ExternalSourceEntityId = string.Empty;

            //--act
            _autoMocker.ClassUnderTest.Validate(newlyCompletedGame);
        }


        private static NewlyCompletedGame CreateNewlyCompletedGame()
        {
            var newlyCompletedGame = new NewlyCompletedGame
            {
                ExternalSourceEntityId = "some id",
                ExternalSourceApplicationName = "some name",
                GamingGroupId = 1
            };
            return newlyCompletedGame;
        }
    }
}
