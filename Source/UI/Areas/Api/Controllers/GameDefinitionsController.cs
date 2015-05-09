using BusinessLogic.Logic.GameDefinitions;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BusinessLogic.Models;
using BusinessLogic.Models.User;
using UI.Areas.Api.Models;
using UI.Attributes;

namespace UI.Areas.Api.Controllers
{
    public class GameDefinitionsController : ApiController
    {
        private readonly IGameDefinitionRetriever gameDefinitionRetriever;
        private readonly IGameDefinitionSaver gameDefinitionSaver;

        public GameDefinitionsController(IGameDefinitionRetriever gameDefinitionRetriever, IGameDefinitionSaver gameDefinitionSaver)
        {
            this.gameDefinitionRetriever = gameDefinitionRetriever;
            this.gameDefinitionSaver = gameDefinitionSaver;
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

        [ApiAuthentication]
        [ApiModelValidation]
        [ApiRoute("GamingGroups/{gamingGroupId}/GameDefinitions/")]
        [HttpPost]
        public virtual HttpResponseMessage SaveNewGameDefinition([FromBody]NewGameDefinitionMessage newGameDefinitionMessage, [FromUri]int gamingGroupId)
        {
            var applicationUser = ActionContext.ActionArguments[ApiAuthenticationAttribute.ACTION_ARGUMENT_APPLICATION_USER] as ApplicationUser;

            var gameDefinition = new GameDefinition
            {
                BoardGameGeekObjectId = newGameDefinitionMessage.BoardGameGeekObjectId,
                Name = newGameDefinitionMessage.GameDefinitionName
            };

            var newGameDefinition = gameDefinitionSaver.Save(gameDefinition, applicationUser);

            var newlyCreatedGameDefinitionMessage = new NewlyCreatedGameDefinitionMessage
            {
                GameDefinitionId = newGameDefinition.Id
            };

            return Request.CreateResponse(HttpStatusCode.OK, newlyCreatedGameDefinitionMessage);
        }
    }
}