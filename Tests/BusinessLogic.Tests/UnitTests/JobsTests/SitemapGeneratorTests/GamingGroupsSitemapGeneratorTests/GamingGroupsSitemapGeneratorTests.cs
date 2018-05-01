using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BusinessLogic.Jobs.SitemapGenerator;
using BusinessLogic.Logic.GamingGroups;
using BusinessLogic.Models.GamingGroups;
using NSubstitute;
using NSubstituteAutoMocker;
using NUnit.Framework;
using Shouldly;
using X.Web.Sitemap;

namespace BusinessLogic.Tests.UnitTests.JobsTests.SitemapGeneratorTests.GamingGroupsSitemapGeneratorTests
{
    [TestFixture]
    public class GamingGroupsSitemapGeneratorTests
    {
        private NSubstituteAutoMocker<GamingGroupsSitemapGenerator> _autoMocker;
        private List<FileInfo> expectedFileInfo;
        private DirectoryInfo _targetDirectory;
        private List<GamingGroupSitemapInfo> _expectedGamingGroupSitemapInfo;
        private GamingGroupSitemapInfo _sitemapInfoForGamingGroupWithRecentPlay;
        private GamingGroupSitemapInfo _sitemapInfoForGamingGroupWithNoRecentPlays;
        private GamingGroupSitemapInfo _sitemapInfoForGamingGroupWithNoPlays;


        [SetUp]
        public void SetUp()
        {
            _autoMocker = new NSubstituteAutoMocker<GamingGroupsSitemapGenerator>();

            _sitemapInfoForGamingGroupWithRecentPlay = new GamingGroupSitemapInfo
            {
                GamingGroupId = 1,
                DateLastGamePlayed = DateTime.UtcNow.Date.AddDays(-30),
                DateCreated = DateTime.UtcNow.AddDays(-50)
            };
            _sitemapInfoForGamingGroupWithNoRecentPlays = new GamingGroupSitemapInfo
            {
                GamingGroupId = 2,
                DateLastGamePlayed = DateTime.UtcNow.Date.AddDays(-31),
                DateCreated = DateTime.UtcNow.AddDays(-60)
            };
            _sitemapInfoForGamingGroupWithNoPlays = new GamingGroupSitemapInfo
            {
                GamingGroupId = 3,
                DateLastGamePlayed = DateTime.MinValue,
                DateCreated = DateTime.UtcNow.AddDays(-14)
            };
            _expectedGamingGroupSitemapInfo = new List<GamingGroupSitemapInfo>
            {
                _sitemapInfoForGamingGroupWithRecentPlay,
                _sitemapInfoForGamingGroupWithNoRecentPlays,
                _sitemapInfoForGamingGroupWithNoPlays
            };

            _autoMocker.Get<IGamingGroupRetriever>().GetGamingGroupsSitemapInfo().Returns(_expectedGamingGroupSitemapInfo);

            expectedFileInfo = new List<FileInfo>();
            _autoMocker.Get<ISitemapGenerator>().GenerateSitemaps(Arg.Any<List<Url>>(), Arg.Any<DirectoryInfo>(), Arg.Any<string>())
                .Returns(expectedFileInfo);

            _targetDirectory = new DirectoryInfo("some directory");
        }

        [Test]
        public void It_Builds_Sitemaps_For_All_Active_Gaming_Groups()
        {
            //--arrange
            List<Url> generateSiteMapArgs = null;
            _autoMocker.Get<ISitemapGenerator>().GenerateSitemaps(Arg.Do<List<Url>>(x => generateSiteMapArgs = x), Arg.Any<DirectoryInfo>(), Arg.Any<string>());

            //--act
            _autoMocker.ClassUnderTest.BuildGamingGroupSitemaps(_targetDirectory);

            //--assert
            _autoMocker.Get<IGamingGroupRetriever>().Received().GetGamingGroupsSitemapInfo();

            generateSiteMapArgs.ShouldNotBeNull();

            generateSiteMapArgs.Count.ShouldBe(3);
            generateSiteMapArgs.ShouldContain(x => x.Location == "https://nemestats.com/GamingGroup/Details/" + _expectedGamingGroupSitemapInfo[0].GamingGroupId);
            generateSiteMapArgs.ShouldContain(x => x.Location == "https://nemestats.com/GamingGroup/Details/" + _expectedGamingGroupSitemapInfo[1].GamingGroupId);
            generateSiteMapArgs.ShouldContain(x => x.Location == "https://nemestats.com/GamingGroup/Details/" + _expectedGamingGroupSitemapInfo[2].GamingGroupId);
        }

        public class When_Last_Game_Played_Within_Last_30_Days : GamingGroupsSitemapGeneratorTests
        {
            [Test]
            public void It_Sets_The_Priority_To_Point_Four_And_Change_Frequency_To_Weekly()
            {
                //--arrange

                //--act
                _autoMocker.ClassUnderTest.BuildGamingGroupSitemaps(_targetDirectory);

                //--assert
                _autoMocker.Get<IGamingGroupRetriever>().Received().GetGamingGroupsSitemapInfo();
                _autoMocker.Get<ISitemapGenerator>().Received(1).GenerateSitemaps(
                    Arg.Is<List<Url>>(x => x.Any(y => y.ChangeFrequency == ChangeFrequency.Weekly 
                        && y.Priority == .4
                        && y.TimeStamp.Date == _sitemapInfoForGamingGroupWithRecentPlay.DateLastGamePlayed.Date)),
                    Arg.Any<DirectoryInfo>(),
                    Arg.Any<string>());
            }
        }

        public class When_Last_Game_Played_Older_Than_30_Days : GamingGroupsSitemapGeneratorTests
        {
            [Test]
            public void It_Sets_The_Priority_To_Point_Three_And_Change_Frequency_To_Monthly()
            {
                //--arrange

                //--act
                _autoMocker.ClassUnderTest.BuildGamingGroupSitemaps(_targetDirectory);

                //--assert
                _autoMocker.Get<IGamingGroupRetriever>().Received().GetGamingGroupsSitemapInfo();
                _autoMocker.Get<ISitemapGenerator>().Received(1).GenerateSitemaps(
                    Arg.Is<List<Url>>(x => x.Any(y => y.ChangeFrequency == ChangeFrequency.Monthly 
                        && y.Priority == .3 
                        && y.TimeStamp.Date == _sitemapInfoForGamingGroupWithNoRecentPlays.DateLastGamePlayed.Date)),
                    Arg.Any<DirectoryInfo>(),
                    Arg.Any<string>());
            }
        }

        public class When_There_Are_No_Played_Games : GamingGroupsSitemapGeneratorTests
        {
            [Test]
            public void It_Sets_The_Priority_To_Point_One_And_Change_Frequency_To_Yearly_And_Last_Mod_Date_To_Gaming_Group_Created_Date()
            {
                //--arrange

                //--act
                _autoMocker.ClassUnderTest.BuildGamingGroupSitemaps(_targetDirectory);

                //--assert
                _autoMocker.Get<IGamingGroupRetriever>().Received().GetGamingGroupsSitemapInfo();
                _autoMocker.Get<ISitemapGenerator>().Received(1).GenerateSitemaps(
                    Arg.Is<List<Url>>(x => x.Any(y => y.ChangeFrequency == ChangeFrequency.Yearly 
                        && y.Priority == .1 
                        && y.TimeStamp.Date == _sitemapInfoForGamingGroupWithNoPlays.DateCreated.Date)),
                    Arg.Any<DirectoryInfo>(),
                    Arg.Any<string>());
            }
        }


        [Test]
        public void It_Saves_Sitemaps_In_The_Specified_Directory_With_A_Name_Of_GamingGroupsSitemap()
        {
            //--arrange

            //--act
            _autoMocker.ClassUnderTest.BuildGamingGroupSitemaps(_targetDirectory);

            //--assert
            _autoMocker.Get<ISitemapGenerator>().Received().GenerateSitemaps(Arg.Any<List<Url>>(), Arg.Is<DirectoryInfo>(x => x == _targetDirectory), Arg.Is<string>(x => x == "gaminggroupssitemap"));
        }

        [Test]
        public void It_Returns_The_FileInfo_For_Sitemaps_That_Were_Generated()
        {
            //--act
            var fileInfo = _autoMocker.ClassUnderTest.BuildGamingGroupSitemaps(_targetDirectory);

            //--assert
            fileInfo.ShouldBeSameAs(expectedFileInfo);
        }
    }
}
