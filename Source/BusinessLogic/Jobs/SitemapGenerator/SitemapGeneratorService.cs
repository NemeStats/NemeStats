using System.Diagnostics;

namespace BusinessLogic.Jobs.SitemapGenerator
{
    public class SitemapGeneratorService : ISitemapGeneratorService
    {
        public RegenerateSitemapsJobResult RegenerateSitemaps()
        {
            var clock = new Stopwatch();
            clock.Start();
            throw new System.NotImplementedException();
            clock.Stop();
        }
    }
}