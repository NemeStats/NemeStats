using System;
using System.ComponentModel.DataAnnotations.Schema;
using BusinessLogic.DataAccess;

namespace BusinessLogic.Models
{
    public class MVP : EntityWithTechnicalKey<int>
    {
        public override int Id { get; set; }
        public int PlayerGameResultId { get; set; }
        public int PlayerId { get; set; }
        public DateTime DateCreated { get; set; }

        [ForeignKey("PlayerGameResultId")]
        public virtual PlayerGameResult PlayerGameResult { get; set; }

        public decimal PointsScored { get; set; }

        public DateTime DatePlayed { get; set; }


    }
}