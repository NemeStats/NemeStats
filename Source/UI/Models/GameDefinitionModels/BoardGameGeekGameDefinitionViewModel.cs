using System;
using BusinessLogic.Logic.Points;

namespace UI.Models.GameDefinitionModels
{
    public class BoardGameGeekGameDefinitionViewModel
    {
        private const int MaxBggWeight = 5;

        public int? Id { get; set; }
        public string Name { get; set; }
        public string Thumbnail { get; set; }
        public int? MaxPlayers { get; set; }
        public int? MinPlayers { get; set; }
        public int? PlayingTime { get; set; }
        public decimal? AverageWeight { get; set; }
        public string Description { get; set; }

        public Uri BoardGameGeekUri { get; set; }

        public string WeightPercent
        {
            get
            {
                if (AverageWeight.HasValue)
                {
                    
                    return ((AverageWeight.Value * 100) / MaxBggWeight).ToString().Replace(",",".");
                }
                return "0";
            }
        }

        public string WeightDescription
        {
            get
            {
                if (AverageWeight.HasValue)
                {
                    var weight = AverageWeight.Value;
                    if (weight < WeightTierCalculator.BOARD_GAME_GEEK_WEIGHT_INCLUSIVE_LOWER_BOUND_FOR_EASY)
                    {
                        return "Casual";
                    }
                    if (weight < WeightTierCalculator.BOARD_GAME_GEEK_WEIGHT_INCLUSIVE_LOWER_BOUND_FOR_ADVANCED)
                    {
                        return "Easy";
                    }
                    if (weight < WeightTierCalculator.BOARD_GAME_GEEK_WEIGHT_INCLUSIVE_LOWER_BOUND_FOR_CHALLENGING)
                    {
                        return "Advanced";
                    }
                    if (weight < WeightTierCalculator.BOARD_GAME_GEEK_WEIGHT_INCLUSIVE_LOWER_BOUND_FOR_HARDCORE)
                    {
                        return "Challenging";
                    }
                    if (weight < 5)
                    {
                        return "Hardcore";
                    }
                }
                return string.Empty;
            } 
        }
    }
}