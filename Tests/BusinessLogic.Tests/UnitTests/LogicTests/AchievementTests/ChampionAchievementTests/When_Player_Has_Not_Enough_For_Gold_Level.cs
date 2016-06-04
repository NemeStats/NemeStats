using System.Linq;
using BusinessLogic.Models.Achievements;
using NUnit.Framework;

namespace BusinessLogic.Tests.UnitTests.LogicTests.AchievementTests.ChampionAchievementTests
{
    [TestFixture]
    public class When_Player_Has_Not_Enough_For_Gold_Level : Base_ChampionAchievementTest
    {
        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            this.InsertChampionedGames(Achievement.ClassUnderTest.LevelThresholds[AchievementLevel.Silver] + 1, this.PlayerId);
            this.InsertChampionedGames(1, OtherPlayerId);
        }

        [Test]
        public void Then_Returns_Silver_Achievement()
        {
            var result = Achievement.ClassUnderTest.IsAwardedForThisPlayer(PlayerId);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.LevelAwarded.HasValue);

            Assert.That(result.LevelAwarded.Value, Is.EqualTo(AchievementLevel.Silver));
            foreach (var championedGame in ChampionedGames.Where(c => c.PlayerId == PlayerId))
            {
                Assert.IsTrue(result.RelatedEntities.Contains(championedGame.GameDefinitionId));
            }
        }
    }
}