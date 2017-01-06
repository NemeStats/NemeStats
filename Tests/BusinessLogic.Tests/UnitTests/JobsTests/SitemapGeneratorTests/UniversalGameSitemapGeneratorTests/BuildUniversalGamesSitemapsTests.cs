using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BusinessLogic.Jobs.SitemapGenerator;
using BusinessLogic.Logic.BoardGameGeekGameDefinitions;
using BusinessLogic.Models.Games;
using NSubstitute;
using NSubstituteAutoMocker;
using NUnit.Framework;
using Shouldly;
using X.Web.Sitemap;

namespace BusinessLogic.Tests.UnitTests.JobsTests.SitemapGeneratorTests.UniversalGameSitemapGeneratorTests
{
    [TestFixture]
    public class BuildUniversalGamesSitemapsTests
    {
        private UniversalGameSitemapInfo _gameWithRecentPlay;
        private UniversalGameSitemapInfo _gameWithNoRecentPlay;
        private UniversalGameSitemapInfo _gameWithNoPlays;

        private NSubstituteAutoMocker<UniversalGameSitemapGenerator> _autoMocker;
        private List<FileInfo> expectedFileInfo;
        private DirectoryInfo _targetDirectory;

        [SetUp]
        public void SetUp()
        {
            _autoMocker = new NSubstituteAutoMocker<UniversalGameSitemapGenerator>();

            _gameWithRecentPlay = new UniversalGameSitemapInfo
            {
                BoardGameGeekGameDefinitionId = 1,
                DateLastGamePlayed = DateTime.UtcNow.Date.AddDays(-30),
                DateCreated = DateTime.UtcNow.AddDays(-100)
            };
            _gameWithNoRecentPlay = new UniversalGameSitemapInfo
            {
                BoardGameGeekGameDefinitionId = 2,
                DateLastGamePlayed = DateTime.UtcNow.Date.AddDays(-31),
                DateCreated = DateTime.UtcNow.AddDays(-200)
            };
            _gameWithNoPlays = new UniversalGameSitemapInfo
            {
                BoardGameGeekGameDefinitionId = 3,
                DateLastGamePlayed = DateTime.MinValue,
                DateCreated = DateTime.UtcNow.AddDays(-300)
            };
            var expectedUniversalGameSitemapInfos = new List<UniversalGameSitemapInfo>
            {
                _gameWithRecentPlay,
                _gameWithNoRecentPlay,
                _gameWithNoPlays
            };
            _autoMocker.Get<IUniversalGameRetriever>().GetAllActiveBoardGameGeekGameDefinitionSitemapInfos().Returns(expectedUniversalGameSitemapInfos);

            expectedFileInfo = new List<FileInfo>();
            _autoMocker.Get<ISitemapGenerator>().GenerateSitemaps(Arg.Any<List<Url>>(), Arg.Any<DirectoryInfo>(), Arg.Any<string>())
                .Returns(expectedFileInfo);

            _targetDirectory = new DirectoryInfo("some directory");
        }

        [Test]
        public void It_Builds_Sitemaps_For_All_Active_BoardGameGeekGameDefinitions()
        {
            //--arrange
            List<Url> generateSiteMapArgs = null;
            _autoMocker.Get<ISitemapGenerator>().GenerateSitemaps(Arg.Do<List<Url>>(x => generateSiteMapArgs = x), Arg.Any<DirectoryInfo>(), Arg.Any<string>());

            //--act
            _autoMocker.ClassUnderTest.BuildUniversalGamesSitemaps(_targetDirectory);

            //--assert
            _autoMocker.Get<IUniversalGameRetriever>().Received().GetAllActiveBoardGameGeekGameDefinitionSitemapInfos();

            generateSiteMapArgs.ShouldNotBeNull();

            generateSiteMapArgs.Count.ShouldBe(3);
            generateSiteMapArgs.ShouldContain(x => x.Location == "https://nemestats.com/UniversalGame/Details/" + _gameWithRecentPlay.BoardGameGeekGameDefinitionId);
            generateSiteMapArgs.ShouldContain(x => x.Location == "https://nemestats.com/UniversalGame/Details/" + _gameWithNoPlays.BoardGameGeekGameDefinitionId);
            generateSiteMapArgs.ShouldContain(x => x.Location == "https://nemestats.com/UniversalGame/Details/" + _gameWithNoRecentPlay.BoardGameGeekGameDefinitionId);
        }

        [Test]
        public void It_Sets_The_LastMod_Date_To_The_Date_Of_The_Last_Played_Game()
        {
            //--arrange

            //--act
            _autoMocker.ClassUnderTest.BuildUniversalGamesSitemaps(_targetDirectory);

            //--assert
            _autoMocker.Get<ISitemapGenerator>().Received().GenerateSitemaps(
                Arg.Is<List<Url>>(x => x.Any(y => y.TimeStamp.Date == _gameWithRecentPlay.DateLastGamePlayed.Date)),
                Arg.Any<DirectoryInfo>(),
                Arg.Any<string>());
            _autoMocker.Get<ISitemapGenerator>().Received().GenerateSitemaps(
                Arg.Is<List<Url>>(x => x.Any(y => y.TimeStamp.Date == _gameWithNoRecentPlay.DateLastGamePlayed)),
                Arg.Any<DirectoryInfo>(),
                Arg.Any<string>());
        }

        [Test]
        public void It_Sets_The_LastMod_Date_To_The_DateCreated_Of_The_BoardGameGeekGameDefinition_If_There_Are_No_Plays()
        {
            //--arrange

            //--act
            _autoMocker.ClassUnderTest.BuildUniversalGamesSitemaps(_targetDirectory);

            //--assert
            _autoMocker.Get<ISitemapGenerator>().Received().GenerateSitemaps(
                Arg.Is<List<Url>>(x => x.Any(y => y.TimeStamp.Date == _gameWithNoPlays.DateCreated.Date)),
                Arg.Any<DirectoryInfo>(),
                Arg.Any<string>());
        }

        public class When_Last_Game_Played_Within_Last_30_Days : BuildUniversalGamesSitemapsTests
        {
            [Test]
            public void It_Sets_The_Priority_To_Point_Eight_And_Change_Frequency_To_Weekly()
            {
                //--arrange

                //--act
                _autoMocker.ClassUnderTest.BuildUniversalGamesSitemaps(_targetDirectory);

                //--assert
                _autoMocker.Get<IUniversalGameRetriever>().Received().GetAllActiveBoardGameGeekGameDefinitionSitemapInfos();
                _autoMocker.Get<ISitemapGenerator>().Received(1).GenerateSitemaps(
                    Arg.Is<List<Url>>(x => x.Any(y => y.ChangeFrequency == ChangeFrequency.Weekly && y.Priority == .8)),
                    Arg.Any<DirectoryInfo>(),
                    Arg.Any<string>());
            }
        }

        public class When_Last_Game_Played_Older_Than_30_Days : BuildUniversalGamesSitemapsTests
        {
            [Test]
            public void It_Sets_The_Priority_To_Point_Seven_And_Change_Frequency_To_Monthly()
            {
                //--arrange

                //--act
                _autoMocker.ClassUnderTest.BuildUniversalGamesSitemaps(_targetDirectory);

                //--assert
                _autoMocker.Get<IUniversalGameRetriever>().Received().GetAllActiveBoardGameGeekGameDefinitionSitemapInfos();
                _autoMocker.Get<ISitemapGenerator>().Received(1).GenerateSitemaps(
                    Arg.Is<List<Url>>(x => x.Any(y => y.ChangeFrequency == ChangeFrequency.Monthly && y.Priority == .7)),
                    Arg.Any<DirectoryInfo>(),
                    Arg.Any<string>());
            }
        }

        [Test]
        public void It_Saves_Sitemaps_In_The_Specified_Directory_With_A_Name_Of_UniversalGamesSitemap()
        {
            //--arrange

            //--act
            _autoMocker.ClassUnderTest.BuildUniversalGamesSitemaps(_targetDirectory);

            //--assert
            _autoMocker.Get<ISitemapGenerator>().Received().GenerateSitemaps(Arg.Any<List<Url>>(), Arg.Is<DirectoryInfo>(x => x == _targetDirectory), Arg.Is<string>(x => x == "universalgamessitemap"));
        }

        [Test]
        public void It_Returns_The_FileInfo_For_Sitemaps_That_Were_Generated()
        {
            //--act
            var fileInfo = _autoMocker.ClassUnderTest.BuildUniversalGamesSitemaps(_targetDirectory);

            //--assert
            fileInfo.ShouldBeSameAs(expectedFileInfo);
        }
    }
}
