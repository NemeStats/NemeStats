using System;
using System.Collections.Generic;
using System.Linq;
using UI.Models;

namespace UI.Models.PlayedGame
{
    public class PlayedGameDetailsViewModel : IEditableViewModel, IGamingGroupAssignedViewModel
    {
        public int PlayedGameId { get; set; }
        public string GameDefinitionName { get; set; }
        public int GameDefinitionId { get; set; }
        public int GamingGroupId { get; set; }
        public string GamingGroupName { get; set; }
        public DateTime DatePlayed { get; set; }
        public IList<GameResultViewModel> PlayerResults { get; set; }
        public bool UserCanEdit { get; set; }
    }
}