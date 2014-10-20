using BusinessLogic.EventTracking;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Tests.UnitTests.EventTrackingTests.UniversalAnalyticsNemeStatsEventTrackerTests
{
    [TestFixture]
    public class TrackPlayerCreationTests : UniversalAnalyticsNemeStatsEventTrackerTestBase
    {
        [Test]
        public void ItSetsTheAnonymousClientId()
        {
            eventFactoryMock.Expect(mock => mock.MakeUniversalAnalyticsEvent(
                Arg<string>.Is.Equal(currentUser.AnonymousClientId),
                Arg<string>.Is.Anything,
                Arg<string>.Is.Anything,
                Arg<string>.Is.Anything,
                Arg<string>.Is.Anything))
                .Return(analyticsEvent);

            tracker.TrackPlayerCreation(currentUser);

            eventTrackerMock.AssertWasCalled(mock => mock.TrackEvent(analyticsEvent));
        }

        [Test]
        public void ItSetsTheEventCategoryToPlayers()
        {
            eventFactoryMock.Expect(mock => mock.MakeUniversalAnalyticsEvent(
                Arg<string>.Is.Anything,
                Arg<string>.Is.Equal(EventCategoryEnum.Players.ToString()),
                Arg<string>.Is.Anything,
                Arg<string>.Is.Anything,
                Arg<string>.Is.Anything))
                .Return(analyticsEvent);

            tracker.TrackPlayerCreation(currentUser);

            eventTrackerMock.AssertWasCalled(mock => mock.TrackEvent(analyticsEvent));
        }

        [Test]
        public void ItSetsTheEventActionToCreated()
        {
            eventFactoryMock.Expect(mock => mock.MakeUniversalAnalyticsEvent(
                Arg<string>.Is.Anything,
                Arg<string>.Is.Anything,
                Arg<string>.Is.Equal(EventActionEnum.Created.ToString()),
                Arg<string>.Is.Anything,
                Arg<string>.Is.Anything))
                .Return(analyticsEvent);

            tracker.TrackPlayerCreation(currentUser);

            eventTrackerMock.AssertWasCalled(mock => mock.TrackEvent(analyticsEvent));
        }
    }
}
