using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using BusinessLogic.DataAccess;

namespace BusinessLogic.Models
{
    public class VotableFeature : EntityWithTechnicalKey<string>
    {
        public VotableFeature()
        {
            DateCreated = DateTime.UtcNow;
            DateModified = DateTime.UtcNow;
        }

        [MaxLength(255), Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public override string Id { get; set; }
        public string FeatureDescription { get; set; }
        public int NumberOfUpvotes { get; set; }
        public int NumberOfDownvotes { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
    }
}
