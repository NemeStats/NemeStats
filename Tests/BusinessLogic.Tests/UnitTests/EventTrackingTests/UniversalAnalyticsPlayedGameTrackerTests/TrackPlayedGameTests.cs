using BusinessLogic.EventTracking;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniversalAnalyticsHttpWrapper;

namespace BusinessLogic.Tests.UnitTests.EventTrackingTests.PlayedGameTrackerTests
{
    [TestFixture]
    public class TrackPlayedGameTests
    {
        private IEventTracker eventTrackerMock;
        private IUniversalAnalyticsEventFactory eventFactoryMock;
        private IUniversalAnalyticsEvent analyticsEvent;
        private UniversalAnalyticsPlayedGameTracker tracker;
        private ApplicationUser currentUser;

        [SetUp]
        public void SetUp()
        {
            eventTrackerMock = MockRepository.GenerateMock<IEventTracker>();
            eventFactoryMock = MockRepository.GenerateMock<IUniversalAnalyticsEventFactory>();
            analyticsEvent = MockRepository.GenerateMock<IUniversalAnalyticsEvent>();
            
            tracker = new UniversalAnalyticsPlayedGameTracker(eventTrackerMock, eventFactoryMock);
            currentUser = new ApplicationUser()
            {
                AnonymousClientId = "anonymous id"
            };
        }

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

            tracker.TrackPlayedGame(currentUser, string.Empty, 0);

            eventTrackerMock.AssertWasCalled(mock => mock.TrackEvent(analyticsEvent));
        }

        [Test]
        public void ItSetsTheEventValueToTheNumberOfPlayers()
        {
            int numberOfPlayers = 1351;
            eventFactoryMock.Expect(mock => mock.MakeUniversalAnalyticsEvent(
                Arg<string>.Is.Anything,
                Arg<string>.Is.Anything,
                Arg<string>.Is.Anything,
                Arg<string>.Is.Anything,
                Arg<string>.Is.Equal(numberOfPlayers.ToString())))
                .Return(analyticsEvent);

            tracker.TrackPlayedGame(currentUser, string.Empty, numberOfPlayers);

            eventTrackerMock.AssertWasCalled(mock => mock.TrackEvent(analyticsEvent));
        }

        [Test]
        public void ItSetsTheEventCategoryToPlayedGames()
        {
            eventFactoryMock.Expect(mock => mock.MakeUniversalAnalyticsEvent(
                Arg<string>.Is.Anything,
                Arg<string>.Is.Equal(EventCategoryEnum.PlayedGames.ToString()),
                Arg<string>.Is.Anything,
                Arg<string>.Is.Anything,
                Arg<string>.Is.Anything))
                .Return(analyticsEvent);
            tracker.TrackPlayedGame(currentUser, string.Empty, 0);

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

            tracker.TrackPlayedGame(currentUser, string.Empty, 0);

            eventTrackerMock.AssertWasCalled(mock => mock.TrackEvent(analyticsEvent));
        }

        [Test]
        public void ItSetsTheEventLabelToTheGameName()
        {
            string gameName = "the name of the game";
            eventFactoryMock.Expect(mock => mock.MakeUniversalAnalyticsEvent(
                Arg<string>.Is.Anything,
                Arg<string>.Is.Anything,
                Arg<string>.Is.Anything,
                Arg<string>.Is.Equal(gameName),
                Arg<string>.Is.Anything))
                .Return(analyticsEvent);

            tracker.TrackPlayedGame(currentUser, gameName, 0);

            eventTrackerMock.AssertWasCalled(mock => mock.TrackEvent(analyticsEvent));
        }
    }
}
