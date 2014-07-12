using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace UI.Tests.UnitTests.ControllerTests.GameDefinitionControllerTests
{
    [TestFixture]
    public class CreateHttpGetTests : GameDefinitionControllerTestBase
    {
        [Test]
        public void ItReturnsACreateView()
        {
            ViewResult viewResult = gameDefinitionControllerPartialMock.Create() as ViewResult;

            Assert.AreEqual(MVC.GameDefinition.Views.Create, viewResult.ViewName);
        }
    }
}
