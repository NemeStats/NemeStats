using System;
using System.Collections.Generic;
using System.Linq;
using BusinessLogic.Logic.Achievements;
using BusinessLogic.Models.Achievements;

namespace BusinessLogic.Events.HandlerFactory
{
    public static class AchievementFactory
    {

        private static readonly Type AchievementInterface = typeof(IAchievement);        
        private static readonly Dictionary<AchievementId, Type> AchievementTypes = new Dictionary<AchievementId, Type>();

        static AchievementFactory()
        {
            var types = AchievementInterface.Assembly
                .GetTypes()
                .Where(p => AchievementInterface.IsAssignableFrom(p) && !p.IsInterface && !p.IsAbstract);


            foreach (var type in types)
            {
                var achievement = (IAchievement)EventHandlerObjectFactory.Container.GetInstance(type);
                AchievementTypes.Add(achievement.Id, type);
            }
        }

        public static List<IAchievement> GetAchievements()
        {
            return AchievementTypes.Values.Select(achievementType => (IAchievement)EventHandlerObjectFactory.Container.GetInstance(achievementType)).ToList();
        }

        public static T GetAchievement<T>() where T : IAchievement
        {
            var type = AchievementTypes.Values.FirstOrDefault(t => t == typeof(T));
            return (T)EventHandlerObjectFactory.Container.GetInstance(type);
        }

        public static IAchievement GetAchievementById(AchievementId id)
        {
            var type = AchievementTypes[id];
            return (IAchievement)EventHandlerObjectFactory.Container.GetInstance(type);
        }
    }
}