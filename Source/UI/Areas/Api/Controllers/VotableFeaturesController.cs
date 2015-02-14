using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web.Http;
using BusinessLogic.Exceptions;
using BusinessLogic.Logic.VotableFeatures;
using BusinessLogic.Models;

namespace UI.Areas.Api.Controllers
{
    public class VotableFeaturesController : ApiController
    {
        private readonly IVotableFeatureRetriever votableFeatureRetriever;
        private readonly IVotableFeatureVoter votableFeatureVoter;

        public VotableFeaturesController(
            IVotableFeatureRetriever votableFeatureRetriever, 
            IVotableFeatureVoter votableFeatureVoter)
        {
            this.votableFeatureRetriever = votableFeatureRetriever;
            this.votableFeatureVoter = votableFeatureVoter;
        }

        // GET /api/VotableFeatures/<id>
        public VotableFeature Get(int id)
        {
            try
            {
                return votableFeatureRetriever.RetrieveVotableFeature(id);
            }
            catch (EntityDoesNotExistException)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
        }

        // POST /api/VotableFeatures/
        public void Post([FromBody]int id, bool voteDirection)
        {
            try
            {
                votableFeatureRetriever.RetrieveVotableFeature(id);
            }
            catch (EntityDoesNotExistException)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
        }
    }
}