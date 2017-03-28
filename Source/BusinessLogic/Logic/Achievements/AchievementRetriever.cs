using System;
using System.Collections.Generic;
using System.Linq;
using BusinessLogic.DataAccess;
using BusinessLogic.Events.HandlerFactory;
using BusinessLogic.Models;
using BusinessLogic.Models.Achievements;
using BusinessLogic.Models.User;

namespace BusinessLogic.Logic.Achievements
{
    public class AchievementRetriever : IAchievementRetriever
    {
        private readonly IDataContext _dataContext;

        public AchievementRetriever(IDataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public List<AggregateAchievementSummary> GetAllAchievementSummaries(ApplicationUser currentUser)
        {
            var allAchievementsInTheDatabase = _dataContext.GetQueryable<PlayerAchievement>()
                .GroupBy(x => x.AchievementId)
                .Select(x => new
                {
                    Id = x.Key,
                    NumberOfPlayersWithThisAchievement = x.Count()
                }).ToDictionary(x => x.Id, x => x);

            var allAchievementsThisPlayerHasEarned = new List<AchievementId>();
            if (currentUser.UserName != AnonymousApplicationUser.USER_NAME_ANONYMOUS)
            {
                allAchievementsThisPlayerHasEarned = _dataContext.GetQueryable<PlayerAchievement>()
                .Where(x => x.Player.ApplicationUserId == currentUser.Id)
                .Select(x => x.AchievementId)
                .ToList();
            }

            var allAchievements = GetAllAchievements();

            return allAchievements.Select(x => new AggregateAchievementSummary
            {
                AchievementId = x.Id,
                AchievementName = x.Name,
                CurrentPlayerUnlockedThisAchievement = allAchievementsThisPlayerHasEarned.Contains(x.Id),
                NumberOfPlayersWithThisAchievement =
                    allAchievementsInTheDatabase.ContainsKey(x.Id) ? allAchievementsInTheDatabase[x.Id].NumberOfPlayersWithThisAchievement : 0,
                Description = x.Description,
                Group = x.Group,
                IconClass = x.IconClass,
                LevelThresholds = x.LevelThresholds
            }).OrderByDescending(x => x.NumberOfPlayersWithThisAchievement).ToList();
        }

        internal virtual List<IAchievement> GetAllAchievements()
        {
            return AchievementFactory.GetAchievements();
        }

        public IAchievement GetAchievement(AchievementId achievementId)
        {
            return AchievementFactory.GetAchievementById(achievementId);
        }
    }
}