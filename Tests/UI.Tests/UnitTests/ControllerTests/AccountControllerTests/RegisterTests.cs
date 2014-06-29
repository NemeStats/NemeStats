using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using UI.Controllers;
using UI.Models;

namespace UI.Tests.UnitTests.ControllerTests.AccountControllerTests
{
    [TestFixture]
    public class RegisterTests
    {
        [Test, Ignore("had to stop this test because I need to install Microsoft.aspnet.identity.core but my internet was down.")]
        public void ItSavesTheEmailAddressAfterSigningInTheNewlyCreatedUser()
        {
            //TODO had to stop this test because I need to install Microsoft.aspnet.identity.core but my internet was down.
            //UserManager<ApplicationUser>
            AccountController accountController = MockRepository.GeneratePartialMock<AccountController>();
            ModelStateDictionary modelStateDictionaryMock = MockRepository.GenerateMock<ModelStateDictionary>();
            accountController.Expect(controller => controller.ModelState)
                .Repeat.Once()
                .Return(modelStateDictionaryMock);
            modelStateDictionaryMock.Expect(modelState => modelState.IsValid)
                .Repeat.Once()
                .Return(true);

            //accountController.Register(RegisterViewModel viewModel);

            //userManager.AssertWasCalled(userMgr => userMgr.SetEmail(userId, emailAddress));
        }
    }
}
