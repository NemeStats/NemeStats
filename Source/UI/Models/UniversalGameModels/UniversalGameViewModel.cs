using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UI.Models.UniversalGameModels
{
    public class UniversalGameViewModel
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
        public string BoardGameGeekWeightDescription { get; set; }
        public int? BoardGameGeekYearPublished { get; set; }
        public Uri BoardGameGeekUri { get; set; }
        public List<string> BoardGameGeekCategories { get; set; }
        public List<string> BoardGameGeekMechanics { get; set; }
        public string BoardGameGeekDescription { get; set; }
        public int TotalNumberOfGamesPlayed { get; set; }
        public int AveragePlayersPerGame { get; set; }
        public int TotalNemePointsAwarded { get; set; }
    }
}