using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UI.Models.GameDefinitionModels
{
    public class GameDefinitionSummaryViewModel : IEditableViewModel, IGamingGroupAssignedViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int TotalNumberOfGamesPlayed { get; set; }
        public bool UserCanEdit { get; set; }
        public string GamingGroupName { get; set; }
        public int GamingGroupId { get; set; }
        public int? BoardGameGeekObjectId { get; set; }
        public Uri BoardGameGeekUri { get; set; }
        public string ChampionName { get; set; }
        public int? ChampionPlayerId { get; set; }
        public int? NumberOfWins { get; set; }
        public int? NumberOfGamesPlayed { get; set; }
        public float? WinPercentage { get; set; }
        public string PreviousChampionName { get; set; }
        public int? PreviousChampionPlayerId { get; set; }
    }
}