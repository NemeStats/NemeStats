using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;
using System.Web.Http;
using AutoMapper;
using BusinessLogic.Logic.PlayedGames;
using BusinessLogic.Models;
using BusinessLogic.Models.Games;
using CsvHelper;
using CsvHelper.Configuration;
using RollbarSharp;
using UI.Models.PlayedGame;

namespace UI.Areas.Api.Controllers
{
    public class GamingGroupsController : ApiController
    {
        public const int MAX_PLAYED_GAMES_TO_EXPORT = 1000;
        private readonly IPlayedGameRetriever playedGameRetriever;
        private MemoryStream csvFileMemoryStream;

        public GamingGroupsController(IPlayedGameRetriever playedGameRetriever)
        {
            this.playedGameRetriever = playedGameRetriever;
        }

        [Route("api/v1/GamingGroups/{gamingGroupId}/PlayedGames")]
        [HttpGet]
        public HttpResponseMessage Get(int gamingGroupId)
        {
            var playedGames = playedGameRetriever.GetRecentGames(MAX_PLAYED_GAMES_TO_EXPORT, gamingGroupId);
            var playedGamesForExport = playedGames.Select(playedGame => new PlayedGameExportModel
            {
                BoardGameGeekObjectId = playedGame.GameDefinition.BoardGameGeekObjectId,
                DateCreated = playedGame.DateCreated,
                DatePlayed = playedGame.DatePlayed,
                GameDefinitionId = playedGame.GameDefinitionId,
                GameDefinitionName = playedGame.GameDefinition.Name,
                GamingGroupId = playedGame.GamingGroupId,
                Id = playedGame.Id,
                Notes = playedGame.Notes,
                NumberOfPlayers = playedGame.NumberOfPlayers,
                WinningPlayerIds = String.Join("|", playedGame.PlayerGameResults.Where(result => result.GameRank == 1).Select(result => result.PlayerId).ToList()),
                WinningPlayerNames = String.Join("|", playedGame.PlayerGameResults.Where(result => result.GameRank == 1).Select(result => result.Player.Name).ToList())
            });
            csvFileMemoryStream = new MemoryStream();

            using (StreamWriter streamWriter = new StreamWriter(csvFileMemoryStream, Encoding.UTF8, 1024, leaveOpen: true))
            {
                
                CsvConfiguration csvConfiguration = new CsvConfiguration();
                csvConfiguration.AutoMap(typeof(PlayedGameExportModel));
                csvConfiguration.Delimiter = "|";
                using (var csvWriter = new CsvWriter(streamWriter, csvConfiguration))
                {
                    csvWriter.WriteRecords(playedGamesForExport);

                    csvFileMemoryStream.Position = 0;
                }
            }

            csvFileMemoryStream.Position = 0;

            HttpResponseMessage responseMessage = new HttpResponseMessage(HttpStatusCode.OK);
            responseMessage.Content = new StreamContent(csvFileMemoryStream);
            responseMessage.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = "played_game_export.csv"
            };
            responseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

            return responseMessage;
        }

        protected override void Dispose(bool disposing)
        {
            csvFileMemoryStream.Dispose();
            base.Dispose(disposing);
        }
    }
}
