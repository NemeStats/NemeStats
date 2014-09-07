using BusinessLogic.DataAccess;
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

namespace BusinessLogic.Tests.UnitTests.LogicTests.GameDefinitionsTests.GameDefinitionRetrieverImplTests
{
    [TestFixture]
    public class GetAllTests : GameDefinitionRetrieverImplTestBase
    {
        [Test]
        public void ItReturnsAListOfAllGameDefinitions()
        {
            IQueryable<GameDefinition> gameDefinitionQueryable = new List<GameDefinition>().AsQueryable();
            dataContext.Expect(mock => mock.GetQueryable<GameDefinition>())
                .Repeat.Once()
                .Return(gameDefinitionQueryable);

            IList<GameDefinition> gameDefinitions = retriever.GetAllGameDefinitions(currentUser.CurrentGamingGroupId.Value);

            Assert.AreEqual(gameDefinitionQueryable.ToList(), gameDefinitions);
        }
    }
}
