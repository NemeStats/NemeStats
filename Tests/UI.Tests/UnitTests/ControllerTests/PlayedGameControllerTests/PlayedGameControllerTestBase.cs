using BusinessLogic.DataAccess;
using BusinessLogic.Logic.GameDefinitions;
using BusinessLogic.Logic.PlayedGames;
using BusinessLogic.Logic.Players;
using BusinessLogic.Models;
using BusinessLogic.Models.Games;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using System.Collections.Generic;
using System.Web.Mvc;
using UI.Controllers;
using UI.Controllers.Helpers;
using UI.Models.PlayedGame;
using UI.Transformations;

namespace UI.Tests.UnitTests.ControllerTests.PlayedGameControllerTests
{
	public class PlayedGameControllerTestBase
	{
		protected NemeStatsDataContext dataContext;
		protected PlayedGameController playedGameController;
		protected PlayedGameController playedGameControllerPartialMock;
		protected IPlayedGameRetriever playedGameRetriever;
		protected IPlayerRetriever playerRetrieverMock;
		protected IPlayedGameDetailsViewModelBuilder playedGameDetailsBuilderMock;
		protected IGameDefinitionRetriever gameDefinitionRetrieverMock;
		protected IPlayedGameCreator playedGameCreatorMock;
		protected IShowingXResultsMessageBuilder showingXResultsMessageBuilderMock;
		protected IPlayedGameDeleter playedGameDeleterMock;
		protected UrlHelper urlHelperMock;
		protected string testUserName = "the test user name";
		protected ApplicationUser currentUser;
		protected List<GameDefinitionSummary> gameDefinitionSummaries;
		protected List<PublicGameSummary> expectedViewModel;
		protected PlayedGameEditViewModel expectedDefaultCompletedGameViewModel;
		protected PlayedGameEditViewModel expectedPopulatedCompletedGameViewModel;
		protected List<Player> playerList;
		protected List<SelectListItem> playerSelectList;
		protected List<GameDefinition> gameDefinitionList;
		protected List<SelectListItem> gameDefinitionSelectList;

		[SetUp]
		public virtual void TestSetUp()
		{
			dataContext = MockRepository.GenerateMock<NemeStatsDataContext>();
			playedGameRetriever = MockRepository.GenerateMock<IPlayedGameRetriever>();
			playerRetrieverMock = MockRepository.GenerateMock<IPlayerRetriever>();
			playedGameDetailsBuilderMock = MockRepository.GenerateMock<IPlayedGameDetailsViewModelBuilder>();
			gameDefinitionRetrieverMock = MockRepository.GenerateMock<IGameDefinitionRetriever>();
			playedGameCreatorMock = MockRepository.GenerateMock<IPlayedGameCreator>();
			showingXResultsMessageBuilderMock = MockRepository.GenerateMock<IShowingXResultsMessageBuilder>();
			playedGameDeleterMock = MockRepository.GenerateMock<IPlayedGameDeleter>();
			urlHelperMock = MockRepository.GenerateMock<UrlHelper>();
			playedGameController = new Controllers.PlayedGameController(
				dataContext,
				playedGameRetriever,
				playerRetrieverMock,
				playedGameDetailsBuilderMock,
				gameDefinitionRetrieverMock,
				showingXResultsMessageBuilderMock,
				playedGameCreatorMock,
				playedGameDeleterMock);
			playedGameController.Url = urlHelperMock;

			playedGameControllerPartialMock = MockRepository.GeneratePartialMock<PlayedGameController>(
				dataContext,
				playedGameRetriever,
				playerRetrieverMock,
				playedGameDetailsBuilderMock,
				gameDefinitionRetrieverMock,
				showingXResultsMessageBuilderMock,
				playedGameCreatorMock,
				playedGameDeleterMock);
			playedGameControllerPartialMock.Url = urlHelperMock;

			currentUser = new ApplicationUser()
			{
				CurrentGamingGroupId = 1
			};
			gameDefinitionSummaries = new List<GameDefinitionSummary>();
			gameDefinitionRetrieverMock.Expect(mock => mock.GetAllGameDefinitions(currentUser.CurrentGamingGroupId.Value))
				.Repeat.Once()
				.Return(gameDefinitionSummaries);
		}
	}
}