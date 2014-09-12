using BusinessLogic.Models.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UI.Models.Players;

namespace UI.Transformations.Player
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