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
using BusinessLogic.DataAccess.Repositories;
using BusinessLogic.Logic.Nemeses;
using BusinessLogic.Logic.Players;
using BusinessLogic.Models;
using BusinessLogic.Models.Players;
using NUnit.Framework;
using System.Linq;

namespace BusinessLogic.Tests.IntegrationTests.DataAccessTests.RepositoriesTests.EntityFrameworkPlayerRepositoryTests
{
    [TestFixture]
    public class GetPlayerStatisticsIntegrationTests : IntegrationTestBase
    {
        [Test]
        public void ItGetsTheGamesPlayedMetrics()
        {
            using (IDataContext dataContext = new NemeStatsDataContext())
            {
                IPlayerRepository playerRepository = new EntityFrameworkPlayerRepository(dataContext);
                IPlayerRetriever playerRetriever = new PlayerRetriever(dataContext, playerRepository);
                PlayerStatistics playerStatistics = playerRetriever.GetPlayerStatistics(testPlayer1.Id);

                int totalGamesForPlayer1 = testPlayedGames
                    .Count(playedGame => playedGame.PlayerGameResults
                                                   .Any(playerGameResult => playerGameResult.PlayerId == testPlayer1.Id));
                Assert.That(playerStatistics.TotalGames, Is.EqualTo(totalGamesForPlayer1));

                int totalWinsForPlayer1 = testPlayedGames
                    .Count(playedGame => playedGame.PlayerGameResults
                                                   .Any(playerGameResult => playerGameResult.PlayerId == testPlayer1.Id && playerGameResult.GameRank == 1));
                Assert.That(playerStatistics.TotalGamesWon, Is.EqualTo(totalWinsForPlayer1));

                int totalLossesForPlayer1 = testPlayedGames
                    .Count(playedGame => playedGame.PlayerGameResults
                                                   .Any(playerGameResult => playerGameResult.PlayerId == testPlayer1.Id && playerGameResult.GameRank != 1));
                Assert.That(playerStatistics.TotalGamesLost, Is.EqualTo(totalLossesForPlayer1));

                int winPercentageForPlayer1 = (int)((decimal)totalWinsForPlayer1 / (totalGamesForPlayer1) * 100);

                Assert.That(playerStatistics.WinPercentage, Is.EqualTo(winPercentageForPlayer1));
            }

        }

        [Test]
        public void ItGetsTheTotalPoints()
        {
            using (IDataContext dataContext = new NemeStatsDataContext())
            {
                IPlayerRepository playerRepository = new EntityFrameworkPlayerRepository(dataContext);
                IPlayerRetriever playerRetriever = new PlayerRetriever(dataContext, playerRepository); 
                PlayerStatistics playerStatistics = playerRetriever.GetPlayerStatistics(testPlayer1.Id);

                int totalPoints = 0;

                foreach(PlayedGame playedGame in testPlayedGames)
                {
                    if(playedGame.PlayerGameResults.Any(result => result.PlayerId == testPlayer1.Id))
                    {
                        totalPoints += playedGame.PlayerGameResults.First(result => result.PlayerId == testPlayer1.Id).NemeStatsPointsAwarded;
                    }
                }

                Assert.AreEqual(totalPoints, playerStatistics.TotalPoints);
            }
        }

        [Test]
        public void ItGetsTheAveragePlayersPerGame()
        {
            using (IDataContext dataContext = new NemeStatsDataContext())
            {
                IPlayerRepository playerRepository = new EntityFrameworkPlayerRepository(dataContext);
                IPlayerRetriever playerRetriever = new PlayerRetriever(dataContext, playerRepository);
                PlayerStatistics playerStatistics = playerRetriever.GetPlayerStatistics(testPlayer1.Id);

                float averagePlayersPerGame = (float)testPlayedGames.Where(game => game.PlayerGameResults.Any(result => result.PlayerId == testPlayer1.Id))
                    .Average(game => game.NumberOfPlayers);

                Assert.AreEqual(averagePlayersPerGame, playerStatistics.AveragePlayersPerGame);
            }
        }
    }
}
