using System;
using System.Collections.Generic;
using System.Linq;

namespace BusinessLogic.Models.Games.Validation
{
    public class PlayerRankValidator
    {
        internal const string EXCEPTION_MESSAGE_NO_PLAYER_CAN_HAVE_A_RANK_LESS_THAN_ONE = "No player may have a rank less than 1.";
        internal const string EXCEPTION_MESSAGE_EACH_PLAYER_RANK_MUST_HAVE_A_PLAYER_ID = "Each PlayerRank must have a valid PlayerId.";
        internal const string EXCEPTION_MESSAGE_EACH_PLAYER_RANK_MUST_HAVE_A_GAME_RANK = "Each PlayerRank must have a valid GameRank.";
        internal const string EXCEPTION_MESSAGE_MUST_PASS_AT_LEAST_TWO_PLAYERS = "Must pass in at least two players.";
        internal const string EXCEPTION_MESSAGE_GAME_MUST_HAVE_A_WINNER = "The game must have at least one winner (GameRank = 1).";
        internal const string EXCEPTION_MESSAGE_NO_PLAYER_CAN_HAVE_A_HIGHER_RANK_THAN_THE_NUMBER_OF_PLAYERS = "No player can have a higher rank than the total number of players in the game.";

        public static void ValidatePlayerRanks(List<PlayerRank> playerRanks)
        {
            ValidateThatPlayerRanksIsNotNull(playerRanks);
            ValidateThatAllPlayerIdsAreSet(playerRanks);
            ValidateThatAllGameRanksAreSet(playerRanks);
            ValidateThatThereAreAtLeastTwoPlayers(playerRanks);
            ValidateThatThereIsAWinner(playerRanks);
            ValidateThatNoPlayerHasARankGreaterThanTheNumberOfPlayers(playerRanks);
            ValidateNoPlayerHasARankLessThanOne(playerRanks);
        }

        private static void ValidateThatNoPlayerHasARankGreaterThanTheNumberOfPlayers(List<PlayerRank> playerRanks)
        {
            if(playerRanks.Max(playerRank => playerRank.GameRank).Value > playerRanks.Count)
            {
                throw new ArgumentException(EXCEPTION_MESSAGE_NO_PLAYER_CAN_HAVE_A_HIGHER_RANK_THAN_THE_NUMBER_OF_PLAYERS);
            }
        }

        private static void ValidateThatPlayerRanksIsNotNull(List<PlayerRank> playerRanks)
        {
            if (playerRanks == null)
            {
                throw new ArgumentException(EXCEPTION_MESSAGE_MUST_PASS_AT_LEAST_TWO_PLAYERS);
            }
        }

        private static void ValidateThatAllPlayerIdsAreSet(List<PlayerRank> playerRanks)
        {
            if (playerRanks.Any(playerRank => playerRank.PlayerId == null))
            {
                throw new ArgumentException(EXCEPTION_MESSAGE_EACH_PLAYER_RANK_MUST_HAVE_A_PLAYER_ID);
            }
        }

        private static void ValidateThatAllGameRanksAreSet(List<PlayerRank> playerRanks)
        {
            if (playerRanks.Any(playerRank => playerRank.GameRank == null))
            {
                throw new ArgumentException(EXCEPTION_MESSAGE_EACH_PLAYER_RANK_MUST_HAVE_A_GAME_RANK);
            }
        }

        private static void ValidateThatThereAreAtLeastTwoPlayers(List<PlayerRank> playerRanks)
        {
            if (playerRanks.Count < 2)
            {
                throw new ArgumentException(EXCEPTION_MESSAGE_MUST_PASS_AT_LEAST_TWO_PLAYERS);
            }
        }

        private static void ValidateThatThereIsAWinner(List<PlayerRank> playerRanks)
        {
            if (playerRanks.All(playerRank => playerRank.GameRank != 1))
            {
                throw new ArgumentException(EXCEPTION_MESSAGE_GAME_MUST_HAVE_A_WINNER);
            }
        }

        private static void ValidateNoPlayerHasARankLessThanOne(List<PlayerRank> playerRanks)
        {
            if (playerRanks.Any(rank => rank.GameRank < 1))
            {
                throw new ArgumentException(EXCEPTION_MESSAGE_NO_PLAYER_CAN_HAVE_A_RANK_LESS_THAN_ONE);
            }
        }
    }
}
