using BusinessLogic.EventTracking;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using System.Linq;
using UniversalAnalyticsHttpWrapper;

namespace BusinessLogic.Tests.UnitTests.EventTrackingTests.UniversalAnalyticsNemeStatsEventTrackerTests
{
    [TestFixture]
    public class UniversalAnalyticsNemeStatsEventTrackerTestBase
    {
        protected IEventTracker eventTrackerMock;
        protected IUniversalAnalyticsEventFactory eventFactoryMock;
        protected IUniversalAnalyticsEvent analyticsEvent;
        protected UniversalAnalyticsNemeStatsEventTracker tracker;
        protected ApplicationUser currentUser;

        [SetUp]
        public void SetUp()
        {
            eventTrackerMock = MockRepository.GenerateMock<IEventTracker>();
            eventFactoryMock = MockRepository.GenerateMock<IUniversalAnalyticsEventFactory>();
            analyticsEvent = MockRepository.GenerateMock<IUniversalAnalyticsEvent>();

            tracker = new UniversalAnalyticsNemeStatsEventTracker(eventTrackerMock, eventFactoryMock);
            currentUser = new ApplicationUser()
            {
                AnonymousClientId = "anonymous id"
            };
        }
    }
}
