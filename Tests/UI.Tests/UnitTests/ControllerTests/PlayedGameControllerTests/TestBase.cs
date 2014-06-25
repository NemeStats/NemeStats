using BusinessLogic.DataAccess;
using BusinessLogic.Models;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UI.Controllers;
using UI.Transformations;

namespace UI.Tests.UnitTests.ControllerTests.PlayedGameControllerTests
{
    public class TestBase
    {
        protected NemeStatsDbContext dbContexMock;
        protected PlayedGameController playedGameController;
        protected PlayedGameController playedGameControllerPartialMock;
        protected PlayedGameLogic playedGameLogicMock;
        protected PlayerLogic playerLogicMock;
        protected PlayedGameDetailsViewModelBuilder playedGameDetailsBuilder;

        [SetUp]
        public virtual void TestSetUp()
        {
            dbContexMock = MockRepository.GenerateMock<NemeStatsDbContext>();
            playedGameLogicMock = MockRepository.GenerateMock<PlayedGameLogic>();
            playerLogicMock = MockRepository.GenerateMock<PlayerLogic>();
            playedGameDetailsBuilder = MockRepository.GenerateMock<PlayedGameDetailsViewModelBuilder>();
            playedGameController = new Controllers.PlayedGameController(dbContexMock, playedGameLogicMock, playerLogicMock, playedGameDetailsBuilder);
            playedGameControllerPartialMock = MockRepository.GeneratePartialMock<PlayedGameController>(dbContexMock, playedGameLogicMock, playerLogicMock, playedGameDetailsBuilder);
        }
    }
}
