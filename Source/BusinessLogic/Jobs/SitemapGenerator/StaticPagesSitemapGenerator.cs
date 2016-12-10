using System;
using System.Collections.Generic;
using System.IO;
using X.Web.Sitemap;

namespace BusinessLogic.Jobs.SitemapGenerator
{
    public class StaticPagesSitemapGenerator : IStaticPagesSitemapGenerator
    {
        private readonly ISitemapGenerator _sitemapGenerator;

        public StaticPagesSitemapGenerator(ISitemapGenerator sitemapGenerator)
        {
            _sitemapGenerator = sitemapGenerator;
        }

        public FileInfo BuildStaticPagesSitemap(DirectoryInfo targetDirectory)
        {
            var urls = new List<Url>
            {
                new Url
                {
                    ChangeFrequency = ChangeFrequency.Daily,
                    Location = "https://nemestats.com/",
                    Priority = .9,
                    TimeStamp = DateTime.UtcNow.Date
                },
                 new Url
                {
                    ChangeFrequency = ChangeFrequency.Daily,
                    Location = "https://nemestats.com/GameDefinition/ShowTrendingGames",
                    Priority = .9,
                    TimeStamp = DateTime.UtcNow.Date
                },
                new Url
                {
                    ChangeFrequency = ChangeFrequency.Daily,
                    Location = "https://nemestats.com/PlayedGame/ShowRecentlyPlayedGames",
                    Priority = .9,
                    TimeStamp = DateTime.UtcNow.Date
                },
                new Url
                {
                    ChangeFrequency = ChangeFrequency.Monthly,
                    Location = "https://nemestats.com/Home/About",
                    Priority = .9,
                    TimeStamp = DateTime.UtcNow.Date
                },
                new Url
                {
                    ChangeFrequency = ChangeFrequency.Daily,
                    Location = "https://nemestats.com/achievements",
                    Priority = .9,
                    TimeStamp = DateTime.UtcNow.Date
                },
                
                new Url
                {
                    ChangeFrequency = ChangeFrequency.Daily,
                    Location = "https://nemestats.com/achievements/recent-unlocks/",
                    Priority = .8,
                    TimeStamp = DateTime.UtcNow.Date
                },
                new Url
                {
                    ChangeFrequency = ChangeFrequency.Weekly,
                    Location = "https://nemestats.com/GamingGroup/GetTopGamingGroups",
                    Priority = .8,
                    TimeStamp = DateTime.UtcNow.Date
                },
                new Url
                {
                    ChangeFrequency = ChangeFrequency.Daily,
                    Location = "https://nemestats.com/Player/ShowTopPlayers",
                    Priority = .8,
                    TimeStamp = DateTime.UtcNow.Date
                },
                new Url
                {
                    ChangeFrequency = ChangeFrequency.Monthly,
                    Location = "https://nemestats.com/Home/AboutBadgesAndAchievements",
                    Priority = .8,
                    TimeStamp = DateTime.UtcNow.Date
                },
                new Url
                {
                    ChangeFrequency = ChangeFrequency.Monthly,
                    Location = "https://nemestats.com/Home/AboutNemePoints",
                    Priority = .8,
                    TimeStamp = DateTime.UtcNow.Date
                },
                new Url
                {
                    ChangeFrequency = ChangeFrequency.Monthly,
                    Location = "https://nemestats.com/Account/Login",
                    Priority = .8,
                    TimeStamp = DateTime.UtcNow.Date
                }
            };

            return _sitemapGenerator.GenerateSitemaps(urls, targetDirectory, "staticpagessitemap")[0];
        }
    }
}