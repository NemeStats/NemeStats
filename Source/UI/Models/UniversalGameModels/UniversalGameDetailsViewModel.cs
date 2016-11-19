using System;
using System.Collections.Generic;
using UI.Models.GameDefinitionModels;

namespace UI.Models.UniversalGameModels
{
    public class UniversalGameDetailsViewModel
    {
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public string ThumbnailImageUrl { get; set; }
        public int? MaxPlayers { get; set; }
        public int? MinPlayers { get; set; }
        public int? AveragePlayTime { get; set; }
        public decimal? BoardGameGeekAverageWeight { get; set; }
        public string BoardGameGeekAverageWeightDescription { get; set; }
        public string BoardGameGeekWeightPercent { get; set; }
        public int? BoardGameGeekYearPublished { get; set; }
        public Uri BoardGameGeekUri { get; set; }
        public List<string> BoardGameGeekCategories { get; set; }
        public List<string> BoardGameGeekMechanics { get; set; }
        public string BoardGameGeekDescription { get; set; }
        public int TotalNumberOfGamesPlayed { get; set; }
        public string AveragePlayersPerGame { get; set; }
        public int TotalGamingGroupsWithThisGame { get; set; }
        public GamingGroupGameDefinitionViewModel GamingGroupGameDefinitionSummary { get; set; }
    }
}