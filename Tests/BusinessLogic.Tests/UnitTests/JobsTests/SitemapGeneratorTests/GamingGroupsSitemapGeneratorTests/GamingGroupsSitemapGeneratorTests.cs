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

        [SetUp]
        public void SetUp()
        {
            _autoMocker = new NSubstituteAutoMocker<GamingGroupsSitemapGenerator>();

            _sitemapInfoForGamingGroupWithRecentPlay = new GamingGroupSitemapInfo
            {
                GamingGroupId = 1,
                DateLastGamePlayed = DateTime.UtcNow.Date.AddDays(-30)
            };
            _sitemapInfoForGamingGroupWithNoRecentPlays = new GamingGroupSitemapInfo
            {
                GamingGroupId = 2,
                DateLastGamePlayed = DateTime.UtcNow.Date.AddDays(-31)
            };
            _expectedGamingGroupSitemapInfo = new List<GamingGroupSitemapInfo>
            {
                _sitemapInfoForGamingGroupWithRecentPlay,
                _sitemapInfoForGamingGroupWithNoRecentPlays
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

            generateSiteMapArgs.Count.ShouldBe(2);
            generateSiteMapArgs[0].Location.ShouldBe("https://nemestats.com/GamingGroup/Details/" + _expectedGamingGroupSitemapInfo[0].GamingGroupId);
            generateSiteMapArgs[1].Location.ShouldBe("https://nemestats.com/GamingGroup/Details/" + _expectedGamingGroupSitemapInfo[1].GamingGroupId);
        }

        [Test]
        public void It_Sets_The_Last_Mod_Date_To_The_Date_Of_The_Last_Play()
        {
            //--arrange

            //--act
            _autoMocker.ClassUnderTest.BuildGamingGroupSitemaps(_targetDirectory);

            //--assert
            _autoMocker.Get<IGamingGroupRetriever>().Received().GetGamingGroupsSitemapInfo();
            _autoMocker.Get<ISitemapGenerator>().Received().GenerateSitemaps(
                Arg.Is<List<Url>>(x => x.Any(y => y.TimeStamp.Date == _sitemapInfoForGamingGroupWithRecentPlay.DateLastGamePlayed.Date)),
                Arg.Any<DirectoryInfo>(),
                Arg.Any<string>());
            _autoMocker.Get<ISitemapGenerator>().Received().GenerateSitemaps(
                Arg.Is<List<Url>>(x => x.Any(y => y.TimeStamp.Date == _sitemapInfoForGamingGroupWithNoRecentPlays.DateLastGamePlayed.Date)),
                Arg.Any<DirectoryInfo>(),
                Arg.Any<string>());
        }

        public class When_Last_Game_Played_Within_Last_30_Days : GamingGroupsSitemapGeneratorTests
        {
            [Test]
            public void It_Sets_The_Priority_To_Point_Seven_And_Change_Frequency_To_Weekly()
            {
                //--arrange

                //--act
                _autoMocker.ClassUnderTest.BuildGamingGroupSitemaps(_targetDirectory);

                //--assert
                _autoMocker.Get<IGamingGroupRetriever>().Received().GetGamingGroupsSitemapInfo();
                _autoMocker.Get<ISitemapGenerator>().Received(1).GenerateSitemaps(
                    Arg.Is<List<Url>>(x => x.Any(y => y.ChangeFrequency == ChangeFrequency.Weekly && y.Priority == .7)),
                    Arg.Any<DirectoryInfo>(),
                    Arg.Any<string>());
            }
        }

        public class When_Last_Game_Played_Older_Than_30_Days : GamingGroupsSitemapGeneratorTests
        {
            [Test]
            public void It_Sets_The_Priority_To_Point_Six_And_Change_Frequency_To_Yearly()
            {
                //--arrange

                //--act
                _autoMocker.ClassUnderTest.BuildGamingGroupSitemaps(_targetDirectory);

                //--assert
                _autoMocker.Get<IGamingGroupRetriever>().Received().GetGamingGroupsSitemapInfo();
                _autoMocker.Get<ISitemapGenerator>().Received(1).GenerateSitemaps(
                    Arg.Is<List<Url>>(x => x.Any(y => y.ChangeFrequency == ChangeFrequency.Yearly && y.Priority == .6)),
                    Arg.Any<DirectoryInfo>(),
                    Arg.Any<string>());
            }
        }


        [Test]
        public void It_Saves_Sitemaps_In_The_Specified_Directory()
        {
            //--arrange

            //--act
            _autoMocker.ClassUnderTest.BuildGamingGroupSitemaps(_targetDirectory);

            //--assert
            _autoMocker.Get<ISitemapGenerator>().GenerateSitemaps(Arg.Any<List<Url>>(), Arg.Is<DirectoryInfo>(x => x == _targetDirectory), Arg.Any<string>());
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
