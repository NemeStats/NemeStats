using BusinessLogic.Logic.Players;
using BusinessLogic.Models;
using BusinessLogic.Models.Players;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using UI.Areas.Api.Models;
using UI.Attributes;
using VersionedRestApi;

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

        [ApiModelValidation]
        [ApiRoute("Players/", StartingVersion = 2)]
        [HttpGet]
        public virtual HttpResponseMessage GetPlayersVersion2([FromUri] int gamingGroupId)
        {
            return GetPlayers(gamingGroupId);
        }

        [ApiAuthentication]
        [ApiModelValidation]
        [ApiRoute("GamingGroups/{gamingGroupId}/Players/", AcceptedVersions = new[]{1})]
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
        [ApiRoute("Players/", StartingVersion = 2)]
        [HttpPost]
        public virtual HttpResponseMessage SaveNewPlayer([FromBody] NewPlayerMessage newPlayerMessage)
        {
            return SaveNewPlayer(newPlayerMessage, CurrentUser.CurrentGamingGroupId);
        }

        [ApiAuthentication]
        [ApiModelValidation]
        [ApiRoute("GamingGroups/{gamingGroupId}/Players/", AcceptedVersions = new []{1})]
        [HttpPost]
        public virtual HttpResponseMessage SaveNewPlayer([FromBody]NewPlayerMessage newPlayerMessage, [FromUri]int gamingGroupId)
        {
            var requestedPlayer = new CreatePlayerRequest
            {
                Name = newPlayerMessage.PlayerName,
                GamingGroupId = newPlayerMessage.GamingGroupId
            };

            var actualNewlyCreatedPlayer = playerSaver.CreatePlayer(requestedPlayer, CurrentUser);

            var newlyCreatedPlayerMessage = new NewlyCreatedPlayerMessage
            {
                PlayerId = actualNewlyCreatedPlayer.Id,
                GamingGroupId = actualNewlyCreatedPlayer.GamingGroupId
            };

            return Request.CreateResponse(HttpStatusCode.OK, newlyCreatedPlayerMessage);
        }

        [ApiAuthentication]
        [ApiModelValidation]
        [ApiRoute("Players/{playerId}/", StartingVersion = 2)]
        [HttpPut]
        public virtual HttpResponseMessage UpdatePlayerVersion2([FromBody] UpdatePlayerMessage updatePlayerMessage, [FromUri] int playerId)
        {
            return UpdatePlayer(updatePlayerMessage, playerId, CurrentUser.CurrentGamingGroupId);
        }

        [ApiAuthentication]
        [ApiModelValidation]
        [ApiRoute("GamingGroups/{gamingGroupId}/Players/{playerId}/", AcceptedVersions = new []{1})]
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