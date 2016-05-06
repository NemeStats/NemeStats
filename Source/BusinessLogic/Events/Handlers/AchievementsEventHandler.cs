using System;
using System.Collections.Generic;
using System.Linq;
using BusinessLogic.DataAccess;
using BusinessLogic.Events.Interfaces;
using BusinessLogic.Logic.Achievements;
using BusinessLogic.Models;

namespace BusinessLogic.Events.Handlers
{
    public class AchievementsEventHandler : IBusinessLogicEventHandler<PlayedGameCreatedEvent>
    {
        private List<IAchievement> _achievements;
        private readonly IDataContext _dataContext;

        public AchievementsEventHandler(IDataContext dataContext)
        {
            _dataContext = dataContext;

            InitAchievements();
        }

        private void InitAchievements()
        {
            var achievementInterface = typeof (IAchievement);
            var achievementTypes = achievementInterface.Assembly
                .GetTypes()
                .Where(p => achievementInterface.IsAssignableFrom(p) && !p.IsInterface);

            foreach (var achievementType in achievementTypes)
            {
                _achievements.Add((IAchievement) Activator.CreateInstance(achievementType));
            }
        }

        public void Handle(PlayedGameCreatedEvent @event)
        {
            var playedGame = _dataContext.FindById<PlayedGame>(@event.PlayedGameId);

            var players = playedGame.PlayerGameResults.Select(p => p.Player);

            foreach (var player in players)
            {
                foreach (var achievement in _achievements)
                {
                    var currentPlayerAchievement =
                        player.PlayerAchievements.FirstOrDefault(
                            pa => pa.AchievementId == (int) achievement.AchievementType); //TODO: Change the Id for the AchievementType enum on PlayerAchievements
                    var levelAwarded = achievement.AchievementLevelAwarded(player);

                    if (levelAwarded.HasValue && currentPlayerAchievement != null && levelAwarded > currentPlayerAchievement.AchievementLevel)
                    {
                        //TODO: Save the PlayerAchievement on DB. We can save the trigger played game id too to show on the UI "Achievement triggered on THAT play"
                    }
                }
            }

            
        }
    }


}
