using System.IO;
using X.Web.Sitemap;

namespace BusinessLogic.Jobs.SitemapGenerator
{
    public interface IStaticPagesSitemapGenerator
    {
        FileInfo BuildStaticPagesSitemap(DirectoryInfo @is);
    }
}
