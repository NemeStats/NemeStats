using BusinessLogic.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UniversalAnalyticsHttpWrapper;

namespace BusinessLogic.EventTracking
{
    public class UniversalAnalyticsPlayedGameTracker : PlayedGameTracker
    {
        private IEventTracker eventTracker;
        private IUniversalAnalyticsEventFactory eventFactory;

        public UniversalAnalyticsPlayedGameTracker(
            IEventTracker eventTracker,
            IUniversalAnalyticsEventFactory eventFactory)
        {
            this.eventTracker = eventTracker;
            this.eventFactory = eventFactory;
        }

        public void TrackPlayedGame(ApplicationUser currentUser, string gameName, int numberOfPlayers)
        {
            IUniversalAnalyticsEvent universalAnalyticsEvent = eventFactory.MakeUniversalAnalyticsEvent(
                currentUser.AnonymousClientId,
                EventCategoryEnum.PlayedGames.ToString(),
                EventActionEnum.Created.ToString(),
                gameName,
                numberOfPlayers.ToString());

            eventTracker.TrackEvent(universalAnalyticsEvent);
        }
    }
}
