#region LICENSE
// NemeStats is a free website for tracking the results of board games.
//     Copyright (C) 2015 Jacob Gordon
// 
//     This program is free software: you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     This program is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU General Public License for more details.
// 
//     You should have received a copy of the GNU General Public License
//     along with this program.  If not, see <http://www.gnu.org/licenses/>
#endregion
using BusinessLogic.Models.User;
using System.Linq;
using UniversalAnalyticsHttpWrapper;

namespace BusinessLogic.EventTracking
{
    public class UniversalAnalyticsNemeStatsEventTracker : INemeStatsEventTracker
    {
        /// <summary>
        /// This was randomly generated and will be used for any client whose anonymous ID cannot be determined.
        /// </summary>
        public const string DEFAULT_ANONYMOUS_CLIENT_ID = "D4151681-B52E-415B-975C-D1C8FD56C645";
        public const string DEFAULT_EVENT_LABEL = "Blank";

        private readonly IEventTracker eventTracker;
        private readonly IUniversalAnalyticsEventFactory eventFactory;

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

        public void TrackUserRegistration(RegistrationSource registrationSource)
        {
            IUniversalAnalyticsEvent universalAnalyticsEvent = eventFactory.MakeUniversalAnalyticsEvent(
                DEFAULT_ANONYMOUS_CLIENT_ID,
                EventCategoryEnum.Users.ToString(),
                EventActionEnum.Created.ToString(),
                registrationSource.ToString());

            eventTracker.TrackEvent(universalAnalyticsEvent);
        }

        public void TrackGamingGroupCreation(RegistrationSource registrationSource)
        {
            IUniversalAnalyticsEvent universalAnalyticsEvent = eventFactory.MakeUniversalAnalyticsEvent(
                DEFAULT_ANONYMOUS_CLIENT_ID,
                EventCategoryEnum.GamingGroups.ToString(),
                EventActionEnum.Created.ToString(),
                registrationSource.ToString());

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
                  EventActionEnum.Created.ToString(),
                  DEFAULT_EVENT_LABEL);

            eventTracker.TrackEvent(universalAnalyticsEvent);
        }
    }
}
