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
using System;
using System.Web;
using System.Web.Routing;
using BusinessLogic.DataAccess;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using System.Linq;
using UI.Controllers;
using UI.Transformations;
using UI.Transformations.PlayerTransformations;
using UI.Controllers.Helpers;
using BusinessLogic.Logic.Players;
using System.Web.Mvc;
using BusinessLogic.Models.Players;
using System.Collections.Generic;
using BusinessLogic.Logic.Nemeses;
using UI.Models.Nemeses;
using UI.Models.Players;

namespace UI.Tests.UnitTests.ControllerTests.PlayerControllerTests
{
	[TestFixture]
	public class PlayerControllerTestBase
	{
		protected IDataContext dataContextMock;
		protected IPlayerRetriever playerRetrieverMock;
		protected IGameResultViewModelBuilder playerGameResultDetailsBuilderMock;
		protected IPlayerDetailsViewModelBuilder playerDetailsViewModelBuilderMock;
		protected IShowingXResultsMessageBuilder showingXResultsMessageBuilderMock;
		protected IPlayerSaver playerSaverMock;
		protected IPlayerInviter playerInviterMock;
		protected IPlayerEditViewModelBuilder playerEditViewModelBuilderMock;
		protected IPlayerSummaryBuilder playerSummaryBuilderMock;
		protected ITopPlayerViewModelBuilder topPlayerViewModelBuilderMock;
		protected INemesisHistoryRetriever nemesisHistoryRetrieverMock;
		protected INemesisChangeViewModelBuilder nemesisChangeViewModelBuilderMock;
		protected UrlHelper urlHelperMock;
		protected PlayerController playerController;
		protected ApplicationUser currentUser;
		protected HttpRequestBase asyncRequestMock;
		protected readonly TopPlayerViewModel expectedTopPlayersViewModel = new TopPlayerViewModel();
		protected readonly NemesisChangeViewModel expectedNemesisChangeViewModel = new NemesisChangeViewModel();

		[SetUp]
		public virtual void SetUp()
		{
			currentUser = new ApplicationUser()
			{
				CurrentGamingGroupId = 123,
				Id = "app user id"
			};
			dataContextMock = MockRepository.GenerateMock<IDataContext>();
			playerRetrieverMock = MockRepository.GenerateMock<IPlayerRetriever>();
			playerGameResultDetailsBuilderMock = MockRepository.GenerateMock<IGameResultViewModelBuilder>();
			playerDetailsViewModelBuilderMock = MockRepository.GenerateMock<IPlayerDetailsViewModelBuilder>();
			showingXResultsMessageBuilderMock = MockRepository.GenerateMock<IShowingXResultsMessageBuilder>();
			playerSummaryBuilderMock = MockRepository.GenerateMock<IPlayerSummaryBuilder>();
			topPlayerViewModelBuilderMock = MockRepository.GenerateMock<ITopPlayerViewModelBuilder>();
			nemesisHistoryRetrieverMock = MockRepository.GenerateMock<INemesisHistoryRetriever>();
			nemesisChangeViewModelBuilderMock = MockRepository.GenerateMock<INemesisChangeViewModelBuilder>();
			playerSaverMock = MockRepository.GenerateMock<IPlayerSaver>();
			urlHelperMock = MockRepository.GenerateMock<UrlHelper>();
			playerInviterMock = MockRepository.GenerateMock<IPlayerInviter>();
			playerEditViewModelBuilderMock = MockRepository.GenerateMock<IPlayerEditViewModelBuilder>();
			playerController = MockRepository.GeneratePartialMock<PlayerController>(
								dataContextMock,
								playerGameResultDetailsBuilderMock,
								playerDetailsViewModelBuilderMock,
								showingXResultsMessageBuilderMock,
								playerSaverMock,
								playerRetrieverMock,
								playerInviterMock,
								playerEditViewModelBuilderMock,
								playerSummaryBuilderMock,
								topPlayerViewModelBuilderMock,
								nemesisHistoryRetrieverMock,
								nemesisChangeViewModelBuilderMock);
			playerController.Url = urlHelperMock;

			asyncRequestMock = MockRepository.GenerateMock<HttpRequestBase>();
			asyncRequestMock.Expect(x => x.Headers)
				.Repeat.Any()
				.Return(new System.Net.WebHeaderCollection
                {
                    { "X-Requested-With", "XMLHttpRequest" }
                });

			var context = MockRepository.GenerateMock<HttpContextBase>();
			context.Expect(x => x.Request)
				.Repeat.Any()
				.Return(asyncRequestMock);

			asyncRequestMock.Expect(mock => mock.Url)
							.Return(new Uri("https://nemestats.com/Details/1"));

			playerController.ControllerContext = new ControllerContext(context, new RouteData(), playerController);
			playerSummaryBuilderMock.Expect(mock => mock.GetTopPlayers(Arg<int>.Is.Anything)).Return(new List<TopPlayer>());
			nemesisHistoryRetrieverMock.Expect(mock => mock.GetRecentNemesisChanges(Arg<int>.Is.Anything)).Return(new List<NemesisChange>());
			nemesisChangeViewModelBuilderMock.Expect(mock => mock.Build(Arg<List<NemesisChange>>.Is.Anything)).Return(new List<NemesisChangeViewModel>());
			playerController.Expect(mock => mock.ShowTopPlayers()).Return(new ViewResult { ViewName = MVC.Player.Views.TopPlayers, ViewData = new ViewDataDictionary(expectedTopPlayersViewModel) });
			playerController.Expect(mock => mock.ShowRecentNemesisChanges()).Return(new ViewResult { ViewName = MVC.Player.Views.RecentNemesisChanges, ViewData = new ViewDataDictionary(expectedNemesisChangeViewModel) });
		}
	}
}