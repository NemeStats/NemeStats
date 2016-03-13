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

using BoardGameGeekApiClient.Interfaces;
using BusinessLogic.DataAccess;
using BusinessLogic.Logic.BoardGameGeek;
using BusinessLogic.Logic.GameDefinitions;
using BusinessLogic.Logic.Users;
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
using UI.Controllers.Helpers;
using UI.Models.GameDefinitionModels;
using UI.Transformations;

namespace UI.Tests.UnitTests.ControllerTests.GameDefinitionControllerTests
{
    [TestFixture]
    public class GameDefinitionControllerTestBase
    {
        protected RhinoAutoMocker<GameDefinitionController> autoMocker;
        protected GameDefinitionController gameDefinitionControllerPartialMock;
        protected IGameDefinitionRetriever gameDefinitionRetrieverMock;
        protected IGameDefinitionDetailsViewModelBuilder gameDefinitionTransformationMock;
        protected IShowingXResultsMessageBuilder showingXResultsMessageBuilderMock;
        protected IGameDefinitionSaver gameDefinitionCreatorMock;
        protected IBoardGameGeekApiClient boardGameGeekApiClient;
        protected NemeStatsDataContext dataContextMock;
        protected UrlHelper urlHelperMock;
        protected ApplicationUser currentUser;
        protected HttpRequestBase asyncRequestMock;
        protected IUserRetriever userRetriever;
        protected IBoardGameGeekGamesImporter BoardGameGeekGamesImporter;
        protected ITransformer transformer;
        protected List<TrendingGame> trendingGames;
        protected List<TrendingGameViewModel> trendingGamesGameViewModels;

        [SetUp]
        public virtual void SetUp()
        {
            autoMocker = new RhinoAutoMocker<GameDefinitionController>();
            autoMocker.Get<IGameDefinitionRetriever>()
                .Expect(mock => mock.GetTrendingGames(GameDefinitionController.NUMBER_OF_TRENDING_GAMES_TO_SHOW,
                    GameDefinitionController.NUMBER_OF_DAYS_OF_TRENDING_GAMES))
                    .Return(trendingGames);
            AutomapperConfiguration.Configure();
            dataContextMock = MockRepository.GenerateMock<NemeStatsDataContext>();
            gameDefinitionRetrieverMock = MockRepository.GenerateMock<IGameDefinitionRetriever>();
            gameDefinitionTransformationMock = MockRepository.GenerateMock<IGameDefinitionDetailsViewModelBuilder>();
            showingXResultsMessageBuilderMock = MockRepository.GenerateMock<IShowingXResultsMessageBuilder>();
            gameDefinitionCreatorMock = MockRepository.GenerateMock<IGameDefinitionSaver>();
            urlHelperMock = MockRepository.GenerateMock<UrlHelper>();
            boardGameGeekApiClient = MockRepository.GenerateMock<IBoardGameGeekApiClient>();
            userRetriever = MockRepository.GenerateMock<IUserRetriever>();
            BoardGameGeekGamesImporter = MockRepository.GenerateMock<IBoardGameGeekGamesImporter>();
            transformer = MockRepository.GenerateMock<ITransformer>();
            gameDefinitionControllerPartialMock = MockRepository.GeneratePartialMock<GameDefinitionController>(
                dataContextMock,
                gameDefinitionRetrieverMock,
                gameDefinitionTransformationMock,
                showingXResultsMessageBuilderMock,
                gameDefinitionCreatorMock,
                boardGameGeekApiClient,
                userRetriever,
                BoardGameGeekGamesImporter,
                transformer);
            gameDefinitionControllerPartialMock.Url = urlHelperMock;

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

            gameDefinitionControllerPartialMock.ControllerContext = new ControllerContext(context, new RouteData(), gameDefinitionControllerPartialMock);

            currentUser = new ApplicationUser()
            {
                Id = "user id",
                CurrentGamingGroupId = 15151
            };
        }
    }
}