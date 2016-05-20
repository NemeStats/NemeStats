using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.DataAccess;
using BusinessLogic.Logic.Achievements;
using BusinessLogic.Models.Achievements;
using NUnit.Framework;
using Rhino.Mocks;

namespace BusinessLogic.Tests.UnitTests.LogicTests.AchievementTests.BaseAchievementTests
{
    [TestFixture]
    public class DescriptionTests
    {
        private IDataContext _dataContext;

        [SetUp]
        public void SetUp()
        {
            _dataContext = MockRepository.GenerateMock<IDataContext>();
        }

        internal class AchievementWithNoLevels : BaseAchievement
        {
            public AchievementWithNoLevels(IDataContext dataContext) : base(dataContext)
            {
            }

            public override AchievementId Id => AchievementId.BusyBee;

            public override AchievementGroup Group => AchievementGroup.PlayedGame;

            public override string Name => "Achievement Without Thresholds";

            public override string DescriptionFormat => "This achievement has no thresholds.";

            public override string IconClass => "Not Applicable";

            public override Dictionary<AchievementLevel, int> LevelThresholds => new Dictionary<AchievementLevel, int>();
            public override AchievementAwarded IsAwardedForThisPlayer(int playerId)
            {
                throw new NotImplementedException();
            }
        }

        [Test]
        public void It_Returns_The_Description_Format_If_There_Are_No_Achievement_Levels()
        {
            //--arrange
            var achievement = new AchievementWithNoLevels(_dataContext);

            //--act
            var description = achievement.Description;

            //--assert
            Assert.That(description, Is.EqualTo(achievement.DescriptionFormat));
        }

        internal class AchievementWithOneLevel : BaseAchievement
        {
            public AchievementWithOneLevel(IDataContext dataContext) : base(dataContext)
            {
            }

            public override AchievementId Id => AchievementId.BusyBee;

            public override AchievementGroup Group => AchievementGroup.PlayedGame;

            public override string Name => "Achievement With one Level";

            public override string DescriptionFormat => "Earn this achievement by doing {0} things.";

            public override string IconClass => "Not Applicable";

            public override Dictionary<AchievementLevel, int> LevelThresholds => new Dictionary<AchievementLevel, int>
            {
                {AchievementLevel.Gold, 1}
            };
            public override AchievementAwarded IsAwardedForThisPlayer(int playerId)
            {
                throw new NotImplementedException();
            }
        }

        [Test]
        public void It_Returns_The_Description_Format_With_The_Sole_Level_Threshold()
        {
            //--arrange
            var achievement = new AchievementWithOneLevel(_dataContext);
            var expectedDescription = string.Format(achievement.DescriptionFormat, achievement.LevelThresholds.First().Value);

            //--act
            var description = achievement.Description;

            //--assert
            Assert.That(description, Is.EqualTo(expectedDescription));
        }

        internal class AchievementWithThreeLevels : BaseAchievement
        {
            public AchievementWithThreeLevels(IDataContext dataContext) : base(dataContext)
            {
            }

            public override AchievementId Id => AchievementId.BusyBee;

            public override AchievementGroup Group => AchievementGroup.PlayedGame;

            public override string Name => "Achievement With 3 Levels";

            public override string DescriptionFormat => "Earn this achievement by doing {0} things.";

            public override string IconClass => "Not Applicable";

            public override Dictionary<AchievementLevel, int> LevelThresholds => new Dictionary<AchievementLevel, int>
            {
                {AchievementLevel.Silver, 1},
                {AchievementLevel.Gold, 1}
            };

            public override AchievementAwarded IsAwardedForThisPlayer(int playerId)
            {
                throw new NotImplementedException();
            }
        }

        [Test]
        public void It_Returns_The_Description_Format_With_Forward_Slashes_Separating_Each_Threshold_In_The_Description()
        {
            //--arrange
            var achievement = new AchievementWithThreeLevels(_dataContext);
            string thresholdString = string.Join("/", achievement.LevelThresholds.Values.ToArray());
            var expectedDescription = string.Format(achievement.DescriptionFormat, thresholdString);

            //--act
            var description = achievement.Description;

            //--assert
            Assert.That(description, Is.EqualTo(expectedDescription));
        }
    }
}
