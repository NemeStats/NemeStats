using System;

namespace BusinessLogic.Models.MVPModels
{
    public class MVPData
    {
        public decimal PointsScored { get; set; }

        public DateTime DatePlayed { get; set; }

        public int PlayedGameResultId { get; set; }
        public int PlayerId { get; set; }
    }
}
