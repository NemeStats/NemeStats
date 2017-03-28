using System.Web.Mvc;
using BusinessLogic.Logic;
using BusinessLogic.Logic.PlayerAchievements;
using BusinessLogic.Models.Achievements;
using NUnit.Framework;
using Rhino.Mocks;
using Shouldly;
using UI.Models.Achievements;

namespace UI.Tests.UnitTests.ControllerTests.AchievementControllerTests
{
    [TestFixture]
    public class DetailsForCurrentUserTests : AchievementControllerTestBase
    {
        [Test]
        public void It_Returns_Achievement_Details_For_The_Current_User()
        {
            //--arrange
            var achievementId = AchievementId.BusyBee;
            var expectedPlayerAchievementDetails = new PlayerAchievementDetails();
            AutoMocker.Get<IPlayerAchievementRetriever>().Expect(mock => mock.GetCurrentPlayerAchievementDetails(achievementId, CurrentUser))
                .Return(expectedPlayerAchievementDetails);
            var expectedPlayerAchievementViewModel = new PlayerAchievementViewModel();
            AutoMocker.Get<ITransformer>().Expect(mock => mock.Transform<PlayerAchievementViewModel>(expectedPlayerAchievementDetails))
                .Return(expectedPlayerAchievementViewModel);

            //--act
            var viewResult = AutoMocker.ClassUnderTest.DetailsForCurrentUser(achievementId, CurrentUser) as ViewResult;

            //--assert
            viewResult.ShouldNotBeNull();
            viewResult.ViewName.ShouldBe(MVC.Achievement.Views.Details);
            var playerAchievementViewModel = viewResult.Model as PlayerAchievementViewModel;
            playerAchievementViewModel.ShouldBe(expectedPlayerAchievementViewModel);
        }
    }
}
