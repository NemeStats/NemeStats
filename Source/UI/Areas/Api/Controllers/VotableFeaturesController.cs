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

        public VotableFeaturesController(IVotableFeatureRetriever votableFeatureRetriever)
        {
            this.votableFeatureRetriever = votableFeatureRetriever;
        }

        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<controller>/5
        public VotableFeature Get(int id)
        {
            //HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, "value");
            //response.Content = new StringContent("hello", Encoding.Unicode);

            //return response;
            try
            {
                return votableFeatureRetriever.RetrieveVotableFeature(id);
            }
            catch (EntityDoesNotExistException exception)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
        }

        // POST api/<controller>
        public IHttpActionResult Post([FromBody]int id, bool voteDirection)
        {
            try
            {
                return this.Ok(votableFeatureRetriever.RetrieveVotableFeature(id));
            }
            catch (EntityDoesNotExistException)
            {
                return this.NotFound();
            }
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