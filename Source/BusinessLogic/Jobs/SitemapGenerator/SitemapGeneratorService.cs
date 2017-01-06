using System;
using System.Collections.Generic;
using System.Configuration.Abstractions;
using System.Diagnostics;
using System.IO;
using System.Linq;
using X.Web.Sitemap;

namespace BusinessLogic.Jobs.SitemapGenerator
{
    public class SitemapGeneratorService : ISitemapGeneratorService
    {
        private readonly IUniversalGameSitemapGenerator _universalGameSitemapGenerator;
        private readonly IGamingGroupsSitemapGenerator _gamingGroupsSitemapGenerator;
        private readonly IStaticPagesSitemapGenerator _staticPagesSitemapGenerator;
        private readonly ISitemapIndexGenerator _sitemapIndexGenerator;
        private readonly IConfigurationManager _configurationManager;

        internal const string AppSettingsKeySitemapLocationFilePath = "sitemapLocationFilePath";
        internal const string AppSettingsKeySitemapLocationHttpPath = "sitemapLocationHttpPath";

        public SitemapGeneratorService(
            ISitemapIndexGenerator sitemapIndexGenerator,
            IUniversalGameSitemapGenerator universalGameSitemapGenerator, 
            IGamingGroupsSitemapGenerator gamingGroupsSitemapGenerator, 
            IStaticPagesSitemapGenerator staticPagesSitemapGenerator, 
            IConfigurationManager configurationManager)
        {
            _sitemapIndexGenerator = sitemapIndexGenerator;
            _universalGameSitemapGenerator = universalGameSitemapGenerator;
            _gamingGroupsSitemapGenerator = gamingGroupsSitemapGenerator;
            _staticPagesSitemapGenerator = staticPagesSitemapGenerator;
            _configurationManager = configurationManager;
        }

        public RegenerateSitemapsJobResult RegenerateSitemaps()
        {
            var clock = new Stopwatch();
            clock.Start();

            var appSettings = _configurationManager.AppSettings;
            var targetFileSystemDirectoryPath = appSettings.Get(AppSettingsKeySitemapLocationFilePath);
            var targetFileSystemDirectoryInfo = new DirectoryInfo(targetFileSystemDirectoryPath);

            var fileInfos = new List<FileInfo>(3);

            fileInfos.Add(_staticPagesSitemapGenerator.BuildStaticPagesSitemap(targetFileSystemDirectoryInfo));

            fileInfos.AddRange(_universalGameSitemapGenerator.BuildUniversalGamesSitemaps(targetFileSystemDirectoryInfo));

            fileInfos.AddRange(_gamingGroupsSitemapGenerator.BuildGamingGroupSitemaps(targetFileSystemDirectoryInfo));

            string baseUri = appSettings.Get(AppSettingsKeySitemapLocationHttpPath);
            var dateModified = DateTime.UtcNow.Date;
            var sitemapInfos = fileInfos.Select(x => new SitemapInfo(new Uri(baseUri + x.Name), dateModified)).ToList();
            _sitemapIndexGenerator.GenerateSitemapIndex(sitemapInfos, targetFileSystemDirectoryInfo, "sitemapindex.xml");

            clock.Stop();
            return new RegenerateSitemapsJobResult
            {
                TimeElapsedInMilliseconds = clock.ElapsedMilliseconds,
                NumberOfSitemapsGenerated = fileInfos.Count
            };
        }
    }
}