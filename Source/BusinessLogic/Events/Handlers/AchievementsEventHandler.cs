using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using BusinessLogic.DataAccess;
using BusinessLogic.Events.HandlerFactory;
using BusinessLogic.Events.Interfaces;
using BusinessLogic.Logic.Achievements;
using BusinessLogic.Models;
using BusinessLogic.Models.Achievements;
using BusinessLogic.Models.User;
using Microsoft.AspNet.SignalR;
using NemeStats.Hubs;

namespace BusinessLogic.Events.Handlers
{
    public class AchievementsEventHandler : BaseEventHandler, IBusinessLogicEventHandler<PlayedGameCreatedEvent>
    {


        public AchievementsEventHandler(IDataContext dataContext) : base(dataContext)
        {
        }


        public void Handle(PlayedGameCreatedEvent @event)
        {
            var players =
                DataContext.GetQueryable<PlayerGameResult>().Where(p => p.PlayedGameId == @event.TriggerEntityId)
                .Select(p => p.Player)
                .Include(p => p.PlayerAchievements);

            foreach (var player in players.ToList())
            {
                foreach (var achievement in AchievementFactory.Achievements)
                {
                    var currentPlayerAchievement =
                        player.PlayerAchievements.FirstOrDefault(
                            pa => pa.AchievementId == achievement.Id);

                    var achievementAwarded = achievement.IsAwardedForThisPlayer(player.Id, DataContext);

                    if (achievementAwarded.LevelAwarded.HasValue)
                    {
                        if (currentPlayerAchievement == null)
                        {
                            var playerAchievement = new PlayerAchievement
                            {
                                Player = player,
                                PlayerId = player.Id,
                                AchievementId = achievement.Id,
                                AchievementLevel = achievementAwarded.LevelAwarded.Value,
                                RelatedEntities = achievementAwarded.RelatedEntities
                            };

                            DataContext.Save(playerAchievement, new AnonymousApplicationUser());
                            DataContext.CommitAllChanges();

                            NotifyPlayer(player, achievement, achievementAwarded.LevelAwarded);

                        }
                        else if (achievementAwarded.LevelAwarded.Value > currentPlayerAchievement.AchievementLevel)
                        {
                            currentPlayerAchievement.AchievementLevel = achievementAwarded.LevelAwarded.Value;
                            currentPlayerAchievement.LastUpdatedDate = DateTime.UtcNow;
                            currentPlayerAchievement.RelatedEntities = achievementAwarded.RelatedEntities;

                            DataContext.CommitAllChanges();

                            NotifyPlayer(player, achievement, achievementAwarded.LevelAwarded);
                        }
                    }
                }
            }

        }

        private static void NotifyPlayer(Player player, IAchievement achievement, AchievementLevel? levelAwarded)
        {
            if (player.ApplicationUserId != null)
            {
                var notificationClient =
                    GlobalHost.ConnectionManager.GetHubContext<NotificationsHub>().Clients.Group(player.ApplicationUserId);

                notificationClient.NewAchievementUnlocked(achievement.Id,
                    levelAwarded.Value.ToString());
            }
        }
    }


}
