using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UI.Models.API
{
    public class FeatureVote
    {
        public string VotableFeatureId { get; set; }

        public bool VoteUp { get; set; }
    }
}