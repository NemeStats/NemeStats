using BusinessLogic.Models;
using BusinessLogic.Models.Players;
using BusinessLogic.Models.User;
using UI.Models.Players;

namespace UI.Transformations.PlayerTransformations
{
    public class PlayerQuickStatsViewModelBuilder : IPlayerQuickStatsViewModelBuilder
    {
        private readonly IPlayedGameDetailsViewModelBuilder _playedGameDetailsViewModelBuilder;

        public PlayerQuickStatsViewModelBuilder(IPlayedGameDetailsViewModelBuilder playedGameDetailsViewModelBuilder)
        {
            _playedGameDetailsViewModelBuilder = playedGameDetailsViewModelBuilder;
        }

        public PlayerQuickStatsViewModel Build(PlayerStatistics playerStatistics, PlayedGame lastGamingGroupGame, ApplicationUser currentUser)
        {
            var playerQuickStatsViewModel = new PlayerQuickStatsViewModel
            {
                TotalPoints = playerStatistics.TotalPoints,
                TotalGamesPlayed = playerStatistics.TotalGames,
                TotalGamesWon = playerStatistics.TotalGamesWon
            };

            if (lastGamingGroupGame != null)
            {
                playerQuickStatsViewModel.LastGamingGroupGame =
                    _playedGameDetailsViewModelBuilder.Build(lastGamingGroupGame, currentUser);
            }

            return playerQuickStatsViewModel;
        }
    }
}