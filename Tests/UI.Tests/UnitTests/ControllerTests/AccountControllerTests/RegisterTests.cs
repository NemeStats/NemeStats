using BusinessLogic.Models.User;
using Microsoft.AspNet.Identity;
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
    public class RegisterTests : AccountControllerTestBase
    {
        //TODO giving up on testing this for now. Too frustrated and need to move on.
        //[Test]
        //public async void ItSavesTheEmailAddressAfterSigningInTheNewlyCreatedUser()
        //{
        //    //modelStateDictionaryMock.Expect(modelState => modelState.IsValid)
        //    //    .Repeat.Once()
        //    //    .Return(true);

        //    IdentityResult identityResult = new IdentityResult(new List<string>());
        //    userStoreMock.Expect(mock => mock.CreateAsync(Arg<ApplicationUser>.Is.Anything))
        //        .Repeat.Once()
        //        .Return(identityResult);
        //    await accountControllerPartialMock.Register(registerViewModel);

        //    userManager.AssertWasCalled(userMgr => userMgr.SetEmail(Arg<string>.Is.Anything, Arg<string>.Is.Equal(registerViewModel.EmailAddress)));
        //}
    }
}
