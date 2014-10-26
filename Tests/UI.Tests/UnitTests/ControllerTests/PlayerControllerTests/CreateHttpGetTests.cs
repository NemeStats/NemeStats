using NUnit.Framework;
using System.Linq;
using System.Web.Mvc;

namespace UI.Tests.UnitTests.ControllerTests.PlayerControllerTests
{
    [TestFixture]
    public class CreateHttpGetTests : PlayerControllerTestBase
    {
        [Test]
        public void ItReturnsTheParameterlessCreateView()
        {
            ViewResult result = playerController.Create() as ViewResult;

            Assert.AreEqual(MVC.Player.Views.Create, result.ViewName);
        }

        [Test]
        public void ItSetsAnEmptyPlayerAsTheViewModel()
        {
            ViewResult result = playerController.Create() as ViewResult;

            Assert.NotNull(result.Model);
        }
    }
}
