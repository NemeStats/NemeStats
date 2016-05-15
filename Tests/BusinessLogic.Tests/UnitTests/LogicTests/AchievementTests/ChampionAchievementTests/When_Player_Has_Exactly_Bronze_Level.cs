using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.Models;
using BusinessLogic.Models.Achievements;
using NUnit.Framework;
using Rhino.Mocks;

namespace BusinessLogic.Tests.UnitTests.LogicTests.AchievementTests.ChampionAchievementTests
{
    [TestFixture]
    public class When_Player_Has_Exactly_Bronze_Level : Base_ChampionAchievementTest
    {
       [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            this.InsertChampionedGames(Achievement.LevelThresholds[AchievementLevel.Bronze],this.PlayerId);
            this.InsertChampionedGames(Achievement.LevelThresholds[AchievementLevel.Bronze], OtherPlayerId);
            DataContext.Stub(s => s.GetQueryable<Champion>()).Return(ChampionedGames.AsQueryable());
        }

        [Test]
        public void Then_Returns_Bronze_Achievement()
        {
            var result = Achievement.IsAwardedForThisPlayer(PlayerId, DataContext);
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
