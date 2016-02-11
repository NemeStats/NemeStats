using System;

namespace UI.Models.GameDefinitionModels
{
    public class TrendingGameViewModel
    {
        public Uri BoardGameGeekUri { get; set; }
        public string ThumbnailImageUrl { get; set; }
        public int GamesPlayed { get; set; }
        public int GamingGroupsPlayingThisGame { get; set; }
        public string Name { get; set; }
    }
}