using System.Linq;
using BusinessLogic.EventTracking;
using NUnit.Framework;
using Rhino.Mocks;
using UniversalAnalyticsHttpWrapper;

namespace BusinessLogic.Tests.UnitTests.EventTrackingTests.UniversalAnalyticsNemeStatsEventTrackerTests
{
    [TestFixture]
    public class TrackGamingGroupUpdateTests : UniversalAnalyticsNemeStatsEventTrackerTestBase
    {
        [Test]
        public void ItSetsTheAnonymousClientId()
        {
            _autoMocker.Get<IUniversalAnalyticsEventFactory>().Expect(mock => mock.MakeUniversalAnalyticsEvent(
                Arg<string>.Is.Equal(currentUser.AnonymousClientId),
                Arg<string>.Is.Anything,
                Arg<string>.Is.Anything,
                Arg<string>.Is.Anything,
                Arg<string>.Is.Anything, 
                Arg<string>.Is.Anything,
                Arg<bool>.Is.Anything))
                .Return(analyticsEvent);

            _autoMocker.ClassUnderTest.TrackGamingGroupUpdate(currentUser);

            _autoMocker.Get<IEventTracker>().AssertWasCalled(mock => mock.TrackEvent(analyticsEvent));
        }

        [Test]
        public void ItSetsTheEventCategoryToGamingGroups()
        {
            _autoMocker.Get<IUniversalAnalyticsEventFactory>().Expect(mock => mock.MakeUniversalAnalyticsEvent(
                Arg<string>.Is.Anything,
                Arg<string>.Is.Equal(EventCategoryEnum.GamingGroups.ToString()),
                Arg<string>.Is.Anything,
                Arg<string>.Is.Anything,
                Arg<string>.Is.Anything, 
                Arg<string>.Is.Anything,
                Arg<bool>.Is.Anything))
                .Return(analyticsEvent);

            _autoMocker.ClassUnderTest.TrackGamingGroupUpdate(currentUser);

            _autoMocker.Get<IEventTracker>().AssertWasCalled(mock => mock.TrackEvent(analyticsEvent));
        }

        [Test]
        public void ItSetsTheEventActionToUpdated()
        {
            _autoMocker.Get<IUniversalAnalyticsEventFactory>().Expect(mock => mock.MakeUniversalAnalyticsEvent(
                Arg<string>.Is.Anything,
                Arg<string>.Is.Anything,
                Arg<string>.Is.Equal(EventActionEnum.Updated.ToString()),
                Arg<string>.Is.Anything,
                Arg<string>.Is.Anything, 
                Arg<string>.Is.Anything,
                Arg<bool>.Is.Anything))
                .Return(analyticsEvent);

            _autoMocker.ClassUnderTest.TrackGamingGroupUpdate(currentUser);

            _autoMocker.Get<IEventTracker>().AssertWasCalled(mock => mock.TrackEvent(analyticsEvent));
        }

        [Test]
        public void ItSetsTheLabelToDefault()
        {
            _autoMocker.Get<IUniversalAnalyticsEventFactory>().Expect(mock => mock.MakeUniversalAnalyticsEvent(
                Arg<string>.Is.Anything,
                Arg<string>.Is.Anything,
                Arg<string>.Is.Anything,
                Arg<string>.Is.Equal(UniversalAnalyticsNemeStatsEventTracker.DEFAULT_EVENT_LABEL),
                Arg<string>.Is.Anything, 
                Arg<string>.Is.Anything,
                Arg<bool>.Is.Anything))
                .Return(analyticsEvent);

            _autoMocker.ClassUnderTest.TrackGamingGroupUpdate(currentUser);

            _autoMocker.Get<IEventTracker>().AssertWasCalled(mock => mock.TrackEvent(analyticsEvent));
        }
    }
}
