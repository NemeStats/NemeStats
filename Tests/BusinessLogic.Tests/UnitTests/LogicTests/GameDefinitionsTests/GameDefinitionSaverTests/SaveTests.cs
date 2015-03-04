#region LICENSE
// NemeStats is a free website for tracking the results of board games.
//     Copyright (C) 2015 Jacob Gordon
// 
//     This program is free software: you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     This program is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU General Public License for more details.
// 
//     You should have received a copy of the GNU General Public License
//     along with this program.  If not, see <http://www.gnu.org/licenses/>
#endregion
using BusinessLogic.DataAccess;
using BusinessLogic.EventTracking;
using BusinessLogic.Logic.GameDefinitions;
using BusinessLogic.Models;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Linq;

namespace BusinessLogic.Tests.UnitTests.LogicTests.GameDefinitionsTests.GameDefinitionSaverTests
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
