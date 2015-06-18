using BusinessLogic.Logic.GamingGroups;
using BusinessLogic.Models.GamingGroups;
using NUnit.Framework;
using Rhino.Mocks;
using System.Web.Mvc;

namespace UI.Tests.UnitTests.ControllerTests.GamingGroupControllerTests
{
    public class EditPostTests : GamingGroupControllerTestBase
    {
        private const int GAMING_GROUP_ID = 1;

        [Test]
        public void ItUpdatesGamingGroupPublicDetails()
        {
            var request = new GamingGroupEditRequest
            {
                PublicDescription = "Description",
                Website = "Website"
            };
            var httpStatusCodeResult = autoMocker.ClassUnderTest.Edit(request, currentUser) as HttpStatusCodeResult;

            autoMocker.Get<IGamingGroupSaver>().AssertWasCalled(x => x.UpdatePublicGamingGroupDetails(request, currentUser));
        }
    }
}