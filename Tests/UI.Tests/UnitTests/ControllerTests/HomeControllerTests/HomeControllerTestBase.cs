using BusinessLogic.Logic.GamingGroups;
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
        protected ITopPlayerViewModelBuilder topPlayerViewModelBuilderMock;
        protected IPlayedGameRetriever playedGameRetrieverMock;
        protected INemesisHistoryRetriever nemesisHistoryRetrieverMock;
        protected INemesisChangeViewModelBuilder nemesisChangeViewModelBuilderMock;
        protected IGamingGroupRetriever gamingGroupRetrieverMock;
        protected TopGamingGroupSummaryViewModelBuilder topGamingGroupSummaryViewModelBuilderMock;

        [SetUp]
        public virtual void SetUp()
        {
            playerSummaryBuilderMock = MockRepository.GenerateMock<IPlayerSummaryBuilder>();
            topPlayerViewModelBuilderMock = MockRepository.GenerateMock<ITopPlayerViewModelBuilder>();
            playedGameRetrieverMock = MockRepository.GenerateMock<IPlayedGameRetriever>();
            nemesisHistoryRetrieverMock = MockRepository.GenerateMock<INemesisHistoryRetriever>();
            nemesisChangeViewModelBuilderMock = MockRepository.GenerateMock<INemesisChangeViewModelBuilder>();
            gamingGroupRetrieverMock = MockRepository.GenerateMock<IGamingGroupRetriever>();
            topGamingGroupSummaryViewModelBuilderMock = MockRepository.GenerateMock<TopGamingGroupSummaryViewModelBuilder>();
            homeControllerPartialMock = MockRepository.GeneratePartialMock<HomeController>(
                playerSummaryBuilderMock, 
                topPlayerViewModelBuilderMock,
                playedGameRetrieverMock,
                nemesisHistoryRetrieverMock,
                nemesisChangeViewModelBuilderMock,
                gamingGroupRetrieverMock,
                topGamingGroupSummaryViewModelBuilderMock);
        }
    }
}
