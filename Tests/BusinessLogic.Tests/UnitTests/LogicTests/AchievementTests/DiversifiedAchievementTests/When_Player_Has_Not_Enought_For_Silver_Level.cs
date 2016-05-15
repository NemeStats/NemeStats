using System.Linq;
using BusinessLogic.Models.Achievements;
using NUnit.Framework;

namespace BusinessLogic.Tests.UnitTests.LogicTests.AchievementTests.DiversifiedAchievementTests
{
    [TestFixture]
    public class When_Player_Has_Not_Enought_For_Silver_Level : Base_DiversifiedAchievementTests
    {
        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            this.InsertPlayedGames(Achievement.LevelThresholds[AchievementLevel.Bronze]+1, this.PlayerId);
            this.InsertPlayedGames(1, OtherPlayerId);
        }

        [Test]
        public void Then_Returns_Bronze_Achievement()
        {
            var result = Achievement.IsAwardedForThisPlayer(PlayerId);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.LevelAwarded.HasValue);

            Assert.That(result.LevelAwarded.Value, Is.EqualTo(AchievementLevel.Bronze));
            foreach (var game in PlayedGames.Where(c => c.PlayerId == PlayerId))
            {
                Assert.IsTrue(result.RelatedEntities.Contains(game.PlayedGame.GameDefinitionId));
            }
        }
    }
}