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
        private GamingGroup _gamingGroupWithPlays;
        private readonly DateTime _expectedMostRecentDate = DateTime.UtcNow;

        [SetUp]
        public void LocalSetUp()
        {
            _gamingGroupWithNoPlays = new GamingGroup
            {
                Id = 2,
                DateCreated = DateTime.UtcNow,
                PlayedGames = new List<PlayedGame>()
            };

            _gamingGroupWithPlays = new GamingGroup
            {
                Id = 1,
                DateCreated = DateTime.UtcNow.AddDays(-1),
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
                _gamingGroupWithPlays
            }.AsQueryable();

            AutoMocker.Get<IDataContext>().Expect(x => x.GetQueryable<GamingGroup>()).Return(gamingGroupQueryable);
        }

        [Test]
        public void It_Sets_The_Date_Last_Played_Based_On_The_Most_Recent_Played_Game()
        {
            //--arrange

            //--act
            var results = AutoMocker.ClassUnderTest.GetGamingGroupsSitemapInfo();

            //--assert
            results.ShouldNotBeNull();
            results.Count.ShouldBe(2);
            results.ShouldContain(x => x.GamingGroupId == _gamingGroupWithNoPlays.Id && x.DateLastGamePlayed == new DateTime());
            results.ShouldContain(x => x.GamingGroupId == _gamingGroupWithPlays.Id && x.DateLastGamePlayed == _expectedMostRecentDate);
        }

        [Test]
        public void It_Returns_The_Date_The_Gaming_Group_Was_Created()
        {
            //--arrange

            //--act
            var results = AutoMocker.ClassUnderTest.GetGamingGroupsSitemapInfo();

            //--assert
            results.ShouldContain(x => x.GamingGroupId == _gamingGroupWithNoPlays.Id && x.DateCreated == _gamingGroupWithNoPlays.DateCreated);
            results.ShouldContain(x => x.GamingGroupId == _gamingGroupWithPlays.Id && x.DateCreated == _gamingGroupWithPlays.DateCreated);
        }

        [Test]
        public void It_Returns_Records_Ordered_By_Gaming_Group_Id()
        {
            //--arrange

            //--act
            var results = AutoMocker.ClassUnderTest.GetGamingGroupsSitemapInfo();

            //--assert
            results[0].GamingGroupId.ShouldBeLessThan(results[1].GamingGroupId);
        }
    }
}
