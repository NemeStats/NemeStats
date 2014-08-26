using BusinessLogic.DataAccess.Repositories;
using BusinessLogic.Logic.Players;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UI.Controllers;
using UI.Transformations.Player;

namespace UI.Tests.UnitTests.ControllerTests.HomeControllerTests
{
    [TestFixture]
    public class HomeControllerTestBase
    {
        protected HomeController homeControllerPartialMock;
        protected PlayerSummaryBuilder playerSummaryBuilderMock;
        protected TopPlayerViewModelBuilder viewModelBuilderMock;
        protected PlayedGameRepository playedGameRepository;

        [SetUp]
        public void SetUp()
        {
            playerSummaryBuilderMock = MockRepository.GenerateMock<PlayerSummaryBuilder>();
            viewModelBuilderMock = MockRepository.GenerateMock<TopPlayerViewModelBuilder>();
            playedGameRepository = MockRepository.GenerateMock<PlayedGameRepository>();
            homeControllerPartialMock = MockRepository.GeneratePartialMock<HomeController>(
                playerSummaryBuilderMock, 
                viewModelBuilderMock,
                playedGameRepository);
        }
    }
}
