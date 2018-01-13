#region LICENSE
// NemeStats is a free website for tracking the results of board games.
//     Copyright (C) 2015 Jacob Gordon
// 
//     This program is free software: you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     This program is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU General Public License for more details.
// 
//     You should have received a copy of the GNU General Public License
//     along with this program.  If not, see <http://www.gnu.org/licenses/>
#endregion
using System.Net;
using System.Web.Http;
using AutoMapper;
using BusinessLogic.Exceptions;
using BusinessLogic.Logic.VotableFeatures;
using BusinessLogic.Models;
using UI.Areas.Api.Models;
using UI.Models;

namespace UI.Areas.Api.Controllers
{
    public class VotableFeaturesController : ApiControllerBase
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
            catch (EntityDoesNotExistException<VotableFeature>)
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
            catch (EntityDoesNotExistException<VotableFeature>)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
        }
    }
}