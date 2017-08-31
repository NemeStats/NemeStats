using BusinessLogic.Logic.GamingGroups;
using BusinessLogic.Models.GamingGroups;
using BusinessLogic.Models.Utility;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using UI.Controllers;

namespace UI.Tests.UnitTests.ControllerTests.GamingGroupControllerTests
{
    [TestFixture]
    public class GetGamingGroupSummaryTests : GamingGroupControllerTestBase
    {
        [Test]
        public void ItRetrievesTheSpecifiedNumberOfGamingGroupSummaries()
        {
            autoMocker.ClassUnderTest.GetGamingGroupSummary(0);

            autoMocker.Get<IGamingGroupRetriever>().AssertWasCalled(
                mock => mock.GetGamingGroupDetails(
                    Arg<GamingGroupFilter>.Matches(x =>
                    x.NumberOfRecentGamesToShow == GamingGroupController.MAX_NUMBER_OF_RECENT_GAMES)));
        }

        [Test]
        public void ItFiltersOnGamingGroupId()
        {
            int expectedGamingGroupId = 1;

            autoMocker.ClassUnderTest.GetGamingGroupSummary(expectedGamingGroupId);

            autoMocker.Get<IGamingGroupRetriever>().AssertWasCalled(
                mock => mock.GetGamingGroupDetails(
                    Arg<GamingGroupFilter>.Matches(x =>
                    x.GamingGroupId == expectedGamingGroupId)));
        }

        [Test]
        public void ItFiltersOnTheDateRangeIfSpecified()
        {
            var expectedFromDate = new DateTime(2016, 1, 1);
            var expectedToDate = new DateTime(2017, 2, 3);
            var dateRangeFilterMock = MockRepository.GenerateMock<IDateRangeFilter>();
            dateRangeFilterMock.Expect(mock => mock.FromDate).Return(expectedFromDate);
            dateRangeFilterMock.Expect(mock => mock.ToDate).Return(expectedToDate);

            autoMocker.ClassUnderTest.GetGamingGroupSummary(0, dateRangeFilterMock);

            autoMocker.Get<IGamingGroupRetriever>().AssertWasCalled(
                mock => mock.GetGamingGroupDetails(
                    Arg<GamingGroupFilter>.Matches(x =>
                    x.DateRangeFilter.FromDate == expectedFromDate
                    && x.DateRangeFilter.ToDate == expectedToDate)));
        }

        [Test]
        public void ItUsesTheDefaultDateRangeIfNoneIsSpecified()
        {
            var expectedDateRange = new BasicDateRangeFilter();

            autoMocker.ClassUnderTest.GetGamingGroupSummary(0);

            autoMocker.Get<IGamingGroupRetriever>().AssertWasCalled(
                mock => mock.GetGamingGroupDetails(
                    Arg<GamingGroupFilter>.Matches(x =>
                    x.DateRangeFilter.FromDate == expectedDateRange.FromDate
                    && x.DateRangeFilter.ToDate == expectedDateRange.ToDate)));
        }
    }
}
