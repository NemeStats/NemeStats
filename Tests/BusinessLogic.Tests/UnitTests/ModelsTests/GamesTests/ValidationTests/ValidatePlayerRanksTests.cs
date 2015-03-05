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

#endregion LICENSE

using BusinessLogic.Models.Games;
using BusinessLogic.Models.Games.Validation;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace BusinessLogic.Tests.UnitTests.ModelsTests.GamesTests.ValidationTests
{
	[TestFixture]
	public class ValidatePlayerRanksTests
	{
		[Test]
		[ExpectedException(typeof(ArgumentException), ExpectedMessage = PlayerRankValidator.EXCEPTION_MESSAGE_MUST_PASS_AT_LEAST_ONE_PLAYER)]
		public void ItRequiresPlayerRanks()
		{
			List<PlayerRank> playerRanks = null;

			PlayerRankValidator.ValidatePlayerRanks(playerRanks);
		}

		[Test]
		[ExpectedException(typeof(ArgumentException), ExpectedMessage = PlayerRankValidator.EXCEPTION_MESSAGE_MUST_PASS_AT_LEAST_ONE_PLAYER)]
		public void ItRequiresAtLeastOnePlayer()
		{
			List<PlayerRank> playerRanks = new List<PlayerRank>();

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
		[ExpectedException(typeof(ArgumentException), ExpectedMessage = PlayerRankValidator.EXCEPTION_MESSAGE_NO_PLAYER_CAN_HAVE_A_HIGHER_RANK_THAN_THE_NUMBER_OF_PLAYERS)]
		public void NoPlayerMayHaveARankGreaterThanTheTotalNumberOfPlayers()
		{
			List<PlayerRank> playerRanks = new List<PlayerRank>();
			playerRanks.Add(new PlayerRank() { PlayerId = 1, GameRank = 1 });
			playerRanks.Add(new PlayerRank() { PlayerId = 2, GameRank = 3 });

			PlayerRankValidator.ValidatePlayerRanks(playerRanks);
		}

		[Test]
		[ExpectedException(typeof(ArgumentException), ExpectedMessage = PlayerRankValidator.EXCEPTION_MESSAGE_NO_PLAYER_CAN_HAVE_A_RANK_LESS_THAN_ONE)]
		public void NoPlayersMayHaveARankLessThanOne()
		{
			List<PlayerRank> playerRanks = new List<PlayerRank>();
			playerRanks.Add(new PlayerRank() { PlayerId = 1, GameRank = 1 });
			playerRanks.Add(new PlayerRank() { PlayerId = 2, GameRank = 0 });

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