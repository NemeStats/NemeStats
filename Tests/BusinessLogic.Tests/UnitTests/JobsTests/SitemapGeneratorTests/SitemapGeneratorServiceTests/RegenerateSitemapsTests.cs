using System.Collections.Generic;
using System.Diagnostics;
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
        private List<SitemapInfo> _universalGamesSitemapInfos;
        private List<SitemapInfo> _gamingGroupsSitemapInfos;
        private SitemapInfo _staticPagesSitemapInfo;


        [SetUp]
        public void SetUp()
        {
            _autoMocker = new NSubstituteAutoMocker<SitemapGeneratorService>();

            _universalGamesSitemapInfos = new List<SitemapInfo>
            {
                new SitemapInfo(),
                new SitemapInfo()
            };
            _autoMocker.Get<IUniversalGameSitemapGenerator>().BuildUniversalGamesSitemaps().Returns(_universalGamesSitemapInfos);

            _gamingGroupsSitemapInfos = new List<SitemapInfo>
            {
                new SitemapInfo(),
                new SitemapInfo()
            };
            _autoMocker.Get<IGamingGroupsSitemapGenerator>().BuildGamingGroupSitemaps().Returns(_gamingGroupsSitemapInfos);

            _staticPagesSitemapInfo = new SitemapInfo();
            _autoMocker.Get<IStaticPagesSitemapGenerator>().BuildStaticPagesSitemap().Returns(_staticPagesSitemapInfo);
        }

        [Test]

        public void It_Creates_Sitemaps_For_Universal_Game_Details()
        {
            //--arrange

            //--act
            _autoMocker.ClassUnderTest.RegenerateSitemaps();

            //--assert
            _autoMocker.Get<IUniversalGameSitemapGenerator>().Received().BuildUniversalGamesSitemaps();
        }

        [Test]

        public void It_Creates_Sitemaps_For_Gaming_Groups()
        {
            //--arrange

            //--act
            _autoMocker.ClassUnderTest.RegenerateSitemaps();

            //--assert
            _autoMocker.Get<IGamingGroupsSitemapGenerator>().Received().BuildGamingGroupSitemaps();
        }

        [Test]
        public void It_Creates_Sitemaps_For_The_Static_Site_Pages()
        {
            //--arrange

            //--act
            _autoMocker.ClassUnderTest.RegenerateSitemaps();

            //--assert
            _autoMocker.Get<IStaticPagesSitemapGenerator>().Received().BuildStaticPagesSitemap();
        }

        [Test]
        public void It_Creates_The_Sitemap_Index_File_Using_The_Sitemap_Info_From_The_Generated_Sitemaps()
        {
            //--arrange

            //--act
            _autoMocker.ClassUnderTest.RegenerateSitemaps();

            //--assert
            _autoMocker.Get<ISitemapIndexGenerator>().Received().GenerateSitemapIndex(Arg.Is<List<SitemapInfo>>(x => Matches(x)));
        }

        private bool Matches(List<SitemapInfo> sitemapInfos)
        {
            sitemapInfos.ShouldContain(_staticPagesSitemapInfo);
            sitemapInfos.ShouldContain(_universalGamesSitemapInfos[0]);
            sitemapInfos.ShouldContain(_universalGamesSitemapInfos[1]);
            sitemapInfos.ShouldContain(_gamingGroupsSitemapInfos[0]);
            sitemapInfos.ShouldContain(_gamingGroupsSitemapInfos[1]);
            return true;
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
