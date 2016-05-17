using System.Web.Mvc;
using BusinessLogic.Logic.PlayerAchievements;
using BusinessLogic.Models.Achievements;
using BusinessLogic.Models.User;
using UI.Attributes.Filters;
using UI.Controllers.Helpers;
using UI.Models;
using UI.Transformations;

namespace UI.Controllers
{
    [RoutePrefix("achievements")]
    public partial class AchievementController : BaseController
    {
        private readonly IPlayerAchievementViewModelBuilder _playerAchievementViewModelBuilder;
        private readonly IPlayerAchievementRetriever _playerAchievementRetriever;

        public AchievementController(IPlayerAchievementViewModelBuilder playerAchievementViewModelBuilder, IPlayerAchievementRetriever playerAchievementRetriever)
        {
            _playerAchievementViewModelBuilder = playerAchievementViewModelBuilder;
            _playerAchievementRetriever = playerAchievementRetriever;
        }

        [Route("{achievementId}/player/{playerId}")]
        public virtual ActionResult Details(AchievementId achievementId, int playerId)
        {
            var playerAchievement = _playerAchievementRetriever.GetPlayerAchievement(playerId, achievementId);
            var model = _playerAchievementViewModelBuilder.Build(playerAchievement);
            return View(MVC.Achievement.Views.Details, model);
        }
    }
}