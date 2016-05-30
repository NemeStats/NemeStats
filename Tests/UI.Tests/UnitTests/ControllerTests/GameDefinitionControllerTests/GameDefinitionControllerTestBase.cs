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

#endregion LICENSE

using BusinessLogic.Logic.GameDefinitions;
using BusinessLogic.Models.Games;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using StructureMap.AutoMocking;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using UI.Controllers;
using UI.Transformations;

namespace UI.Tests.UnitTests.ControllerTests.GameDefinitionControllerTests
{
    [TestFixture]
    public class GameDefinitionControllerTestBase
    {
        protected RhinoAutoMocker<GameDefinitionController> autoMocker;
        protected UrlHelper urlHelperMock;
        protected ApplicationUser currentUser;
        protected HttpRequestBase asyncRequestMock;
        protected ITransformer transformer;
        protected List<TrendingGame> trendingGames;

        [SetUp]
        public virtual void SetUp()
        {
            autoMocker = new RhinoAutoMocker<GameDefinitionController>();
            autoMocker.Get<IGameDefinitionRetriever>()
                .Expect(mock => mock.GetTrendingGames(GameDefinitionController.NUMBER_OF_TRENDING_GAMES_TO_SHOW,
                    GameDefinitionController.NUMBER_OF_DAYS_OF_TRENDING_GAMES))
                    .Return(trendingGames);
            AutomapperConfiguration.Configure();
            urlHelperMock = MockRepository.GenerateMock<UrlHelper>();
            autoMocker.ClassUnderTest.Url = urlHelperMock;
           
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
            autoMocker.ClassUnderTest.ControllerContext = new ControllerContext(context, new RouteData(), autoMocker.ClassUnderTest);
            currentUser = new ApplicationUser()
            {
                Id = "user id",
                CurrentGamingGroupId = 15151
            };
        }
    }
}