using System;
using BusinessLogic.Logic.BoardGameGeek;

namespace BusinessLogic.Models.Games
{
    public class TopGame
    {
        public Uri BoardGameGeekUri => BoardGameGeekUriBuilder.BuildBoardGameGeekGameUri(BoardGameGeekGameDefinitionId);
        public int BoardGameGeekGameDefinitionId { get; set; }
        public string ThumbnailImageUrl { get; set; }
        public int GamesPlayed { get; set; }
        public int GamingGroupsPlayingThisGame { get; set; }
        public string Name { get; set; }
    }
}
