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

namespace BusinessLogic.Logic.Players
{
    public class PlayerSummaryBuilder : IPlayerSummaryBuilder
    {
        private const string SQL_GET_TOP_PLAYERS = @"SELECT TOP {0} Player.Id AS PlayerId, 
            Player.Name AS PlayerName, 
            COUNT(*) AS TotalNumberOfGamesPlayed, 
            SUM(PlayerGameResult.NemeStatsPointsAwarded) AS TotalPoints,
            SUM(CASE WHEN PlayerGameResult.GameRank = 1 THEN 1 ELSE 0 END) AS WinPercentage
            FROM Player INNER JOIN PlayerGameResult ON Player.Id = PlayerGameResult.PlayerId
            WHERE Player.Active = 1
            GROUP BY Player.Id, Player.Name
            ORDER BY TotalPoints DESC";

        private readonly IDataContext dataContext;

        public PlayerSummaryBuilder(IDataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        public virtual List<TopPlayer> GetTopPlayers(int numberOfPlayersToRetrieve)
        {
            DbRawSqlQuery<TopPlayer> data = dataContext.MakeRawSqlQuery<TopPlayer>(string.Format(SQL_GET_TOP_PLAYERS, numberOfPlayersToRetrieve));

            List<TopPlayer> topPlayers = data.ToList();
            //WinPercentage as it is originally pulled back from the query contains the number of games won and we have to
            //do the below math to switch it to a win %
            topPlayers.ForEach(player => player.WinPercentage = WinPercentageCalculator.CalculateWinPercentage(player.WinPercentage, player.TotalNumberOfGamesPlayed));

            return topPlayers;
        }
    }
}
