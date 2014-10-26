using BusinessLogic.EventTracking;
using NUnit.Framework;
using Rhino.Mocks;
using System.Linq;

namespace BusinessLogic.Tests.UnitTests.EventTrackingTests.UniversalAnalyticsNemeStatsEventTrackerTests
{
    [TestFixture]
    public class TrackGameDefinitionCreationTests : UniversalAnalyticsNemeStatsEventTrackerTestBase
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

            tracker.TrackGameDefinitionCreation(currentUser, "game definition");

            eventTrackerMock.AssertWasCalled(mock => mock.TrackEvent(analyticsEvent));
        }

        [Test]
        public void ItSetsTheEventCategoryToGameDefinition()
        {
            eventFactoryMock.Expect(mock => mock.MakeUniversalAnalyticsEvent(
                Arg<string>.Is.Anything,
                Arg<string>.Is.Equal(EventCategoryEnum.GameDefinitions.ToString()),
                Arg<string>.Is.Anything,
                Arg<string>.Is.Anything,
                Arg<string>.Is.Anything))
                .Return(analyticsEvent);

            tracker.TrackGameDefinitionCreation(currentUser, "game definition");

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

            tracker.TrackGameDefinitionCreation(currentUser, "game definition");

            eventTrackerMock.AssertWasCalled(mock => mock.TrackEvent(analyticsEvent));
        }

        [Test]
        public void ItSetsTheEventLabelToTheNameOfTheGameDefinition()
        {
            string gameDefinitionName = "game definition name";
            eventFactoryMock.Expect(mock => mock.MakeUniversalAnalyticsEvent(
                Arg<string>.Is.Anything,
                Arg<string>.Is.Anything,
                Arg<string>.Is.Anything,
                Arg<string>.Is.Equal(gameDefinitionName),
                Arg<string>.Is.Anything))
                .Return(analyticsEvent);

            tracker.TrackGameDefinitionCreation(currentUser, gameDefinitionName);

            eventTrackerMock.AssertWasCalled(mock => mock.TrackEvent(analyticsEvent));
        }
    }
}
