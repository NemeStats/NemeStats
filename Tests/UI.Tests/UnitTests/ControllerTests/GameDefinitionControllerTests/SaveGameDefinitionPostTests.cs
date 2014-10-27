using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using BusinessLogic.Models;
using NUnit.Framework;
using Rhino.Mocks;

namespace UI.Tests.UnitTests.ControllerTests.GameDefinitionControllerTests
{
    [TestFixture]
    public class SaveGameDefinitionPostTests : GameDefinitionControllerTestBase
    {
        private HttpRequestBase asyncRequestMock;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

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
        }

        [Test]
        public void ItSavesThePlayer()
        {
            var game = new GameDefinition()
            {
                Name = "New Game"
            };

            gameDefinitionControllerPartialMock.Save(game, currentUser);

            gameDefinitionCreatorMock.AssertWasCalled(mock => mock.Save(game, currentUser));
        }

        [Test]
        public void ItReturnsBadRequestWhenTheRequestIsNotAjax()
        {
            var context = MockRepository.GenerateMock<HttpContextBase>();
            context.Expect(x => x.Request)
                .Repeat.Any()
                .Return(asyncRequestMock);
            asyncRequestMock.Headers.Clear();

            gameDefinitionControllerPartialMock.ControllerContext = new ControllerContext(context, new RouteData(), gameDefinitionControllerPartialMock);

            var result = gameDefinitionControllerPartialMock.Save(new GameDefinition(), currentUser) as HttpStatusCodeResult;

            Assert.AreEqual((int)HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Test]
        public void ItReturnsANotModifiedStatusIfValidationFails()
        {
            var game = new GameDefinition();
            gameDefinitionControllerPartialMock.ModelState.AddModelError("key", "message");

            var result = gameDefinitionControllerPartialMock.Save(game, currentUser) as HttpStatusCodeResult;

            Assert.AreEqual((int)HttpStatusCode.NotModified, result.StatusCode);
        }
    }
}
