using System.Collections.Generic;
using System.Web.Mvc;
using BusinessLogic.Facades;
using BusinessLogic.Logic;
using BusinessLogic.Logic.PlayerAchievements;
using BusinessLogic.Models.Achievements;
using BusinessLogic.Models.Games;
using BusinessLogic.Paging;
using NUnit.Framework;
using PagedList;
using Rhino.Mocks;
using Shouldly;
using UI.Controllers;
using UI.Models.GameDefinitionModels;
using UI.Models.Players;

namespace UI.Tests.UnitTests.ControllerTests.HomeControllerTests
{
    public class RecentAchievementsUnlockedTests : HomeControllerTestBase
    {
        [Test]
        public void It_Returns_The_Last_Ten_Achievements_Unlocked()
        {
            //--arrange
            var expectedResults = new List<PlayerAchievementWinner>
            {
                new PlayerAchievementWinner(),
                new PlayerAchievementWinner()
            }.ToPagedList(1, int.MaxValue);

            _autoMocker.Get<IRecentPlayerAchievementsUnlockedRetriever>().Expect(mock => mock.GetResults(Arg<GetRecentPlayerAchievementsUnlockedQuery>.Is.Anything))
                .Return(expectedResults);

            //--this is the transformation that happens in the ToTransformedPagedList
            _autoMocker.Get<ITransformer>().Expect(mock => mock.Transform<PlayerAchievementWinnerViewModel>(Arg<PlayerAchievementWinner>.Is.Anything))
                .Repeat.Any()
                .Return(new PlayerAchievementWinnerViewModel());

            //--act
            var results = _autoMocker.ClassUnderTest.RecentAchievementsUnlocked();

            //--assert
            _autoMocker.Get<IRecentPlayerAchievementsUnlockedRetriever>().AssertWasCalled(
                mock => mock.GetResults(Arg<GetRecentPlayerAchievementsUnlockedQuery>.Matches(
                    x => x.PageSize == HomeController.NUMBER_OF_RECENT_ACHIEVEMENTS_TO_SHOW)));
            var viewResult = results as PartialViewResult;
            viewResult.ShouldNotBeNull();
            var viewModel = viewResult.Model as IPagedList<PlayerAchievementWinnerViewModel>;
            viewModel.ShouldNotBeNull();
            viewResult.ViewName.ShouldBe(MVC.Achievement.Views._RecentAchievementsUnlocked);
            viewModel.Count.ShouldBe(2);
        }

    }
}
