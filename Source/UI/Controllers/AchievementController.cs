using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using BusinessLogic.Events.HandlerFactory;
using BusinessLogic.Logic;
using BusinessLogic.Logic.Achievements;
using BusinessLogic.Logic.PlayerAchievements;
using BusinessLogic.Logic.Players;
using BusinessLogic.Models;
using BusinessLogic.Models.Achievements;
using BusinessLogic.Models.User;
using BusinessLogic.Paging;
using UI.Attributes.Filters;
using UI.Controllers.Helpers;
using UI.Models.Achievements;
using UI.Mappers.Extensions;
using UI.Mappers.Interfaces;
using UI.Models.GameDefinitionModels;
using UI.Models.PlayedGame;
using UI.Models.Players;

namespace UI.Controllers
{
    [RoutePrefix("achievements")]
    public partial class AchievementController : BaseController
    {

        private readonly IPlayerAchievementRetriever _playerAchievementRetriever;
        private readonly IRecentPlayerAchievementsUnlockedRetriever _recentPlayerAchievementsUnlockedRetriever;
        private readonly IMapperFactory _mapperFactory;
        private readonly IAchievementRetriever _achievementRetriever;
        private readonly ITransformer _transformer;

        public AchievementController(IPlayerAchievementRetriever playerAchievementRetriever,
            IRecentPlayerAchievementsUnlockedRetriever recentPlayerAchievementsUnlockedRetriever,
            IMapperFactory mapperFactory, 
            IAchievementRetriever achievementRetriever, 
            ITransformer transformer)
        {
            _playerAchievementRetriever = playerAchievementRetriever;
            _recentPlayerAchievementsUnlockedRetriever = recentPlayerAchievementsUnlockedRetriever;
            _mapperFactory = mapperFactory;
            _achievementRetriever = achievementRetriever;
            _transformer = transformer;
        }

        [Route("")]
        [UserContext(RequiresGamingGroup = false)]
        public virtual ActionResult Index(ApplicationUser currentUser)
        {
            var achievements = _achievementRetriever.GetAllAchievementSummaries(currentUser);
            var model = new AchievementListViewModel
            {
                CurrentUserId = currentUser?.Id,
                Achievements = _transformer.Transform<List<AchievementTileViewModel>>(achievements)
            };

            return View(MVC.Achievement.Views.Index, model);
        }

        [Route("{achievementId}/currentplayer")]
        [UserContext]
        public virtual ActionResult DetailsForCurrentUser(AchievementId achievementId, ApplicationUser currentUser)
        {
            var achievementQuery = new PlayerAchievementQuery(achievementId, currentUser.Id, currentUser.CurrentGamingGroupId.Value);
            var playerAchievementDetails = _playerAchievementRetriever.GetPlayerAchievement(achievementQuery);
            var playerAchievementViewModel =
                _transformer.Transform<PlayerAchievementViewModel>(playerAchievementDetails);

            return View(MVC.Achievement.Views.Details, playerAchievementViewModel);
        }

        [Route("{achievementId}")]
        [UserContext(RequiresGamingGroup = false)]
        public virtual ActionResult Details(AchievementId achievementId, ApplicationUser currentUser)
        {
            if (currentUser != null && currentUser.CurrentGamingGroupId > 0)
            {
                return DetailsForCurrentUser(achievementId, currentUser);
            }

            var achievement = _achievementRetriever.GetAchievement(achievementId);

            var viewModel = _transformer.Transform<PlayerAchievementViewModel>(achievement);
            return View(MVC.Achievement.Views.Details, viewModel);
        }

        [Route("{achievementId}/player/{playerId}")]
        public virtual ActionResult PlayerAchievement(AchievementId achievementId, int playerId)
        {
            var query = new PlayerAchievementQuery(achievementId, playerId);
            var playerAchievementDetails = _playerAchievementRetriever.GetPlayerAchievement(query);
            if (playerAchievementDetails == null)
            {
                return new HttpNotFoundResult();
            }

            var playerAchievementViewModel =
                _transformer.Transform<PlayerAchievementViewModel>(playerAchievementDetails);

            return View(MVC.Achievement.Views.Details, playerAchievementViewModel);
        }

        [Route("recent-unlocks/{page}")]
        public virtual ActionResult RecentAchievementsUnlocked(int page = 1)
        {
            var recentUnlocks =
                _recentPlayerAchievementsUnlockedRetriever.GetResults(new GetRecentPlayerAchievementsUnlockedQuery
                {
                    PageSize = 25,
                    Page = page
                });

            var model = recentUnlocks.ToTransformedPagedList<PlayerAchievementWinner, PlayerAchievementWinnerViewModel>(_transformer);

            return View(MVC.Achievement.Views.RecentAchievementsUnlocked, model);
        }
    }
}