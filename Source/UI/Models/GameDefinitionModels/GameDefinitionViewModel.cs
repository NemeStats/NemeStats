using System.Collections.Generic;
using System.Linq;
using UI.Models.PlayedGame;

namespace UI.Models.GameDefinitionModels
{
    public class GameDefinitionViewModel : IEditableViewModel, IGamingGroupAssignedViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int TotalNumberOfGamesPlayed { get; set; }
        public IList<PlayedGameDetailsViewModel> PlayedGames { get; set; }
        public bool UserCanEdit { get; set; }
        public string GamingGroupName { get; set; }
        public int GamingGroupId { get; set; }
    }
}