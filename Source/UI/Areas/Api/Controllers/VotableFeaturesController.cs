using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BusinessLogic.Logic.VotableFeatures;
using BusinessLogic.Models.VotableFeatures;

namespace UI.Areas.Api.Controllers
{
    public class VotableFeaturesController : ApiController
    {
        private IVotableFeatureRetriever votableFeatureRetriever;

        public VotableFeaturesController(IVotableFeatureRetriever votableFeatureRetriever)
        {
            this.votableFeatureRetriever = votableFeatureRetriever;
        }

        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<controller>/5
        public VotableFeature Get(string id)
        {
            throw new NotImplementedException();
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