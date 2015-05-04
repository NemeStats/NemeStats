using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using AutoMapper;
using BusinessLogic.Export;
using BusinessLogic.Logic;
using BusinessLogic.Logic.PlayedGames;
using BusinessLogic.Models;
using BusinessLogic.Models.Games;
using BusinessLogic.Models.PlayedGames;
using BusinessLogic.Models.User;
using UI.Areas.Api.Models;
using UI.Attributes;

namespace UI.Areas.Api.Controllers
{
    public class PlayedGamesController : ApiController
    {
        public const int MAX_PLAYED_GAMES_TO_EXPORT = 1000;

        private readonly IPlayedGameRetriever playedGameRetriever;
        private readonly IExcelGenerator excelGenerator;
        private readonly IPlayedGameCreator playedGameCreator;


        private MemoryStream exportMemoryStream;

        public PlayedGamesController(IPlayedGameRetriever playedGameRetriever, IExcelGenerator excelGenerator, IPlayedGameCreator playedGameCreator)
        {
            this.playedGameRetriever = playedGameRetriever;
            this.excelGenerator = excelGenerator;
            this.playedGameCreator = playedGameCreator;
        }

        [ApiRoute("GamingGroups/{gamingGroupId}/PlayedGamesExcel")]
        [HttpGet]
        public virtual HttpResponseMessage ExportPlayedGamesToExcel(int gamingGroupId)
        {
            var playedGames = playedGameRetriever.GetRecentGames(MAX_PLAYED_GAMES_TO_EXPORT, gamingGroupId);

            if (playedGames.Count == 0)
            {
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            }

            var playedGamesForExport = playedGames.Select(playedGame => new PlayedGameExportModel
            {
                BoardGameGeekObjectId = playedGame.GameDefinition.BoardGameGeekObjectId,
                DateCreated = playedGame.DateCreated,
                DatePlayed = playedGame.DatePlayed,
                GameDefinitionId = playedGame.GameDefinitionId,
                GameDefinitionName = playedGame.GameDefinition.Name,
                GamingGroupId = playedGame.GamingGroupId,
                GamingGroupName = playedGame.GamingGroup.Name,
                Id = playedGame.Id,
                Notes = playedGame.Notes,
                NumberOfPlayers = playedGame.NumberOfPlayers,
                WinningPlayerIds = String.Join(",", playedGame.PlayerGameResults.Where(result => result.GameRank == 1).Select(result => result.PlayerId).ToList()),
                WinningPlayerNames = String.Join(",", playedGame.PlayerGameResults.Where(result => result.GameRank == 1).Select(result => result.Player.Name).ToList())
            }).ToList();

            exportMemoryStream = new MemoryStream();

            excelGenerator.GenerateExcelFile(playedGamesForExport, exportMemoryStream);

            HttpResponseMessage responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StreamContent(this.exportMemoryStream)
            };
            responseMessage.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = "played_game_export.xlsx"
            };
            responseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

            return responseMessage;
        }

        protected override void Dispose(bool disposing)
        {
            if (exportMemoryStream != null)
            {
                exportMemoryStream.Dispose();
            }
            base.Dispose(disposing);
        }

        [ApiRoute("GamingGroups/{gamingGroupId}/PlayedGames/")]
        [HttpGet]
        public HttpResponseMessage GetPlayedGames([FromBody]PlayedGameFilterMessage playedGameFilterMessage, [FromUri]int gamingGroupId)
        {
            var filter = new PlayedGameFilter
            {
                GamingGroupId = gamingGroupId
            };
            if(playedGameFilterMessage != null)
            {
                filter.StartDateGameLastUpdated = playedGameFilterMessage.StartDateGameLastUpdated;
                filter.MaximumNumberOfResults = playedGameFilterMessage.MaximumNumberOfResults;
            }
            var searchResults = playedGameRetriever.SearchPlayedGames(filter);

            var searchResultsMessage = searchResults.Select(Mapper.Map<PlayedGameSearchResultMessage>).ToList();

            return Request.CreateResponse(HttpStatusCode.OK, searchResultsMessage);
        }

        [ApiRoute("GamingGroups/{gamingGroupId}/PlayedGames/")]
        [HttpPost]
        [ApiAuthentication]
        [ApiModelValidation]
        public HttpResponseMessage RecordPlayedGame([FromBody]PlayedGameMessage playedGameMessage, [FromUri]int gamingGroupId)
        {
            ApplicationUser applicationUser = ActionContext.ActionArguments[ApiAuthenticationAttribute.ACTION_ARGUMENT_APPLICATION_USER] as ApplicationUser;

            if (gamingGroupId != applicationUser.CurrentGamingGroupId)
            {
                return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, ApiAuthenticationAttribute.ERROR_MESSAGE_INVALID_AUTH_TOKEN);
            }

            var newlyCompletedGame = BuildNewlyPlayedGame(playedGameMessage);

            PlayedGame playedGame = playedGameCreator.CreatePlayedGame(newlyCompletedGame, TransactionSource.RestApi, applicationUser);
            var newlyRecordedPlayedGameMessage = new NewlyRecordedPlayedGameMessage
            {
                PlayedGameId = playedGame.Id
            };

            return Request.CreateResponse(HttpStatusCode.OK, newlyRecordedPlayedGameMessage);
        }

        private static NewlyCompletedGame BuildNewlyPlayedGame(PlayedGameMessage playedGameMessage)
        {
            DateTime datePlayed = DateTime.UtcNow;

            if (!string.IsNullOrWhiteSpace(playedGameMessage.DatePlayed))
            {
                datePlayed = DateTime.ParseExact(playedGameMessage.DatePlayed, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None);
            }

            return new NewlyCompletedGame
            {
                DatePlayed = datePlayed,
                GameDefinitionId = playedGameMessage.GameDefinitionId,
                Notes = playedGameMessage.Notes,
                PlayerRanks = playedGameMessage.PlayerRanks
            };
        }
    }
}
