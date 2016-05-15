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
        private static readonly IEnumerable<Type> AchievementTypes;

        static AchievementFactory()
        {
            AchievementTypes = AchievementInterface.Assembly
                .GetTypes()
                .Where(p => AchievementInterface.IsAssignableFrom(p) && !p.IsInterface);
        }

        public static List<IAchievement> GetAchivements()
        {
            return AchievementTypes.Select(achievementType => (IAchievement) EventHandlerObjectFactory.Container.GetInstance(achievementType)).ToList();
        }

        public static T GetAchievement<T>() where T:  IAchievement
        {
            var type = AchievementTypes.FirstOrDefault(a => a.GetType() == typeof (T));
            return (T)EventHandlerObjectFactory.Container.GetInstance(type) ;
        }

        public static IAchievement GetAchievementById(AchievementId id)
        {
            var type = AchievementTypes.FirstOrDefault(a => (AchievementId)a.GetProperty("Id").GetValue(a) == id);
            return (IAchievement) EventHandlerObjectFactory.Container.GetInstance(type);
        }
    }
}