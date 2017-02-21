using System.Collections.Generic;
using System.Web.Mvc;
using BusinessLogic.Logic;
using BusinessLogic.Logic.Achievements;
using BusinessLogic.Models.Achievements;
using NUnit.Framework;
using Rhino.Mocks;
using Shouldly;
using UI.Models.Achievements;

namespace UI.Tests.UnitTests.ControllerTests.AchievementControllerTests
{
    [TestFixture]
    public class IndexTests : AchievementControllerTestBase
    {
        [Test]
        public void It_Returns_All_Achievements()
        {
            //--arrange
            var achievementSummaries = new List<AggregateAchievementSummary>();
            AutoMocker.Get<IAchievementRetriever>().Expect(mock => mock.GetAllAchievementSummaries(CurrentUser)).Return(achievementSummaries);
            var expectedAchievementViewModels = new List<AchievementTileViewModel>();
            AutoMocker.Get<ITransformer>()
                .Expect(mock => mock.Transform<List<AchievementTileViewModel>>(achievementSummaries))
                .Return(expectedAchievementViewModels);

            //--act
            var results = AutoMocker.ClassUnderTest.Index(CurrentUser) as ViewResult;

            //--assert
            results.ShouldNotBeNull();
            var viewModel = results.Model as AchievementListViewModel;
            viewModel.ShouldNotBeNull();
            viewModel.CurrentUserId.ShouldBe(CurrentUser.Id);
            viewModel.Achievements.ShouldBeSameAs(expectedAchievementViewModels);
        }

    }
}
