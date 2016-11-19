using System;

namespace BusinessLogic.Models.Games
{
    public class GameDefinitionDisplayInfo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ThumbnailImageUrl { get; set; }

        public int? BoardGameGeekGameDefinitionId { get; set; }
        public int PlayedTimes { get; set; }
        public DateTime? LastDatePlayed { get; set; }
        public string ImageUrl { get; set; }
    }
}
