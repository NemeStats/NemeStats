using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using UI.Areas.Api.Models;
using UI.Attributes;

namespace UI.Areas.Api.Controllers
{
    public class PlayersController : ApiController
    {
        [ApiRoute("GamingGroups/{gamingGroupId}/Players/")]
        [HttpPost]
        public virtual HttpResponseMessage SaveNewPlayer([FromBody]NewPlayerMessage newPlayerMessage, [FromUri]int gamingGroupId)
        {
            throw new NotImplementedException();
        }
    }
}