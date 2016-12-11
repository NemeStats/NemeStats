using System;
using System.Collections.Generic;
using System.Linq;
using BusinessLogic.DataAccess;
using BusinessLogic.Models;
using NUnit.Framework;
using Rhino.Mocks;
using Shouldly;

namespace BusinessLogic.Tests.UnitTests.LogicTests.GamingGroupsTests.GamingGroupRetrieverTests
{
    [TestFixture]
    public class GetGamingGroupsSitemapInfoTests : GamingGroupRetrieverTestBase
    {
        private GamingGroup _gamingGroupWithNoPlays;
        private GamingGroup _gamingGroupWithNoPlaysInLastYear;
        private GamingGroup _gamingGroupWithPlaysInLastYear;
        private readonly DateTime _expectedMostRecentDate = DateTime.UtcNow;

        [SetUp]
        public void LocalSetUp()
        {
            _gamingGroupWithNoPlays = new GamingGroup
            {
                Id = 1,
                PlayedGames = new List<PlayedGame>()
            };
            _gamingGroupWithNoPlaysInLastYear = new GamingGroup
            {
                Id = 2,
                PlayedGames = new List<PlayedGame>
                {
                    new PlayedGame
                    {
                        DatePlayed = DateTime.UtcNow.AddDays(-366)
                    }
                }
            };

            _gamingGroupWithPlaysInLastYear = new GamingGroup
            {
                Id = 3,
                PlayedGames = new List<PlayedGame>
                {
                    new PlayedGame
                    {
                        DatePlayed = _expectedMostRecentDate
                    },
                    new PlayedGame
                    {
                        DatePlayed = DateTime.UtcNow.AddDays(-20)
                    }
                }
            };
            var gamingGroupQueryable = new List<GamingGroup>
            {
                _gamingGroupWithNoPlays,
                _gamingGroupWithNoPlaysInLastYear,
                _gamingGroupWithPlaysInLastYear
            }.AsQueryable();

            AutoMocker.Get<IDataContext>().Expect(x => x.GetQueryable<GamingGroup>()).Return(gamingGroupQueryable);
        }

        [Test]
        public void It_Only_Retrieves_Info_For_Gaming_Groups_That_Have_Had_A_Play_In_The_Last_Year()
        {
            //--arrange

            //--act
            var results = AutoMocker.ClassUnderTest.GetGamingGroupsSitemapInfo();

            //--assert
            results.ShouldNotBeNull();
            results.Count.ShouldBe(1);
            results[0].GamingGroupId.ShouldBe(_gamingGroupWithPlaysInLastYear.Id);
        }

        [Test]
        public void It_Sets_The_Date_Last_Played_Based_On_The_Most_Recent_Played_Game()
        {
            //--arrange

            //--act
            var results = AutoMocker.ClassUnderTest.GetGamingGroupsSitemapInfo();

            //--assert
            results.ShouldNotBeNull();
            results[0].DateLastGamePlayed.ShouldBe(_expectedMostRecentDate);
        }
    }
}
