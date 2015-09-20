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

using BusinessLogic.Logic.GameDefinitions;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BusinessLogic.Models;
using UI.Areas.Api.Models;
using UI.Attributes;

namespace UI.Areas.Api.Controllers
{
    public class GameDefinitionsController : ApiControllerBase
    {
        private readonly IGameDefinitionRetriever gameDefinitionRetriever;
        private readonly IGameDefinitionSaver gameDefinitionSaver;

        public GameDefinitionsController(IGameDefinitionRetriever gameDefinitionRetriever, IGameDefinitionSaver gameDefinitionSaver)
        {
            this.gameDefinitionRetriever = gameDefinitionRetriever;
            this.gameDefinitionSaver = gameDefinitionSaver;
        }

        [ApiModelValidation]
        [ApiRoute("GameDefinitions/", StartingVersion =  2)]
        [HttpGet]
        public virtual HttpResponseMessage GetGameDefinitionsVersion2([FromUri] int gamingGroupId)
        {
            return this.GetGameDefinitions(gamingGroupId);
        }

        [ApiAuthentication]
        [ApiModelValidation]
        [ApiRoute("GamingGroups/{gamingGroupId}/GameDefinitions/", AcceptedVersions = new[] { 1 })]
        [HttpGet]
        public virtual HttpResponseMessage GetGameDefinitions([FromUri] int gamingGroupId)
        {
            var results = gameDefinitionRetriever.GetAllGameDefinitions(gamingGroupId);

            var gameDefinitionsSearchResultsMessage = new GameDefinitionsSearchResultsMessage
            {
                GameDefinitions = results.Select(result => new GameDefinitionSearchResultMessage
                {
                    Active = result.Active,
                    BoardGameGeekObjectId = result.BoardGameGeekObjectId,
                    GameDefinitionId = result.Id,
                    GameDefinitionName = result.Name
                }).ToList()
            };

            return Request.CreateResponse(HttpStatusCode.OK, gameDefinitionsSearchResultsMessage);
        }

        [ApiAuthentication]
        [ApiModelValidation]
        [ApiRoute("GameDefinitions/", StartingVersion = 2)]
        [HttpPost]
        public virtual HttpResponseMessage SaveNewGameDefinitionVersion2([FromBody]NewGameDefinitionMessage newGameDefinitionMessage, [FromUri]int gamingGroupId)
        {
            return this.SaveNewGameDefinition(newGameDefinitionMessage, gamingGroupId);
        }

        [ApiAuthentication]
        [ApiModelValidation]
        [ApiRoute("GamingGroups/{gamingGroupId}/GameDefinitions/")]
        [HttpPost]
        public virtual HttpResponseMessage SaveNewGameDefinition([FromBody]NewGameDefinitionMessage newGameDefinitionMessage, [FromUri]int gamingGroupId)
        {
            var gameDefinition = new GameDefinition
            {
                BoardGameGeekObjectId = newGameDefinitionMessage.BoardGameGeekObjectId,
                Name = newGameDefinitionMessage.GameDefinitionName
            };

            var newGameDefinition = gameDefinitionSaver.Save(gameDefinition, CurrentUser);

            var newlyCreatedGameDefinitionMessage = new NewlyCreatedGameDefinitionMessage
            {
                GameDefinitionId = newGameDefinition.Id
            };

            return Request.CreateResponse(HttpStatusCode.OK, newlyCreatedGameDefinitionMessage);
        }

        [ApiAuthentication]
        [ApiModelValidation]
        [ApiRoute("GamingGroups/{gamingGroupId}/GameDefinitions/{gameDefinitionId}/")]
        [HttpPut]
        public HttpResponseMessage UpdateGameDefinition(UpdateGameDefinitionMessage updateGameDefinitionMessage, int gameDefinitionId, int gamingGroupId)
        {
            if (updateGameDefinitionMessage == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "You must pass at least one valid parameter.");
            }

            var gameDefinitionUpdateRequest = new GameDefinitionUpdateRequest
            {
                Active = updateGameDefinitionMessage.Active,
                Name = updateGameDefinitionMessage.GameDefinitionName,
                BoardGameGeekObjectId = updateGameDefinitionMessage.BoardGameGeekObjectId,
                GameDefinitionId = gameDefinitionId
            };

            gameDefinitionSaver.UpdateGameDefinition(gameDefinitionUpdateRequest, CurrentUser);

            return Request.CreateResponse(HttpStatusCode.NoContent);
        }
    }
}