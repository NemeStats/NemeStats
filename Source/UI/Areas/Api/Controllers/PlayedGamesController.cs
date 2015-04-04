using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web.Http;
using BusinessLogic.Logic;
using BusinessLogic.Logic.PlayedGames;
using BusinessLogic.Models.Games;
using BusinessLogic.Models.User;
using UI.Areas.Api.Models;

namespace UI.Areas.Api.Controllers
{
    public class PlayedGamesController : ApiController
    {
        private readonly IPlayedGameCreator playedGameCreator;

        public PlayedGamesController(IPlayedGameCreator playedGameCreator)
        {
            this.playedGameCreator = playedGameCreator;
        }

        public void RecordPlayedGame(PlayedGameMessage playedGameMessage, ApplicationUser applicationUser)
        {
            DateTime datePlayed = DateTime.UtcNow;

            if (!string.IsNullOrWhiteSpace(playedGameMessage.DatePlayed))
            {
                datePlayed = DateTime.ParseExact(playedGameMessage.DatePlayed, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None);
            }

            NewlyCompletedGame newlyCompletedGame = new NewlyCompletedGame
            {
                DatePlayed = datePlayed,
                GameDefinitionId = playedGameMessage.GameDefinitionId,
                Notes = playedGameMessage.Notes
            };

            playedGameCreator.CreatePlayedGame(newlyCompletedGame, TransactionSource.RestApi, applicationUser);
        }
    }
}
