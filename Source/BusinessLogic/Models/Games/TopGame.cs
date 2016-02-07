using System;

namespace BusinessLogic.Models.Games
{
    public class TopGame
    {
        public Uri BoardGameGeekUri { get; set; }
        public string ThumbnailImageUrl { get; set; }
        public int GamesPlayed { get; set; }
        public int GamingGroupsPlayingThisGame { get; set; }
    }
}
