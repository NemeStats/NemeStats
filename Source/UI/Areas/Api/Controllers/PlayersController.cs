using BusinessLogic.Logic.Players;
using BusinessLogic.Models;
using BusinessLogic.Models.Players;
using BusinessLogic.Models.User;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using UI.Areas.Api.Models;
using UI.Attributes;

namespace UI.Areas.Api.Controllers
{
    public class PlayersController : ApiControllerBase
    {
        private readonly IPlayerSaver playerSaver;
        private readonly IPlayerRetriever playerRetriever;

        public PlayersController(IPlayerSaver playerSaver, IPlayerRetriever playerRetriever)
        {
            this.playerSaver = playerSaver;
            this.playerRetriever = playerRetriever;
        }

        [ApiAuthentication]
        [ApiModelValidation]
        [ApiRoute("GamingGroups/{gamingGroupId}/Players/")]
        [HttpGet]
        public virtual HttpResponseMessage GetPlayers([FromUri] int gamingGroupId)
        {
            var results = playerRetriever.GetAllPlayers(gamingGroupId);

            var playerSearchResultsMessage = new PlayersSearchResultsMessage
            {
                Players = results.Select(player => new PlayerSearchResultMessage
                {
                    Active = player.Active,
                    PlayerId = player.Id,
                    CurrentNemesisPlayerId = player.NemesisId,
                    PlayerName = player.Name
                }).ToList()
            };

            return Request.CreateResponse(HttpStatusCode.OK, playerSearchResultsMessage);
        }

        [ApiAuthentication]
        [ApiModelValidation]
        [ApiRoute("GamingGroups/{gamingGroupId}/Players/")]
        [HttpPost]
        public virtual HttpResponseMessage SaveNewPlayer([FromBody]NewPlayerMessage newPlayerMessage, [FromUri]int gamingGroupId)
        {
            var requestedPlayer = new Player
            {
                Name = newPlayerMessage.PlayerName
            };

            var actualNewlyCreatedPlayer = playerSaver.Save(requestedPlayer, CurrentUser);

            var newlyCreatedPlayerMessage = new NewlyCreatedPlayerMessage
            {
                PlayerId = actualNewlyCreatedPlayer.Id
            };

            return Request.CreateResponse(HttpStatusCode.OK, newlyCreatedPlayerMessage);
        }

        [ApiAuthentication]
        [ApiModelValidation]
        [ApiRoute("GamingGroups/{gamingGroupId}/Players/{playerId}/")]
        [HttpPut]
        public virtual HttpResponseMessage UpdatePlayer([FromBody]UpdatePlayerMessage updatePlayerMessage, [FromUri] int playerId, [FromUri]int gamingGroupId)
        {
            if (updatePlayerMessage == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "You must pass at least one valid parameter.");
            }

            var requestedPlayer = new UpdatePlayerRequest
            {
                PlayerId = playerId,
                Active = updatePlayerMessage.Active,
                Name = updatePlayerMessage.PlayerName
            };

            playerSaver.UpdatePlayer(requestedPlayer, CurrentUser);

            return Request.CreateResponse(HttpStatusCode.NoContent);
        }
    }
}