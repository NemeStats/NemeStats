using System;
using System.IO;
using X.Web.Sitemap;

namespace BusinessLogic.Jobs.SitemapGenerator
{
    public class StaticPagesSitemapGenerator : IStaticPagesSitemapGenerator
    {
        FileInfo IStaticPagesSitemapGenerator.BuildStaticPagesSitemap(DirectoryInfo @is)
        {
            throw new NotImplementedException();
        }
    }
}