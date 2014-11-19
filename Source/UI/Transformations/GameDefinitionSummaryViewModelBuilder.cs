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
        public GameDefinitionSummaryViewModel Build(GameDefinitionSummary gameDefinitionSummary, ApplicationUser currentUser)
        {
            return new GameDefinitionDetailsViewModel()
            {
                Id = gameDefinitionSummary.Id,
                Name = gameDefinitionSummary.Name,
                Description = gameDefinitionSummary.Description,
                TotalNumberOfGamesPlayed = gameDefinitionSummary.TotalNumberOfGamesPlayed,
                GamingGroupId = gameDefinitionSummary.GamingGroupId,
                GamingGroupName = gameDefinitionSummary.GamingGroupName,
                BoardGameGeekUri = gameDefinitionSummary.BoardGameGeekUri,
                UserCanEdit = (currentUser != null && gameDefinitionSummary.GamingGroupId == currentUser.CurrentGamingGroupId)
            };
        }
    }
}