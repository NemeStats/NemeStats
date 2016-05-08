using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using BusinessLogic.DataAccess;
using BusinessLogic.Events.Interfaces;
using BusinessLogic.Logic.Achievements;
using BusinessLogic.Models;
using BusinessLogic.Models.User;

namespace BusinessLogic.Events.Handlers
{
    public class AchievementsEventHandler : BaseEventHandler, IBusinessLogicEventHandler<PlayedGameCreatedEvent>
    {

        private List<IAchievement> _achievements;


        public AchievementsEventHandler(IDataContext dataContext) : base(dataContext)
        {
            InitAchievements();
        }

        private void InitAchievements()
        {
            _achievements = new List<IAchievement>();
            var achievementInterface = typeof(IAchievement);
            var achievementTypes = achievementInterface.Assembly
                .GetTypes()
                .Where(p => achievementInterface.IsAssignableFrom(p) && !p.IsInterface);

            foreach (var achievementType in achievementTypes)
            {
                _achievements.Add((IAchievement)Activator.CreateInstance(achievementType));
            }
        }

        public void Handle(PlayedGameCreatedEvent @event)
        {
            var players =
                DataContext.GetQueryable<PlayerGameResult>().Where(p => p.PlayedGameId == @event.TriggerEntityId)
                .Select(p => p.Player)
                .Include(p => p.PlayerAchievements);

            foreach (var player in players.ToList())
            {
                foreach (var achievement in _achievements)
                {
                    var currentPlayerAchievement =
                        player.PlayerAchievements.FirstOrDefault(
                            pa => pa.AchievementId == achievement.AchievementType);

                    var levelAwarded = achievement.AchievementLevelAwarded(player.Id, DataContext);

                    if (levelAwarded.HasValue && currentPlayerAchievement == null)
                    {
                        var playerAchievement = new PlayerAchievement
                        {
                            Player = player,
                            PlayerId = player.Id,
                            AchievementId = achievement.AchievementType,
                            AchievementLevel = levelAwarded.Value,
                        };                        

                        DataContext.Save(playerAchievement, new AnonymousApplicationUser());
                    }
                    else if (currentPlayerAchievement != null && levelAwarded > currentPlayerAchievement.AchievementLevel)
                    {
                        currentPlayerAchievement.AchievementLevel = levelAwarded.Value;
                        currentPlayerAchievement.LastUpdatedDate = DateTime.UtcNow;
                    }
                }
            }

            DataContext.CommitAllChanges();


        }
    }


}
