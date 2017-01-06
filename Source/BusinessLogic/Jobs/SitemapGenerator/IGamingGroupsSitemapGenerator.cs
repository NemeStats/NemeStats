using System.Collections.Generic;
using System.IO;

namespace BusinessLogic.Jobs.SitemapGenerator
{
    public interface IGamingGroupsSitemapGenerator
    {
        List<FileInfo> BuildGamingGroupSitemaps(DirectoryInfo @is);
    }
}
