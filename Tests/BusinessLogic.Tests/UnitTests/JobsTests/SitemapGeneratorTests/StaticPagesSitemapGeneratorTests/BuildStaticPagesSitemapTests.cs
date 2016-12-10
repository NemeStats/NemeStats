using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.Jobs.SitemapGenerator;
using NSubstitute;
using NUnit.Framework;
using NSubstituteAutoMocker;
using Shouldly;
using X.Web.Sitemap;

namespace BusinessLogic.Tests.UnitTests.JobsTests.SitemapGeneratorTests.StaticPagesSitemapGeneratorTests
{
    [TestFixture]
    public class BuildStaticPagesSitemapTests
    {
        private NSubstituteAutoMocker<StaticPagesSitemapGenerator> _autoMocker;
        private FileInfo _expectedFileInfo;
        private DirectoryInfo _targetDirectory;

        [SetUp]
        public void SetUp()
        {
            _autoMocker = new NSubstituteAutoMocker<StaticPagesSitemapGenerator>();

            _targetDirectory = new DirectoryInfo("some directory");

            _expectedFileInfo = new FileInfo("some sitemap file");
            _autoMocker.Get<ISitemapGenerator>()
                .GenerateSitemaps(Arg.Any<List<Url>>(), Arg.Any<DirectoryInfo>(), Arg.Any<string>())
                .Returns(new List<FileInfo>
                {
                    _expectedFileInfo
                });
        }

        [Test]
        public void It_Saves_A_Single_Sitemap_To_The_Specified_Directory()
        {
            //--arrange

            //--act
            var result = _autoMocker.ClassUnderTest.BuildStaticPagesSitemap(_targetDirectory);

            //--assert
            _autoMocker.Get<ISitemapGenerator>()
                .Received()
                .GenerateSitemaps(
                    Arg.Any<List<Url>>(), 
                    Arg.Is<DirectoryInfo>(x => x == _targetDirectory),
                    Arg.Is<string>(x => x == "staticpagessitemap"));

            result.ShouldBe(_expectedFileInfo);
        }

        [Test]
        [TestCase("https://nemestats.com/", .9, ChangeFrequency.Daily)]
        [TestCase("https://nemestats.com/GameDefinition/ShowTrendingGames", .9, ChangeFrequency.Daily)]
        [TestCase("https://nemestats.com/PlayedGame/ShowRecentlyPlayedGames", .9, ChangeFrequency.Daily)]
        [TestCase("https://nemestats.com/Home/About", .9, ChangeFrequency.Monthly)]
        [TestCase("https://nemestats.com/achievements", .9, ChangeFrequency.Daily)]
        [TestCase("https://nemestats.com/achievements/recent-unlocks/", .8, ChangeFrequency.Daily)]
        [TestCase("https://nemestats.com/GamingGroup/GetTopGamingGroups", .8, ChangeFrequency.Weekly)]
        [TestCase("https://nemestats.com/Player/ShowTopPlayers", .8, ChangeFrequency.Daily)]
        [TestCase("https://nemestats.com/Home/AboutBadgesAndAchievements", .8, ChangeFrequency.Monthly)]
        [TestCase("https://nemestats.com/Home/AboutNemePoints", .8, ChangeFrequency.Monthly)]
        [TestCase("https://nemestats.com/Account/Login", .8, ChangeFrequency.Monthly)]
        public void It_Saves_The_Most_Important_Static_Pages_In_The_Sitemap(string url, double priority, ChangeFrequency changeFrequency)
        {
            //--act/assert
            AssertStaticPageIsIncludedInSitemap(url, priority, changeFrequency);
        }

        private void AssertStaticPageIsIncludedInSitemap(string url, double priority, ChangeFrequency changeFrequency)
        {
            _autoMocker.ClassUnderTest.BuildStaticPagesSitemap(_targetDirectory);

            //--assert
            _autoMocker.Get<ISitemapGenerator>()
                .Received()
                .GenerateSitemaps(
                    Arg.Is<List<Url>>(x => MatchUrl(x, url, priority, changeFrequency)),
                    Arg.Any<DirectoryInfo>(),
                    Arg.Any<string>());
        }

        private bool MatchUrl(List<Url> urls, string url, double priority, ChangeFrequency changeFrequency)
        {
            return
                urls.Any(
                    x =>
                        x.ChangeFrequency == changeFrequency && x.Priority == priority && x.Location == url &&
                        x.TimeStamp.Date == DateTime.UtcNow.Date);
        }

        [Test]
        public void It_Returns_The_FileInfo_For_The_Sitemap_That_Was_Generated()
        {
            //--arrange

            //--act
            var result = _autoMocker.ClassUnderTest.BuildStaticPagesSitemap(_targetDirectory);

            //--assert

            result.ShouldBe(_expectedFileInfo);
        }
    }
}
