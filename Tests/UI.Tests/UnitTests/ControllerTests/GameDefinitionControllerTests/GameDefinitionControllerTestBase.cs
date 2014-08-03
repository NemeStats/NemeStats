using BusinessLogic.DataAccess;
using BusinessLogic.DataAccess.Repositories;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UI.Controllers;

namespace UI.Tests.UnitTests.ControllerTests.GameDefinitionControllerTests
{
    [TestFixture]
    public class GameDefinitionControllerTestBase
    {
        protected GameDefinitionController gameDefinitionControllerPartialMock;
        protected GameDefinitionRepository gameDefinitionRepository;
        protected NemeStatsDataContext dataContext;
        protected ApplicationUser currentUser;

        [SetUp]
        public void SetUp()
        {
            gameDefinitionRepository = MockRepository.GenerateMock<GameDefinitionRepository>();
            dataContext = MockRepository.GenerateMock<NemeStatsDataContext>();
            gameDefinitionControllerPartialMock = MockRepository.GeneratePartialMock<GameDefinitionController>(dataContext, gameDefinitionRepository);
            currentUser = new ApplicationUser()
            {
                Id = "user id",
                CurrentGamingGroupId = 15151
            };
        }
    }
}
