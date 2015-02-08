using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BusinessLogic.DataAccess;
using BusinessLogic.Models;

namespace BusinessLogic.Logic.VotableFeatures
{
    public class VotableFeatureRetriever : IVotableFeatureRetriever
    {
        private readonly IDataContext dataContext;

        public VotableFeatureRetriever(IDataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        public VotableFeature RetrieveVotableFeature(int id)
        {
            return dataContext.FindById<VotableFeature>(id);
        }
    }
}
