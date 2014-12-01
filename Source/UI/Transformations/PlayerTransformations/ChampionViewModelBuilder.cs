using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BusinessLogic.Models;
using UI.Models;

namespace UI.Transformations.PlayerTransformations
{
    public class ChampionViewModelBuilder : IChampionViewModelBuilder
    {
        public ChampionViewModel Build(Champion champion)
        {
            return new ChampionViewModel
            {
                GameDefinitionName = champion.GameDefinition.Name,
                GameDefinitionId = champion.GameDefinition.Id,
                NumberOfGames = champion.NumberOfGames,
                NumberOfWins = champion.NumberOfWins,
                WinPercentage = champion.WinPercentage
            };
        }
    }
}