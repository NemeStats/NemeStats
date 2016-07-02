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
using System;
using System.Collections.Generic;
using System.Linq;

namespace BusinessLogic.Models.Games.Validation
{
	public class PlayerRankValidator
	{
	    public const int MINIMUM_NUMBER_OF_PLAYERS = 2;
        public const int MAXIMUM_NUMBER_OF_PLAYERS = 25;
		internal const string EXCEPTION_MESSAGE_NO_PLAYER_CAN_HAVE_A_RANK_LESS_THAN_ONE = "No player may have a rank less than 1.";
		internal const string EXCEPTION_MESSAGE_EACH_PLAYER_RANK_MUST_HAVE_A_GAME_RANK = "Each PlayerRank must have a valid GameRank.";
		internal const string EXCEPTION_MESSAGE_MUST_PASS_AT_LEAST_TWO_PLAYERS = "Each game must have at least two players.";
	    internal const string EXCEPTION_MESSAGE_CANNOT_HAVE_MORE_THAN_25_PLAYERS = "A game cannot have more than 25 players.";
		internal const string EXCEPTION_MESSAGE_NO_PLAYER_CAN_HAVE_A_HIGHER_RANK_THAN_THE_NUMBER_OF_PLAYERS = "No player can have a higher rank than the total number of players in the game.";

		public static void ValidatePlayerRanks(List<IPlayerRank> playerRanks)
		{
			ValidateThatPlayerRanksIsNotNull(playerRanks);
		    ValidateThatAllGameRanksAreSet(playerRanks);
			ValidateThatThereAreAtLeastTwoPlayers(playerRanks);
		    ValidateThatThereAreNoMoreThanTwentyFivePlayers(playerRanks);
			ValidateThatNoPlayerHasARankGreaterThanTheNumberOfPlayers(playerRanks);
			ValidateNoPlayerHasARankLessThanOne(playerRanks);
		}

	    private static void ValidateThatNoPlayerHasARankGreaterThanTheNumberOfPlayers(List<IPlayerRank> playerRanks)
		{
			if (playerRanks.Max(playerRank => playerRank.GameRank) > playerRanks.Count)
			{
				throw new ArgumentException(EXCEPTION_MESSAGE_NO_PLAYER_CAN_HAVE_A_HIGHER_RANK_THAN_THE_NUMBER_OF_PLAYERS);
			}
		}

		private static void ValidateThatPlayerRanksIsNotNull(List<IPlayerRank> playerRanks)
		{
			if (playerRanks == null)
			{
				throw new ArgumentException(EXCEPTION_MESSAGE_MUST_PASS_AT_LEAST_TWO_PLAYERS);
			}
		}

	    private static void ValidateThatAllGameRanksAreSet(List<IPlayerRank> playerRanks)
		{
			if (playerRanks.Any(playerRank => playerRank.GameRank == 0))
			{
				throw new ArgumentException(EXCEPTION_MESSAGE_EACH_PLAYER_RANK_MUST_HAVE_A_GAME_RANK);
			}
		}

		private static void ValidateThatThereAreAtLeastTwoPlayers(List<IPlayerRank> playerRanks)
		{
            if (playerRanks.Count < MINIMUM_NUMBER_OF_PLAYERS)
			{
				throw new ArgumentException(EXCEPTION_MESSAGE_MUST_PASS_AT_LEAST_TWO_PLAYERS);
			}
		}

        private static void ValidateThatThereAreNoMoreThanTwentyFivePlayers(List<IPlayerRank> playerRanks)
        {
            if (playerRanks.Count > MAXIMUM_NUMBER_OF_PLAYERS)
            {
                throw new ArgumentException(EXCEPTION_MESSAGE_CANNOT_HAVE_MORE_THAN_25_PLAYERS);
            }
        }

		private static void ValidateNoPlayerHasARankLessThanOne(List<IPlayerRank> playerRanks)
		{
			if (playerRanks.Any(rank => rank.GameRank < 1))
			{
				throw new ArgumentException(EXCEPTION_MESSAGE_NO_PLAYER_CAN_HAVE_A_RANK_LESS_THAN_ONE);
			}
		}
	}
}
