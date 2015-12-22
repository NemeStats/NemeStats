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
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using UI.Controllers;
using UI.Transformations;
using BusinessLogic.Logic.Players;
using System.Web.Mvc;
using BusinessLogic.Models.Players;
using System.Collections.Generic;
using BusinessLogic.Logic.Nemeses;
using UI.Models.Nemeses;
using UI.Models.Players;
using StructureMap.AutoMocking;
using AutoMapper;

namespace UI.Tests.UnitTests.ControllerTests.PlayerControllerTests
{
    [TestFixture]
	public class PlayerControllerTestBase
	{
        protected RhinoAutoMocker<PlayerController> autoMocker;
		protected ApplicationUser currentUser;
		protected readonly TopPlayerViewModel expectedTopPlayersViewModel = new TopPlayerViewModel();
		protected readonly NemesisChangeViewModel expectedNemesisChangeViewModel = new NemesisChangeViewModel();

		[SetUp]
		public virtual void SetUp()
		{
            AutomapperConfiguration.Configure();
            autoMocker = new RhinoAutoMocker<PlayerController>();
            autoMocker.PartialMockTheClassUnderTest();
            autoMocker.ClassUnderTest.Url = MockRepository.GenerateMock<UrlHelper>();

            currentUser = new ApplicationUser()
			{
				CurrentGamingGroupId = 123,
				Id = "app user id"
			};

			autoMocker.Get<HttpRequestBase>().Expect(x => x.Headers)
				.Repeat.Any()
				.Return(new System.Net.WebHeaderCollection
                {
                    { "X-Requested-With", "XMLHttpRequest" }
                });

			var context = MockRepository.GenerateMock<HttpContextBase>();
			context.Expect(x => x.Request)
				.Repeat.Any()
				.Return(autoMocker.Get<HttpRequestBase>());

			autoMocker.Get<HttpRequestBase>().Expect(mock => mock.Url)
							.Return(new Uri("https://nemestats.com/Details/1"));

			autoMocker.ClassUnderTest.ControllerContext = new ControllerContext(context, new RouteData(), autoMocker.ClassUnderTest);
			autoMocker.Get<IPlayerSummaryBuilder>().Expect(mock => mock.GetTopPlayers(Arg<int>.Is.Anything)).Return(new List<TopPlayer>());
			autoMocker.Get<INemesisHistoryRetriever>().Expect(mock => mock.GetRecentNemesisChanges(Arg<int>.Is.Anything)).Return(new List<NemesisChange>());
			autoMocker.Get<INemesisChangeViewModelBuilder>().Expect(mock => mock.Build(Arg<List<NemesisChange>>.Is.Anything)).Return(new List<NemesisChangeViewModel>());
			autoMocker.ClassUnderTest.Expect(mock => mock.ShowTopPlayers()).Return(new ViewResult { ViewName = MVC.Player.Views.TopPlayers, ViewData = new ViewDataDictionary(expectedTopPlayersViewModel) });
			autoMocker.ClassUnderTest.Expect(mock => mock.ShowRecentNemesisChanges()).Return(new ViewResult { ViewName = MVC.Player.Views.RecentNemesisChanges, ViewData = new ViewDataDictionary(expectedNemesisChangeViewModel) });
		}
	}
}