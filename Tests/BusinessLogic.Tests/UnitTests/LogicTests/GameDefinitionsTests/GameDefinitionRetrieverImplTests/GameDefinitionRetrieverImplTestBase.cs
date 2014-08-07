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

namespace BusinessLogic.Tests.UnitTests.LogicTests.GameDefinitionsTests.GameDefinitionRetrieverImplTests
{
    [TestFixture]
    public class GameDefinitionRetrieverImplTestBase
    {
        protected DataContext dataContext;
        protected ApplicationUser currentUser;
        protected GameDefinitionRetriever retriever;

        [SetUp]
        public void SetUp()
        {
            dataContext = MockRepository.GenerateMock<DataContext>();
            currentUser = new ApplicationUser()
            {
                Id = "user id",
                CurrentGamingGroupId = 123
            };
            retriever = new GameDefinitionRetrieverImpl(dataContext);
        }
    }
}
