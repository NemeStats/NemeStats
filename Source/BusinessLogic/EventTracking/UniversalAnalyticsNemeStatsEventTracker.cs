using BusinessLogic.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UniversalAnalyticsHttpWrapper;

namespace BusinessLogic.EventTracking
{
    public class UniversalAnalyticsNemeStatsEventTracker : NemeStatsEventTracker
    {
        /// <summary>
        /// This was randomly generated and will be used for any client whose anonymous ID cannot be determined.
        /// </summary>
        public const string DEFAULT_ANONYMOUS_CLIENT_ID = "D4151681-B52E-415B-975C-D1C8FD56C645";

        private IEventTracker eventTracker;
        private IUniversalAnalyticsEventFactory eventFactory;

        public UniversalAnalyticsNemeStatsEventTracker(
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

        public void TrackUserRegistration()
        {
            IUniversalAnalyticsEvent universalAnalyticsEvent = eventFactory.MakeUniversalAnalyticsEvent(
                UniversalAnalyticsNemeStatsEventTracker.DEFAULT_ANONYMOUS_CLIENT_ID,
                EventCategoryEnum.Users.ToString(),
                EventActionEnum.Created.ToString());

            eventTracker.TrackEvent(universalAnalyticsEvent);
        }

        public void TrackGamingGroupCreation()
        {
            IUniversalAnalyticsEvent universalAnalyticsEvent = eventFactory.MakeUniversalAnalyticsEvent(
                UniversalAnalyticsNemeStatsEventTracker.DEFAULT_ANONYMOUS_CLIENT_ID,
                EventCategoryEnum.GamingGroups.ToString(),
                EventActionEnum.Created.ToString());

            eventTracker.TrackEvent(universalAnalyticsEvent);
        }

        public void TrackGameDefinitionCreation(ApplicationUser currentUser, string gameDefinitionName)
        {
            IUniversalAnalyticsEvent universalAnalyticsEvent = eventFactory.MakeUniversalAnalyticsEvent(
                  currentUser.AnonymousClientId,
                  EventCategoryEnum.GameDefinitions.ToString(),
                  EventActionEnum.Created.ToString(),
                  gameDefinitionName);

            eventTracker.TrackEvent(universalAnalyticsEvent);
        }

        public void TrackPlayerCreation(ApplicationUser currentUser)
        {
            IUniversalAnalyticsEvent universalAnalyticsEvent = eventFactory.MakeUniversalAnalyticsEvent(
                  currentUser.AnonymousClientId,
                  EventCategoryEnum.Players.ToString(),
                  EventActionEnum.Created.ToString());

            eventTracker.TrackEvent(universalAnalyticsEvent);
        }
    }
}
