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

using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using BoardGameGeekApiClient.Interfaces;
using BoardGameGeekApiClient.Models;
using BusinessLogic.DataAccess;
using BusinessLogic.EventTracking;
using BusinessLogic.Exceptions;
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
        public void ItThrowsADuplicateKeyExceptionIfThereIsAnExistingActiveGameDefinitionWithTheSameName()
        {
            var gameDefinition = new GameDefinition
            {
                Name = "existing game definition name",
                Active = true,
                GamingGroupId = currentUser.CurrentGamingGroupId.Value
            };
            var gameDefinitionQueryable = new List<GameDefinition>
            {
                gameDefinition
            }.AsQueryable();

            autoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<GameDefinition>()).Return(gameDefinitionQueryable);

            Exception exception = Assert.Throws<DuplicateKeyException>(
                () => autoMocker.ClassUnderTest.Save(gameDefinition, currentUser));

            Assert.That(exception.Message, Is.EqualTo("An active Game Definition with name '" + gameDefinition.Name + "' already exists in this Gaming Group."));
        }

        [Test]
        public void AnExistingInactiveGameDefinitionIsReactivatedIfSomeoneTriesToAddItAgain()
        {
            var existingGameDefinition = new GameDefinition
            {
                Id = 1,
                Name = "existing game definition name",
                GamingGroupId = currentUser.CurrentGamingGroupId.Value,
                Active = false
            };
            var gameDefinitionQueryable = new List<GameDefinition>
            {
                existingGameDefinition
            }.AsQueryable();
            var newGameDefinition = new GameDefinition
            {
                Name = existingGameDefinition.Name,
                GamingGroupId = existingGameDefinition.GamingGroupId
            };
            autoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<GameDefinition>()).Return(gameDefinitionQueryable); 

            autoMocker.ClassUnderTest.Save(newGameDefinition, currentUser);

            var gameDefinitionThatWasSaved =
                (GameDefinition)autoMocker.Get<IDataContext>().GetArgumentsForCallsMadeOn(mock => mock.Save(Arg<GameDefinition>.Is.Anything, Arg<ApplicationUser>.Is.Anything))[0][0];

            Assert.That(gameDefinitionThatWasSaved.Id, Is.EqualTo(existingGameDefinition.Id));
            Assert.That(gameDefinitionThatWasSaved.BoardGameGeekObjectId, Is.EqualTo(existingGameDefinition.BoardGameGeekObjectId));
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
                GamingGroupId = currentUser.CurrentGamingGroupId.Value
            };
            var gameDefinitionQueryable = new List<GameDefinition>
            {
                existingGameDefinition
            }.AsQueryable();
            var newGameDefinition = new GameDefinition
            {
                Name = existingGameDefinition.Name,
                Description = "new description"
            };
            autoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<GameDefinition>()).Return(gameDefinitionQueryable);

            autoMocker.ClassUnderTest.Save(newGameDefinition, currentUser);

            var gameDefinitionThatWasSaved =
                (GameDefinition)autoMocker.Get<IDataContext>().GetArgumentsForCallsMadeOn(mock => mock.Save(Arg<GameDefinition>.Is.Anything, Arg<ApplicationUser>.Is.Anything))[0][0];

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
                GamingGroupId = currentUser.CurrentGamingGroupId.Value
            };
            var gameDefinitionQueryable = new List<GameDefinition>
            {
                existingGameDefinition
            }.AsQueryable();
            var newGameDefinition = new GameDefinition
            {
                Name = existingGameDefinition.Name
            };
            autoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<GameDefinition>()).Return(gameDefinitionQueryable);

            autoMocker.ClassUnderTest.Save(newGameDefinition, currentUser);

            var gameDefinitionThatWasSaved =
                (GameDefinition)autoMocker.Get<IDataContext>().GetArgumentsForCallsMadeOn(mock => mock.Save(Arg<GameDefinition>.Is.Anything, Arg<ApplicationUser>.Is.Anything))[0][0];

            Assert.That(gameDefinitionThatWasSaved.Description, Is.EqualTo(existingGameDefinition.Description));
        }


        [Test]
        public void ItSetsTheGameDefinitionName()
        {
            var gameDefinition = new GameDefinition
            {
                Name = "game definition name"
            };
            autoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<GameDefinition>()).Return(new List<GameDefinition>().AsQueryable());

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
            autoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<GameDefinition>()).Return(new List<GameDefinition>().AsQueryable());

            autoMocker.ClassUnderTest.Save(gameDefinition, currentUser);

            autoMocker.Get<IDataContext>().AssertWasCalled(mock => mock.Save(
                Arg<GameDefinition>.Matches(newGameDefinition => newGameDefinition.Description == gameDefinition.Description),
                Arg<ApplicationUser>.Is.Anything));
        }

        [Test]
        [Explicit("This test has race conditions and sometimes fails")]
        public void ItRecordsAGameDefinitionCreatedEvent()
        {
            var gameDefinition = MockRepository.GeneratePartialMock<GameDefinition>();
            gameDefinition.Name = "name";
            gameDefinition.Expect(mock => mock.AlreadyInDatabase())
                .Return(false);

            autoMocker.ClassUnderTest.Save(gameDefinition, currentUser);

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

        [Test]
        public void ItSetsTheBoardGameGeekThumbnailIfThereIsOne()
        {
            var gameDefinition = MockRepository.GeneratePartialMock<GameDefinition>();
            gameDefinition.Name = "name";
            gameDefinition.BoardGameGeekObjectId = 1;
            gameDefinition.Expect(mock => mock.AlreadyInDatabase())
                .Return(false);
            var expectedGameDetails = new GameDetails
            {
                Thumbnail = "some thumbnail URL"
            };

            autoMocker.Get<IBoardGameGeekApiClient>().Expect(mock => mock.GetGameDetails(gameDefinition.BoardGameGeekObjectId.Value))
                      .Return(expectedGameDetails);
            autoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<GameDefinition>()).Return(new List<GameDefinition>().AsQueryable());

            autoMocker.ClassUnderTest.Save(gameDefinition, currentUser);

            autoMocker.Get<IDataContext>().AssertWasCalled(mock => mock.Save(
                         Arg<GameDefinition>.Matches(newGameDefinition => newGameDefinition.ThumbnailImageUrl == expectedGameDetails.Thumbnail),
                         Arg<ApplicationUser>.Is.Anything));
        }

        [Test]
        public void ItDoesNotSetAThumbnailIfTheClientDoesntReturnAGameDefinition()
        {
            var gameDefinition = MockRepository.GeneratePartialMock<GameDefinition>();
            gameDefinition.Name = "name";
            gameDefinition.BoardGameGeekObjectId = 1;
            gameDefinition.Expect(mock => mock.AlreadyInDatabase())
                .Return(false);

            autoMocker.Get<IBoardGameGeekApiClient>().Expect(mock => mock.GetGameDetails(gameDefinition.BoardGameGeekObjectId.Value))
                      .Return(null);
            autoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<GameDefinition>()).Return(new List<GameDefinition>().AsQueryable());

            autoMocker.ClassUnderTest.Save(gameDefinition, currentUser);

            autoMocker.Get<IDataContext>().AssertWasCalled(mock => mock.Save(
                         Arg<GameDefinition>.Matches(newGameDefinition => newGameDefinition.ThumbnailImageUrl == null),
                         Arg<ApplicationUser>.Is.Anything)); 
        }

    }
}
