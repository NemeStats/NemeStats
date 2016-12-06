using System;
using System.Collections.Generic;
using BusinessLogic.Jobs.SitemapGenerator;
using BusinessLogic.Logic.BoardGameGeekGameDefinitions;
using NSubstitute;
using NSubstitute.Core;
using NUnit.Framework;
using NSubstituteAutoMocker;
using Shouldly;
using X.Web.Sitemap;

namespace BusinessLogic.Tests.UnitTests.JobsTests.SitemapGeneratorTests
{
    public class BuildUniversalGamesSitemapsTests
    {
        private List<int> _expectedUniversalGameIds;

        private NSubstituteAutoMocker<UniversalGameSitemapGenerator> _autoMocker;

        [SetUp]
        public void SetUp()
        {
            _autoMocker = new NSubstituteAutoMocker<UniversalGameSitemapGenerator>();

            var gameRetrieverMock = _autoMocker.Get<IUniversalGameRetriever>();

            _expectedUniversalGameIds = new List<int>
            {
                1,
                2
            };
            gameRetrieverMock.GetAllActiveBoardGameGeekGameDefinitionIds().Returns(_expectedUniversalGameIds);
        }

        [Test]
        public void It_Builds_Sitemaps_For_All_Active_BoardGameGeekGameDefinitions()
        {
            //--arrange
            List<Url> generateSiteMapArgs = null;
            _autoMocker.Get<ISitemapGenerator>().GenerateSitemaps(Arg.Do<List<Url>>(x => generateSiteMapArgs = x));

            //--act
            _autoMocker.ClassUnderTest.BuildUniversalGamesSitemaps();

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
        public void It_Returns_The_Sitemap_Info_For_Sitemaps_That_Were_Generated()
        {
            //--act
            var siteMapInfo = _autoMocker.ClassUnderTest.BuildUniversalGamesSitemaps();
        }
    }
}
