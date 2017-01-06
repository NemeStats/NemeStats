using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BusinessLogic.Logic.BoardGameGeekGameDefinitions;
using X.Web.Sitemap;

namespace BusinessLogic.Jobs.SitemapGenerator
{
    public class UniversalGameSitemapGenerator : IUniversalGameSitemapGenerator
    {
        private readonly IUniversalGameRetriever _universalGameRetriever;
        private readonly ISitemapGenerator _sitemapGenerator;

        public UniversalGameSitemapGenerator(IUniversalGameRetriever universalGameRetriever, ISitemapGenerator sitemapGenerator)
        {
            _universalGameRetriever = universalGameRetriever;
            _sitemapGenerator = sitemapGenerator;
        }

        public List<FileInfo> BuildUniversalGamesSitemaps(DirectoryInfo targetDirectory)
        {
            var boardGameGeekGameDefinitionIds = _universalGameRetriever.GetAllActiveBoardGameGeekGameDefinitionSitemapInfos();

            var urls = boardGameGeekGameDefinitionIds.Select(sitemapInfo =>
            {
                var hasPlayWithinLastThirtyDays = sitemapInfo.DateLastGamePlayed.Date >= DateTime.UtcNow.Date.AddDays(-30);
                return new Url
                {
                    ChangeFrequency = hasPlayWithinLastThirtyDays ? ChangeFrequency.Weekly : ChangeFrequency.Monthly,
                    Location = $"https://nemestats.com/UniversalGame/Details/{sitemapInfo.BoardGameGeekGameDefinitionId}",
                    Priority = hasPlayWithinLastThirtyDays ? .8 : .7,
                    TimeStamp = sitemapInfo.DateLastGamePlayed == DateTime.MinValue ? sitemapInfo.DateCreated : sitemapInfo.DateLastGamePlayed
                };
            }).ToList();

            return _sitemapGenerator.GenerateSitemaps(urls, targetDirectory, "universalgamessitemap");
        }
    }
}