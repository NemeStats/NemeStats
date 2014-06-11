using BusinessLogic.Models.Games;
using BusinessLogic.Models.Games.Validation;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Tests.UnitTests.ModelsTests.GamesTests.ValidationTests
{
    [TestFixture]
    public class ValidatePlayerRanksTests
    {
        [Test]
        [ExpectedException(typeof(ArgumentException), ExpectedMessage = PlayerRankValidator.EXCEPTION_MESSAGE_MUST_PASS_AT_LEAST_TWO_PLAYERS)]
        public void ItRequiresMoreThanOnePlayer()
        {
            List<PlayerRank> playerRanks = new List<PlayerRank>() { new PlayerRank { GameRank = 1, PlayerId = 1 } };
            
            PlayerRankValidator.ValidatePlayerRanks(playerRanks);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException), ExpectedMessage = PlayerRankValidator.EXCEPTION_MESSAGE_EACH_PLAYER_RANK_MUST_HAVE_A_PLAYER_ID)]
        public void ItRequiresEachPlayerRankToHaveAPlayer()
        {
            List<PlayerRank> playerRanks = new List<PlayerRank>() { new PlayerRank() { GameRank = 1 } };

            PlayerRankValidator.ValidatePlayerRanks(playerRanks);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException), ExpectedMessage = PlayerRankValidator.EXCEPTION_MESSAGE_EACH_PLAYER_RANK_MUST_HAVE_A_GAME_RANK)]
        public void ItRequiresEachPlayerRankToAGameRank()
        {
            List<PlayerRank> playerRanks = new List<PlayerRank>() 
                                            { 
                                                new PlayerRank() { PlayerId = 1, GameRank = 1 },
                                                new PlayerRank() { PlayerId = 2 } 
                                            };

            PlayerRankValidator.ValidatePlayerRanks(playerRanks);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException), ExpectedMessage = PlayerRankValidator.EXCEPTION_MESSAGE_GAME_MUST_HAVE_A_WINNER)]
        public void ItRequiresAWinner()
        {
            List<PlayerRank> playerRanks = new List<PlayerRank>();
            playerRanks.Add(new PlayerRank() { PlayerId = 1, GameRank = 2 });
            playerRanks.Add(new PlayerRank() { PlayerId = 1, GameRank = 3 });

            //TODO possibly implement this way instead of ExpectedExceptions
            //Assert.Throws<ArgumentException>(()=> PlayerRankValidator.ValidatePlayerRanks(newlyCompletedGameWithoutAWinner))
            PlayerRankValidator.ValidatePlayerRanks(playerRanks);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException), ExpectedMessage = PlayerRankValidator.EXCEPTION_MESSAGE_GAME_MUST_NOT_HAVE_A_GAP_IN_RANKS)]
        public void ItRequiresContiguousRankingOfPlayers()
        {
            List<PlayerRank> playerRanks = new List<PlayerRank>();
            playerRanks.Add(new PlayerRank() { PlayerId = 1, GameRank = 1 });
            playerRanks.Add(new PlayerRank() { PlayerId = 2, GameRank = 3 });

            PlayerRankValidator.ValidatePlayerRanks(playerRanks);
        }

        [Test]
        public void ItAcceptsAGameWithRanksOneTwoAndThreeRanks()
        {
            List<PlayerRank> playerRanks = new List<PlayerRank>();
            playerRanks.Add(new PlayerRank() { PlayerId = 1, GameRank = 1 });
            playerRanks.Add(new PlayerRank() { PlayerId = 2, GameRank = 1 });
            playerRanks.Add(new PlayerRank() { PlayerId = 3, GameRank = 2 });
            playerRanks.Add(new PlayerRank() { PlayerId = 4, GameRank = 3 });

            PlayerRankValidator.ValidatePlayerRanks(playerRanks);
        }
    }
}
