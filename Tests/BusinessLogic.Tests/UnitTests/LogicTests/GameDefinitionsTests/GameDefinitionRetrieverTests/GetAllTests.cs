using BusinessLogic.Models;
using NUnit.Framework;
using Rhino.Mocks;
using System.Collections.Generic;
using System.Linq;

namespace BusinessLogic.Tests.UnitTests.LogicTests.GameDefinitionsTests.GameDefinitionRetrieverTests
{
    [TestFixture]
    public class GetAllTests : GameDefinitionRetrieverTestBase
    {
        [Test]
        public void ItOnlyReturnsActiveGameDefinitions()
        {
            dataContext.Expect(mock => mock.GetQueryable<GameDefinition>())
                .Repeat.Once()
                .Return(gameDefinitionQueryable);

            IList<GameDefinition> gameDefinitions = retriever.GetAllGameDefinitions(currentUser.CurrentGamingGroupId.Value);

            Assert.True(gameDefinitions.All(gameDefinition => gameDefinition.Active));
        }

        [Test]
        public void ItOnlyReturnsGameDefinitionsForTheCurrentUsersGamingGroup()
        {
            dataContext.Expect(mock => mock.GetQueryable<GameDefinition>())
                .Repeat.Once()
                .Return(gameDefinitionQueryable);

            IList<GameDefinition> gameDefinitions = retriever.GetAllGameDefinitions(currentUser.CurrentGamingGroupId.Value);

            Assert.True(gameDefinitions.All(gameDefinition => gameDefinition.GamingGroupId == currentUser.CurrentGamingGroupId));
        }
    }
}
