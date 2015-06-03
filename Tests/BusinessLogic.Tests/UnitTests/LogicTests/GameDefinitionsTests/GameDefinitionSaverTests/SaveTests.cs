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

using System.Data.Entity.Infrastructure;
using BusinessLogic.DataAccess;
using BusinessLogic.EventTracking;
using BusinessLogic.Exceptions;
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
    public class SaveTests : GameDefinitionSaverTestBase
    {
        [Test]
        public void ItThrowsAnArgumentNullExceptionIfTheGameDefinitionIsNull()
        {
            var expectedException = new ArgumentNullException("gameDefinition");

            Exception exception = Assert.Throws<ArgumentNullException>(() => autoMocker.ClassUnderTest.Save(null, currentUser));

            Assert.AreEqual(expectedException.Message, exception.Message);
        }

        [Test]
        public void ItThrowsAnArgumentExceptionIfTheGameDefinitionNameIsNull()
        {
            var expectedException = new ArgumentException("gameDefinition.Name cannot be null or whitespace.");
            var gameDefinition = new GameDefinition
            { Name = null };

            Exception exception = Assert.Throws<ArgumentException>(() => autoMocker.ClassUnderTest.Save(gameDefinition, currentUser));

            Assert.AreEqual(expectedException.Message, exception.Message);
        }

        [Test]
        public void ItThrowsAnArgumentExceptionIfTheGameDefinitionNameIsWhitespace()
        {
            var gameDefinition = new GameDefinition
            {
                Name = "    "
            };
            var expectedException = new ArgumentException("gameDefinition.Name cannot be null or whitespace.");

            Exception exception = Assert.Throws<ArgumentException>(
                () => autoMocker.ClassUnderTest.Save(gameDefinition, currentUser));

            Assert.AreEqual(expectedException.Message, exception.Message);
        }

        [Test]
        public void ItThrowsADuplicateKeyExceptionIfThereIsADbUpdateException()
        {
            var gameDefinition = new GameDefinition
            {
                Name = "existing game definition name"
            };
            autoMocker.Get<IDataContext>().Expect(mock => mock.Save(
                Arg<GameDefinition>.Is.Anything, 
                Arg<ApplicationUser>.Is.Anything))
                .IgnoreArguments()
                .Throw(new DbUpdateException());

            Exception exception = Assert.Throws<DuplicateKeyException>(
                () => autoMocker.ClassUnderTest.Save(gameDefinition, currentUser));

            Assert.That(exception.Message, Is.EqualTo("A Game Definition with name '" + gameDefinition.Name + "' already exists in this Gaming Group."));
        }


        [Test]
        public void ItSetsTheGameDefinitionName()
        {
            var gameDefinition = new GameDefinition
            {
                Name = "game definition name"
            };

            autoMocker.ClassUnderTest.Save(gameDefinition, currentUser);

            autoMocker.Get<IDataContext>().AssertWasCalled(mock => mock.Save(
                Arg<GameDefinition>.Matches(newGameDefinition => newGameDefinition.Name == gameDefinition.Name),
                Arg<ApplicationUser>.Is.Anything));
        }

        [Test]
        public void ItSetsTheGameDefinitionDescription()
        {
            var gameDefinition = new GameDefinition
            {
                Name = "game definition name",
                Description = "game description"
            };

            autoMocker.ClassUnderTest.Save(gameDefinition, currentUser);

            autoMocker.Get<IDataContext>().AssertWasCalled(mock => mock.Save(
                Arg<GameDefinition>.Matches(newGameDefinition => newGameDefinition.Description == gameDefinition.Description),
                Arg<ApplicationUser>.Is.Anything));
        }

        [Test]
        public void ItRecordsAGameDefinitionCreatedEvent()
        {
            var gameDefinition = MockRepository.GeneratePartialMock<GameDefinition>();
            gameDefinition.Name = "name";
            gameDefinition.Expect(mock => mock.AlreadyInDatabase())
                .Return(false);

            autoMocker.ClassUnderTest.Save(gameDefinition, currentUser);

            //TODO this test has race conditions and may fail at random
            autoMocker.Get<INemeStatsEventTracker>().AssertWasCalled(mock => mock.TrackGameDefinitionCreation(currentUser, gameDefinition.Name));
        }

        [Test]
        public void ItDoesNotRecordsAGameDefinitionCreatedEventIfTheGameDefinitionIsNotNew()
        {
            var gameDefinition = MockRepository.GeneratePartialMock<GameDefinition>();
            gameDefinition.Name = "name";
            gameDefinition.Expect(mock => mock.AlreadyInDatabase())
                .Return(true);

            autoMocker.ClassUnderTest.Save(gameDefinition, currentUser);

            autoMocker.Get<INemeStatsEventTracker>().AssertWasNotCalled(mock => mock.TrackGameDefinitionCreation(currentUser, gameDefinition.Name));
        }
    }
}
