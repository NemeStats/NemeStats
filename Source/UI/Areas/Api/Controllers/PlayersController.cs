using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using BusinessLogic.Logic.Players;
using BusinessLogic.Models;
using BusinessLogic.Models.User;
using UI.Areas.Api.Models;
using UI.Attributes;

namespace UI.Areas.Api.Controllers
{
    public class PlayersController : ApiController
    {
        private readonly IPlayerSaver playerSaver;

        public PlayersController(IPlayerSaver playerSaver)
        {
            this.playerSaver = playerSaver;
        }

        [ApiAuthentication]
        [ApiRoute("GamingGroups/{gamingGroupId}/Players/")]
        [HttpPost]
        public virtual HttpResponseMessage SaveNewPlayer([FromBody]NewPlayerMessage newPlayerMessage, [FromUri]int gamingGroupId)
        {
            ApplicationUser applicationUser = ActionContext.ActionArguments[ApiAuthenticationAttribute.ACTION_ARGUMENT_APPLICATION_USER] as ApplicationUser;

            var requestedPlayer = new Player
            {
                Name = newPlayerMessage.PlayerName
            };

            var actualNewlyCreatedPlayer = playerSaver.Save(requestedPlayer, applicationUser);

            var newlyCreatedPlayerMessage = new NewlyCreatedPlayerMessage
            {
                PlayerId = actualNewlyCreatedPlayer.Id
            };

            return Request.CreateResponse(HttpStatusCode.OK, newlyCreatedPlayerMessage);
        }
    }
}