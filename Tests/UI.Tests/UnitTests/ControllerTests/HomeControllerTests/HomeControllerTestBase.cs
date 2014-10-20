using BusinessLogic.Logic.PlayedGames;
using BusinessLogic.Logic.Players;
using NUnit.Framework;
using Rhino.Mocks;
using System.Linq;
using UI.Controllers;
using UI.Transformations.PlayerTransformations;

namespace UI.Tests.UnitTests.ControllerTests.HomeControllerTests
{
    [TestFixture]
    public class HomeControllerTestBase
    {
        protected HomeController homeControllerPartialMock;
        protected IPlayerSummaryBuilder playerSummaryBuilderMock;
        protected ITopPlayerViewModelBuilder viewModelBuilderMock;
        protected IPlayedGameRetriever playedGameRetriever;

        [SetUp]
        public void SetUp()
        {
            playerSummaryBuilderMock = MockRepository.GenerateMock<IPlayerSummaryBuilder>();
            viewModelBuilderMock = MockRepository.GenerateMock<ITopPlayerViewModelBuilder>();
            playedGameRetriever = MockRepository.GenerateMock<IPlayedGameRetriever>();
            homeControllerPartialMock = MockRepository.GeneratePartialMock<HomeController>(
                playerSummaryBuilderMock, 
                viewModelBuilderMock,
                playedGameRetriever);
        }
    }
}
