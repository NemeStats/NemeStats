using System;
using System.Collections.Generic;
using System.IO;
using BusinessLogic.Jobs.SitemapGenerator;
using BusinessLogic.Logic.BoardGameGeekGameDefinitions;
using NSubstitute;
using NSubstituteAutoMocker;
using NUnit.Framework;
using Shouldly;
using X.Web.Sitemap;

namespace BusinessLogic.Tests.UnitTests.JobsTests.SitemapGeneratorTests.UniversalGameSitemapGeneratorTests
{
    public class BuildUniversalGamesSitemapsTests
    {
        private List<int> _expectedUniversalGameIds;

        private NSubstituteAutoMocker<UniversalGameSitemapGenerator> _autoMocker;
        private List<FileInfo> expectedFileInfo;
        private DirectoryInfo _targetDirectory;

        [SetUp]
        public void SetUp()
        {
            _autoMocker = new NSubstituteAutoMocker<UniversalGameSitemapGenerator>();

            _expectedUniversalGameIds = new List<int>
            {
                1,
                2
            };
            _autoMocker.Get<IUniversalGameRetriever>().GetAllActiveBoardGameGeekGameDefinitionIds().Returns(_expectedUniversalGameIds);

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
            _autoMocker.Get<IUniversalGameRetriever>().Received().GetAllActiveBoardGameGeekGameDefinitionIds();

            generateSiteMapArgs.ShouldNotBeNull();

            generateSiteMapArgs.Count.ShouldBe(2);
            generateSiteMapArgs[0].Location.ShouldBe("https://nemestats.com/UniversalGame/Details/" + _expectedUniversalGameIds[0]);
            generateSiteMapArgs[0].LastMod.ShouldBe(DateTime.UtcNow.ToString("yyyy-MM-dd"));
            generateSiteMapArgs[0].Priority.ShouldBe(.7);
            generateSiteMapArgs[0].ChangeFrequency.ShouldBe(ChangeFrequency.Daily);

            generateSiteMapArgs[1].Location.ShouldBe("https://nemestats.com/UniversalGame/Details/" + _expectedUniversalGameIds[1]);
            generateSiteMapArgs[1].LastMod.ShouldBe(DateTime.UtcNow.ToString("yyyy-MM-dd"));
            generateSiteMapArgs[1].Priority.ShouldBe(.7);
            generateSiteMapArgs[1].ChangeFrequency.ShouldBe(ChangeFrequency.Daily);
        }

        [Test]
        public void It_Saves_Sitemaps_In_The_Specified_Directory()
        {
            //--arrange
            List<Url> generateSiteMapArgs = null;
            _autoMocker.Get<ISitemapGenerator>().GenerateSitemaps(Arg.Do<List<Url>>(x => generateSiteMapArgs = x), Arg.Any<DirectoryInfo>(), Arg.Any<string>());

            //--act
            _autoMocker.ClassUnderTest.BuildUniversalGamesSitemaps(_targetDirectory);

            //--assert
            _autoMocker.Get<ISitemapGenerator>().GenerateSitemaps(Arg.Any<List<Url>>(), Arg.Is<DirectoryInfo>(x => x == _targetDirectory), Arg.Any<string>());

            generateSiteMapArgs.ShouldNotBeNull();

            generateSiteMapArgs.Count.ShouldBe(2);
            generateSiteMapArgs[0].Location.ShouldBe("https://nemestats.com/UniversalGame/Details/" + _expectedUniversalGameIds[0]);
            generateSiteMapArgs[0].LastMod.ShouldBe(DateTime.UtcNow.ToString("yyyy-MM-dd"));
            generateSiteMapArgs[0].Priority.ShouldBe(.7);
            generateSiteMapArgs[0].ChangeFrequency.ShouldBe(ChangeFrequency.Daily);

            generateSiteMapArgs[1].Location.ShouldBe("https://nemestats.com/UniversalGame/Details/" + _expectedUniversalGameIds[1]);
            generateSiteMapArgs[1].LastMod.ShouldBe(DateTime.UtcNow.ToString("yyyy-MM-dd"));
            generateSiteMapArgs[1].Priority.ShouldBe(.7);
            generateSiteMapArgs[1].ChangeFrequency.ShouldBe(ChangeFrequency.Daily);
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
