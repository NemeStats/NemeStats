using System.Web.Mvc;
using BusinessLogic.Logic.GamingGroups;
using NUnit.Framework;
using Rhino.Mocks;
using Shouldly;
using UI.Controllers;

namespace UI.Tests.UnitTests.ControllerTests.GamingGroupControllerTests
{
    [TestFixture]
    public class DeleteTests : GamingGroupControllerTestBase
    {
        [Test]
        public void It_Deletes_The_Specified_Gaming_Group_And_Redirects_Back_To_The_Manage_Groups_Page()
        {
            //--arrange
            var gamingGroupId = 1;
            autoMocker.PartialMockTheClassUnderTest();
            var expectedRedirectResult = new RedirectResult("some url");
            autoMocker.ClassUnderTest.Expect(mock =>
                    mock.MakeRedirectResultToManageAccountPageGamingGroupsTab(AccountController.ManageMessageId
                        .GamingGroupDeleted))
                .Return(expectedRedirectResult);

            //--act
            var result = autoMocker.ClassUnderTest.Delete(gamingGroupId, currentUser) as RedirectResult;

            //--assert
            autoMocker.Get<IDeleteGamingGroupComponent>().AssertWasCalled(mock => mock.Execute(gamingGroupId, currentUser));
            result.ShouldBeSameAs(expectedRedirectResult);
        }
    }
}
