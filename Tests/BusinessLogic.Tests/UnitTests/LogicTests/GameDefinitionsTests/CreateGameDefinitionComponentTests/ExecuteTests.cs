using System;
using System.Collections.Generic;
using System.Linq;
using BusinessLogic.DataAccess;
using BusinessLogic.EventTracking;
using BusinessLogic.Exceptions;
using BusinessLogic.Logic.BoardGameGeek;
using BusinessLogic.Logic.GameDefinitions;
using BusinessLogic.Models;
using BusinessLogic.Models.Games;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using Shouldly;
using StructureMap.AutoMocking;

namespace BusinessLogic.Tests.UnitTests.LogicTests.GameDefinitionsTests.CreateGameDefinitionComponentTests
{
    [TestFixture]
    public class ExecuteTests
    {
        private RhinoAutoMocker<CreateGameDefinitionComponent> _autoMocker;
        private IDataContext _dataContextMock;
        private ApplicationUser _currentUser;
        private int _gamingGroupId = 100;

        [SetUp]
        public void BaseSetUp()
        {
            _autoMocker = new RhinoAutoMocker<CreateGameDefinitionComponent>();
            _dataContextMock = MockRepository.GenerateMock<IDataContext>();

            _currentUser = new ApplicationUser
            {
                Id = "some application user id",
                CurrentGamingGroupId = _gamingGroupId
            };
        }

        [Test]
        public void It_Throws_A_NoValidGamingGroupException_If_The_User_Has_No_Gaming_Group_And_There_Is_None_Specified_On_The_Request()
        {
            //--arrange
            _currentUser.CurrentGamingGroupId = null;
            var createGameDefinitionRequest = new CreateGameDefinitionRequest
            {
                Name = "some name",
                GamingGroupId = null
            };
            var expectedException = new NoValidGamingGroupException(_currentUser.Id);

            //--act
            var actualException = Assert.Throws<NoValidGamingGroupException>(
                () => _autoMocker.ClassUnderTest.Execute(createGameDefinitionRequest, _currentUser, _dataContextMock));

            //--assert
            actualException.Message.ShouldBe(expectedException.Message);
        }

        [Test]
        public void ItThrowsAnArgumentNullExceptionIfTheModelIsNull()
        {
            var expectedException = new ArgumentNullException("createGameDefinitionRequest");

            Exception exception = Assert.Throws<ArgumentNullException>(() => _autoMocker.ClassUnderTest.Execute((CreateGameDefinitionRequest)null, _currentUser, _dataContextMock));

            Assert.AreEqual(expectedException.Message, exception.Message);
        }

        [Test]
        public void ItThrowsAnArgumentExceptionIfTheNameIsNull()
        {
            var expectedException = new ArgumentException("createGameDefinitionRequest.Name cannot be null or whitespace.");
            var gameDefinition = new CreateGameDefinitionRequest
            { Name = null };

            Exception exception = Assert.Throws<ArgumentException>(() => _autoMocker.ClassUnderTest.Execute(gameDefinition, _currentUser, _dataContextMock));

            Assert.AreEqual(expectedException.Message, exception.Message);
        }

        [Test]
        public void ItThrowsAnArgumentExceptionIfTheNameIsWhitespace()
        {
            var gameDefinition = new CreateGameDefinitionRequest
            {
                Name = "    "
            };
            var expectedException = new ArgumentException("createGameDefinitionRequest.Name cannot be null or whitespace.");

            Exception exception = Assert.Throws<ArgumentException>(
                () => _autoMocker.ClassUnderTest.Execute(gameDefinition, _currentUser, _dataContextMock));

            Assert.AreEqual(expectedException.Message, exception.Message);
        }

        [Test]
        public void ItThrowsADuplicateKeyExceptionIfThereIsAnExistingActiveGameDefinitionWithTheSameNameInTheCurrentUsersGamingGroupAndTheGamingGroupIsNotExplicitlySet()
        {
            var gameDefinition = new GameDefinition
            {
                Name = "existing game definition name",
                Active = true,
                GamingGroupId = _gamingGroupId
            };
            var createGameDefinitionRequest = new CreateGameDefinitionRequest
            {
                Name = gameDefinition.Name
            };
            var gameDefinitionQueryable = new List<GameDefinition>
            {
                gameDefinition
            }.AsQueryable();

            _dataContextMock.Expect(mock => mock.GetQueryable<GameDefinition>()).Return(gameDefinitionQueryable);

            Exception exception = Assert.Throws<DuplicateKeyException>(
                () => _autoMocker.ClassUnderTest.Execute(createGameDefinitionRequest, _currentUser, _dataContextMock));

            Assert.That(exception.Message, Is.EqualTo("An active Game Definition with name '" + gameDefinition.Name + "' already exists in this Gaming Group."));
        }

        [Test]
        public void ItThrowsADuplicateKeyExceptionIfThereIsAnExistingActiveGameDefinitionInTheSpecifiedGamingGroup()
        {
            var expectedGamingGroupId = 2;

            var gameDefinition = new GameDefinition
            {
                Name = "existing game definition name",
                Active = true,
                GamingGroupId = expectedGamingGroupId
            };
            var createGameDefinitionRequest = new CreateGameDefinitionRequest
            {
                Name = gameDefinition.Name,
                GamingGroupId = expectedGamingGroupId
            };
            var gameDefinitionQueryable = new List<GameDefinition>
            {
                gameDefinition
            }.AsQueryable();

            _dataContextMock.Expect(mock => mock.GetQueryable<GameDefinition>()).Return(gameDefinitionQueryable);

            Exception exception = Assert.Throws<DuplicateKeyException>(
                () => _autoMocker.ClassUnderTest.Execute(createGameDefinitionRequest, _currentUser, _dataContextMock));

            Assert.That(exception.Message, Is.EqualTo("An active Game Definition with name '" + gameDefinition.Name + "' already exists in this Gaming Group."));
        }

        [Test]
        public void ItThrowsADuplicateKeyExceptionIfThereIsAnExistingActiveGameDefinitionWithTheSameNameAndTheGamingGroupIsNotExplicitlySet()
        {
            var gameDefinition = new GameDefinition
            {
                Name = "existing game definition name",
                Active = true,
                GamingGroupId = _gamingGroupId
            };
            var createGameDefinitionRequest = new CreateGameDefinitionRequest
            {
                Name = gameDefinition.Name
            };
            var gameDefinitionQueryable = new List<GameDefinition>
            {
                gameDefinition
            }.AsQueryable();

            _dataContextMock.Expect(mock => mock.GetQueryable<GameDefinition>()).Return(gameDefinitionQueryable);

            Exception exception = Assert.Throws<DuplicateKeyException>(
                () => _autoMocker.ClassUnderTest.Execute(createGameDefinitionRequest, _currentUser, _dataContextMock));

            Assert.That(exception.Message, Is.EqualTo("An active Game Definition with name '" + gameDefinition.Name + "' already exists in this Gaming Group."));
        }

        [Test]
        public void AnExistingInactiveGameDefinitionIsReactivatedIfSomeoneTriesToAddItAgain()
        {
            var existingGameDefinition = new GameDefinition
            {
                Id = 1,
                Name = "existing game definition name",
                GamingGroupId = _gamingGroupId,
                Active = false
            };
            var gameDefinitionQueryable = new List<GameDefinition>
            {
                existingGameDefinition
            }.AsQueryable();
            var newGameDefinition = new CreateGameDefinitionRequest
            {
                Name = existingGameDefinition.Name
            };
            _dataContextMock.Expect(mock => mock.GetQueryable<GameDefinition>()).Return(gameDefinitionQueryable);

            _autoMocker.ClassUnderTest.Execute(newGameDefinition, _currentUser, _dataContextMock);

            var gameDefinitionThatWasSaved =
                (GameDefinition)_dataContextMock.GetArgumentsForCallsMadeOn(mock => mock.Save(Arg<GameDefinition>.Is.Anything, Arg<ApplicationUser>.Is.Anything))[0][0];

            Assert.That(gameDefinitionThatWasSaved.Id, Is.EqualTo(existingGameDefinition.Id));
            Assert.That(gameDefinitionThatWasSaved.BoardGameGeekGameDefinitionId, Is.EqualTo(existingGameDefinition.BoardGameGeekGameDefinitionId));
            Assert.That(gameDefinitionThatWasSaved.Active, Is.EqualTo(true));
        }

        [Test]
        public void ReactivatingAnExistingGameDefinitionTakesTheNewDescriptionIfItIsNotBlank()
        {
            var existingGameDefinition = new GameDefinition
            {
                Id = 1,
                Name = "existing game definition name",
                Active = false,
                GamingGroupId = _gamingGroupId
            };
            var gameDefinitionQueryable = new List<GameDefinition>
            {
                existingGameDefinition
            }.AsQueryable();
            var newGameDefinition = new CreateGameDefinitionRequest
            {
                Name = existingGameDefinition.Name,
                Description = "new description"
            };
            _dataContextMock.Expect(mock => mock.GetQueryable<GameDefinition>()).Return(gameDefinitionQueryable);

            _autoMocker.ClassUnderTest.Execute(newGameDefinition, _currentUser, _dataContextMock);

            var gameDefinitionThatWasSaved =
                (GameDefinition)_dataContextMock.GetArgumentsForCallsMadeOn(mock => mock.Save(Arg<GameDefinition>.Is.Anything, Arg<ApplicationUser>.Is.Anything))[0][0];

            Assert.That(gameDefinitionThatWasSaved.Description, Is.EqualTo(newGameDefinition.Description));
        }

        [Test]
        public void ReactivatingAnExistingGameDefinitionTakesTheOldDescriptionIfTheNewOneIsBlank()
        {
            var existingGameDefinition = new GameDefinition
            {
                Id = 1,
                Name = "existing game definition name",
                Description = "existing game definition description",
                Active = false,
                GamingGroupId = _gamingGroupId
            };
            var gameDefinitionQueryable = new List<GameDefinition>
            {
                existingGameDefinition
            }.AsQueryable();
            var newGameDefinition = new CreateGameDefinitionRequest
            {
                Name = existingGameDefinition.Name
            };
            _dataContextMock.Expect(mock => mock.GetQueryable<GameDefinition>()).Return(gameDefinitionQueryable);

            _autoMocker.ClassUnderTest.Execute(newGameDefinition, _currentUser, _dataContextMock);

            var gameDefinitionThatWasSaved =
                (GameDefinition)_dataContextMock.GetArgumentsForCallsMadeOn(mock => mock.Save(Arg<GameDefinition>.Is.Anything, Arg<ApplicationUser>.Is.Anything))[0][0];

            Assert.That(gameDefinitionThatWasSaved.Description, Is.EqualTo(existingGameDefinition.Description));
        }

        [Test]
        public void ItSavesANewGameDefinitionOnTheCurrentUsersGamingGroupIfNoneIsSpecified()
        {
            var createGameDefinitionRequest = new CreateGameDefinitionRequest
            {
                Name = "game definition name",
                Active = true,
                BoardGameGeekGameDefinitionId = 12,
                Description = "some description"
            };
            _dataContextMock.Expect(mock => mock.GetQueryable<GameDefinition>()).Return(new List<GameDefinition>().AsQueryable());
            _autoMocker.Get<IBoardGameGeekGameDefinitionCreator>().Expect(
                mock => mock.CreateBoardGameGeekGameDefinition(Arg<int>.Is.Anything))
                .Return(null);

            _autoMocker.ClassUnderTest.Execute(createGameDefinitionRequest, _currentUser, _dataContextMock);

            var args = _dataContextMock.GetArgumentsForCallsMadeOn(mock => mock.Save(
                Arg<GameDefinition>.Is.Anything,
                Arg<ApplicationUser>.Is.Anything));
            var actualGameDefinition = args[0][0] as GameDefinition;
            Assert.That(actualGameDefinition, Is.Not.Null);
            Assert.That(actualGameDefinition.Name, Is.EqualTo(createGameDefinitionRequest.Name));
            Assert.That(actualGameDefinition.GamingGroupId, Is.EqualTo(_currentUser.CurrentGamingGroupId));
        }

        [Test]
        public void ItSavesANewGameDefinitionOnTheSpecifiedGamingGroup()
        {
            var createGameDefinitionRequest = new CreateGameDefinitionRequest
            {
                Name = "game definition name",
                GamingGroupId = _currentUser.CurrentGamingGroupId - 1
            };
            _dataContextMock.Expect(mock => mock.GetQueryable<GameDefinition>()).Return(new List<GameDefinition>().AsQueryable());
            _autoMocker.Get<IBoardGameGeekGameDefinitionCreator>().Expect(
                mock => mock.CreateBoardGameGeekGameDefinition(Arg<int>.Is.Anything))
                .Return(null);

            _autoMocker.ClassUnderTest.Execute(createGameDefinitionRequest, _currentUser, _dataContextMock);

            _dataContextMock.AssertWasCalled(mock => mock.Save(
                Arg<GameDefinition>.Matches(x => x.GamingGroupId == createGameDefinitionRequest.GamingGroupId),
                Arg<ApplicationUser>.Is.Anything));
        }

        [Test]
        [Explicit("This test has race conditions and sometimes fails")]
        public void zzz_ItRecordsAGameDefinitionCreatedEvent()
        {
            var gameDefinition = new CreateGameDefinitionRequest
            {
                Name = "some name"
            };

            _dataContextMock.Expect(mock => mock.GetQueryable<GameDefinition>()).Return(new List<GameDefinition>().AsQueryable());

            _autoMocker.ClassUnderTest.Execute(gameDefinition, _currentUser, _dataContextMock);

            _autoMocker.Get<INemeStatsEventTracker>().AssertWasCalled(mock => mock.TrackGameDefinitionCreation(_currentUser, gameDefinition.Name));
        }

        [Test]
        public void ItDoesNotRecordAGameDefinitionCreatedEventIfTheGameDefinitionIsNotNew()
        {
            var existingGameDefinition = new GameDefinition
            {
                Id = 1,
                Name = "existing game definition name",
                GamingGroupId = _gamingGroupId,
                Active = false
            };
            var gameDefinitionQueryable = new List<GameDefinition>
            {
                existingGameDefinition
            }.AsQueryable();
            var newGameDefinition = new CreateGameDefinitionRequest
            {
                Name = existingGameDefinition.Name
            };
            _dataContextMock.Expect(mock => mock.GetQueryable<GameDefinition>()).Return(gameDefinitionQueryable); ;

            _autoMocker.ClassUnderTest.Execute(newGameDefinition, _currentUser, _dataContextMock);

            _autoMocker.Get<INemeStatsEventTracker>().AssertWasNotCalled(mock => mock.TrackGameDefinitionCreation(_currentUser, newGameDefinition.Name));
        }

        [Test]
        public void ItAttachesToABoardGameGeekGameDefinitionIfItHasABoardGameGeekGameDefinitionId()
        {
            var createGameDefinitionRequest = new CreateGameDefinitionRequest
            {
                Name = "some new game name",
                BoardGameGeekGameDefinitionId = 1
            };
            _dataContextMock.Expect(mock => mock.GetQueryable<GameDefinition>()).Return(new List<GameDefinition>().AsQueryable());
            const int EXPECTED_BGG_ID = 2;
            var expectedBoardGameGeekGameDefinition = new BoardGameGeekGameDefinition
            {
                Id = 2
            };
            _autoMocker.Get<IBoardGameGeekGameDefinitionCreator>().Expect(
                mock => mock.CreateBoardGameGeekGameDefinition(createGameDefinitionRequest.BoardGameGeekGameDefinitionId.Value))
                .Return(expectedBoardGameGeekGameDefinition);

            _autoMocker.ClassUnderTest.Execute(createGameDefinitionRequest, _currentUser, _dataContextMock);

            var args = _dataContextMock.GetArgumentsForCallsMadeOn(
                dataContext => dataContext.Save(Arg<GameDefinition>.Is.Anything, Arg<ApplicationUser>.Is.Anything));

            var actualGameDefinition = args[0][0] as GameDefinition;
            Assert.That(actualGameDefinition, Is.Not.Null);
            Assert.That(actualGameDefinition.BoardGameGeekGameDefinitionId, Is.EqualTo(EXPECTED_BGG_ID));
        }

    }
}
