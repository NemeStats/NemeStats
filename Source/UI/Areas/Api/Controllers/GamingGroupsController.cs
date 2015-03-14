#region LICENSE
// NemeStats is a free website for tracking the results of board games.
//     Copyright (C) 2015 Jacob Gordon
// 
//     This program is free software: you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     This program is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU General Public License for more details.
// 
//     You should have received a copy of the GNU General Public License
//     along with this program.  If not, see <http://www.gnu.org/licenses/>
#endregion
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
using BusinessLogic.Models.PlayedGames;
using OfficeOpenXml;
using RollbarSharp;
using UI.Attributes;
using UI.Models.PlayedGame;
using BusinessLogic.Export;

namespace UI.Areas.Api.Controllers
{
    public class GamingGroupsController : ApiController
    {
        public const int MAX_PLAYED_GAMES_TO_EXPORT = 1000;

        private readonly IPlayedGameRetriever playedGameRetriever;
        private readonly IExcelGenerator excelGenerator;

        private MemoryStream exportMemoryStream;

        public GamingGroupsController(IPlayedGameRetriever playedGameRetriever, IExcelGenerator excelGenerator)
        {
            this.playedGameRetriever = playedGameRetriever;
            this.excelGenerator = excelGenerator;
        }

        [ApiRoute("GamingGroups/{gamingGroupId}/PlayedGames")]
        [HttpGet]
        public virtual HttpResponseMessage GetPlayedGames(int gamingGroupId)
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
    }
}
