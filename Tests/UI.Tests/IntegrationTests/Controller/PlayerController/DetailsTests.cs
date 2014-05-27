using BusinessLogic.DataAccess;
using BusinessLogic.Logic;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UI.Tests.UnitTests.Controller
{
    [TestFixture]
    public class PlayerControllerTests
    {
        private NerdScorekeeperDbContext dbContext;
        private PlayerLogic playerLogicMock;

        [TestFixtureSetUp]
        public void SetUp()
        {
            dbContext = MockRepository.GenerateMock<NerdScorekeeperDbContext>();
            playerLogicMock = MockRepository.GenerateMock<PlayerLogic>();
        }

        [Test]
        public void ItGetsThePlayerDetails()
        {
            //TODO NEED GRANT HELP TO UNDERSTAND HOW TO TEST THIS
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            dbContext.Dispose();
        }
  
    }
}
