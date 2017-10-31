using BusinessLogic.Models.User;
using NUnit.Framework;
using StructureMap.AutoMocking;
using UI.Controllers;

namespace UI.Tests.UnitTests.ControllerTests.AchievementControllerTests
{
    [TestFixture]
    public abstract class AchievementControllerTestBase
    {
        protected RhinoAutoMocker<AchievementController> AutoMocker;
        protected ApplicationUser CurrentUser;

        [SetUp]
        public void SetUp()
        {
            AutoMocker = new RhinoAutoMocker<AchievementController>();
            CurrentUser = new ApplicationUser
            {
                Id = "some id",
                CurrentGamingGroupId = 1
            };
        }
    }
}
