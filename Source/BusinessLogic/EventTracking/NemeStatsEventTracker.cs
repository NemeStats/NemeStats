using BusinessLogic.Models.User;
using System.Linq;

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
