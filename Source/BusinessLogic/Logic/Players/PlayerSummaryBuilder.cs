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
using BusinessLogic.DataAccess;
using BusinessLogic.Logic.Utilities;
using BusinessLogic.Models.Players;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using BusinessLogic.Models.Points;

namespace BusinessLogic.Logic.Players
{
    public class PlayerSummaryBuilder : IPlayerSummaryBuilder
    {
        private const string SQL_GET_TOP_PLAYERS = @"SELECT TOP {0} Player.Id AS PlayerId, 
            Player.Name AS PlayerName, 
            COUNT(*) AS TotalNumberOfGamesPlayed, 
            SUM(PlayerGameResult.NemeStatsPointsAwarded) AS BaseNemePoints,
            SUM(PlayerGameResult.GameDurationBonusPoints) AS GameDurationBonusPoints,
            SUM(PlayerGameResult.GameWeightBonusPoints) AS GameWeightBonusPoints,
            SUM(PlayerGameResult.NemeStatsPointsAwarded + PlayerGameResult.GameDurationBonusPoints + PlayerGameResult.GameWeightBonusPoints) as TotalPoints,
            SUM(CASE WHEN PlayerGameResult.GameRank = 1 THEN 1 ELSE 0 END) AS WinPercentage
            FROM Player INNER JOIN PlayerGameResult ON Player.Id = PlayerGameResult.PlayerId
            WHERE Player.Active = 1
            GROUP BY Player.Id, Player.Name
            ORDER BY TotalPoints DESC, PlayerName";

        private readonly IDataContext dataContext;

        public PlayerSummaryBuilder(IDataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        internal class TopPlayerWithFlatPoints : TopPlayer
        {
            public int BaseNemePoints { get; set; }
            public int GameWeightBonusPoints { get; set; }
            public int GameDurationBonusPoints { get; set; }
            public int TotalPoints { get; set; }
        }

        public virtual List<TopPlayer> GetTopPlayers(int numberOfPlayersToRetrieve)
        {
            var data = dataContext.MakeRawSqlQuery<TopPlayerWithFlatPoints>(string.Format(SQL_GET_TOP_PLAYERS, numberOfPlayersToRetrieve));

            var topPlayers = data.Select(x => new TopPlayer
            {
                NemePointsSummary = new NemePointsSummary(x.BaseNemePoints, x.GameDurationBonusPoints, x.GameWeightBonusPoints),
                PlayerName = x.PlayerName,
                WinPercentage = x.WinPercentage,
                PlayerId = x.PlayerId,
                TotalNumberOfGamesPlayed = x.TotalNumberOfGamesPlayed
            });
            //WinPercentage as it is originally pulled back from the query contains the number of games won and we have to
            //do the below math to switch it to a win %
            foreach(var player in topPlayers)
            {
                player.WinPercentage = WinPercentageCalculator.CalculateWinPercentage(player.WinPercentage, player.TotalNumberOfGamesPlayed - player.WinPercentage);
            }

            return topPlayers.ToList();
        }
    }
}
