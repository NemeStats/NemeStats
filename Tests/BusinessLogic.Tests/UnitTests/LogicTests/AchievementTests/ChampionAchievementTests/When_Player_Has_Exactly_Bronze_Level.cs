using System.Linq;
using BusinessLogic.Models.Achievements;
using NUnit.Framework;

namespace BusinessLogic.Tests.UnitTests.LogicTests.AchievementTests.ChampionAchievementTests
{
    [TestFixture]
    public class When_Player_Has_Exactly_Bronze_Level : Base_ChampionAchievementTest
    {
       [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            this.InsertChampionedGames(Achievement.ClassUnderTest.LevelThresholds[AchievementLevel.Bronze],this.PlayerId);
            this.InsertChampionedGames(Achievement.ClassUnderTest.LevelThresholds[AchievementLevel.Bronze], OtherPlayerId);
            
        }

        [Test]
        public void Then_Returns_Bronze_Achievement()
        {
            var result = Achievement.ClassUnderTest.IsAwardedForThisPlayer(PlayerId);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.LevelAwarded.HasValue);

            Assert.That(result.LevelAwarded.Value, Is.EqualTo(AchievementLevel.Bronze));

            foreach (var championedGame in ChampionedGames.Where(c=>c.PlayerId == PlayerId))
            {
                Assert.IsTrue(result.RelatedEntities.Contains(championedGame.GameDefinitionId));
            }
            
        }
    }
}
