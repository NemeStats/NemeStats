using X.Web.Sitemap;

namespace BusinessLogic.Jobs.SitemapGenerator
{
    public interface IStaticPagesSitemapGenerator
    {
        SitemapInfo BuildStaticPagesSitemap();
    }
}
