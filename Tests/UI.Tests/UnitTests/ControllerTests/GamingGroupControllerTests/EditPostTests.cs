using BusinessLogic.Logic.GamingGroups;
using BusinessLogic.Models.GamingGroups;
using NUnit.Framework;
using Rhino.Mocks;
using System.Web.Mvc;

namespace UI.Tests.UnitTests.ControllerTests.GamingGroupControllerTests
{
    public class EditPostTests : GamingGroupControllerTestBase
    {
        [Test]
        public void ItUpdatesGamingGroupPublicDetails()
        {
            var request = new GamingGroupEditRequest();

            autoMocker.ClassUnderTest.Edit(request, currentUser);

            autoMocker.Get<IGamingGroupSaver>().AssertWasCalled(x => x.UpdatePublicGamingGroupDetails(request, currentUser));
        }
    }
}