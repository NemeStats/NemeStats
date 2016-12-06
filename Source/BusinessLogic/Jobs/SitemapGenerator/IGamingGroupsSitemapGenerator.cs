using System.Collections.Generic;
using X.Web.Sitemap;

namespace BusinessLogic.Jobs.SitemapGenerator
{
    public interface IGamingGroupsSitemapGenerator
    {
        List<SitemapInfo> BuildGamingGroupSitemaps();
    }
}
