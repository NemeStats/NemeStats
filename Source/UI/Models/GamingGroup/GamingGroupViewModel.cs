#region LICENSE

// NemeStats is a free website for tracking the results of board games. Copyright (C) 2015 Jacob Gordon
// 
// This program is free software: you can redistribute it and/or modify it under the terms of the
// GNU General Public License as published by the Free Software Foundation, either version 3 of the
// License, or (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without
// even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
// General Public License for more details.
// 
// You should have received a copy of the GNU General Public License along with this program. If
// not, see <http://www.gnu.org/licenses/>

#endregion LICENSE

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using UI.Models.GameDefinitionModels;
using UI.Models.PlayedGame;
using UI.Models.Players;

namespace UI.Models.GamingGroup
{
    public class GamingGroupViewModel
    {
        public GamingGroupViewModel()
        {
            this.PlayedGames = new PlayedGamesViewModel
            {
                PlayedGameDetailsViewModels = new List<PlayedGameDetailsViewModel>()
            };
            this.PublicDetailsView = new GamingGroupPublicDetailsViewModel();
            ShowStatsToDate = DateTime.UtcNow;
        }

        public int Id { get; set; }
        [DisplayName("Gaming Group Name")]
        public string Name { get; set; }
        public string OwningUserId { get; set; }
        [DisplayName("Owning User Name")]
        public string OwningUserName { get; set; }
        public GamingGroupPublicDetailsViewModel PublicDetailsView { get; set; }
        [DataType(DataType.EmailAddress)]
        [DisplayName("Invitee Email")]
        [Required(ErrorMessage = "Please enter an e-mail!", AllowEmptyStrings = false)]
        public string InviteeEmail { get; set; }
        public DateTime? ShowStatsFromDate { get; set; }
        public DateTime? ShowStatsToDate { get; set; }
        public IList<GameDefinitionSummaryViewModel> GameDefinitionSummaries { get; set; }
        public IList<PlayerWithNemesisViewModel> Players { get; set; }
        public PlayedGamesViewModel PlayedGames { get; set; }
    }
}