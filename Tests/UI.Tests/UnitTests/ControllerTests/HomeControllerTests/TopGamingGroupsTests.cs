//using System.Collections.Generic;
//using System.Web.Mvc;
//using BusinessLogic.Facades;
//using BusinessLogic.Logic;
//using BusinessLogic.Models.GamingGroups;
//using NUnit.Framework;
//using Rhino.Mocks;
//using Shouldly;
//using UI.Controllers;
//using UI.Models.GamingGroup;

//namespace UI.Tests.UnitTests.ControllerTests.HomeControllerTests
//{
//    public class TopGamingGroupsTests : HomeControllerTestBase
//    {
//        [Test]
//        public void It_Returns_The_TopGamingGroups_Partial_View_With_The_Specified_Number_Of_Gaming_Groups()
//        {
//            //--arrange
//            var expectedGamingGroups = new List<TopGamingGroupSummary>
//            {
//                new TopGamingGroupSummary(),
//                new TopGamingGroupSummary()
//            };
//            _autoMocker.Get<ITopGamingGroupsRetriever>()
//                .Expect(mock => mock.GetResults(Arg<int>.Is.Anything))
//                .Return(expectedGamingGroups);
//            var expectedViewModel1 = new GamingGroupSummaryViewModel();
//            var expectedViewModel2 = new GamingGroupSummaryViewModel();
//            _autoMocker.Get<ITransformer>().Expect(mock => mock.Transform<GamingGroupSummaryViewModel>(expectedGamingGroups[0])).Return(expectedViewModel1);
//            _autoMocker.Get<ITransformer>().Expect(mock => mock.Transform<GamingGroupSummaryViewModel>(expectedGamingGroups[1])).Return(expectedViewModel2);

//            //--act
//            var results = _autoMocker.ClassUnderTest.TopGamingGroups();

//            //--assert
//            _autoMocker.Get<ITopGamingGroupsRetriever>().AssertWasCalled(
//                mock => mock.GetResults(Arg<int>.Is.Equal(HomeController.NUMBER_OF_TOP_GAMING_GROUPS_TO_SHOW)));
//            var viewResult = results as PartialViewResult;
//            viewResult.ShouldNotBeNull();
//            var viewModel = viewResult.Model as GamingGroupsSummaryViewModel;
//            viewModel.ShouldNotBeNull();
//            viewModel.ShowForEdit.ShouldBeFalse();
//            viewModel.GamingGroups.Count.ShouldBe(1);
//            viewModel.GamingGroups[0].ShouldBeSameAs(expectedViewModel1);
//            viewModel.GamingGroups[0].ShouldBeSameAs(expectedViewModel2);
//        }
//    }
//}
