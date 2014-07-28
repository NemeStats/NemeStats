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
    public class GetAllTests
    {
        protected ApplicationDataContext dataContext;
        protected ApplicationUser currentUser;
        protected GameDefinitionRetriever retriever;

        [SetUp]
        public void SetUp()
        {
            dataContext = MockRepository.GenerateMock<ApplicationDataContext>();
            currentUser = new ApplicationUser()
            {
                Id = "user id",
                CurrentGamingGroupId = 123
            };
            retriever = new GameDefinitionRetrieverImpl(dataContext);
        }

        [Test]
        public void ItReturnsAListOfAllGameDefinitions()
        {
            IQueryable<GameDefinition> gameDefinitionQueryable = new List<GameDefinition>().AsQueryable();
            dataContext.Expect(mock => mock.GetQueryable<GameDefinition>(currentUser))
                .Repeat.Once()
                .Return(gameDefinitionQueryable);

            IList<GameDefinition> gameDefinitions = retriever.GetAllGameDefinitions(currentUser);

            Assert.AreEqual(gameDefinitionQueryable.ToList(), gameDefinitions);
        }
    }
}
