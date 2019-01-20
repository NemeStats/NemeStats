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
using UI.Areas.Api.Models;
using UI.Attributes;
using VersionedRestApi;
using BusinessLogic.Models.Games;
using UI.HtmlHelpers;

namespace UI.Areas.Api.Controllers
{
    public class GameDefinitionsController : ApiControllerBase
    {
        private readonly IGameDefinitionRetriever _gameDefinitionRetriever;
        private readonly IGameDefinitionSaver _gameDefinitionSaver;
        private readonly ICreateGameDefinitionComponent _createGameDefinitionComponent;

        public GameDefinitionsController(IGameDefinitionRetriever gameDefinitionRetriever, IGameDefinitionSaver gameDefinitionSaver, ICreateGameDefinitionComponent createGameDefinitionComponent)
        {
            _gameDefinitionRetriever = gameDefinitionRetriever;
            _gameDefinitionSaver = gameDefinitionSaver;
            _createGameDefinitionComponent = createGameDefinitionComponent;
        }

        [ApiModelValidation]
        [ApiRoute("GameDefinitions/", StartingVersion =  2)]
        [HttpGet]
        public virtual HttpResponseMessage GetGameDefinitionsVersion2([FromUri] int gamingGroupId)
        {
            return GetGameDefinitions(gamingGroupId);
        }

        [ApiAuthentication]
        [ApiModelValidation]
        [ApiRoute("GamingGroups/{gamingGroupId}/GameDefinitions/", AcceptedVersions = new[] { 1 })]
        [HttpGet]
        public virtual HttpResponseMessage GetGameDefinitions([FromUri] int gamingGroupId)
        {
            var results = _gameDefinitionRetriever.GetAllGameDefinitions(gamingGroupId);

            var gameDefinitionsSearchResultsMessage = new GameDefinitionsSearchResultsMessage
            {
                GameDefinitions = results.Select(result => new GameDefinitionSearchResultMessage
                {
                    Active = result.Active,
                    BoardGameGeekGameDefinitionId = result.BoardGameGeekGameDefinitionId,
                    BoardGameGeekObjectId = result.BoardGameGeekGameDefinitionId,
                    GameDefinitionId = result.Id,
                    GameDefinitionName = result.Name,
                    NemeStatsUrl = AbsoluteUrlBuilder.GetGameDefinitionUrl(result.Id)
                }).ToList()
            };

            return Request.CreateResponse(HttpStatusCode.OK, gameDefinitionsSearchResultsMessage);
        }

        [ApiAuthentication]
        [ApiModelValidation]
        [ApiRoute("GameDefinitions/", StartingVersion = 2)]
        [HttpPost]
        public virtual HttpResponseMessage SaveNewGameDefinition([FromBody]NewGameDefinitionMessage newGameDefinitionMessage)
        {
            //--gamingGroupId is obsolete here and will be on the request or else used from the current user context -- so just pass "0"
            return SaveNewGameDefinition(newGameDefinitionMessage, 0);
        }

        [ApiAuthentication]
        [ApiModelValidation]
        [ApiRoute("GamingGroups/{gamingGroupId}/GameDefinitions/", AcceptedVersions = new[] { 1 })]
        [HttpPost]
        public virtual HttpResponseMessage SaveNewGameDefinition([FromBody]NewGameDefinitionMessage newGameDefinitionMessage, [FromUri]int gamingGroupId)
        {
            var createGameDefinitionRequest = new CreateGameDefinitionRequest
            {
                BoardGameGeekGameDefinitionId = newGameDefinitionMessage.BoardGameGeekObjectId,
                Name = newGameDefinitionMessage.GameDefinitionName,
                GamingGroupId = newGameDefinitionMessage.GamingGroupId
            };

            var newGameDefinition = _createGameDefinitionComponent.Execute(createGameDefinitionRequest, CurrentUser);

            var newlyCreatedGameDefinitionMessage = new NewlyCreatedGameDefinitionMessage
            {
                GameDefinitionId = newGameDefinition.Id,
                GamingGroupId = newGameDefinition.GamingGroupId,
                NemeStatsUrl = AbsoluteUrlBuilder.GetGameDefinitionUrl(newGameDefinition.Id)
        };

            return Request.CreateResponse(HttpStatusCode.OK, newlyCreatedGameDefinitionMessage);
        }

        [ApiAuthentication]
        [ApiModelValidation]
        [ApiRoute("GameDefinitions/{gameDefinitionId}/", StartingVersion = 2)]
        [HttpPut]
        public HttpResponseMessage UpdateGameDefinition(UpdateGameDefinitionMessage updateGameDefinitionMessage, int gameDefinitionId)
        {
            //--gamingGroupId is obsolete here and will be on the request or else used from the current user context -- so just pass "0"
            return UpdateGameDefinition(updateGameDefinitionMessage, gameDefinitionId, 0);
        }

        [ApiAuthentication]
        [ApiModelValidation]
        [ApiRoute("GamingGroups/{gamingGroupId}/GameDefinitions/{gameDefinitionId}/", AcceptedVersions = new[] { 1 })]
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
                BoardGameGeekGameDefinitionId = updateGameDefinitionMessage.BoardGameGeekGameDefinitionId,
                GameDefinitionId = gameDefinitionId
            };

            _gameDefinitionSaver.UpdateGameDefinition(gameDefinitionUpdateRequest, CurrentUser);

            return Request.CreateResponse(HttpStatusCode.NoContent);
        }
    }
}