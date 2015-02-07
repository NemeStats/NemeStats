using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BusinessLogic.Logic.FeatureVoting;

namespace UI.Areas.Api.Controllers
{
    public class FeatureVotingController : ApiController
    {
        private IFeatureInterestSummaryRetriever featureInterestSummaryRetriever;

        public FeatureVotingController(IFeatureInterestSummaryRetriever featureInterestSummaryRetriever)
        {
            this.featureInterestSummaryRetriever = featureInterestSummaryRetriever;
        }

        // GET api/<controller>
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<controller>/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<controller>
        public void Post([FromBody]string value)
        {
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}