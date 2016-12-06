using System.Collections.Generic;
using System.Diagnostics;
using X.Web.Sitemap;

namespace BusinessLogic.Jobs.SitemapGenerator
{
    public class SitemapGeneratorService : ISitemapGeneratorService
    {
        private readonly IUniversalGameSitemapGenerator _universalGameSitemapGenerator;
        private readonly IGamingGroupsSitemapGenerator _gamingGroupsSitemapGenerator;
        private readonly IStaticPagesSitemapGenerator _staticPagesSitemapGenerator;
        private readonly ISitemapIndexGenerator _sitemapIndexGenerator;

        internal const string AppKeySitemapLocation = "sitemapLocation";

        public SitemapGeneratorService(
            ISitemapIndexGenerator sitemapIndexGenerator,
            IUniversalGameSitemapGenerator universalGameSitemapGenerator, 
            IGamingGroupsSitemapGenerator gamingGroupsSitemapGenerator, 
            IStaticPagesSitemapGenerator staticPagesSitemapGenerator)
        {
            _sitemapIndexGenerator = sitemapIndexGenerator;
            _universalGameSitemapGenerator = universalGameSitemapGenerator;
            _gamingGroupsSitemapGenerator = gamingGroupsSitemapGenerator;
            _staticPagesSitemapGenerator = staticPagesSitemapGenerator;
        }

        public RegenerateSitemapsJobResult RegenerateSitemaps()
        {
            var clock = new Stopwatch();
            clock.Start();

            var sitemapInfos = new List<SitemapInfo>(3);

            sitemapInfos.Add(_staticPagesSitemapGenerator.BuildStaticPagesSitemap());

            sitemapInfos.AddRange(_universalGameSitemapGenerator.BuildUniversalGamesSitemaps());

            sitemapInfos.AddRange(_gamingGroupsSitemapGenerator.BuildGamingGroupSitemaps());

            _sitemapIndexGenerator.GenerateSitemapIndex(sitemapInfos);

            clock.Stop();
            return new RegenerateSitemapsJobResult
            {
                TimeElapsedInMilliseconds = clock.ElapsedMilliseconds,
                NumberOfSitemapsGenerated = sitemapInfos.Count
            };
        }
    }
}