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
using BusinessLogic.Models.PlayedGames;
using BusinessLogic.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using BusinessLogic.Logic.BoardGameGeek;
using UI.Models.PlayedGame;

namespace UI.Transformations
{
    public class PlayedGameDetailsViewModelBuilder : IPlayedGameDetailsViewModelBuilder
    {
        internal const string EXCEPTION_MESSAGE_GAME_DEFINITION_CANNOT_BE_NULL = "PlayedGame.GameDefinition cannot be null.";
        internal const string EXCEPTION_MESSAGE_PLAYER_GAME_RESULTS_CANNOT_BE_NULL = "PlayedGame.PlayerGameResults cannot be null.";
        internal const string EXCEPTION_MESSAGE_GAMING_GROUP_CANNOT_BE_NULL = "PlayedGame.GamingGroup cannnot be null.";
        internal const string NEWLINE_REPLACEMENT_FOR_HTML = "<br/>";

        private readonly IGameResultViewModelBuilder playerResultBuilder;

        public PlayedGameDetailsViewModelBuilder(IGameResultViewModelBuilder playerGameResultBuilder)
        {
            playerResultBuilder = playerGameResultBuilder;
        }

        public PlayedGameDetailsViewModel Build(PlayedGame playedGame, ApplicationUser currentUser)
        {
            ValidateArguments(playedGame);

            PlayedGameDetailsViewModel summary = new PlayedGameDetailsViewModel();
            summary.GameDefinitionName = playedGame.GameDefinition.Name;
            summary.GameDefinitionId = playedGame.GameDefinitionId;
            summary.PlayedGameId = playedGame.Id;
            summary.DatePlayed = playedGame.DatePlayed;
            summary.GamingGroupId = playedGame.GamingGroup.Id;
            summary.GamingGroupName = playedGame.GamingGroup.Name;
            summary.WinnerType = playedGame.WinnerType;

            if (playedGame.GameDefinition.BoardGameGeekGameDefinition != null)
            {
                summary.ThumbnailImageUrl = playedGame.GameDefinition.BoardGameGeekGameDefinition.Thumbnail;
            }
            summary.BoardGameGeekUri =
                BoardGameGeekUriBuilder.BuildBoardGameGeekGameUri(playedGame.GameDefinition.BoardGameGeekGameDefinitionId);
            if (playedGame.Notes != null)
            {
                summary.Notes = playedGame.Notes.Replace(Environment.NewLine, NEWLINE_REPLACEMENT_FOR_HTML);
            }

            summary.UserCanEdit = (currentUser != null && playedGame.GamingGroupId == currentUser.CurrentGamingGroupId);
            summary.PlayerResults = new List<GameResultViewModel>();

            foreach (PlayerGameResult playerGameResult in playedGame.PlayerGameResults)
            {
                summary.PlayerResults.Add(playerResultBuilder.Build(playerGameResult));
            }
            var gameRanks = playedGame.PlayerGameResults.Select(x => x.GameRank).ToList();

            return summary;
        }

        private static void ValidateArguments(PlayedGame playedGame)
        {
            if (playedGame == null)
            {
                throw new ArgumentNullException("playedGame");
            }

            if (playedGame.GamingGroup == null)
            {
                throw new ArgumentException(EXCEPTION_MESSAGE_GAMING_GROUP_CANNOT_BE_NULL);
            }

            if (playedGame.GameDefinition == null)
            {
                throw new ArgumentException(EXCEPTION_MESSAGE_GAME_DEFINITION_CANNOT_BE_NULL);
            }

            if (playedGame.PlayerGameResults == null)
            {
                throw new ArgumentException(EXCEPTION_MESSAGE_PLAYER_GAME_RESULTS_CANNOT_BE_NULL);
            }
        }
    }
}