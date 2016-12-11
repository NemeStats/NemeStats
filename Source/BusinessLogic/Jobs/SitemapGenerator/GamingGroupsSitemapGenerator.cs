using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BusinessLogic.Logic.GamingGroups;
using X.Web.Sitemap;

namespace BusinessLogic.Jobs.SitemapGenerator
{
    public class GamingGroupsSitemapGenerator : IGamingGroupsSitemapGenerator
    {
        private readonly ISitemapGenerator _sitemapGenerator;
        private readonly IGamingGroupRetriever _gamingGroupRetriever;

        public GamingGroupsSitemapGenerator(ISitemapGenerator sitemapGenerator, IGamingGroupRetriever gamingGroupRetriever)
        {
            _sitemapGenerator = sitemapGenerator;
            _gamingGroupRetriever = gamingGroupRetriever;
        }

        public List<FileInfo> BuildGamingGroupSitemaps(DirectoryInfo targetDirectory)
        {
            var gamingGroupSitemapInfo = _gamingGroupRetriever.GetGamingGroupsSitemapInfo();

            var urls = gamingGroupSitemapInfo.Select(sitemapInfo =>
            {
                var hasPlayWithinLastThirtyDays = sitemapInfo.DateLastGamePlayed.Date >= DateTime.UtcNow.Date.AddDays(-30);
                return new Url
                {
                    ChangeFrequency = hasPlayWithinLastThirtyDays ? ChangeFrequency.Weekly : ChangeFrequency.Yearly,
                    Location = $"https://nemestats.com/GamingGroup/Details/{sitemapInfo.GamingGroupId}",
                    Priority = hasPlayWithinLastThirtyDays ? .7 : .6,
                    TimeStamp = sitemapInfo.DateLastGamePlayed
                };
            }).ToList();

            return _sitemapGenerator.GenerateSitemaps(urls, targetDirectory);
        }
    }
}