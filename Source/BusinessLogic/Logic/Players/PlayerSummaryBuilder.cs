using BusinessLogic.DataAccess;
using BusinessLogic.Models.Players;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.Logic;
using BusinessLogic.Models;
using BusinessLogic.Models.User;
using System.Data.Entity;

namespace BusinessLogic.Logic.Players
{
    public class PlayerSummaryBuilder : IPlayerSummaryBuilder
    {
        private static readonly string SQL_GET_TOP_PLAYERS = @"SELECT TOP {0} Player.Id AS PlayerId, 
            Player.Name AS PlayerName, 
            COUNT(*) AS TotalNumberOfGamesPlayed, 
            SUM(PlayerGameResult.GordonPoints) AS TotalPoints,
            SUM(CASE WHEN PlayerGameResult.GameRank = 1 THEN 1 ELSE 0 END) AS WinPercentage
            FROM Player INNER JOIN PlayerGameResult ON Player.Id = PlayerGameResult.PlayerId
            WHERE Player.Active = 1
            GROUP BY Player.Id, Player.Name
            ORDER BY TotalNumberOfGamesPlayed DESC";

        private IDataContext dataContext;

        public PlayerSummaryBuilder(IDataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        public virtual List<TopPlayer> GetTopPlayers(int numberOfPlayersToRetrieve)
        {
            DbRawSqlQuery<TopPlayer> data = dataContext.MakeRawSqlQuery<TopPlayer>(string.Format(SQL_GET_TOP_PLAYERS, numberOfPlayersToRetrieve));

            List<TopPlayer> topPlayers = data.ToList<TopPlayer>();
            //WinPercentage as it is originally pulled back from the query contains the number of games won and we have to
            //do the below math to switch it to a win %
            topPlayers.ForEach(player => player.WinPercentage = CalculateWinPercentage(player.WinPercentage, player.TotalNumberOfGamesPlayed));

            return topPlayers;
        }

        internal virtual int CalculateWinPercentage(int totalGamesWon, int totalGamesPlayed)
        {
            return (int)(((float)totalGamesWon / (float)totalGamesPlayed) * 100);
        }
    }
}
