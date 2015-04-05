using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using BusinessLogic.Logic;
using BusinessLogic.Logic.PlayedGames;
using BusinessLogic.Models;
using BusinessLogic.Models.Games;
using BusinessLogic.Models.User;
using UI.Areas.Api.Models;
using UI.Attributes;

namespace UI.Areas.Api.Controllers
{
    public class PlayedGamesController : ApiController
    {
        private readonly IPlayedGameCreator playedGameCreator;

        public PlayedGamesController(IPlayedGameCreator playedGameCreator)
        {
            this.playedGameCreator = playedGameCreator;
        }

        [ApiRoute("GamingGroups/{gamingGroupId}/PlayedGames")]
        [HttpPost]
        public HttpResponseMessage RecordPlayedGame(PlayedGameMessage playedGameMessage, int gamingGroupId, ApplicationUser applicationUser)
        {
            if (gamingGroupId != applicationUser.CurrentGamingGroupId)
            {
                return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, ApiAuthenticationAttribute.UNAUTHORIZED_MESSAGE);
            }

            DateTime datePlayed = DateTime.UtcNow;

            if (!string.IsNullOrWhiteSpace(playedGameMessage.DatePlayed))
            {
                datePlayed = DateTime.ParseExact(playedGameMessage.DatePlayed, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None);
            }

            NewlyCompletedGame newlyCompletedGame = new NewlyCompletedGame
            {
                DatePlayed = datePlayed,
                GameDefinitionId = playedGameMessage.GameDefinitionId,
                Notes = playedGameMessage.Notes,
                PlayerRanks = playedGameMessage.PlayerRanks
            };

            PlayedGame playedGame = playedGameCreator.CreatePlayedGame(newlyCompletedGame, TransactionSource.RestApi, applicationUser);
            var newlyRecordedPlayedGameMessage = new NewlyRecordedPlayedGameMessage
            {
                PlayedGameId = playedGame.Id
            };

            return Request.CreateResponse(HttpStatusCode.OK, newlyRecordedPlayedGameMessage);
        }
    }
}
