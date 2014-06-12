using BusinessLogic.DataAccess;
using BusinessLogic.Models;
using BusinessLogic.Logic;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.Models.Games;
using System.Data.Entity;

namespace BusinessLogic.Tests.UnitTests.LogicTests.PlayedGameRepositoryTests
{
    [TestFixture]
    public class CreatePlayedGameTests
    {
        private NemeStatsDbContext dbContext = null;
        PlayedGameLogic playedGameLogic = null;

        [TestFixtureSetUp]
        public void SetUp()
        {
            dbContext = MockRepository.GenerateMock<NemeStatsDbContext>();
            playedGameLogic = new PlayedGameRepository(dbContext);
        }

        [Test, Ignore("need to check this with someone who knows how to test EF stuff. "
            + "Doesn't look like I'm setting my expectations right. Also need clarification on how many separate tests there should be.")]
        public void ItSavesAPlayedGameIfThereIsAGameDefinition()
        {
            int gameDefinitionId = 1354;
            int playerOneId = 3515;
            int playerTwoId = 15151;
            int playerOneRank = 1;
            int playerTwoRank = 2;
            NewlyCompletedGame newlyCompletedGame = new NewlyCompletedGame() { GameDefinitionId = gameDefinitionId };
            List<PlayerRank> playerRanks = new List<PlayerRank>();
            playerRanks.Add(new PlayerRank() { PlayerId = playerOneId, GameRank = playerOneRank });
            playerRanks.Add(new PlayerRank() { PlayerId = playerTwoId, GameRank = playerTwoRank });
            newlyCompletedGame.PlayerRanks = playerRanks;
            
            DbSet<PlayedGame> playedGamesDbSet = MockRepository.GenerateMock<DbSet<PlayedGame>>();
            dbContext.Expect(context => context.PlayedGames).Repeat.Once().Return(playedGamesDbSet);
            playedGamesDbSet.Expect(dbSet => dbSet.Add(Arg<PlayedGame>.Is.Anything));

            playedGameLogic.CreatePlayedGame(newlyCompletedGame);

            dbContext.AssertWasCalled(context => context.PlayedGames);
            //TODO need grant help on this test
            playedGamesDbSet.AssertWasCalled(dbSet => dbSet.Add(
                    Arg<PlayedGame>.Matches(game => game.GameDefinitionId == gameDefinitionId
                                                && game.NumberOfPlayers == playerRanks.Count()
                                                && game.DatePlayed.Date.Equals(DateTime.UtcNow))));
        }
    }
}
