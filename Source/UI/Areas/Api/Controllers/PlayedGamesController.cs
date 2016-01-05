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
using UI.Areas.Api.Models;
using UI.Attributes;
using VersionedRestApi;

namespace UI.Areas.Api.Controllers
{
    public class PlayedGamesController : ApiControllerBase
    {
        public const int MAX_PLAYED_GAMES_TO_EXPORT = 1000;

        private readonly IPlayedGameRetriever playedGameRetriever;
        private readonly IExcelGenerator excelGenerator;
        private readonly IPlayedGameCreator playedGameCreator;
        private readonly IPlayedGameDeleter playedGameDeleter;


        private MemoryStream exportMemoryStream;

        public PlayedGamesController(
            IPlayedGameRetriever playedGameRetriever, 
            IExcelGenerator excelGenerator, 
            IPlayedGameCreator playedGameCreator, 
            IPlayedGameDeleter playedGameDeleter)
        {
            this.playedGameRetriever = playedGameRetriever;
            this.excelGenerator = excelGenerator;
            this.playedGameCreator = playedGameCreator;
            this.playedGameDeleter = playedGameDeleter;
        }


        [ApiRoute("PlayedGamesExcel/", StartingVersion = 2)]
        [HttpGet]
        public virtual HttpResponseMessage ExportPlayedGamesToExcelVersion2([FromUri]int gamingGroupId)
        {
            return ExportPlayedGamesToExcel(gamingGroupId);
        }

        [ApiRoute("GamingGroups/{gamingGroupId}/PlayedGamesExcel", AcceptedVersions = new[] { 1 })]
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
                BoardGameGeekGameDefinitionId = playedGame.GameDefinition.BoardGameGeekGameDefinitionId,
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

        [ApiRoute("PlayedGames/", StartingVersion = 2)]
        [HttpGet]
        public HttpResponseMessage GetPlayedGamesVersion2([FromUri] PlayedGameFilterMessage playedGameFilterMessage)
        {
            return GetPlayedGameSearchResults(playedGameFilterMessage);
        }

        [ApiRoute("GamingGroups/{gamingGroupId}/PlayedGames/", AcceptedVersions = new[] { 1 })]
        [HttpGet]
        public HttpResponseMessage GetPlayedGames([FromBody]PlayedGameFilterMessage playedGameFilterMessage, [FromUri]int gamingGroupId)
        {
            playedGameFilterMessage.GamingGroupId = gamingGroupId;
            return GetPlayedGameSearchResults(playedGameFilterMessage);
        }

        private HttpResponseMessage GetPlayedGameSearchResults(PlayedGameFilterMessage playedGameFilterMessage)
        {
            var filter = new PlayedGameFilter();

            if (playedGameFilterMessage != null)
            {
                filter.GamingGroupId = playedGameFilterMessage.GamingGroupId;
                filter.StartDateGameLastUpdated = playedGameFilterMessage.StartDateGameLastUpdated;
                filter.MaximumNumberOfResults = playedGameFilterMessage.MaximumNumberOfResults;
                filter.PlayerId = playedGameFilterMessage.PlayerId;
                filter.DatePlayedFrom = playedGameFilterMessage.DatePlayedFrom;
                filter.DatePlayedTo = playedGameFilterMessage.DatePlayedTo;
            }
            var searchResults = this.playedGameRetriever.SearchPlayedGames(filter);

            var playedGamesSearchResultMessage = new PlayedGameSearchResultsMessage
            {
                PlayedGames = searchResults.Select(Mapper.Map<PlayedGameSearchResultMessage>).ToList()
            };

            return this.Request.CreateResponse(HttpStatusCode.OK, playedGamesSearchResultMessage);
        }

        [ApiRoute("PlayedGames/", StartingVersion = 2)]
        [HttpPost]
        [ApiAuthentication]
        [ApiModelValidation]
        public HttpResponseMessage RecordPlayedGameVersion2([FromBody] PlayedGameMessage playedGameMessage)
        {
            return RecordPlayedGame(playedGameMessage, CurrentUser.CurrentGamingGroupId);
        }

        [ApiRoute("GamingGroups/{gamingGroupId}/PlayedGames/", AcceptedVersions = new[] { 1 })]
        [HttpPost]
        [ApiAuthentication]
        [ApiModelValidation]
        public HttpResponseMessage RecordPlayedGame([FromBody]PlayedGameMessage playedGameMessage, [FromUri]int gamingGroupId)
        {
            var newlyCompletedGame = BuildNewlyPlayedGame(playedGameMessage);

            PlayedGame playedGame = playedGameCreator.CreatePlayedGame(newlyCompletedGame, TransactionSource.RestApi, CurrentUser);
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

        [ApiRoute("PlayedGames/{playedGameID}", StartingVersion = 2)]
        [HttpDelete]
        [ApiAuthentication]
        [ApiModelValidation]
        public HttpResponseMessage DeletePlayedGame(int playedGameID)
        {
            return DeletePlayedGame(playedGameID, CurrentUser.CurrentGamingGroupId);
        }

        [ApiRoute("GamingGroups/{gamingGroupId}/PlayedGames/{playedGameID}", AcceptedVersions = new[] { 1 })]
        [HttpDelete]
        [ApiAuthentication]
        [ApiModelValidation]
        public HttpResponseMessage DeletePlayedGame(int playedGameID, int gamingGroupId)
        {
            playedGameDeleter.DeletePlayedGame(playedGameID, CurrentUser); 

            return Request.CreateResponse(HttpStatusCode.NoContent);
        }
    }
}
