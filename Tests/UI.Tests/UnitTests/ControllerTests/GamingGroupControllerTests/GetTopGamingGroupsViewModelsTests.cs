using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using BusinessLogic.Facades;
using BusinessLogic.Logic;
using BusinessLogic.Models.GamingGroups;
using NUnit.Framework;
using Rhino.Mocks;
using Shouldly;
using UI.Controllers;
using UI.Models.GamingGroup;

namespace UI.Tests.UnitTests.ControllerTests.GamingGroupControllerTests
{
    [TestFixture]
    public class GetTopGamingGroupsViewModelsTests : GamingGroupControllerTestBase
    {
        [Test]
        public void It_Returns_The_TopGamingGroups_Partial_View_With_The_Default_Number_Of_Gaming_Groups()
        {
            //--arrange
            var expectedGamingGroups = new List<TopGamingGroupSummary>
            {
                new TopGamingGroupSummary(),
                new TopGamingGroupSummary()
            };
            autoMocker.Get<ITopGamingGroupsRetriever>()
                .Expect(mock => mock.GetResults(Arg<int>.Is.Anything))
                .Return(expectedGamingGroups);
            var expectedViewModel1 = new GamingGroupSummaryViewModel();
            var expectedViewModel2 = new GamingGroupSummaryViewModel();
            autoMocker.Get<ITransformer>().Expect(mock => mock.Transform<GamingGroupSummaryViewModel>(expectedGamingGroups[0])).Return(expectedViewModel1);
            autoMocker.Get<ITransformer>().Expect(mock => mock.Transform<GamingGroupSummaryViewModel>(expectedGamingGroups[1])).Return(expectedViewModel2);
            int numberOfGamingGroups = 2;

            //--act
            var result = autoMocker.ClassUnderTest.GetGamingGroupsSummaryViewModel(numberOfGamingGroups);

            //--assert
            autoMocker.Get<ITopGamingGroupsRetriever>().AssertWasCalled(
                mock => mock.GetResults(Arg<int>.Is.Equal(numberOfGamingGroups)));

            result.ShouldNotBeNull();
            result.ShouldNotBeNull();
            result.ShowForEdit.ShouldBeFalse();
            result.GamingGroups.Count.ShouldBe(2);
            result.GamingGroups[0].ShouldBeSameAs(expectedViewModel1);
            result.GamingGroups[1].ShouldBeSameAs(expectedViewModel2);
        }

    }
}
