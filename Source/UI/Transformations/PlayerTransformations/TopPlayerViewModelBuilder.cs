using BusinessLogic.Models.Players;
using System.Linq;
using UI.Models.Players;

namespace UI.Transformations.PlayerTransformations
{
    public class TopPlayerViewModelBuilder : ITopPlayerViewModelBuilder
    {
        public TopPlayerViewModel Build(TopPlayer topPlayer)
        {
            return new TopPlayerViewModel()
            {
                PlayerId = topPlayer.PlayerId,
                PlayerName = topPlayer.PlayerName,
                TotalNumberOfGamesPlayed = topPlayer.TotalNumberOfGamesPlayed,
                TotalPoints = topPlayer.TotalPoints,
                WinPercentage = topPlayer.WinPercentage
            };
        }
    }
}