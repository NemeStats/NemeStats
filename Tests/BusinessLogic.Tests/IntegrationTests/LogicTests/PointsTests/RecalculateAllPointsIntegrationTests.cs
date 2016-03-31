using BusinessLogic.DataAccess;
using BusinessLogic.DataAccess.Security;
using BusinessLogic.Logic.Points;
using NUnit.Framework;

namespace BusinessLogic.Tests.IntegrationTests.LogicTests.PointsTests
{
    [TestFixture]
    public class RecalculateAllPointsIntegrationTests
    {
        [Test]
        [Ignore("Recalculates all points using the current points logic!")]
        public void RecalculateAllPoints()
        {
            //--arrange
            using (var dbContext = new NemeStatsDbContext())
            {
                using (var dataContext = new NemeStatsDataContext(dbContext, new SecuredEntityValidatorFactory()))
                {
                    var weightTierCalculator = new WeightTierCalculator();
                    var weightBonusCalculator = new WeightBonusCalculator(weightTierCalculator);
                    var gameDurationBonusCalculator = new GameDurationBonusCalculator();
                    var pointsCalculator = new PointsCalculator(weightBonusCalculator, gameDurationBonusCalculator);

                    //--act
                    new GlobalPointsRecalculator().RecalculateAllPoints(dataContext, pointsCalculator, 0, int.MaxValue);
                }   
            } 
        }

        [Test]
        [Ignore("Recalculates all points for games that have either no play time or no BGG Game Definition at all.")]
        public void RecalculateAllPointsForGamesWithNoPlayTime()
        {
            //--arrange
            using (var dbContext = new NemeStatsDbContext())
            {
                using (var dataContext = new NemeStatsDataContext(dbContext, new SecuredEntityValidatorFactory()))
                {
                    var weightTierCalculator = new WeightTierCalculator();
                    var weightBonusCalculator = new WeightBonusCalculator(weightTierCalculator);
                    var gameDurationBonusCalculator = new GameDurationBonusCalculator();
                    var pointsCalculator = new PointsCalculator(weightBonusCalculator, gameDurationBonusCalculator);

                    //--act
                    new GlobalPointsRecalculator().RecalculateAllPointsForGamesWithNoPlayTime(dataContext, pointsCalculator);
                }
            }
        }
    }
}
