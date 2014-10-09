using BusinessLogic.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BusinessLogic.EventTracking
{
    public interface INemeStatsEventTracker
    {
        void TrackPlayedGame(ApplicationUser currentUser, string gameName, int numberOfPlayers);
        void TrackUserRegistration();
        void TrackGamingGroupCreation();
        void TrackGameDefinitionCreation(ApplicationUser currentUser, string gameDefinitionName);
        void TrackPlayerCreation(ApplicationUser currentUser);
    }
}
