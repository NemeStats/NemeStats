using BusinessLogic.Jobs.SitemapGenerator;
using NUnit.Framework;

namespace BusinessLogic.Tests.IntegrationTests.JobsTests.SitemapGeneratorTests.SitemapGeneratorServiceTests
{
    [TestFixture]
    public class RegenerateSitemapsIntegrationTests : IntegrationTestIoCBase
    {
        [Test]
        [Category("Integration")]
        [Ignore("This is a long-running integration test that will regenerate all sitemaps")]
        public void It_Regenerates_All_Of_The_Sitemaps_And_The_Sitemap_Index_Files()
        {
            var sitemapGeneratorService = GetInstance<ISitemapGeneratorService>();

            //--acts
            sitemapGeneratorService.RegenerateSitemaps();
        }
    }
}
