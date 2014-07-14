using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace UI.Tests.UnitTests.ControllerTests.GamingGroupControllerTests
{
    [TestFixture]
    public class GrantAccessTests : GamingGroupControllerTestBase
    {
        [Test]
        public void ItReturnsTheIndexView()
        {
            ViewResult viewResult = gamingGroupController.Index(userContext) as ViewResult;

            Assert.AreEqual(MVC.GamingGroup.Views.Index, viewResult.ViewName);
        }
    }
}
