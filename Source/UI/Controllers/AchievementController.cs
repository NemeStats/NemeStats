using System.Web.Mvc;
using BusinessLogic.Logic.PlayerAchievements;
using BusinessLogic.Models.Achievements;
using BusinessLogic.Models.User;
using UI.Attributes.Filters;
using UI.Controllers.Helpers;
using UI.Mappers;
using UI.Models;

namespace UI.Controllers
{
    [RoutePrefix("achievements")]
    public partial class AchievementController : BaseController
    {
        
        private readonly IPlayerAchievementRetriever _playerAchievementRetriever;
        private readonly PlayerAchievementToPlayerAchievementViewModelMapper _playerAchievementToPlayerAchievementViewModelMapper;

        public AchievementController(IPlayerAchievementRetriever playerAchievementRetriever, PlayerAchievementToPlayerAchievementViewModelMapper playerAchievementToPlayerAchievementViewModelMapper)
        {
            _playerAchievementRetriever = playerAchievementRetriever;
            _playerAchievementToPlayerAchievementViewModelMapper = playerAchievementToPlayerAchievementViewModelMapper;
        }

        [Route("{achievementId}/player/{playerId}")]
        public virtual ActionResult Details(AchievementId achievementId, int playerId)
        {
            var playerAchievement = _playerAchievementRetriever.GetPlayerAchievement(playerId, achievementId);
            var model = _playerAchievementToPlayerAchievementViewModelMapper.Map(playerAchievement);
            return View(MVC.Achievement.Views.Details, model);
        }
    }
}