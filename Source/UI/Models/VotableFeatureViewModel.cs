using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UI.Models
{
    public class VotableFeatureViewModel
    {
        public string Id { get; set; }
        public string FeatureDescription { get; set; }
        public int NumberOfUpvotes { get; set; }
        public int NumberOfDownvotes { get; set; }
    }
}
