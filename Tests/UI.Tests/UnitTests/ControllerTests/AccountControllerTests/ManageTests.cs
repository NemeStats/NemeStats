using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using UI.Controllers;

namespace UI.Tests.UnitTests.ControllerTests.AccountControllerTests
{
    [TestFixture]
    public class ManageTests
    {
        private AccountController accountController;

        [SetUp]
        public void SetUp()
        {
            accountController = new AccountController();
        }

        [Test, Ignore("Need a reference to Microsoft.aspnet.identity.core but internet won't let me download at the moment.")]
        public void ItLoadsTheManageView()
        {
            UI.Controllers.AccountController.ManageMessageId? nullMessageId = null;
            ViewResult viewResult = accountController.Manage(nullMessageId) as ViewResult;

            Assert.AreEqual(MVC.Account.Views.Manage, viewResult.ViewName);
        }
    }
}
