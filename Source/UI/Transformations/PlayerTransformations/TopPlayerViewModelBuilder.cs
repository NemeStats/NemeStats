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
using BusinessLogic.Models.Players;
using System.Linq;
using UI.Models.Players;
using UI.Models.Points;

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
                //TODO fix this
                NemePointsSummary = new NemePointsSummaryViewModel(0, 0, 0),
                //TotalPoints = topPlayer.TotalPoints,
                WinPercentage = topPlayer.WinPercentage
            };
        }
    }
}