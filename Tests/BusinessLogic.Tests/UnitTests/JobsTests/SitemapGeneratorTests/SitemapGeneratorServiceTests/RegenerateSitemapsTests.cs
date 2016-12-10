using System;
using System.Collections.Generic;
using System.Configuration.Abstractions;
using System.Diagnostics;
using System.IO;
using System.Linq;
using BusinessLogic.Jobs.SitemapGenerator;
using NSubstitute;
using NSubstituteAutoMocker;
using NUnit.Framework;
using Shouldly;
using X.Web.Sitemap;

namespace BusinessLogic.Tests.UnitTests.JobsTests.SitemapGeneratorTests.SitemapGeneratorServiceTests
{
    [TestFixture]
    public class RegenerateSitemapsTests
    {
        private NSubstituteAutoMocker<SitemapGeneratorService> _autoMocker;
        private List<FileInfo> _universalGamesSitemapFileInfos;
        private List<FileInfo> _gamingGroupsSitemapFileInfos;
        private FileInfo _staticPagesSitemapFileInfoInfo;
        private const string _sitemapFileLocation = "c:\\temp\\sitemaps\\";
        private string _sitemapHttpLocation = "https://nemestats.com/sitemaps/";

        [SetUp]
        public void SetUp()
        {
            _autoMocker = new NSubstituteAutoMocker<SitemapGeneratorService>();

            _universalGamesSitemapFileInfos = new List<FileInfo>
            {
                new FileInfo("file1"),
                new FileInfo("file2")
            };
            _autoMocker.Get<IUniversalGameSitemapGenerator>().BuildUniversalGamesSitemaps(Arg.Any<DirectoryInfo>()).Returns(_universalGamesSitemapFileInfos);

            _gamingGroupsSitemapFileInfos = new List<FileInfo>
            {
                new FileInfo("file3"),
                new FileInfo("file4")
            };
            _autoMocker.Get<IGamingGroupsSitemapGenerator>().BuildGamingGroupSitemaps(Arg.Any<DirectoryInfo>()).Returns(_gamingGroupsSitemapFileInfos);

            _staticPagesSitemapFileInfoInfo = new FileInfo("file5");
            _autoMocker.Get<IStaticPagesSitemapGenerator>().BuildStaticPagesSitemap(Arg.Any<DirectoryInfo>()).Returns(_staticPagesSitemapFileInfoInfo);

            var appSettings = Substitute.For<IAppSettings>();
            appSettings.Get(SitemapGeneratorService.AppSettingsKeySitemapLocationFilePath).Returns(_sitemapFileLocation);

            appSettings.Get(SitemapGeneratorService.AppSettingsKeySitemapLocationHttpPath).Returns(_sitemapHttpLocation);
            _autoMocker.Get<IConfigurationManager>().AppSettings.Returns(appSettings);
        }

        [Test]

        public void It_Creates_Sitemaps_For_Universal_Game_Details()
        {
            //--arrange

            //--act
            _autoMocker.ClassUnderTest.RegenerateSitemaps();

            //--assert
            _autoMocker.Get<IUniversalGameSitemapGenerator>().Received().BuildUniversalGamesSitemaps(Arg.Is<DirectoryInfo>(x => x.FullName == _sitemapFileLocation));
        }

        [Test]

        public void It_Creates_Sitemaps_For_Gaming_Groups()
        {
            //--arrange

            //--act
            _autoMocker.ClassUnderTest.RegenerateSitemaps();

            //--assert
            _autoMocker.Get<IGamingGroupsSitemapGenerator>().Received().BuildGamingGroupSitemaps(Arg.Is<DirectoryInfo>(x => x.FullName == _sitemapFileLocation));
        }

        [Test]
        public void It_Creates_Sitemaps_For_The_Static_Site_Pages()
        {
            //--arrange

            //--act
            _autoMocker.ClassUnderTest.RegenerateSitemaps();

            //--assert
            _autoMocker.Get<IStaticPagesSitemapGenerator>().Received().BuildStaticPagesSitemap(Arg.Is<DirectoryInfo>(x => x.FullName == _sitemapFileLocation));
        }

        [Test]
        public void It_Creates_The_Sitemap_Index_File_Using_The_Sitemap_Info_From_The_Generated_Sitemaps_And_The_Directory_From_The_Config()
        {
            //--arrange

            //--act
            _autoMocker.ClassUnderTest.RegenerateSitemaps();

            //--assert
            _autoMocker.Get<ISitemapIndexGenerator>().Received().GenerateSitemapIndex(
                Arg.Is<List<SitemapInfo>>(x => Matches(x)), 
                Arg.Is<DirectoryInfo>(x => x.FullName == _sitemapFileLocation),
                Arg.Is<string>(x => x == "sitemapindex.xml"));
        }

        private bool Matches(List<SitemapInfo> sitemapInfos)
        {
            AssertSitemapInfoWasIncludedInSitemapIndex(sitemapInfos, _universalGamesSitemapFileInfos[0].Name);
            AssertSitemapInfoWasIncludedInSitemapIndex(sitemapInfos, _universalGamesSitemapFileInfos[1].Name);
            AssertSitemapInfoWasIncludedInSitemapIndex(sitemapInfos, _gamingGroupsSitemapFileInfos[0].Name);
            AssertSitemapInfoWasIncludedInSitemapIndex(sitemapInfos, _gamingGroupsSitemapFileInfos[1].Name);
            AssertSitemapInfoWasIncludedInSitemapIndex(sitemapInfos, _staticPagesSitemapFileInfoInfo.Name);
            return true;
        }

        private void AssertSitemapInfoWasIncludedInSitemapIndex(List<SitemapInfo> sitemapInfos, string fileName)
        {
            var expectedFullPathToSitemap = _sitemapHttpLocation + fileName;
            Assert.True(sitemapInfos.Any(
                x => x.AbsolutePathToSitemap == expectedFullPathToSitemap
                     && x.DateLastModified == DateTime.UtcNow.ToString("yyyy-MM-dd")));
        }

        [Test]
        public void It_Returns_The_Job_Result()
        {
            //--arrange
            var clock = new Stopwatch();

            //--act
            clock.Start();
            var result = _autoMocker.ClassUnderTest.RegenerateSitemaps();
            clock.Stop();

            //--assert
            result.ShouldNotBeNull();
            //--the test sometimes runs faster enough that this returns 0
            result.TimeElapsedInMilliseconds.ShouldBeGreaterThanOrEqualTo(0);
            result.TimeElapsedInMilliseconds.ShouldBeLessThanOrEqualTo(clock.ElapsedMilliseconds);
            result.NumberOfSitemapsGenerated.ShouldBe(5);
        }
    }
}
