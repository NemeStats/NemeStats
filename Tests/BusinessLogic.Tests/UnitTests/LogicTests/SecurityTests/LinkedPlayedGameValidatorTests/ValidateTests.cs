using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BusinessLogic.DataAccess;
using BusinessLogic.Exceptions;
using BusinessLogic.Logic.Security;
using BusinessLogic.Models;
using BusinessLogic.Models.Games;
using BusinessLogic.Models.PlayedGames;
using NUnit.Framework;
using Rhino.Mocks;
using Shouldly;
using StructureMap.AutoMocking;

namespace BusinessLogic.Tests.UnitTests.LogicTests.SecurityTests.LinkedPlayedGameValidatorTests
{
    [TestFixture]
    public class ValidateTests
    {
        private RhinoAutoMocker<LinkedPlayedGameValidator> _autoMocker;
        private string _expectedApplicationName = "some application name";
        private string _expectedEntityId = "some id";
        private int _expectedGamingGroupId = 10;

        [SetUp]
        public void SetUp()
        {
            _autoMocker = new RhinoAutoMocker<LinkedPlayedGameValidator>();

            var applicationLinkagesQueryable = new List<PlayedGameApplicationLinkage>
            {
                CreateExpectedApplicationLinkage(),
                CreateExpectedApplicationLinkage(overrideApplicationName: "some non-matching application name"),
                CreateExpectedApplicationLinkage(overrideEntityId: "some non-matching entity id"),
                CreateExpectedApplicationLinkage(overrideGamingGroupId: -1)
            }.AsQueryable();

            _autoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<PlayedGameApplicationLinkage>()).Return(applicationLinkagesQueryable);
        }

        private PlayedGameApplicationLinkage CreateExpectedApplicationLinkage(
            string overrideApplicationName = null, 
            string overrideEntityId = null,
            int? overrideGamingGroupId = null)
        {
            return new PlayedGameApplicationLinkage
            {
                ApplicationName = overrideApplicationName ?? _expectedApplicationName,
                EntityId = overrideEntityId ?? _expectedEntityId,
                PlayedGame = new PlayedGame
                {
                    GamingGroupId = overrideGamingGroupId ?? _expectedGamingGroupId
                }
            };
        }

        private List<PlayedGameApplicationLinkage> CreateApplicationLinkages(string sourceApplicationName, string sourceEntityId, bool alsoAddExpectedOne = false)
        {
            var linkages = new List<PlayedGameApplicationLinkage>
            {
                new PlayedGameApplicationLinkage
                {
                    ApplicationName = sourceApplicationName,
                    EntityId = sourceEntityId,
                }
            };

            if (alsoAddExpectedOne)
            {
                linkages.Add(new PlayedGameApplicationLinkage
                {
                    ApplicationName = _expectedApplicationName,
                    EntityId = _expectedEntityId,
                });
            }

            return linkages;
        }

        [Test]
        public void It_Throws_An_EntityAlreadySynchedException_If_An_Entity_With_This_Source_And_Id_Already_Exists_In_This_Gaming_Group()
        {
            //--arrange
            var newlyCompletedGame = CreateNewlyCompletedGame();
            var applicationLinkage = new ApplicationLinkage
            {
                ApplicationName = _expectedApplicationName,
                EntityId = _expectedEntityId
            };
            newlyCompletedGame.ApplicationLinkages.Add(applicationLinkage);
            var expectedException = new EntityAlreadySynchedException(_expectedApplicationName, _expectedEntityId, newlyCompletedGame.GamingGroupId.Value);

            //--act
            var exception = Assert.Throws<EntityAlreadySynchedException>(() => _autoMocker.ClassUnderTest.Validate(newlyCompletedGame));

            //--assert
            exception.Message.ShouldBe(expectedException.Message);
        }

        [Test]
        public void It_Doesnt_Throw_An_EntityAlreadySynchedException_If_An_Entity_With_This_ApplicationName_Doesnt_Exist()
        {
            //--arrange
            var newlyCompletedGame = CreateNewlyCompletedGame();
            var applicationLinkage = new ApplicationLinkage
            {
                ApplicationName = "name that doesnt exist",
                EntityId = _expectedEntityId
            }; newlyCompletedGame.ApplicationLinkages.Add(applicationLinkage);

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
            string applicationName,
            string externalSourceEntityId)
        {
            //--arrange
            var newlyCompletedGame = CreateNewlyCompletedGame();
            var applicationLinkage = new ApplicationLinkage
            {
                ApplicationName = applicationName,
                EntityId = externalSourceEntityId
            };
            newlyCompletedGame.ApplicationLinkages.Add(applicationLinkage);

            var expectedException = new InvalidSourceException(applicationName, externalSourceEntityId);

            //--act
            var exception = Assert.Throws<InvalidSourceException>(() => _autoMocker.ClassUnderTest.Validate(newlyCompletedGame));

            //--assert
            exception.Message.ShouldBe(expectedException.Message);
        }
        
        private NewlyCompletedGame CreateNewlyCompletedGame()
        {
            var newlyCompletedGame = new NewlyCompletedGame
            {
                GamingGroupId = _expectedGamingGroupId
            };
            return newlyCompletedGame;
        }
    }
}
