using BusinessLogic.DataAccess;
using BusinessLogic.Logic.GameDefinitions;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Tests.UnitTests.LogicTests.GameDefinitionsTests.GameDefinitionRetrieverTests
{
    [TestFixture]
    public class GameDefinitionRetrieverTestBase
    {
        protected IDataContext dataContext;
        protected ApplicationUser currentUser;
        protected IGameDefinitionRetriever retriever;

        [SetUp]
        public void SetUp()
        {
            dataContext = MockRepository.GenerateMock<IDataContext>();
            currentUser = new ApplicationUser()
            {
                Id = "user id",
                CurrentGamingGroupId = 123
            };
            retriever = new GameDefinitionRetriever(dataContext);
        }
    }
}
