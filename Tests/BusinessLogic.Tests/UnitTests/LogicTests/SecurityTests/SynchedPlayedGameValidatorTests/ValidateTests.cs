using System.Collections;
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

        public class ExternalSourceTestCases
        {
            public static IEnumerable TestCases
            {
                get
                {
                    yield return new TestCaseData(string.Empty, string.Empty);
                    yield return new TestCaseData(string.Empty, null);
                    yield return new TestCaseData(null, string.Empty);
                    yield return new TestCaseData(null, null);
                }
            }
        }

        [Test, TestCaseSource(typeof(ExternalSourceTestCases), nameof(ExternalSourceTestCases.TestCases))]
        public void It_Doesnt_Throw_An_EntityAlreadySynchedException_If_Both_ExternalSourceApplicationName_And_ExternalSourceEntityId_Are_Not_Set(
            string externalSourceApplicationName,
            string externalSourceEntityId)
        {
            //--arrange
            var newlyCompletedGame = CreateNewlyCompletedGame();
            newlyCompletedGame.ExternalSourceApplicationName = externalSourceApplicationName;
            newlyCompletedGame.ExternalSourceEntityId = externalSourceEntityId;

            //--act
            _autoMocker.ClassUnderTest.Validate(newlyCompletedGame);
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

        public class OnlyOneOfTwoFieldsSetTestCases
        {
            public static IEnumerable TestCases
            {
                get
                {
                    yield return new TestCaseData(string.Empty, "some value");
                    yield return new TestCaseData(null, "some value");
                    yield return new TestCaseData("some value", string.Empty);
                    yield return new TestCaseData("some value", null);
                }
            }
        }

        [Test, TestCaseSource(typeof(OnlyOneOfTwoFieldsSetTestCases), nameof(OnlyOneOfTwoFieldsSetTestCases.TestCases))]
        public void It_Throws_An_InvalidSourceException_If_One_Of_The_External_Source_Fields_Is_Set_But_Not_The_Other(
            string externalSourceApplicationName,
            string externalSourceEntityId)
        {
            //--arrange
            var newlyCompletedGame = CreateNewlyCompletedGame();
            newlyCompletedGame.ExternalSourceEntityId = externalSourceEntityId;
            newlyCompletedGame.ExternalSourceApplicationName = externalSourceApplicationName;
            var expectedException = new InvalidSourceException(externalSourceApplicationName, externalSourceEntityId);

            //--act
            var exception = Assert.Throws<InvalidSourceException>(() => _autoMocker.ClassUnderTest.Validate(newlyCompletedGame));

            //--assert
            exception.Message.ShouldBe(expectedException.Message);
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
