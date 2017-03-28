using System.Web.Mvc;
using BusinessLogic.Logic;
using BusinessLogic.Logic.PlayerAchievements;
using BusinessLogic.Models.Achievements;
using NUnit.Framework;
using Rhino.Mocks;
using Shouldly;
using StructureMap.AutoMocking;
using UI.Controllers;
using UI.Models.Achievements;

namespace UI.Tests.UnitTests.ControllerTests.AchievementControllerTests
{
    [TestFixture]
    public class PlayerAchievementTests
    {
        private RhinoAutoMocker<AchievementController> _autoMocker;

        [SetUp]
        public void SetUp()
        {
            _autoMocker = new RhinoAutoMocker<AchievementController>();
        }

        [Test]
        public void It_Returns_A_404_If_The_Player_Doesnt_Have_The_Achievement()
        {
            //--arrange
            var achievementId = AchievementId.BusyBee;
            int playerId = -1;
            var playerAchievementQuery = new PlayerAchievementQuery(achievementId, playerId);
            _autoMocker.Get<IPlayerAchievementRetriever>().Expect(mock => mock.GetPlayerAchievement(playerAchievementQuery))
                .IgnoreArguments()
                .Return(null);

            //--act
            var result = _autoMocker.ClassUnderTest.PlayerAchievement(achievementId, playerId);

            //--assert
            result.ShouldBeAssignableTo<HttpNotFoundResult>();
        }

        [Test]
        public void It_Returns_The_Player_Achievement_For_The_Specified_Player()
        {
            //--arrange
            var achievementId = AchievementId.BusyBee;
            int playerId = 1;
            var expectedPlayerAchievementDetails = new PlayerAchievementDetails();
            _autoMocker.Get<IPlayerAchievementRetriever>().Expect(mock => mock.GetPlayerAchievement(Arg<PlayerAchievementQuery>.Is.Anything))
                .Return(expectedPlayerAchievementDetails);
            var expectedViewModel = new PlayerAchievementViewModel();
            _autoMocker.Get<ITransformer>().Expect(mock => mock.Transform<PlayerAchievementViewModel>(expectedPlayerAchievementDetails))
                .Return(expectedViewModel);

            //--act
            var result = _autoMocker.ClassUnderTest.PlayerAchievement(achievementId, playerId) as ViewResult;

            //--assert
            _autoMocker.Get<IPlayerAchievementRetriever>().AssertWasCalled(
                x => x.GetPlayerAchievement(Arg<PlayerAchievementQuery>.Matches(y => y.AchievementId == achievementId && y.PlayerId == playerId)));
            result.ShouldNotBeNull();
            var viewModel = result.Model as PlayerAchievementViewModel;
            viewModel.ShouldNotBeNull();
            viewModel.ShouldBeSameAs(expectedViewModel);
        }

    }
}
