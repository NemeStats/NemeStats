using System.Collections.Generic;
using System.Linq;
using BusinessLogic.DataAccess;
using BusinessLogic.Models;
using NUnit.Framework;
using Rhino.Mocks;

namespace BusinessLogic.Tests.UnitTests.LogicTests.GameDefinitionsTests.GameDefinitionRetrieverTests
{
    [TestFixture]
    public class GetAllGameDefinitionNamesTests : GameDefinitionRetrieverTestBase
    {
        [Test]
        public void ItReturnsOnlyActiveGameDefinitions()
        {
            const int ACTIVE_GAME_DEFINITION_ID = 1;
            var gameDefinitionList = new List<GameDefinition>
            {
                new GameDefinition
                {
                    Id = ACTIVE_GAME_DEFINITION_ID,
                    Active = true,
                    GamingGroupId = currentUser.CurrentGamingGroupId.Value
                },
                new GameDefinition
                {
                    Id = ACTIVE_GAME_DEFINITION_ID + 1,
                    Active = false,
                    GamingGroupId = currentUser.CurrentGamingGroupId.Value
                }
            };

            autoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<GameDefinition>()).Return(gameDefinitionList.AsQueryable());

            var results = autoMocker.ClassUnderTest.GetAllGameDefinitionNames(currentUser);

            Assert.That(results.Count, Is.EqualTo(1));
            Assert.That(results[0].Id, Is.EqualTo(ACTIVE_GAME_DEFINITION_ID));
        }

        [Test]
        public void ItReturnsOnlyGameDefinitionsInTheCurrentUsersGamingGroup()
        {
            const int VALID_GAME_DEFINITION_ID = 1;
            var gameDefinitionList = new List<GameDefinition>
            {
                new GameDefinition
                {
                    Id = VALID_GAME_DEFINITION_ID,
                    Active = true,
                    GamingGroupId = currentUser.CurrentGamingGroupId.Value
                },
                new GameDefinition
                {
                    Id = VALID_GAME_DEFINITION_ID + 1,
                    Active = true,
                    GamingGroupId = currentUser.CurrentGamingGroupId.Value + 1
                }
            };

            autoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<GameDefinition>()).Return(gameDefinitionList.AsQueryable());

            var results = autoMocker.ClassUnderTest.GetAllGameDefinitionNames(currentUser);

            Assert.That(results.Count, Is.EqualTo(1));
            Assert.That(results[0].Id, Is.EqualTo(VALID_GAME_DEFINITION_ID));
        }

        [Test]
        public void ItSetsTheGameDefinitionNameAndId()
        {
            const int ACTIVE_GAME_DEFINITION_ID = 1;
            var gameDefinitionList = new List<GameDefinition>
            {
                new GameDefinition
                {
                    Id = ACTIVE_GAME_DEFINITION_ID,
                    Name = "some game definitionName",
                    GamingGroupId = currentUser.CurrentGamingGroupId.Value,
                    Active = true
                }
            };

            autoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<GameDefinition>()).Return(gameDefinitionList.AsQueryable());

            var results = autoMocker.ClassUnderTest.GetAllGameDefinitionNames(currentUser);

            Assert.That(results[0].Name, Is.EqualTo(gameDefinitionList[0].Name));
            Assert.That(results[0].Id, Is.EqualTo(gameDefinitionList[0].Id));
        }

    }
}
