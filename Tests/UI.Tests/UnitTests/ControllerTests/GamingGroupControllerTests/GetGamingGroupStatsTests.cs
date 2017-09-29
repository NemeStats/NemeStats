using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using BusinessLogic.Logic;
using BusinessLogic.Logic.GamingGroups;
using BusinessLogic.Models.GamingGroups;
using BusinessLogic.Models.Utility;
using NUnit.Framework;
using Rhino.Mocks;
using Shouldly;
using UI.Models.GamingGroup;

namespace UI.Tests.UnitTests.ControllerTests.GamingGroupControllerTests
{
    [TestFixture]
    public class GetGamingGroupStatsTests : GamingGroupControllerTestBase
    {
        private GamingGroupStatsViewModel _expectedStatsViewModel;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            var expectedStats = new GamingGroupStats();

            autoMocker.Get<IGamingGroupRetriever>().Expect(mock => mock.GetGamingGroupStats(Arg<int>.Is.Anything, Arg<BasicDateRangeFilter>.Is.Anything)).Return(expectedStats);

            _expectedStatsViewModel = new GamingGroupStatsViewModel();
            autoMocker.Get<ITransformer>().Expect(mock => mock.Transform<GamingGroupStatsViewModel>(expectedStats)).Return(_expectedStatsViewModel);
        }

        [Test]
        public void It_Returns_The_Gaming_Group_Stats_Partial_View()
        {
            //--arrange
            var gamingGroupId = 1;
            var dateFilter = new BasicDateRangeFilter();

            //--act
            var viewResult = autoMocker.ClassUnderTest.GetGamingGroupStats(gamingGroupId, dateFilter) as PartialViewResult;

            //--assert
            viewResult.ViewName.ShouldBe(MVC.GamingGroup.Views._GamingGroupStatsPartial);
            autoMocker.Get<IGamingGroupRetriever>().AssertWasCalled(mock => mock.GetGamingGroupStats(gamingGroupId, dateFilter));
            var viewModel = viewResult.Model as GamingGroupStatsViewModel;
            viewModel.ShouldNotBeNull();
            viewModel.ShouldBeSameAs(_expectedStatsViewModel);
        }
    }
}
