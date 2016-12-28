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
using BusinessLogic.DataAccess.Repositories;
using NUnit.Framework;
using BusinessLogic.DataAccess;
using BusinessLogic.Models.Players;
using BusinessLogic.Models;
using BusinessLogic.Logic.Players;
using BusinessLogic.Logic.PlayedGames;

namespace BusinessLogic.Tests.IntegrationTests.DataAccessTests.RepositoriesTests.EntityFrameworkPlayerRepositoryTests
{
    [TestFixture]
    public class GetNemesisTests : IntegrationTestBase
    {
        private IDataContext dataContext;
        private IPlayerRetriever playerRetriever;
        private PlayerDetails player1Details;
        private PlayerDetails player5Details;

        [OneTimeSetUp]
        public override void FixtureSetUp()
        {
            base.FixtureSetUp();

            dataContext = new NemeStatsDataContext();
            IPlayerRepository playerRepository = new EntityFrameworkPlayerRepository(dataContext);
            IPlayedGameRetriever playedGameRetriever = new PlayedGameRetriever(dataContext);
            playerRetriever = new PlayerRetriever(dataContext, playerRepository, playedGameRetriever);
            player1Details = playerRetriever.GetPlayerDetails(testPlayer1.Id, 0);
            player5Details = playerRetriever.GetPlayerDetails(testPlayer5.Id, 0);
        }

        [Test]
        public void ItGetsThePlayerWithTheHighestWinPercentageAgainstMe()
        {
            //player 4 beat player 1 3 times
            Assert.AreEqual(testPlayer4.Id, player1Details.CurrentNemesis.NemesisPlayerId);
        }

        [Test]
        public void ANemesisMustBeActive()
        {
            //player 5 is inactive but beat player 1 three times
            PlayerDetails player1Details = playerRetriever.GetPlayerDetails(testPlayer1.Id, 0);
            Assert.AreNotEqual(testPlayer5.Id, player1Details.CurrentNemesis.NemesisPlayerId);
        }
        
        [Test]
        public void ItReturnsANullNemesisIfThereIsNoNemesis()
        {
            //player 5 has no nemesis
            Assert.True(player5Details.CurrentNemesis is NullNemesis);
        }

        [Test]
        public void ANemesisMustHaveWonAtLeastACertainNumberOfGames()
        {
            //player2 beat player5 once (100% of the time) but this isn't enough to be a nemesis
            Assert.AreNotEqual(testPlayer2.Id, player5Details.CurrentNemesis.NemesisPlayerId);
        }

        [Test]
        public void ItSetsTheNemesisPlayerId()
        {
            Assert.AreEqual(testPlayer4.Id, player1Details.CurrentNemesis.NemesisPlayerId);
        }

        [Test]
        public void ItSetsTheLossPercentageVersusTheNemesis()
        {
            //player 1 lost 100% of their games against player 4
            Assert.AreEqual(100, player1Details.CurrentNemesis.LossPercentage);
        }

        [Test]
        public void ItSetsTheNumberOfGamesLostVersusTheNemesis()
        {
            Assert.AreEqual(3, player1Details.CurrentNemesis.NumberOfGamesLost);
        }

        [Test]
        public void ItSetsTheNemesisIdOnThePlayer()
        {
            using(NemeStatsDbContext nemeStatsDbContext = new NemeStatsDbContext())
            {
                using(NemeStatsDataContext nemeStatsDataContext = new NemeStatsDataContext())
                {
                    Player player1 = nemeStatsDataContext.FindById<Player>(testPlayer1.Id);

                    Assert.NotNull(player1.NemesisId);
                }
            }
        }

        [Test]
        public void ItClearsTheNemesisIdIfThePlayerHasNoNemesis()
        {
            using (NemeStatsDbContext nemeStatsDbContext = new NemeStatsDbContext())
            {
                using (NemeStatsDataContext nemeStatsDataContext = new NemeStatsDataContext())
                {
                    Player player5 = nemeStatsDataContext.FindById<Player>(testPlayer5.Id);

                    Assert.Null(player5.NemesisId);
                }
            }
        }

        [OneTimeTearDown]
        public override void FixtureTearDown()
        {
            base.FixtureTearDown();
            dataContext.Dispose();
        }
    }
}
