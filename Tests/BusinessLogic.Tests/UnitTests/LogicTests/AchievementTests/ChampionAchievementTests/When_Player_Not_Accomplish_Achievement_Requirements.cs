using System.Linq;
using BusinessLogic.Models;
using BusinessLogic.Models.Achievements;
using NUnit.Framework;
using Rhino.Mocks;

namespace BusinessLogic.Tests.UnitTests.LogicTests.AchievementTests.ChampionAchievementTests
{
    [TestFixture]
    public class When_Player_Not_Accomplish_Achievement_Requirements : Base_ChampionAchievementTest
    {
        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            
            this.InsertChampionedGames(Achievement.LevelThresholds[AchievementLevel.Bronze], OtherPlayerId);
            DataContext.Stub(s => s.GetQueryable<Champion>()).Return(ChampionedGames.AsQueryable());
        }

        [Test]
        public void Then_Returns_No_Achievement()
        {
            var result = Achievement.IsAwardedForThisPlayer(PlayerId);
            Assert.IsNotNull(result);
            Assert.IsFalse(result.LevelAwarded.HasValue);
            Assert.IsEmpty(result.RelatedEntities);
        }
    }
}