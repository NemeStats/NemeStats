using System.Collections.Generic;
using System.Linq;
using BusinessLogic.Logic.Achievements;

namespace BusinessLogic.Events.HandlerFactory
{
    public static class AchievementFactory
    {
        public static List<IAchievement> Achievements;

        static AchievementFactory()
        {
            Achievements = new List<IAchievement>();
            var achievementInterface = typeof(IAchievement);
            var achievementTypes = achievementInterface.Assembly
                .GetTypes()
                .Where(p => achievementInterface.IsAssignableFrom(p) && !p.IsInterface);

            foreach (var achievementType in achievementTypes)
            {
                Achievements.Add((IAchievement)EventHandlerObjectFactory.Container.GetInstance(achievementType));
            }
        }
    }
}