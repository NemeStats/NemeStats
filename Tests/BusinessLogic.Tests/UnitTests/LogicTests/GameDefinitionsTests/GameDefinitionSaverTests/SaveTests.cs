using BusinessLogic.DataAccess;
using BusinessLogic.EventTracking;
using BusinessLogic.Logic.GameDefinitions;
using BusinessLogic.Models;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniversalAnalyticsHttpWrapper;

namespace BusinessLogic.Tests.UnitTests.LogicTests.GameDefinitionsTests.GameDefinitionCreatorTests
{
    [TestFixture]
    public class SaveTests
    {
        private IDataContext dataContextMock;
        private INemeStatsEventTracker eventTrackerMock;
        private GameDefinitionSaver gameDefinitionSaver;
        private ApplicationUser currentUser;

        [SetUp]
        public void SetUp()
        {
            dataContextMock = MockRepository.GenerateMock<IDataContext>();
            eventTrackerMock = MockRepository.GenerateMock<INemeStatsEventTracker>();
            gameDefinitionSaver = new GameDefinitionSaver(dataContextMock, eventTrackerMock);
            currentUser = new ApplicationUser();
        }

        [Test]
        public void ItThrowsAnArgumentNullExceptionIfTheGameDefinitionIsNull()
        {
            ArgumentNullException expectedException = new ArgumentNullException("gameDefinition");

            Exception exception = Assert.Throws<ArgumentNullException>(() => gameDefinitionSaver.Save(null, currentUser));

            Assert.AreEqual(expectedException.Message, exception.Message);
        }

        [Test]
        public void ItThrowsAnArgumentExceptionIfTheGameDefinitionNameIsNull()
        {
            ArgumentException expectedException = new ArgumentException("gameDefinition.Name cannot be null or whitespace.");
            GameDefinition gameDefinition = new GameDefinition() { Name = null };

            Exception exception = Assert.Throws<ArgumentException>(() => gameDefinitionSaver.Save(gameDefinition, currentUser));

            Assert.AreEqual(expectedException.Message, exception.Message);
        }

        [Test]
        public void ItThrowsAnArgumentExceptionIfTheGameDefinitionNameIsWhitespace()
        {
            GameDefinition gameDefinition = new GameDefinition()
            {
                Name = "    "
            };
            ArgumentException expectedException = new ArgumentException("gameDefinition.Name cannot be null or whitespace.");

            Exception exception = Assert.Throws<ArgumentException>(
                () => gameDefinitionSaver.Save(gameDefinition, currentUser));

            Assert.AreEqual(expectedException.Message, exception.Message);
        }

        [Test]
        public void ItSetsTheGameDefinitionName()
        {
            GameDefinition gameDefinition = new GameDefinition()
            {
                Name = "game definition name"
            };

            gameDefinitionSaver.Save(gameDefinition, currentUser);

            dataContextMock.AssertWasCalled(mock => mock.Save<GameDefinition>(
                Arg<GameDefinition>.Matches(newGameDefinition => newGameDefinition.Name == gameDefinition.Name),
                Arg<ApplicationUser>.Is.Anything));
        }

        [Test]
        public void ItSetsTheGameDefinitionDescription()
        {
            GameDefinition gameDefinition = new GameDefinition()
            {
                Name = "game definition name",
                Description = "game description"
            };

            gameDefinitionSaver.Save(gameDefinition, currentUser);

            dataContextMock.AssertWasCalled(mock => mock.Save<GameDefinition>(
                Arg<GameDefinition>.Matches(newGameDefinition => newGameDefinition.Description == gameDefinition.Description),
                Arg<ApplicationUser>.Is.Anything));
        }

        [Test]
        public void ItRecordsAGameDefinitionCreatedEvent()
        {
            GameDefinition gameDefinition = MockRepository.GeneratePartialMock<GameDefinition>();
            gameDefinition.Name = "name";
            gameDefinition.Expect(mock => mock.AlreadyInDatabase())
                .Return(false);

            gameDefinitionSaver.Save(gameDefinition, currentUser);

            //TODO just discovered that there is a race condition here... this test failed once but I can't reproduce.
            eventTrackerMock.AssertWasCalled(mock => mock.TrackGameDefinitionCreation(currentUser, gameDefinition.Name));
        }

        [Test]
        public void ItDoesNotRecordsAGameDefinitionCreatedEventIfTheGameDefinitionIsNotNew()
        {
            GameDefinition gameDefinition = MockRepository.GeneratePartialMock<GameDefinition>();
            gameDefinition.Name = "name";
            gameDefinition.Expect(mock => mock.AlreadyInDatabase())
                .Return(true);

            gameDefinitionSaver.Save(gameDefinition, currentUser);

            eventTrackerMock.AssertWasNotCalled(mock => mock.TrackGameDefinitionCreation(currentUser, gameDefinition.Name));
        }
    }
}
