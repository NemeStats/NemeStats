using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.Logic.Achievements;
using BusinessLogic.Logic.PlayerAchievements;
using BusinessLogic.Models.Achievements;
using BusinessLogic.Models.Utility;
using BusinessLogic.Paging;
using NemeStats.TestingHelpers.NemeStatsTestingExtensions;
using NUnit.Framework;
using NUnit.Framework.Internal;
using PagedList;
using Rhino.Mocks;
using Shouldly;

namespace BusinessLogic.Tests.UnitTests.LogicTests.GamingGroupsTests.GamingGroupRetrieverTests
{
    [TestFixture()]
    public class GetRecentChangesTests : GamingGroupRetrieverTestBase
    {
        private int _gamingGroupId = 1;
        private BasicDateRangeFilter _dateFilter;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            _dateFilter = new BasicDateRangeFilter();
        }

        [Test]
        public void It_Returns_The_Specified_Number_Of_Achievements()
        {
            //--arrange
            var expectedResult = new List<PlayerAchievementWinner>().ToPagedList(1, 1);
            AutoMocker.Get<IRecentPlayerAchievementsUnlockedRetriever>().Expect(mock =>
                    mock.GetResults(Arg<GetRecentPlayerAchievementsUnlockedQuery>.Is.Anything))
                .Return(expectedResult);

            //--act
            var results = AutoMocker.ClassUnderTest.GetRecentChanges(_gamingGroupId, _dateFilter);

            //--assert
            var args = AutoMocker.Get<IRecentPlayerAchievementsUnlockedRetriever>().GetArgumentsForCallsMadeOn(mock =>
                mock.GetResults(Arg<GetRecentPlayerAchievementsUnlockedQuery>.Is.Anything));
            var getRecentPlayerAchievementsUnlockedQuery = args.AssertFirstCallIsType<GetRecentPlayerAchievementsUnlockedQuery>();
            getRecentPlayerAchievementsUnlockedQuery.GamingGroupId.ShouldBe(_gamingGroupId);
            getRecentPlayerAchievementsUnlockedQuery.IncludeOnlyOnePage.ShouldBe(true);
            getRecentPlayerAchievementsUnlockedQuery.PageSize.ShouldBe(10);
            results.RecentAchievements.ShouldBe(expectedResult);
        }
    }
}
