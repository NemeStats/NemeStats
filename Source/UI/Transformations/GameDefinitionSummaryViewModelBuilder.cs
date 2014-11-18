using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BusinessLogic.Models.Games;
using BusinessLogic.Models.User;
using UI.Models.GameDefinitionModels;

namespace UI.Transformations
{
    public class GameDefinitionSummaryViewModelBuilder : IGameDefinitionSummaryViewModelBuilder
    {
        public const string BOARD_GAME_GEEK_BOARD_GAME_BASE_URI = "http://boardgamegeek.com/boardgame/{0}";

        public GameDefinitionSummaryViewModel Build(GameDefinitionSummary gameDefinitionSummary, ApplicationUser currentUser)
        {
            Uri boardGameGeekUri = null;

            if (gameDefinitionSummary.BoardGameGeekObjectId.HasValue)
            {
                boardGameGeekUri = new Uri(string.Format(BOARD_GAME_GEEK_BOARD_GAME_BASE_URI, gameDefinitionSummary.BoardGameGeekObjectId.Value));
            }

            return new GameDefinitionDetailsViewModel()
            {
                Id = gameDefinitionSummary.Id,
                Name = gameDefinitionSummary.Name,
                Description = gameDefinitionSummary.Description,
                TotalNumberOfGamesPlayed = gameDefinitionSummary.TotalNumberOfGamesPlayed,
                GamingGroupId = gameDefinitionSummary.GamingGroupId,
                GamingGroupName = gameDefinitionSummary.GamingGroupName,
                BoardGameGeekUri = boardGameGeekUri,
                UserCanEdit = (currentUser != null && gameDefinitionSummary.GamingGroupId == currentUser.CurrentGamingGroupId)
            };
        }
    }
}