using BusinessLogic.DataAccess;
using BusinessLogic.Logic;
using BusinessLogic.Models;
using BusinessLogic.Models.User;
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
using UI.Transformations;

namespace UI.Tests.UnitTests.ControllerTests.PlayedGameControllerTests
{
    public class PlayedGameControllerTestBase
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
        protected string testUserName = "the test user name";
        protected UserContext userContext;

        [SetUp]
        public virtual void TestSetUp()
        {
            dbContexMock = MockRepository.GenerateMock<NemeStatsDbContext>();
            playedGameLogicMock = MockRepository.GenerateMock<PlayedGameLogic>();
            playerLogicMock = MockRepository.GenerateMock<PlayerLogic>();
            playedGameDetailsBuilder = MockRepository.GenerateMock<PlayedGameDetailsViewModelBuilder>();
            userContextBuilder = MockRepository.GenerateMock<UserContextBuilder>();
            userContext = new UserContext()
            {
                ApplicationUserId = "abc",
                GamingGroupId = 123
            };
            userContextBuilder.Expect(builder => builder.GetUserContext(testUserName, dbContexMock))
                .Repeat.Once()
                .Return(userContext);
            playedGameController = new Controllers.PlayedGameController(dbContexMock,
                playedGameLogicMock, 
                playerLogicMock, 
                playedGameDetailsBuilder);
            httpRequestBase = MockRepository.GenerateMock<HttpRequestBase>();
            playedGameControllerPartialMock = MockRepository.GeneratePartialMock<PlayedGameController>(
                dbContexMock, 
                playedGameLogicMock, 
                playerLogicMock, 
                playedGameDetailsBuilder);

            HttpContextBase contextBase = MockRepository.GenerateMock<HttpContextBase>();
            
            contextBase.Expect(cb => cb.Request)
                .Repeat.Once()
                .Return(httpRequestBase);

            principal = MockRepository.GenerateMock<IPrincipal>();
            contextBase.Expect(cb => cb.User)
                .Repeat.Once()
                .Return(principal);
            identity = MockRepository.GenerateMock<IIdentity>();
            principal.Expect(x => x.Identity)
                .Repeat.Once()
                .Return(identity);
            identity.Expect(x => x.Name)
                .Repeat.Once()
                .Return(testUserName);

            playedGameControllerPartialMock.ControllerContext = new System.Web.Mvc.ControllerContext(
                contextBase,
                new RouteData(),
                playedGameControllerPartialMock);
        }
    }
}
