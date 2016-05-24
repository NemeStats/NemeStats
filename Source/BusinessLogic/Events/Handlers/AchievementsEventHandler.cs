using System;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BusinessLogic.DataAccess;
using BusinessLogic.Events.HandlerFactory;
using BusinessLogic.Events.Interfaces;
using BusinessLogic.Logic.Achievements;
using BusinessLogic.Models;
using BusinessLogic.Models.Achievements;
using BusinessLogic.Models.User;
using Microsoft.AspNet.SignalR;
using NemeStats.Hubs;
using RollbarSharp;

namespace BusinessLogic.Events.Handlers
{
    public class AchievementsEventHandler : BaseEventHandler, IBusinessLogicEventHandler<PlayedGameCreatedEvent>
    {
        private readonly IRollbarClient _rollbarClient;


        public AchievementsEventHandler(IDataContext dataContext, IRollbarClient rollbarClient) : base(dataContext)
        {
            _rollbarClient = rollbarClient;
        }


        public void Handle(PlayedGameCreatedEvent @event)
        {
            var players =
                DataContext.GetQueryable<PlayerGameResult>().Where(p => p.PlayedGameId == @event.TriggerEntityId)
                .Select(p => p.Player)
                .Include(p => p.PlayerAchievements);

            foreach (var player in players.ToList())
            {
                foreach (var achievement in AchievementFactory.GetAchievements())
                {
                    try
                    {
                        var currentPlayerAchievement =
                            player.PlayerAchievements.FirstOrDefault(
                                pa => pa.AchievementId == achievement.Id);

                        var achievementAwarded = achievement.IsAwardedForThisPlayer(player.Id);

                        if (achievementAwarded.LevelAwarded.HasValue && achievementAwarded.LevelAwarded.Value > AchievementLevel.None)
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
                    catch (Exception ex)
                    {
                        _rollbarClient.SendException(ex);
                    }
                }
            }

        }

        private static void NotifyPlayer(Player player, IAchievement achievement, AchievementLevel? levelAwarded)
        {
            if (player.ApplicationUserId != null)
            {
                Task.Factory.StartNew(() =>
                {
                    var notificationClient =
                        GlobalHost.ConnectionManager.GetHubContext<NotificationsHub>()
                            .Clients.Group(player.ApplicationUserId);

                    Thread.Sleep(2000); // We wait 2 seconds to avoid notify when user is posting and ensure that notification is received by the signalR client. Ugly, I know...

                    notificationClient.NewAchievementUnlocked(achievement.Id.ToString(), achievement.Name, achievement.IconClass, achievement.Description, levelAwarded.Value.ToString());
                });
            }
        }
    }


}
