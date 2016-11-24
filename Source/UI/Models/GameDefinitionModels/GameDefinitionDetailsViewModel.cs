using System.Collections.Generic;
using UI.Models.PlayedGame;
using UI.Models.Players;
using UI.Models.UniversalGameModels;

namespace UI.Models.GameDefinitionModels
{
    public class GameDefinitionDetailsViewModel : IEditableViewModel, IGamingGroupAssignedViewModel
    {
        public GameDefinitionDetailsViewModel()
        {
            PlayedGames = new List<PlayedGameDetailsViewModel>();
            GameDefinitionPlayersSummary = new List<GameDefinitionPlayerSummaryViewModel>();
        }

        public int GameDefinitionId { get; set; }
        public string GameDefinitionName { get; set; }
        public BoardGameGeekInfoViewModel BoardGameGeekInfo { get; set; }
        public string GamingGroupName { get; set; }
        public int GamingGroupId { get; set; }
        public bool UserCanEdit { get; set; }
        public int TotalNumberOfGamesPlayed { get; set; }
        public string AveragePlayersPerGame { get; set; }
        public string ChampionName { get; set; }
        public int? ChampionPlayerId { get; set; }
        public int? NumberOfWins { get; set; }
        public int? NumberOfGamesPlayed { get; set; }
        public float? WinPercentage { get; set; }
        public string PreviousChampionName { get; set; }
        public int? PreviousChampionPlayerId { get; set; }
        public List<PlayedGameDetailsViewModel> PlayedGames { get; set; }
        public List<GameDefinitionPlayerSummaryViewModel> GameDefinitionPlayersSummary { get; set; }
    }
}