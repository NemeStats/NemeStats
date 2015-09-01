using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using BusinessLogic.Logic.Players;
using BusinessLogic.Models.Players;
using UI.Areas.Api.Models;
using UI.Attributes;
using UI.Transformations;

namespace UI.Areas.Api.Controllers
{
    public class PlayerStatsController : ApiControllerBase
    {
        private readonly IPlayerRetriever playerRetriever;
        private readonly ITransformer transformer;

        public PlayerStatsController(IPlayerRetriever playerRetriever, ITransformer transformer)
        {
            this.playerRetriever = playerRetriever;
            this.transformer = transformer;
        }

        [ApiRoute("GamingGroups/{gamingGroupId}/PlayerStats/{playerId}/")]
        [HttpGet]
        public virtual HttpResponseMessage GetPlayerStats(int id)
        {
            var playerStatistics = playerRetriever.GetPlayerStatistics(id);
            var playerStatisticsMessage = transformer.Transform<PlayerStatistics, PlayerStatisticsMessage>(playerStatistics);

            return Request.CreateResponse(HttpStatusCode.OK, playerStatisticsMessage);
        }
    }
}