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
using System.Linq;
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
		protected NewlyCompletedGameViewModel expectedDefaultCompletedGameViewModel;
		protected NewlyCompletedGameViewModel expectedPopulatedCompletedGameViewModel;
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

			this.expectedViewModel = new List<PublicGameSummary>();
			playedGameRetriever.Expect(mock => mock.GetRecentPublicGames(Arg<int>.Is.Anything)).Return(new List<PublicGameSummary>());
			playedGameControllerPartialMock.Expect(mock => mock.Edit()).Return(new ViewResult { ViewName = MVC.PlayedGame.Views.Edit });
			playedGameControllerPartialMock.Expect(mock => mock.ShowRecentlyPlayedGames()).Return(new ViewResult { ViewName = MVC.PlayedGame.Views.RecentlyPlayedGames, ViewData = new ViewDataDictionary(expectedViewModel) });

			this.expectedDefaultCompletedGameViewModel = new NewlyCompletedGameViewModel();
			playedGameControllerPartialMock.Expect(mock => mock.Edit(Arg<int>.Is.Anything)).Return(new ViewResult { ViewData = new ViewDataDictionary(this.expectedDefaultCompletedGameViewModel) });

			this.playerList = new List<Player> { new Player { Id = 42, Name = "Smitty Werbenjagermanjensen" } };
			this.playerSelectList = this.playerList.Select(item => new SelectListItem { Text = item.Name, Value = item.Id.ToString() }).ToList();
			this.gameDefinitionList = new List<GameDefinition> { new GameDefinition { Id = 1, Name = "Betrayal At The House On The Hill" } };
			this.gameDefinitionSelectList = this.gameDefinitionList.Select(item => new SelectListItem { Text = item.Name, Value = item.Id.ToString() }).ToList();
			this.expectedPopulatedCompletedGameViewModel = new NewlyCompletedGameViewModel { GameDefinitions = this.gameDefinitionSelectList, Players = this.playerSelectList };
		}
	}
}