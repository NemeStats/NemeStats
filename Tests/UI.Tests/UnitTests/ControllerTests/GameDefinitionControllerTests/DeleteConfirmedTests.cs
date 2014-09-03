using BusinessLogic.Models;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace UI.Tests.UnitTests.ControllerTests.GameDefinitionControllerTests
{
    [TestFixture]
    public class DeleteConfirmedTests : GameDefinitionControllerTestBase
    {
        [Test]
        public void ItReturnsAnUnauthorizedAccessHttpStatusCodeIfTheUserIsNotAuthorized()
        {
            int gameDefinitionId = 1;
            dataContextMock.Expect(mock => mock.DeleteById<GameDefinition>(Arg<object>.Is.Anything, Arg<ApplicationUser>.Is.Anything))
                .Throw(new UnauthorizedAccessException());

            HttpStatusCodeResult statusCodeResult = gameDefinitionControllerPartialMock
                                                        .DeleteConfirmed(gameDefinitionId, currentUser) as HttpStatusCodeResult;

            Assert.AreEqual((int)HttpStatusCode.Unauthorized, statusCodeResult.StatusCode);
        }

        [Test]
        public void ItDeletesTheGameDefinition()
        {
            int gameDefinitionId = 1;
            dataContextMock.Expect(mock => mock.DeleteById<GameDefinition>(1, currentUser));

            gameDefinitionControllerPartialMock.DeleteConfirmed(gameDefinitionId, currentUser);

            gameDefinitionRetrieverMock.VerifyAllExpectations();
        }

        [Test]
        public void ItRedirectsToTheIndexAction()
        {
            RedirectToRouteResult redirectResult = gameDefinitionControllerPartialMock
                .DeleteConfirmed(1, currentUser) as RedirectToRouteResult;

            Assert.AreEqual(MVC.GameDefinition.ActionNames.Index, redirectResult.RouteValues["action"]);
        }
    }
}
