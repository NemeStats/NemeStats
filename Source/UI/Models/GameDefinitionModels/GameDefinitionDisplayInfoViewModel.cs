using System;

namespace UI.Models.GameDefinitionModels
{
    public class GameDefinitionDisplayInfoViewModel
    {
        public int Id { get; set; }
        public string ThumbnailImageUrl { get; set; }
        public string Name { get; set; }
        public int PlayedTimes { get; set; }
        public DateTime? LastDatePlayed { get; set; }
    }
}