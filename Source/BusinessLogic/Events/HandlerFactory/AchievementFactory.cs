using System.Collections.Generic;
using System.Linq;
using BusinessLogic.Logic.Achievements;
using BusinessLogic.Models.Achievements;

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

        public static T GetAchievement<T>() where T:  IAchievement
        {
            return (T) Achievements.FirstOrDefault(a => a.GetType() == typeof(T));
        }

        public static IAchievement GetAchievementById(AchievementId id)
        {
            return Achievements.FirstOrDefault(a => a.Id == id);
        }
    }
}