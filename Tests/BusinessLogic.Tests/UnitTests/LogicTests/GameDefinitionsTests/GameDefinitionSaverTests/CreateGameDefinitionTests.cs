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
using BusinessLogic.Models.Games;
using BusinessLogic.Logic.BoardGameGeek;

namespace BusinessLogic.Tests.UnitTests.LogicTests.GameDefinitionsTests.GameDefinitionSaverTests
{
    [TestFixture]
    public class CreateGameDefinitionTests : GameDefinitionSaverTestBase
    {
        [Test]
        public void ItThrowsAnArgumentNullExceptionIfTheModelIsNull()
        {
            var expectedException = new ArgumentNullException("createGameDefinitionRequest");

            Exception exception = Assert.Throws<ArgumentNullException>(() => autoMocker.ClassUnderTest.CreateGameDefinition((CreateGameDefinitionRequest)null, currentUser));

            Assert.AreEqual(expectedException.Message, exception.Message);
        }

        [Test]
        public void ItThrowsAnArgumentExceptionIfTheNameIsNull()
        {
            var expectedException = new ArgumentException("createGameDefinitionRequest.Name cannot be null or whitespace.");
            var gameDefinition = new CreateGameDefinitionRequest
            { Name = null };

            Exception exception = Assert.Throws<ArgumentException>(() => autoMocker.ClassUnderTest.CreateGameDefinition(gameDefinition, currentUser));

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
                () => autoMocker.ClassUnderTest.CreateGameDefinition(gameDefinition, currentUser));

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
            CreateGameDefinitionRequest createGameDefinitionRequest = new CreateGameDefinitionRequest
            {
                Name = gameDefinition.Name
            };
            var gameDefinitionQueryable = new List<GameDefinition>
            {
                gameDefinition
            }.AsQueryable();

            autoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<GameDefinition>()).Return(gameDefinitionQueryable);

            Exception exception = Assert.Throws<DuplicateKeyException>(
                () => autoMocker.ClassUnderTest.CreateGameDefinition(createGameDefinitionRequest, currentUser));

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
            var newGameDefinition = new CreateGameDefinitionRequest 
            {
                Name = existingGameDefinition.Name
            };
            autoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<GameDefinition>()).Return(gameDefinitionQueryable); 

            autoMocker.ClassUnderTest.CreateGameDefinition(newGameDefinition, currentUser);

            var gameDefinitionThatWasSaved =
                (GameDefinition)autoMocker.Get<IDataContext>().GetArgumentsForCallsMadeOn(mock => mock.Save(Arg<GameDefinition>.Is.Anything, Arg<ApplicationUser>.Is.Anything))[0][0];

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
                GamingGroupId = currentUser.CurrentGamingGroupId.Value
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
            autoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<GameDefinition>()).Return(gameDefinitionQueryable);

            autoMocker.ClassUnderTest.CreateGameDefinition(newGameDefinition, currentUser);

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
            var newGameDefinition = new CreateGameDefinitionRequest
            {
                Name = existingGameDefinition.Name
            };
            autoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<GameDefinition>()).Return(gameDefinitionQueryable);

            autoMocker.ClassUnderTest.CreateGameDefinition(newGameDefinition, currentUser);

            var gameDefinitionThatWasSaved =
                (GameDefinition)autoMocker.Get<IDataContext>().GetArgumentsForCallsMadeOn(mock => mock.Save(Arg<GameDefinition>.Is.Anything, Arg<ApplicationUser>.Is.Anything))[0][0];

            Assert.That(gameDefinitionThatWasSaved.Description, Is.EqualTo(existingGameDefinition.Description));
        }

        [Test]
        public void ItSavesANewGameDefinition()
        {
            var createGameDefinitionRequest = new CreateGameDefinitionRequest
            {
                Name = "game definition name",
                Active = true,
                BoardGameGeekGameDefinitionId = 12,
                Description = "some description"
            };
            autoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<GameDefinition>()).Return(new List<GameDefinition>().AsQueryable());
            autoMocker.Get<IBoardGameGeekGameDefinitionAttacher>().Expect(mock => mock.CreateBoardGameGeekGameDefinition(Arg<int?>.Is.Anything)).Return(null);

            autoMocker.ClassUnderTest.CreateGameDefinition(createGameDefinitionRequest, currentUser);

            var args = autoMocker.Get<IDataContext>().GetArgumentsForCallsMadeOn(mock => mock.Save(
                Arg<GameDefinition>.Is.Anything,
                Arg<ApplicationUser>.Is.Anything));
            var actualGameDefinition = args[0][0] as GameDefinition;
            Assert.That(actualGameDefinition, Is.Not.Null);
            Assert.That(actualGameDefinition.Name, Is.EqualTo(createGameDefinitionRequest.Name));

        }

        [Test]
        [Explicit("This test has race conditions and sometimes fails")]
        public void zzz_ItRecordsAGameDefinitionCreatedEvent()
        {
            var gameDefinition = new CreateGameDefinitionRequest
            {
                Name = "some name"
            };

            autoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<GameDefinition>()).Return(new List<GameDefinition>().AsQueryable());

            autoMocker.ClassUnderTest.CreateGameDefinition(gameDefinition, currentUser);

            autoMocker.Get<INemeStatsEventTracker>().AssertWasCalled(mock => mock.TrackGameDefinitionCreation(currentUser, gameDefinition.Name));
        }

        [Test]
        public void ItDoesNotRecordAGameDefinitionCreatedEventIfTheGameDefinitionIsNotNew()
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
            var newGameDefinition = new CreateGameDefinitionRequest
            {
                Name = existingGameDefinition.Name
            };
            autoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<GameDefinition>()).Return(gameDefinitionQueryable); ;

            autoMocker.ClassUnderTest.CreateGameDefinition(newGameDefinition, currentUser);

            autoMocker.Get<INemeStatsEventTracker>().AssertWasNotCalled(mock => mock.TrackGameDefinitionCreation(currentUser, newGameDefinition.Name));
        }

        [Test]
        public void ItAttachesToABoardGameGeekGameDefinitionIfItHasABoardGameGeekGameDefinitionId()
        {
            var createGameDefinitionRequest = new CreateGameDefinitionRequest
            {
                Name = "some new game name",
                BoardGameGeekGameDefinitionId = 1
            };
            autoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<GameDefinition>()).Return(new List<GameDefinition>().AsQueryable());
            const int EXPECTED_BGG_ID = 2;
            var expectedBoardGameGeekGameDefinition = new BoardGameGeekGameDefinition
            {
                Id = 2
            };
            autoMocker.Get<IBoardGameGeekGameDefinitionAttacher>().Expect(mock => mock.CreateBoardGameGeekGameDefinition(createGameDefinitionRequest.BoardGameGeekGameDefinitionId)).Return(EXPECTED_BGG_ID);

            autoMocker.ClassUnderTest.CreateGameDefinition(createGameDefinitionRequest, currentUser);

            var args = autoMocker.Get<IDataContext>().GetArgumentsForCallsMadeOn(
                dataContext => dataContext.Save(Arg<GameDefinition>.Is.Anything, Arg<ApplicationUser>.Is.Anything));

            var actualGameDefinition = args[0][0] as GameDefinition;
            Assert.That(actualGameDefinition, Is.Not.Null);
            Assert.That(actualGameDefinition.BoardGameGeekGameDefinitionId, Is.EqualTo(EXPECTED_BGG_ID));
        }
    }
}
