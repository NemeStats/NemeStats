using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web.Http;
using AutoMapper;
using BusinessLogic.Exceptions;
using BusinessLogic.Logic.VotableFeatures;
using BusinessLogic.Models;
using UI.Models;
using UI.Models.API;

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
        public VotableFeatureViewModel Get(string id)
        {
            try
            {
                VotableFeature votableFeature = votableFeatureRetriever.RetrieveVotableFeature(id);
                return Mapper.Map<VotableFeature, VotableFeatureViewModel>(votableFeature);
            }
            catch (EntityDoesNotExistException)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
        }

        // POST /api/VotableFeatures/
        public void Post(FeatureVote featureVote)
        {
            try
            {
                votableFeatureVoter.CastVote(featureVote.VotableFeatureId, featureVote.VoteUp);
            }
            catch (EntityDoesNotExistException)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
        }
    }
}