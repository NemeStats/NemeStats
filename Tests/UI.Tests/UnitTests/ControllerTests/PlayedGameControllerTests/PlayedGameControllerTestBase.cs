#region LICENSE
// NemeStats is a free website for tracking the results of board games.
//     Copyright (C) 2015 Jacob Gordon
// 
//     This program is free software: you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     This program is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU General Public License for more details.
// 
//     You should have received a copy of the GNU General Public License
//     along with this program.  If not, see <http://www.gnu.org/licenses/>
#endregion
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
		protected readonly List<PublicGameSummary> expectedViewModel = new List<PublicGameSummary>();

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
			playedGameRetriever.Expect(mock => mock.GetRecentPublicGames(Arg<int>.Is.Anything)).Return(new List<PublicGameSummary>());
			playedGameControllerPartialMock.Expect(mock => mock.ShowRecentlyPlayedGames()).Return(new ViewResult { ViewName = MVC.PlayedGame.Views.RecentlyPlayedGames, ViewData = new ViewDataDictionary(expectedViewModel) });
		}
	}
}
