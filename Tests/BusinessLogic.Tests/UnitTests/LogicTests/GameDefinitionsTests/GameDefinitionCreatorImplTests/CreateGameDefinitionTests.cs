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

namespace BusinessLogic.Tests.UnitTests.LogicTests.GameDefinitionsTests.GameDefinitionCreatorImplTests
{
    [TestFixture]
    public class CreateGameDefinitionTests
    {
        private DataContext dataContextMock;
        private NemeStatsEventTracker eventTrackerMock;
        private GameDefinitionCreatorImpl gameDefinitionCreator;
        private ApplicationUser currentUser;

        [SetUp]
        public void SetUp()
        {
            dataContextMock = MockRepository.GenerateMock<DataContext>();
            eventTrackerMock = MockRepository.GenerateMock<NemeStatsEventTracker>();
            gameDefinitionCreator = new GameDefinitionCreatorImpl(dataContextMock, eventTrackerMock);
            currentUser = new ApplicationUser();
        }

        [Test]
        public void ItThrowsAnArgumentNullExceptionIfTheGameDefinitionNameIsNull()
        {
            ArgumentNullException expectedException = new ArgumentNullException("gameDefinitionName");

            Exception exception = Assert.Throws<ArgumentNullException>(() => gameDefinitionCreator.CreateGameDefinition(null, null, currentUser));

            Assert.AreEqual(expectedException.Message, exception.Message);
        }

        [Test]
        public void ItThrowsAnArgumentNullExceptionIfTheGameDefinitionNameIsWhitespace()
        {
            string gameDefinitionName = "    ";
            ArgumentNullException expectedException = new ArgumentNullException("gameDefinitionName");

            Exception exception = Assert.Throws<ArgumentNullException>(
                () => gameDefinitionCreator.CreateGameDefinition(gameDefinitionName, null, currentUser));

            Assert.AreEqual(expectedException.Message, exception.Message);
        }

        [Test]
        public void ItSetsTheGameDefinitionName()
        {
            string gameDefinitionName = "game definition name";

            gameDefinitionCreator.CreateGameDefinition(gameDefinitionName, null, currentUser);

            dataContextMock.AssertWasCalled(mock => mock.Save<GameDefinition>(
                Arg<GameDefinition>.Matches(gameDefinition => gameDefinition.Name == gameDefinitionName),
                Arg<ApplicationUser>.Is.Anything));
        }

        [Test]
        public void ItSetsTheGameDefinitionDescription()
        {
            string gameDefinitionDescription = "game definition description";

            gameDefinitionCreator.CreateGameDefinition("game definition name", gameDefinitionDescription, currentUser);

            dataContextMock.AssertWasCalled(mock => mock.Save<GameDefinition>(
                Arg<GameDefinition>.Matches(gameDefinition => gameDefinition.Description == gameDefinitionDescription),
                Arg<ApplicationUser>.Is.Anything));
        }

        [Test]
        public void ItRecordsAGameDefinitionCreatedEvent()
        {
            string gameDefinitionName = "game definition name";

            gameDefinitionCreator.CreateGameDefinition(gameDefinitionName, null, currentUser);

            eventTrackerMock.AssertWasCalled(mock => mock.TrackGameDefinitionCreation(currentUser, gameDefinitionName));
        }
    }
}
