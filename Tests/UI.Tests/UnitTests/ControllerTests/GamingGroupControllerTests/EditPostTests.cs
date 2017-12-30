using System.Web.Mvc;
using BusinessLogic.Logic.GamingGroups;
using BusinessLogic.Models.GamingGroups;
using NUnit.Framework;
using Rhino.Mocks;
using Shouldly;

namespace UI.Tests.UnitTests.ControllerTests.GamingGroupControllerTests
{
    public class EditPostTests : GamingGroupControllerTestBase
    {
        [Test]
        public void It_Updates_Gaming_Group_Public_Details_And_Redirects_Back_To_Gaming_Group_Listing()
        {
            var expectedResult = new RedirectResult("some url");
            autoMocker.ClassUnderTest.Expect(mock => mock.MakeRedirectResultToManageAccountPageGamingGroupsTab())
                .Return(expectedResult);
            var request = new GamingGroupEditRequest();

            var actualResult = autoMocker.ClassUnderTest.Edit(request, currentUser);

            autoMocker.Get<IGamingGroupSaver>().AssertWasCalled(x => x.UpdatePublicGamingGroupDetails(request, currentUser));
            actualResult.ShouldBe(expectedResult);
        }
    }
}