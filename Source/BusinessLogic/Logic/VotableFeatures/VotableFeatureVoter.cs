using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.DataAccess;
using BusinessLogic.Models;
using BusinessLogic.Models.User;

namespace BusinessLogic.Logic.VotableFeatures
{
    public class VotableFeatureVoter : IVotableFeatureVoter
    {
        private readonly IDataContext dataContext;

        public VotableFeatureVoter(IDataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        public VotableFeature CastVote(int votableFeatureId, bool voteDirection)
        {
            var votableFeature = dataContext.FindById<VotableFeature>(votableFeatureId);

            if (voteDirection)
            {
                votableFeature.NumberOfUpvotes += 1;
            }
            else
            {
                votableFeature.NumberOfDownvotes += 1;
            }

            votableFeature.DateModified = DateTime.UtcNow;

            return dataContext.Save(votableFeature, new ApplicationUser());
        }
    }
}
