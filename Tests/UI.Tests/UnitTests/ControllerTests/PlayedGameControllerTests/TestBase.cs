using BusinessLogic.DataAccess;
using BusinessLogic.Models;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Routing;
using UI.Controllers;
using UI.Logic;
using UI.Transformations;

namespace UI.Tests.UnitTests.ControllerTests.PlayedGameControllerTests
{
    public class TestBase
    {
        protected NemeStatsDbContext dbContexMock;
        protected PlayedGameController playedGameController;
        protected PlayedGameController playedGameControllerPartialMock;
        protected PlayedGameLogic playedGameLogicMock;
        protected PlayerLogic playerLogicMock;
        protected PlayedGameDetailsViewModelBuilder playedGameDetailsBuilder;
        protected UserContextBuilder userContextBuilder;
        protected HttpRequestBase httpRequestBase;
        protected IPrincipal principal;
        protected IIdentity identity;

        [SetUp]
        public virtual void TestSetUp()
        {
            dbContexMock = MockRepository.GenerateMock<NemeStatsDbContext>();
            playedGameLogicMock = MockRepository.GenerateMock<PlayedGameLogic>();
            playerLogicMock = MockRepository.GenerateMock<PlayerLogic>();
            playedGameDetailsBuilder = MockRepository.GenerateMock<PlayedGameDetailsViewModelBuilder>();
            userContextBuilder = MockRepository.GenerateMock<UserContextBuilder>();
            playedGameController = new Controllers.PlayedGameController(dbContexMock,
                playedGameLogicMock, 
                playerLogicMock, 
                playedGameDetailsBuilder, 
                userContextBuilder);
            httpRequestBase = MockRepository.GenerateMock<HttpRequestBase>();
            
            playedGameControllerPartialMock = MockRepository.GeneratePartialMock<PlayedGameController>(
                dbContexMock, 
                playedGameLogicMock, 
                playerLogicMock, 
                playedGameDetailsBuilder,
                userContextBuilder);

            HttpContextBase contextBase = MockRepository.GenerateMock<HttpContextBase>();
            contextBase.Expect(cb => cb.Request)
                .Repeat.Once()
                .Return(httpRequestBase);

            principal = MockRepository.GenerateMock<IPrincipal>();
            contextBase.Expect(cb => cb.User)
                .Repeat.Once()
                .Return(principal);
            principal.Expect(x => x.Identity)
                .Repeat.Once()
                .Return(identity);
            playedGameControllerPartialMock.ControllerContext = new System.Web.Mvc.ControllerContext(
                contextBase,
                new RouteData(),
                playedGameControllerPartialMock);
        }
    }
}
