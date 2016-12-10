using System.IO;

namespace BusinessLogic.Jobs.SitemapGenerator
{
    public interface IStaticPagesSitemapGenerator
    {
        FileInfo BuildStaticPagesSitemap(DirectoryInfo targetDirectory);
    }
}
