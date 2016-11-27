using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using BusinessLogic.Logic.Achievements;
using UI.Areas.Api.Models;
using VersionedRestApi;

namespace UI.Areas.Api.Controllers
{
    public class AchievementsController : ApiControllerBase
    {
        private IAchievementRetriever _achievementRetriever;

        [ApiRoute("Achievements/")]
        [HttpGet]
        public virtual HttpResponseMessage GetAchievements([FromUri]AchievementsFilterMessage filter)
        {
            var achievements = _achievementRetriever.GetAchievements(filter);

            return ExportPlayedGamesToExcel(gamingGroupId);
        }
    }
}