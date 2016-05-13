using BusinessLogic.Models.Achievements;
using BusinessLogic.Tests.UnitTests.LogicTests.AchievementTests.ChampionAchievementTests;
using NUnit.Framework;

namespace BusinessLogic.Tests.UnitTests.LogicTests.AchievementTests.DiversifiedAchievementTests
{
    [TestFixture]
    public class When_Player_Not_Acomplish_Achievement_Requirements : Base_DiversifiedAchievementTests
    {
        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            
            this.InsertPlayedGames(Achievement.LevelThresholds[AchievementLevel.Bronze], OtherPlayerId);
        }

        [Test]
        public void Then_Returns_No_Achievement()
        {
            var result = Achievement.IsAwardedForThisPlayer(PlayerId, DataContext);
            Assert.IsNotNull(result);
            Assert.IsFalse(result.LevelAwarded.HasValue);
            Assert.IsEmpty(result.RelatedEntities);
        }
    }
}