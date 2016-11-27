using BusinessLogic.Logic.Achievements;
using NUnit.Framework;
using Rhino.Mocks;
using UI.Areas.Api.Controllers;
using UI.Areas.Api.Models;
using UI.Tests.UnitTests.AreasTests.ApiTests.ControllersTests;

namespace UI.Tests.UnitTests.ControllerTests.AchievemementsControllerTests
{
    public class GetAchievementsTests : ApiControllerTestBase<AchievementsController>
    {
        [Test]
        public void It_Returns_The_Requested_Achievements()
        {
            //--arrange
            AchievementsFilterMessage filter = new AchievementsFilterMessage();
            _autoMocker.Get<IAchievementRetriever>().Expect(mock => mock.GetAchievements(filter))
                .Return

            //--act
            var results = _autoMocker.ClassUnderTest.GetAchievements(filter);

            //--assert
        }

    }
}
