using System;

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
                    var weight = (double) AverageWeight.Value;
                    if (weight < 1.8)
                    {
                        return "Casual";
                    }
                    if (weight < 2.4)
                    {
                        return "Easy";
                    }
                    if (weight < 3.3)
                    {
                        return "Advanced";
                    }
                    if (weight < 4.1)
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