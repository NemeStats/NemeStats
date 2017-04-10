using System.Web.Mvc;
using BusinessLogic.Logic;
using BusinessLogic.Logic.PlayerAchievements;
using BusinessLogic.Models.Achievements;
using NemeStats.TestingHelpers.NemeStatsTestingExtensions;
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
            AutoMocker.Get<IPlayerAchievementRetriever>().Expect(mock => mock.GetPlayerAchievement(Arg<PlayerAchievementQuery>.Is.Anything))
                .Return(expectedPlayerAchievementDetails);
            var expectedPlayerAchievementViewModel = new PlayerAchievementViewModel();
            AutoMocker.Get<ITransformer>().Expect(mock => mock.Transform<PlayerAchievementViewModel>(expectedPlayerAchievementDetails))
                .Return(expectedPlayerAchievementViewModel);

            //--act
            var viewResult = AutoMocker.ClassUnderTest.DetailsForCurrentUser(achievementId, CurrentUser) as ViewResult;

            //--assert
            viewResult.ShouldNotBeNull();
            viewResult.ViewName.ShouldBe(MVC.Achievement.Views.Details);
            var firstCall =
                AutoMocker.Get<IPlayerAchievementRetriever>()
                    .GetArgumentsForCallsMadeOn(x => x.GetPlayerAchievement(Arg<PlayerAchievementQuery>.Is.Anything));
            var query = firstCall.AssertFirstCallIsType<PlayerAchievementQuery>();
            query.AchievementId.ShouldBe(achievementId);
            query.ApplicationUserId.ShouldBe(CurrentUser.Id);
            var playerAchievementViewModel = viewResult.Model as PlayerAchievementViewModel;
            playerAchievementViewModel.ShouldBe(expectedPlayerAchievementViewModel);
        }
    }
}
