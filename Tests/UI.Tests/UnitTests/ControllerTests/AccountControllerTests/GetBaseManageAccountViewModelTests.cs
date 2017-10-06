using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Shouldly;

namespace UI.Tests.UnitTests.ControllerTests.AccountControllerTests
{
    //TODO shamefully leaving this test class empty as the work to get the UserManager and User.Identity stuff to a testable state doesnt feel worth it at the moment
    // but hey, at least the boilerplate work of creating the test class is out of the way for the next person!
    public class GetBaseManageAccountViewModelTests : AccountControllerTestBase
    {
        [Test]
        public void It_Returns_The_BoardGameGeek_Info()
        {
            //--arrange

            //--act
            //var result = accountControllerPartialMock.GetBaseManageAccountViewModel();

            //--assert
            //result.BoardGameGeekIntegrationModel.ShouldNotBeNull();
        }

    }
}
