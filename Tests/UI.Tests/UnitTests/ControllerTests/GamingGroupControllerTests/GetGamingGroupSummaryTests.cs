using BusinessLogic.Logic.GamingGroups;
using BusinessLogic.Models.GamingGroups;
using BusinessLogic.Models.Utility;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using UI.Controllers;
using UI.Models.GamingGroup;

namespace UI.Tests.UnitTests.ControllerTests.GamingGroupControllerTests
{
    [TestFixture]
    public class GetGamingGroupSummaryTests : GamingGroupControllerTestBase
    {
        [SetUp]
        public void LocalSetUp()
        {

        }

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
                    x.FromDate == expectedFromDate
                    && x.ToDate == expectedToDate)));
        }

        [Test]
        public void ItUsesTheDefaultDateRangeIfNoneIsSpecified()
        {
            var expectedDateRange = new GamingGroupRequest();

            autoMocker.ClassUnderTest.GetGamingGroupSummary(0);

            autoMocker.Get<IGamingGroupRetriever>().AssertWasCalled(
                mock => mock.GetGamingGroupDetails(
                    Arg<GamingGroupFilter>.Matches(x =>
                    x.FromDate == expectedDateRange.FromDate
                    && x.ToDate == expectedDateRange.ToDate)));
        }
    }
}
