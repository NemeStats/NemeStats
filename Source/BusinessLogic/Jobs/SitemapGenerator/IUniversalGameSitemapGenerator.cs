using System.Collections.Generic;
using System.IO;

namespace BusinessLogic.Jobs.SitemapGenerator
{
    public interface IUniversalGameSitemapGenerator
    {
        List<FileInfo> BuildUniversalGamesSitemaps(DirectoryInfo targetDirectory);
    }
}
