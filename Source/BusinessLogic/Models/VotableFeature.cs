using System;
using System.Linq;
using BusinessLogic.DataAccess;

namespace BusinessLogic.Models
{
    public class VotableFeature : EntityWithTechnicalKey<int>
    {
        public VotableFeature()
        {
            DateCreated = DateTime.UtcNow;
            DateModified = DateTime.UtcNow;
        }

        public override int Id { get; set; }
        public string FeatureDescription { get; set; }
        public int NumberOfUpvotes { get; set; }
        public int NumberOfDownvotes { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
    }
}
