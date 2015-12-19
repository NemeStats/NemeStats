using BusinessLogic.Models;
using BusinessLogic.Models.Games;
using BusinessLogic.Models.Players;
using BusinessLogic.Models.User;
using UI.Models.Players;

namespace UI.Transformations.PlayerTransformations
{
    public interface IPlayerQuickStatsViewModelBuilder
    {
        PlayerQuickStatsViewModel Build(PlayerStatistics playerStatistics, PlayedGame lastGamingGroupGame, ApplicationUser currentUser);
    }
}