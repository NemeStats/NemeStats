using System.Collections.Generic;
using System.Web.Mvc;
using BusinessLogic.Logic;
using BusinessLogic.Logic.GamingGroups;
using BusinessLogic.Logic.Nemeses;
using BusinessLogic.Models.Achievements;
using BusinessLogic.Models.GamingGroups;
using BusinessLogic.Models.Nemeses;
using BusinessLogic.Models.Utility;
using NemeStats.TestingHelpers.NemeStatsTestingExtensions;
using NUnit.Framework;
using PagedList;
using Rhino.Mocks;
using Shouldly;
using UI.Controllers;
using UI.Models.GamingGroup;
using UI.Models.Nemeses;

namespace UI.Tests.UnitTests.ControllerTests.GamingGroupControllerTests
{
    [TestFixture]
    public class GetRecentChangesTests : GamingGroupControllerTestBase
    {
        private List<NemesisChangeViewModel> _expectedNemesisChangeViewModels;

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
        }
    }
}
