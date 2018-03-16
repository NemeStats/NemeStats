using System.Collections.Generic;
using System.Web.Mvc;
using BusinessLogic.Logic;
using BusinessLogic.Logic.Champions;
using BusinessLogic.Logic.GamingGroups;
using BusinessLogic.Logic.Nemeses;
using BusinessLogic.Models.Achievements;
using BusinessLogic.Models.Champions;
using BusinessLogic.Models.GamingGroups;
using BusinessLogic.Models.Nemeses;
using BusinessLogic.Models.Utility;
using NemeStats.TestingHelpers.NemeStatsTestingExtensions;
using NUnit.Framework;
using PagedList;
using Rhino.Mocks;
using Shouldly;
using UI.Controllers;
using UI.Models.Champions;
using UI.Models.GamingGroup;
using UI.Models.Nemeses;

namespace UI.Tests.UnitTests.ControllerTests.GamingGroupControllerTests
{
    [TestFixture]
    public class GetRecentChangesTests : GamingGroupControllerTestBase
    {
        private List<NemesisChangeViewModel> _expectedNemesisChangeViewModels;
        private List<ChampionChangeViewModel> _expectedChampionChangeViewModels;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            var expectedBusinessObject = new RecentGamingGroupChanges();
            expectedBusinessObject.RecentAchievements = new List<PlayerAchievementWinner>().ToPagedList(1, 1);

            autoMocker.Get<IGamingGroupRetriever>().Expect(mock => mock.GetRecentChanges(Arg<int>.Is.Anything, Arg<BasicDateRangeFilter>.Is.Anything)).Return(expectedBusinessObject);

            var expectedNemesisChanges = new List<NemesisChange>();

            autoMocker.Get<INemesisHistoryRetriever>()
                .Expect(mock => mock.GetRecentNemesisChanges(Arg<GetRecentNemesisChangesRequest>.Is.Anything)).Return(expectedNemesisChanges);
            _expectedNemesisChangeViewModels = new List<NemesisChangeViewModel>();
            autoMocker.Get<ITransformer>()
                .Expect(mock => mock.Transform<List<NemesisChangeViewModel>>(expectedNemesisChanges))
                .Return(_expectedNemesisChangeViewModels);

            var expectedRecentChampionChanges = new List<ChampionChange>();
            autoMocker.Get<IRecentChampionRetriever>().Expect(mock =>
                    mock.GetRecentChampionChanges(Arg<GetRecentChampionChangesFilter>.Is.Anything))
                .Return(expectedRecentChampionChanges);
            _expectedChampionChangeViewModels = new List<ChampionChangeViewModel>();
            autoMocker.Get<ITransformer>()
                .Expect(mock => mock.Transform<List<ChampionChangeViewModel>>(expectedRecentChampionChanges))
                .Return(_expectedChampionChangeViewModels);
        }

        [Test]
        public void It_Returns_The_Gaming_Group_Recent_Changes_Partial_View()
        {
            //--arrange
            var gamingGroupId = 1;
            var dateFilter = new BasicDateRangeFilter();

            //--act
            var viewResult = autoMocker.ClassUnderTest.GetRecentChanges(gamingGroupId, dateFilter) as PartialViewResult;

            //--assert
            viewResult.ViewName.ShouldBe(MVC.GamingGroup.Views._GamingGroupRecentChanges);
            var viewModel = viewResult.Model as RecentGamingGroupChangesViewModel;

            autoMocker.Get<IGamingGroupRetriever>().AssertWasCalled(mock => mock.GetRecentChanges(gamingGroupId, dateFilter));
            //--paged lists suck to test... taking a shortcut
            viewModel.RecentAchievements.ShouldNotBeNull();

            var args = autoMocker.Get<INemesisHistoryRetriever>()
                .GetArgumentsForCallsMadeOn(mock => mock.GetRecentNemesisChanges(Arg<GetRecentNemesisChangesRequest>.Is.Anything));
            var actualRequest = args.AssertFirstCallIsType<GetRecentNemesisChangesRequest>();
            actualRequest.NumberOfRecentChangesToRetrieve.ShouldBe(GamingGroupController
                .NUMBER_OF_RECENT_NEMESIS_TO_SHOW);
            actualRequest.GamingGroupId.ShouldBe(gamingGroupId);
            viewModel.RecentNemesisChanges.ShouldBeSameAs(_expectedNemesisChangeViewModels);

            var getRecentChampionArgs = autoMocker.Get<IRecentChampionRetriever>()
                .GetArgumentsForCallsMadeOn(mock => mock.GetRecentChampionChanges(Arg<GetRecentChampionChangesFilter>.Is.Anything));
            var actualFilter = getRecentChampionArgs.AssertFirstCallIsType<GetRecentChampionChangesFilter>();
            actualFilter.NumberOfDaysOfRecentChangesToShow.ShouldBe(GamingGroupController.NUMBER_OF_RECENT_CHAMPION_CHANGES_TO_SHOW);
            actualFilter.GamingGroupId.ShouldBe(gamingGroupId);
            viewModel.RecentChampionChanges.ShouldBeSameAs(_expectedChampionChangeViewModels);
        }
    }
}
