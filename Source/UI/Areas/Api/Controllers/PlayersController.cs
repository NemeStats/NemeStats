using BusinessLogic.Logic.Players;
using BusinessLogic.Models.Players;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BusinessLogic.Exceptions;
using BusinessLogic.Models;
using UI.Areas.Api.Models;
using UI.Attributes;
using UI.HtmlHelpers;
using VersionedRestApi;

namespace UI.Areas.Api.Controllers
{
    public class PlayersController : ApiControllerBase
    {
        private readonly IPlayerSaver _playerSaver;
        private readonly IPlayerRetriever _playerRetriever;

        public PlayersController(IPlayerSaver playerSaver, IPlayerRetriever playerRetriever)
        {
            _playerSaver = playerSaver;
            _playerRetriever = playerRetriever;
        }

        [ApiAuthentication(AuthenticateOnly = true)]
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
            var results = _playerRetriever.GetAllPlayers(gamingGroupId);

            var playerIds = results.Select(x => x.Id).ToList();
            //--include the current player so we can attempt to get their email address as well
            var playerIdToRegisteredUserEmailAddressDictionary =
                _playerRetriever.GetRegisteredUserEmailAddresses(playerIds, CurrentUser);

            var playerSearchResultsMessage = new PlayersSearchResultsMessage();
            foreach (var player in results)
            {
                var playerSearchResultMessage = new PlayerSearchResultMessage
                {
                    Active = player.Active,
                    PlayerId = player.Id,
                    CurrentNemesisPlayerId = player.NemesisId,
                    PlayerName = player.Name,
                    NemeStatsUrl = AbsoluteUrlBuilder.GetPlayerDetailsUrl(player.Id)
                };

                if (playerIdToRegisteredUserEmailAddressDictionary.ContainsKey(player.Id))
                {
                    playerSearchResultMessage.RegisteredUserGravatarUrl = UIHelper.BuildGravatarUrl(playerIdToRegisteredUserEmailAddressDictionary[player.Id]);
                }

                playerSearchResultsMessage.Players.Add(playerSearchResultMessage);
            }

            return Request.CreateResponse(HttpStatusCode.OK, playerSearchResultsMessage);
        }

        [ApiAuthentication]
        [ApiModelValidation]
        [ApiRoute("Players/", StartingVersion = 2)]
        [HttpPost]
        public virtual HttpResponseMessage SaveNewPlayer([FromBody] NewPlayerMessage newPlayerMessage)
        {
            //--gamingGroupId is obsolete here and will be on the request or else used from the current user context -- so just pass "0"
            return SaveNewPlayer(newPlayerMessage, 0);
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
                GamingGroupId = newPlayerMessage.GamingGroupId,
                PlayerEmailAddress = newPlayerMessage.PlayerEmailAddress
            };

            Player newPlayer;
            try
            {
                newPlayer = _playerSaver.CreatePlayer(requestedPlayer, CurrentUser);
            }
            catch (PlayerAlreadyExistsException exception)
            {
                exception.ErrorSubCode = 1;
                throw;
            }
            catch (PlayerWithThisEmailAlreadyExistsException exception)
            {
                exception.ErrorSubCode = 2;
                throw;
            }

            var newlyCreatedPlayerMessage = new NewlyCreatedPlayerMessage
            {
                PlayerId = newPlayer.Id,
                GamingGroupId = newPlayer.GamingGroupId,
                NemeStatsUrl = AbsoluteUrlBuilder.GetPlayerDetailsUrl(newPlayer.Id)
            };

            return Request.CreateResponse(HttpStatusCode.OK, newlyCreatedPlayerMessage);
        }

        [ApiAuthentication]
        [ApiModelValidation]
        [ApiRoute("Players/{playerId}/", StartingVersion = 2)]
        [HttpPut]
        public virtual HttpResponseMessage UpdatePlayerVersion2([FromBody] UpdatePlayerMessage updatePlayerMessage, [FromUri] int playerId)
        {
            //--gamingGroupId is obsolete here and will be on the request or else used from the current user context -- so just pass "0"
            return UpdatePlayer(updatePlayerMessage, playerId, 0);
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

            _playerSaver.UpdatePlayer(requestedPlayer, CurrentUser);

            return Request.CreateResponse(HttpStatusCode.NoContent);
        }
    }
}