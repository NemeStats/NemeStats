using System;
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
using BusinessLogic.Models.Games;
using BusinessLogic.Models.PlayedGames;
using UI.Areas.Api.Models;
using UI.Attributes;
using UI.HtmlHelpers;
using VersionedRestApi;

namespace UI.Areas.Api.Controllers
{
    public class PlayedGamesController : ApiControllerBase
    {
        public const int MaxPlayedGamesToExport = 1000;

        private readonly IPlayedGameRetriever _playedGameRetriever;
        private readonly IExcelGenerator _excelGenerator;
        private readonly IPlayedGameSaver _playedGameSaver;
        private readonly ICreatePlayedGameComponent _createPlayedGameComponent;
        private readonly IPlayedGameDeleter _playedGameDeleter;
        private readonly ITransformer _transformer;

        private MemoryStream _exportMemoryStream;

        public PlayedGamesController(
            IPlayedGameRetriever playedGameRetriever, 
            IExcelGenerator excelGenerator, 
            IPlayedGameSaver playedGameSaver, 
            IPlayedGameDeleter playedGameDeleter, ITransformer transformer, 
            ICreatePlayedGameComponent createPlayedGameComponent)
        {
            _playedGameRetriever = playedGameRetriever;
            _excelGenerator = excelGenerator;
            _playedGameSaver = playedGameSaver;
            _playedGameDeleter = playedGameDeleter;
            _transformer = transformer;
            _createPlayedGameComponent = createPlayedGameComponent;
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
            var playedGames = _playedGameRetriever.GetRecentGames(MaxPlayedGamesToExport, gamingGroupId);

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

            _exportMemoryStream = new MemoryStream();

            _excelGenerator.GenerateExcelFile(playedGamesForExport, _exportMemoryStream);

            var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StreamContent(_exportMemoryStream)
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
            _exportMemoryStream?.Dispose();
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

        internal virtual HttpResponseMessage GetPlayedGameSearchResults(PlayedGameFilterMessage playedGameFilterMessage)
        {
            var filter = _transformer.Transform<PlayedGameFilter>(playedGameFilterMessage);

            var searchResults = _playedGameRetriever.SearchPlayedGames(filter);

            var playedGamesSearchResultMessage = new PlayedGameSearchResultsMessage
            {
                PlayedGames = searchResults.Select(Mapper.Map<PlayedGameSearchResultMessage>).ToList()
            };

            playedGamesSearchResultMessage.PlayedGames.ForEach(x => x.NemeStatsUrl = AbsoluteUrlBuilder.GetPlayedGameDetailsUrl(x.PlayedGameId));

            return Request.CreateResponse(HttpStatusCode.OK, playedGamesSearchResultMessage);
        }

        [ApiRoute("PlayedGames/", StartingVersion = 2)]
        [HttpPost]
        [ApiAuthentication]
        [ApiModelValidation]
        public HttpResponseMessage RecordPlayedGameVersion2([FromBody] PlayedGameMessage playedGameMessage)
        {
            return RecordPlayedGame(playedGameMessage, 0);
        }

        [ApiRoute("GamingGroups/{gamingGroupId}/PlayedGames/", AcceptedVersions = new[] { 1 })]
        [HttpPost]
        [ApiAuthentication]
        [ApiModelValidation]
        public HttpResponseMessage RecordPlayedGame([FromBody]PlayedGameMessage playedGameMessage, [FromUri]int gamingGroupId)
        {
            var newlyCompletedGame = _transformer.Transform<NewlyCompletedGame>(playedGameMessage);
            newlyCompletedGame.TransactionSource = TransactionSource.RestApi;

            var playedGame = _createPlayedGameComponent.Execute(newlyCompletedGame, CurrentUser);
            var newlyRecordedPlayedGameMessage = new NewlyRecordedPlayedGameMessage
            {
                PlayedGameId = playedGame.Id,
                GamingGroupId = playedGame.GamingGroupId,
                NemeStatsUrl = AbsoluteUrlBuilder.GetPlayedGameDetailsUrl(playedGame.Id)
            };

            return Request.CreateResponse(HttpStatusCode.OK, newlyRecordedPlayedGameMessage);
        }

        [ApiRoute("PlayedGames/")]
        [HttpPut]
        [ApiAuthentication]
        [ApiModelValidation]
        public HttpResponseMessage UpdatePlayedGame([FromBody]UpdatedPlayedGameMessage playedPlayedGameMessage)
        {
            var newlyCompletedGame = _transformer.Transform<UpdatedGame>(playedPlayedGameMessage);

            _playedGameSaver.UpdatePlayedGame(newlyCompletedGame, TransactionSource.RestApi, CurrentUser);

            return Request.CreateResponse(HttpStatusCode.NoContent);
        }

        [ApiRoute("PlayedGames/{playedGameID}", StartingVersion = 2)]
        [HttpDelete]
        [ApiAuthentication]
        [ApiModelValidation]
        public HttpResponseMessage DeletePlayedGame(int playedGameId)
        {
            return DeletePlayedGame(playedGameId, 0);
        }

        [ApiRoute("GamingGroups/{gamingGroupId}/PlayedGames/{playedGameID}", AcceptedVersions = new[] { 1 })]
        [HttpDelete]
        [ApiAuthentication]
        [ApiModelValidation]
        public HttpResponseMessage DeletePlayedGame(int playedGameId, int gamingGroupId)
        {
            _playedGameDeleter.DeletePlayedGame(playedGameId, CurrentUser); 

            return Request.CreateResponse(HttpStatusCode.NoContent);
        }
    }
}
