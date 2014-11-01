using BusinessLogic.Logic.Nemeses;
using BusinessLogic.Logic.PlayedGames;
using BusinessLogic.Logic.Players;
using NUnit.Framework;
using Rhino.Mocks;
using System.Linq;
using UI.Controllers;
using UI.Transformations.PlayerTransformations;
using UI.Transformations;

namespace UI.Tests.UnitTests.ControllerTests.HomeControllerTests
{
    [TestFixture]
    public class HomeControllerTestBase
    {
        protected HomeController homeControllerPartialMock;
        protected IPlayerSummaryBuilder playerSummaryBuilderMock;
        protected ITopPlayerViewModelBuilder viewModelBuilderMock;
        protected IPlayedGameRetriever playedGameRetriever;
        protected INemesisHistoryRetriever nemesisHistoryRetriever;
        protected INemesisChangeViewModelBuilder nemesisChangeViewModelBuilder;

        [SetUp]
        public virtual void SetUp()
        {
            playerSummaryBuilderMock = MockRepository.GenerateMock<IPlayerSummaryBuilder>();
            viewModelBuilderMock = MockRepository.GenerateMock<ITopPlayerViewModelBuilder>();
            playedGameRetriever = MockRepository.GenerateMock<IPlayedGameRetriever>();
            nemesisHistoryRetriever = MockRepository.GenerateMock<INemesisHistoryRetriever>();
            nemesisChangeViewModelBuilder = MockRepository.GenerateMock<INemesisChangeViewModelBuilder>();
            homeControllerPartialMock = MockRepository.GeneratePartialMock<HomeController>(
                playerSummaryBuilderMock, 
                viewModelBuilderMock,
                playedGameRetriever,
                nemesisHistoryRetriever,
                nemesisChangeViewModelBuilder);
        }
    }
}
