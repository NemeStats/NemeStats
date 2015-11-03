﻿using BusinessLogic.DataAccess;
using BusinessLogic.Logic.GameDefinitions;
using BusinessLogic.Models;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;

namespace BusinessLogic.Tests.UnitTests.LogicTests.GameDefinitionsTests.GameDefinitionSaverTests
{
    [TestFixture]
    public class UpdateGameDefinitionTests : GameDefinitionSaverTestBase
    {
        protected GameDefinition expectedGameDefinition;
        protected const int GAME_DEFINITION_ID = 1;

        [SetUp]
        public void SetUp()
        {
            expectedGameDefinition = new GameDefinition
            {
                Id = GAME_DEFINITION_ID,
                Name = "some old game definition name",
                BoardGameGeekGameDefinitionId = -1
            };
            autoMocker.Get<IDataContext>().Expect(mock => mock.FindById<GameDefinition>(Arg<int>.Is.Anything)).Return(expectedGameDefinition);
        }

        [Test]
        public void ItUpdatesTheActiveFlag()
        {
            var gameDefinitionUpdateRequest = new GameDefinitionUpdateRequest
            {
                Active = true
            };
            autoMocker.ClassUnderTest.UpdateGameDefinition(gameDefinitionUpdateRequest, currentUser);

            autoMocker.ClassUnderTest.AssertWasCalled(
                partialMock => partialMock.CreateGameDefinition(Arg<GameDefinition>.Matches(
                    gameDefinition => gameDefinition.Active == gameDefinitionUpdateRequest.Active), 
                    Arg<ApplicationUser>.Is.Same(currentUser)));
        }

        [Test]
        public void ItUpdatesTheGameDefinitionName()
        {
            var gameDefinitionUpdateRequest = new GameDefinitionUpdateRequest
            {
                Name = "some new game definition name"
            };
            autoMocker.ClassUnderTest.UpdateGameDefinition(gameDefinitionUpdateRequest, currentUser);

            autoMocker.ClassUnderTest.AssertWasCalled(
                partialMock => partialMock.CreateGameDefinition(Arg<GameDefinition>.Matches(
                    gameDefinition => gameDefinition.Name == gameDefinitionUpdateRequest.Name),
                    Arg<ApplicationUser>.Is.Same(currentUser)));
        }

        [Test]
        public void ItUpdatesTheBoardGameGeekGameDefinitionId()
        {
            var gameDefinitionUpdateRequest = new GameDefinitionUpdateRequest
            {
                BoardGameGeekGameDefinitionId = 200
            };
            autoMocker.ClassUnderTest.UpdateGameDefinition(gameDefinitionUpdateRequest, currentUser);

            autoMocker.ClassUnderTest.AssertWasCalled(
                partialMock => partialMock.CreateGameDefinition(Arg<GameDefinition>.Matches(
                    gameDefinition => gameDefinition.BoardGameGeekGameDefinitionId == gameDefinitionUpdateRequest.BoardGameGeekGameDefinitionId),
                    Arg<ApplicationUser>.Is.Same(currentUser)));
        }
    }
}
