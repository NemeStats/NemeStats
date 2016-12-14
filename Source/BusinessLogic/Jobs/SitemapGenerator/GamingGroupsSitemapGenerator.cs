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
                var url = new Url
                {
                    Location = $"https://nemestats.com/GamingGroup/Details/{sitemapInfo.GamingGroupId}"
                };

                if (sitemapInfo.DateLastGamePlayed == DateTime.MinValue)
                {
                    url.Priority = .3;
                    url.ChangeFrequency = ChangeFrequency.Yearly;
                    url.TimeStamp = sitemapInfo.DateCreated;
                }
                else if (sitemapInfo.DateLastGamePlayed < DateTime.UtcNow.Date.AddDays(-30))
                {
                    url.Priority = .5;
                    url.ChangeFrequency = ChangeFrequency.Monthly;
                    url.TimeStamp = sitemapInfo.DateLastGamePlayed;
                }
                else
                {
                    url.Priority = .6;
                    url.ChangeFrequency = ChangeFrequency.Weekly;
                    url.TimeStamp = sitemapInfo.DateLastGamePlayed;
                }
                return url;
            }).ToList();

            return _sitemapGenerator.GenerateSitemaps(urls, targetDirectory, "gaminggroupssitemap");
        }
    }
}