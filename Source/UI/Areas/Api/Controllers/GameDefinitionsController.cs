using BusinessLogic.Logic.GameDefinitions;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using UI.Areas.Api.Models;
using UI.Attributes;

namespace UI.Areas.Api.Controllers
{
    public class GameDefinitionsController : ApiController
    {
        private readonly IGameDefinitionRetriever gameDefinitionRetriever;

        public GameDefinitionsController(IGameDefinitionRetriever gameDefinitionRetriever)
        {
            this.gameDefinitionRetriever = gameDefinitionRetriever;
        }

        [ApiAuthentication]
        [ApiModelValidation]
        [ApiRoute("GamingGroups/{gamingGroupId}/GameDefinitions/")]
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
    }
}