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
using BusinessLogic.Models.GamingGroups;
using BusinessLogic.Models.Players;
using BusinessLogic.Models.User;
using System.Collections.Generic;
using System.Linq;
using UI.Models.GamingGroup;
using UI.Models.PlayedGame;
using UI.Models.Players;
using UI.Transformations.PlayerTransformations;

namespace UI.Transformations
{
    public class GamingGroupViewModelBuilder : IGamingGroupViewModelBuilder
    {
        private readonly IPlayedGameDetailsViewModelBuilder playedGameDetailsViewModelBuilder;
        private readonly IPlayerWithNemesisViewModelBuilder playerWithNemesisViewModelBuilder;
        private readonly IGameDefinitionSummaryViewModelBuilder gameDefinitionSummaryViewModelBuilder;

        public GamingGroupViewModelBuilder(
            IPlayedGameDetailsViewModelBuilder playedGameDetailsViewModelBuilder,
            IPlayerWithNemesisViewModelBuilder playerWithNemesisViewModelBuilder,
            IGameDefinitionSummaryViewModelBuilder gameDefinitionSummaryViewModelBuilder)
        {
            this.playedGameDetailsViewModelBuilder = playedGameDetailsViewModelBuilder;
            this.playerWithNemesisViewModelBuilder = playerWithNemesisViewModelBuilder;
            this.gameDefinitionSummaryViewModelBuilder = gameDefinitionSummaryViewModelBuilder;
        }

        public GamingGroupViewModel Build(GamingGroupSummary gamingGroupSummary, ApplicationUser currentUser = null)
        {
            List<PlayedGameDetailsViewModel> details = BuildPlayedGameDetailsViewModels(gamingGroupSummary, currentUser);

            List<PlayerWithNemesisViewModel> playerWithNemesisList 
                = (from PlayerWithNemesis playerWithNemesis in gamingGroupSummary.Players
                   select playerWithNemesisViewModelBuilder.Build(playerWithNemesis, currentUser)).ToList();
            
            GamingGroupViewModel viewModel = new GamingGroupViewModel()
            {
                Id = gamingGroupSummary.Id,
                OwningUserId = gamingGroupSummary.OwningUserId,
                Name = gamingGroupSummary.Name,
                OwningUserName = gamingGroupSummary.OwningUser.UserName,
                Players = playerWithNemesisList,
                GameDefinitionSummaries = gamingGroupSummary.GameDefinitionSummaries
                    .Select(game => gameDefinitionSummaryViewModelBuilder.Build(game, currentUser)).ToList(),
                PlayedGames = new PlayedGamesViewModel
                {
                    PlayedGameDetailsViewModels = details,
                    PanelTitle = string.Format("Last {0} Played Games", details.Count),
                    UserCanEdit = currentUser != null && currentUser.CurrentGamingGroupId == gamingGroupSummary.Id
                }
            };

            return viewModel;
        }

        private List<PlayedGameDetailsViewModel> BuildPlayedGameDetailsViewModels(GamingGroupSummary gamingGroupSummary, ApplicationUser currentUser)
        {
            int totalGames = gamingGroupSummary.PlayedGames.Count;
            List<PlayedGameDetailsViewModel> details = new List<PlayedGameDetailsViewModel>(totalGames);
            for (int i = 0; i < totalGames; i++)
            {
                details.Add(playedGameDetailsViewModelBuilder.Build(gamingGroupSummary.PlayedGames[i], currentUser));
            }
            return details;
        }
    }
}