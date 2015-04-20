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
using BusinessLogic.Models;
using System;
using System.Linq;
using UI.Models.PlayedGame;

namespace UI.Transformations
{
    public class GameResultViewModelBuilder : IGameResultViewModelBuilder
    {
        internal const string EXCEPTION_PLAYER_CANNOT_BE_NULL = "PlayerGameResult.Player cannot be null.";
        internal const string EXCEPTION_GAME_DEFINITION_CANNOT_BE_NULL = "PlayerGameResult.PlayedGame.GameDefinition cannot be null.";
        internal const string EXCEPTION_PLAYED_GAME_CANNOT_BE_NULL = "PlayerGameResult.PlayedGame cannot be null.";

        public GameResultViewModel Build(PlayerGameResult playerGameResult)
        {
            Validate(playerGameResult);

            GameResultViewModel result = new GameResultViewModel();
            result.PlayerId = playerGameResult.PlayerId;
            result.PlayerName = playerGameResult.Player.Name;
            result.GameRank = playerGameResult.GameRank;
            result.GordonPoints = playerGameResult.NemeStatsPointsAwarded;
            result.GameDefinitionId = playerGameResult.PlayedGame.GameDefinition.Id;
            result.GameName = playerGameResult.PlayedGame.GameDefinition.Name;
            result.PlayedGameId = playerGameResult.PlayedGameId;
            result.DatePlayed = playerGameResult.PlayedGame.DatePlayed;

            return result;
        }

        private static void Validate(PlayerGameResult playerGameResult)
        {
            ValidatePlayerGameResultIsNotNull(playerGameResult);
            ValidatePlayerIsNotNull(playerGameResult);
            ValidatePlayedGameIsNotNull(playerGameResult);
            ValidateGameDefinitionIsNotNull(playerGameResult);
        }

        private static void ValidatePlayerGameResultIsNotNull(PlayerGameResult playerGameResult)
        {
            if (playerGameResult == null)
            {
                throw new ArgumentNullException("playerGameResult");
            }
        }

        private static void ValidatePlayerIsNotNull(PlayerGameResult playerGameResult)
        {
            if (playerGameResult.Player == null)
            {
                throw new ArgumentException(EXCEPTION_PLAYER_CANNOT_BE_NULL);
            }
        }

        private static void ValidatePlayedGameIsNotNull(PlayerGameResult playerGameResult)
        {
            if (playerGameResult.PlayedGame == null)
            {
                throw new ArgumentException(EXCEPTION_PLAYED_GAME_CANNOT_BE_NULL);
            }
        }

        private static void ValidateGameDefinitionIsNotNull(PlayerGameResult playerGameResult)
        {
            if (playerGameResult.PlayedGame.GameDefinition == null)
            {
                throw new ArgumentException(EXCEPTION_GAME_DEFINITION_CANNOT_BE_NULL);
            }
        }
    }
}