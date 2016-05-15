using BusinessLogic.Logic.Achievements;
using StructureMap.AutoMocking;

namespace BusinessLogic.Tests.UnitTests.LogicTests.AchievementTests.DiversifiedAchievementTests
{
    public abstract class Base_AchievementsTests<T> where T : class, IAchievement
    {
        public RhinoAutoMocker<T> Achievement;
        public void InitMock()
        {
            Achievement = new RhinoAutoMocker<T>();
            Achievement.PartialMockTheClassUnderTest();

        }
    }
}