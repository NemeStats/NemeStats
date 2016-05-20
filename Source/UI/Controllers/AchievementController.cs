using System.Linq;
using System.Web.Mvc;
using BusinessLogic.Events.HandlerFactory;
using BusinessLogic.Logic.PlayerAchievements;
using BusinessLogic.Models.Achievements;
using UI.Controllers.Helpers;
using UI.Mappers;
using UI.Models.Achievements;

namespace UI.Controllers
{
    [RoutePrefix("achievements")]
    public partial class AchievementController : BaseController
    {

        private readonly IPlayerAchievementRetriever _playerAchievementRetriever;
        private readonly PlayerAchievementToPlayerAchievementViewModelMapper _playerAchievementToPlayerAchievementViewModelMapper;
        private readonly AchievementToAchievementViewModelMapper _achievementToAchievementViewModelMapper;

        public AchievementController(IPlayerAchievementRetriever playerAchievementRetriever,
            PlayerAchievementToPlayerAchievementViewModelMapper playerAchievementToPlayerAchievementViewModelMapper,
            AchievementToAchievementViewModelMapper achievementToAchievementViewModelMapper)
        {
            _playerAchievementRetriever = playerAchievementRetriever;
            _playerAchievementToPlayerAchievementViewModelMapper = playerAchievementToPlayerAchievementViewModelMapper;
            _achievementToAchievementViewModelMapper = achievementToAchievementViewModelMapper;
        }

        [Route("")]
        public virtual ActionResult Index()
        {
            var achievements = AchievementFactory.GetAchivements();
            var model = new AchievementListViewModel
            {
                Achievements = achievements.Select(a => _achievementToAchievementViewModelMapper.Map(a)).ToList()
            };

            return View(MVC.Achievement.Views.Index, model);

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