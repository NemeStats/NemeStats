using System.Collections.Generic;
using System.Web.Mvc;
using BusinessLogic.Logic;
using BusinessLogic.Logic.GamingGroups;
using BusinessLogic.Models.Achievements;
using BusinessLogic.Models.GamingGroups;
using BusinessLogic.Models.Utility;
using NUnit.Framework;
using PagedList;
using Rhino.Mocks;
using Shouldly;
using UI.Models.GamingGroup;

namespace UI.Tests.UnitTests.ControllerTests.GamingGroupControllerTests
{
    [TestFixture]
    public class GetRecentChangesTests : GamingGroupControllerTestBase
    {
        private RecentGamingGroupChangesViewModel _expectedViewModel;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            var expectedBusinessObject = new RecentGamingGroupChanges();
            expectedBusinessObject.RecentAchievements = new List<PlayerAchievementWinner>().ToPagedList(1, 1);

            autoMocker.Get<IGamingGroupRetriever>().Expect(mock => mock.GetRecentChanges(Arg<int>.Is.Anything, Arg<BasicDateRangeFilter>.Is.Anything)).Return(expectedBusinessObject);

            _expectedViewModel = new RecentGamingGroupChangesViewModel();
            autoMocker.Get<ITransformer>().Expect(mock => mock.Transform<RecentGamingGroupChangesViewModel>(expectedBusinessObject)).Return(_expectedViewModel);
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
            autoMocker.Get<IGamingGroupRetriever>().AssertWasCalled(mock => mock.GetRecentChanges(gamingGroupId, dateFilter));
            var viewModel = viewResult.Model as RecentGamingGroupChangesViewModel;
            //--paged lists suck to test... taking a shortcut
            viewModel.RecentAchievements.ShouldNotBeNull();
        }
    }
}
