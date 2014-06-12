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
        private int playedGameId = 2;

        [TestFixtureSetUp]
        public void SetUp()
        {
            dbContext = MockRepository.GenerateMock<NemeStatsDbContext>();
            playedGameLogic = new PlayedGameRepository(dbContext);
        }

        /*
        [Test]
        [ExpectedException(typeof(ArgumentException), ExpectedMessage = PlayedGameRepository.EXCEPTION_MESSAGE_MUST_PASS_AT_LEAST_TWO_PLAYERS)]
        public void ItRequiresMoreThanOnePlayer()
        {
            NewlyCompletedGame newlyCompletedGameThatHasNoPlayers = new NewlyCompletedGame();
            newlyCompletedGameThatHasNoPlayers.GameDefinitionId = playedGameId;
            newlyCompletedGameThatHasNoPlayers.PlayerRanks = new List<PlayerRank>() { new PlayerRank { GameRank = 1, PlayerId = 1 }};

            playedGameLogic.CreatePlayedGame(newlyCompletedGameThatHasNoPlayers);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException), ExpectedMessage = PlayedGameRepository.EXCEPTION_MESSAGE_MUST_PASS_VALID_GAME_DEFINITION_ID)]
        public void ItRequiresAGame()
        {
            NewlyCompletedGame newlyCompletedGameThatHasNoGameDefinitionId = new NewlyCompletedGame();
            newlyCompletedGameThatHasNoGameDefinitionId.PlayerRanks = new List<PlayerRank>() { new PlayerRank() { PlayerId = 2, GameRank = 1 } };

            playedGameLogic.CreatePlayedGame(newlyCompletedGameThatHasNoGameDefinitionId);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException), ExpectedMessage = PlayedGameRepository.EXCEPTION_MESSAGE_EACH_PLAYER_RANK_MUST_HAVE_A_PLAYER_ID)]
        public void ItRequiresEachPlayerRankToHaveAPlayer()
        {
            NewlyCompletedGame newlyCompletedGameThatHasNoPlayerId = new NewlyCompletedGame() { GameDefinitionId = 1 };
            newlyCompletedGameThatHasNoPlayerId.PlayerRanks = new List<PlayerRank>() { new PlayerRank() { GameRank = 1 } };

            playedGameLogic.CreatePlayedGame(newlyCompletedGameThatHasNoPlayerId);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException), ExpectedMessage = PlayedGameRepository.EXCEPTION_MESSAGE_EACH_PLAYER_RANK_MUST_HAVE_A_GAME_RANK)]
        public void ItRequiresEachPlayerRankToAGameRank()
        {
            NewlyCompletedGame newlyCompletedGameThatHasNoGameRank = new NewlyCompletedGame() { GameDefinitionId = 1 };
            newlyCompletedGameThatHasNoGameRank.PlayerRanks = new List<PlayerRank>() 
                                            { 
                                                new PlayerRank() { PlayerId = 1, GameRank = 1 },
                                                new PlayerRank() { PlayerId = 2 } 
                                            };

            playedGameLogic.CreatePlayedGame(newlyCompletedGameThatHasNoGameRank);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException), ExpectedMessage = PlayedGameRepository.EXCEPTION_MESSAGE_GAME_MUST_HAVE_A_WINNER)]
        public void ItRequiresAWinner()
        {
            NewlyCompletedGame newlyCompletedGameWithoutAWinner = new NewlyCompletedGame() { GameDefinitionId = 1 };
            List<PlayerRank> playerRanks = new List<PlayerRank>();
            playerRanks.Add(new PlayerRank() { PlayerId = 1, GameRank = 2 });
            playerRanks.Add(new PlayerRank() { PlayerId = 1, GameRank = 3 });
            newlyCompletedGameWithoutAWinner.PlayerRanks = playerRanks;

            //TODO possibly implement this way instead of ExpectedExceptions
            //Assert.Throws<ArgumentException>(()=> playedGameLogic.CreatePlayedGame(newlyCompletedGameWithoutAWinner))
            playedGameLogic.CreatePlayedGame(newlyCompletedGameWithoutAWinner);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException), ExpectedMessage = PlayedGameRepository.EXCEPTION_MESSAGE_GAME_MUST_NOT_HAVE_A_GAP_IN_RANKS)]
        public void ItRequiresContiguousRankingOfPlayers()
        {
            NewlyCompletedGame newlyCompletedGameWithUncontiguousGameRanks = new NewlyCompletedGame() { GameDefinitionId = 1 };
            List<PlayerRank> playerRanks = new List<PlayerRank>();
            playerRanks.Add(new PlayerRank() { PlayerId = 1, GameRank = 1 });
            playerRanks.Add(new PlayerRank() { PlayerId = 2, GameRank = 3 });
            newlyCompletedGameWithUncontiguousGameRanks.PlayerRanks = playerRanks;

            playedGameLogic.CreatePlayedGame(newlyCompletedGameWithUncontiguousGameRanks);
        }

        [Test]
        public void ItAcceptsAGameWithRanksOneTwoAndThreeRanks()
        {
            NewlyCompletedGame newlyCompletedGameWithUncontiguousGameRanks = new NewlyCompletedGame() { GameDefinitionId = 1 };
            List<PlayerRank> playerRanks = new List<PlayerRank>();
            playerRanks.Add(new PlayerRank() { PlayerId = 1, GameRank = 1 });
            playerRanks.Add(new PlayerRank() { PlayerId = 2, GameRank = 1 });
            playerRanks.Add(new PlayerRank() { PlayerId = 3, GameRank = 2 });
            playerRanks.Add(new PlayerRank() { PlayerId = 4, GameRank = 3 });
            newlyCompletedGameWithUncontiguousGameRanks.PlayerRanks = playerRanks;

            playedGameLogic.CreatePlayedGame(newlyCompletedGameWithUncontiguousGameRanks);
        }
         */

        [Test]
        public void ItRequiresAGameDefinition()
        {
            NewlyCompletedGame newlyCompletedGameWithoutAGameDefinition = new NewlyCompletedGame();
            List<PlayerRank> playerRanks = new List<PlayerRank>();
            playerRanks.Add(new PlayerRank() { PlayerId = 1, GameRank = 1 });
            playerRanks.Add(new PlayerRank() { PlayerId = 2, GameRank = 2 });
            newlyCompletedGameWithoutAGameDefinition.PlayerRanks = playerRanks;

            //TODO possibly implement this way instead of ExpectedExceptions
            ArgumentException argumentException = Assert.Throws<ArgumentException>(()=> 
                                            playedGameLogic.CreatePlayedGame(newlyCompletedGameWithoutAGameDefinition));
            Assert.AreEqual(PlayedGameRepository.EXCEPTION_MESSAGE_MUST_PASS_VALID_GAME_DEFINITION_ID, argumentException.Message);
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
