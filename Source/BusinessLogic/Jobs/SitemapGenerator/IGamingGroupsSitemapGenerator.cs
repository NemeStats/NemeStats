using System.Collections.Generic;
using System.IO;
using X.Web.Sitemap;

namespace BusinessLogic.Jobs.SitemapGenerator
{
    public interface IGamingGroupsSitemapGenerator
    {
        List<FileInfo> BuildGamingGroupSitemaps(DirectoryInfo @is);
    }
}
