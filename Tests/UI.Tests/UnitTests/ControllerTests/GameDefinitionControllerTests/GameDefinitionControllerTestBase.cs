using System.Web;
using System.Web.Routing;
using BusinessLogic.DataAccess;
using BusinessLogic.Logic.BoardGameGeek;
using BusinessLogic.Logic.GameDefinitions;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using System.Linq;
using System.Web.Mvc;
using UI.Controllers;
using UI.Controllers.Helpers;
using UI.Transformations;

namespace UI.Tests.UnitTests.ControllerTests.GameDefinitionControllerTests
{
    [TestFixture]
    public class GameDefinitionControllerTestBase
    {
        protected GameDefinitionController gameDefinitionControllerPartialMock;
        protected IGameDefinitionRetriever gameDefinitionRetrieverMock;
        protected IGameDefinitionViewModelBuilder gameDefinitionTransformationMock;
        protected IShowingXResultsMessageBuilder showingXResultsMessageBuilderMock;
        protected IGameDefinitionSaver gameDefinitionCreatorMock;
        protected IBoardGameGeekSearcher boardGameGeekSearcherMock;
        protected NemeStatsDataContext dataContextMock;
        protected UrlHelper urlHelperMock;
        protected ApplicationUser currentUser;
        protected HttpRequestBase asyncRequestMock;


        [SetUp]
        public virtual void SetUp()
        {
            dataContextMock = MockRepository.GenerateMock<NemeStatsDataContext>();
            gameDefinitionRetrieverMock = MockRepository.GenerateMock<IGameDefinitionRetriever>();
            gameDefinitionTransformationMock = MockRepository.GenerateMock<IGameDefinitionViewModelBuilder>();
            showingXResultsMessageBuilderMock = MockRepository.GenerateMock<IShowingXResultsMessageBuilder>();
            gameDefinitionCreatorMock = MockRepository.GenerateMock<IGameDefinitionSaver>();
            urlHelperMock = MockRepository.GenerateMock<UrlHelper>();
            boardGameGeekSearcherMock = MockRepository.GenerateMock<IBoardGameGeekSearcher>();
            gameDefinitionControllerPartialMock = MockRepository.GeneratePartialMock<GameDefinitionController>(
                dataContextMock, 
                gameDefinitionRetrieverMock,
                gameDefinitionTransformationMock,
                showingXResultsMessageBuilderMock,
                gameDefinitionCreatorMock,
                boardGameGeekSearcherMock);
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
