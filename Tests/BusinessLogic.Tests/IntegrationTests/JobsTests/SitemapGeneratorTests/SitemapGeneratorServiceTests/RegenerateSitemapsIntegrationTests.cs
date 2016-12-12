using System;
using System.Collections.Generic;
using System.Configuration.Abstractions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.Caching;
using BusinessLogic.DataAccess;
using BusinessLogic.DataAccess.Repositories;
using BusinessLogic.Facades;
using BusinessLogic.Jobs.SitemapGenerator;
using BusinessLogic.Logic.BoardGameGeekGameDefinitions;
using BusinessLogic.Logic.GameDefinitions;
using BusinessLogic.Logic.GamingGroups;
using BusinessLogic.Logic.PlayedGames;
using BusinessLogic.Logic.Players;
using BusinessLogic.Logic.Utilities;
using NemeStats.IoC;
using NUnit.Framework;
using StructureMap;
using StructureMap.Graph;
using X.Web.Sitemap;

namespace BusinessLogic.Tests.IntegrationTests.JobsTests.SitemapGeneratorTests.SitemapGeneratorServiceTests
{
    [TestFixture]
    public class RegenerateSitemapsIntegrationTests// : IntegrationTestIoCBase
    {
        [Test]
        [Category("Integration")]
        //[Ignore("This is a long-running integration test that will regenerate all sitemaps")]
        public void It_Regenerates_All_Of_The_Sitemaps_And_The_Sitemap_Index_Files()
        {
            //--arrange
            using (var dataContext = new NemeStatsDataContext())
            {
                ISitemapIndexGenerator sitemapIndexGenerator = new SitemapIndexGenerator();
                var dateUtilities = new DateUtilities();
                var cacheService = new CacheService();
                IBoardGameGeekGameDefinitionInfoRetriever boardGameGeekGameDefinitionInfoRetriever =
                    new BoardGameGeekGameDefinitionInfoRetriever(dateUtilities, cacheService, dataContext);
                IGameDefinitionRetriever gameDefinitionRetriever = new GameDefinitionRetriever(dataContext,
                    new EntityFrameworkPlayerRepository(dataContext), boardGameGeekGameDefinitionInfoRetriever);
                IUniversalStatsRetriever universalStatsRetriever = new UniversalStatsRetriever(dateUtilities, cacheService, dataContext);
                IPlayedGameRetriever playedGameRetriever = new PlayedGameRetriever(dataContext);
                IRecentPublicGamesRetriever recentPublicGamesRetriever = new RecentPublicGamesRetriever(dateUtilities, playedGameRetriever, cacheService);
                IUniversalTopChampionsRetreiver universalTopChampionsRetreiver = new UniversalTopChampionsRetreiver(dateUtilities, cacheService, dataContext);
                IUniversalGameRetriever universalGameRetriever = new UniversalGameRetriever(boardGameGeekGameDefinitionInfoRetriever,
                    gameDefinitionRetriever, dataContext, universalStatsRetriever, recentPublicGamesRetriever, universalTopChampionsRetreiver);
                ISitemapGenerator sitemapGenerator = new SitemapGenerator();
                IUniversalGameSitemapGenerator universalGameSitemapGenerator = new UniversalGameSitemapGenerator(universalGameRetriever, sitemapGenerator);
                IPlayerRepository playerRepository = new EntityFrameworkPlayerRepository(dataContext);
                IPlayerRetriever playerRetriever = new PlayerRetriever(dataContext, playerRepository, playedGameRetriever);
                IGamingGroupRetriever gamingGroupRetriever = new GamingGroupRetriever(dataContext, playerRetriever, gameDefinitionRetriever, playedGameRetriever);
                IGamingGroupsSitemapGenerator gamingGroupsSitemapGenerator = new GamingGroupsSitemapGenerator(sitemapGenerator, gamingGroupRetriever);
                IStaticPagesSitemapGenerator staticPagesSitemapGenerator = new StaticPagesSitemapGenerator(sitemapGenerator);
                IConfigurationManager configurationManager = new ConfigurationManager();

                var sitemapGeneratorService = new SitemapGeneratorService(sitemapIndexGenerator, universalGameSitemapGenerator,
                    gamingGroupsSitemapGenerator, staticPagesSitemapGenerator, configurationManager);

                //--act
                sitemapGeneratorService.RegenerateSitemaps();

                //--assert
                //--go look and see if the sitemaps are there!
            }
        }

    }
}
