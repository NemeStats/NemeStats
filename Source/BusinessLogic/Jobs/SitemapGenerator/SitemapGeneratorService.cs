using System;
using System.Configuration.Abstractions;
using System.Diagnostics;
using System.Linq;
using BusinessLogic.Logic.BoardGameGeekGameDefinitions;
using X.Web.Sitemap;

namespace BusinessLogic.Jobs.SitemapGenerator
{
    public class SitemapGeneratorService : ISitemapGeneratorService
    {
        private readonly IUniversalGameRetriever _universalGameRetriever;
        private readonly IConfigurationManager _configurationManager;

        public SitemapGeneratorService(IUniversalGameRetriever universalGameRetriever)
        {
            _universalGameRetriever = universalGameRetriever;
        }

        public RegenerateSitemapsJobResult RegenerateSitemaps()
        {
            var clock = new Stopwatch();
            clock.Start();
            BuildUniversalGameDefininitionsSitemaps();
            clock.Stop();
            return null;
        }

        private void BuildUniversalGameDefininitionsSitemaps()
        {
            Sitemap sitemap = new Sitemap();
            var boardGameGeekGameDefinitionIds = _universalGameRetriever.GetAllActiveBoardGameGeekGameDefinitionIds();

            var urls = boardGameGeekGameDefinitionIds.Select(id => new Url
            {
                ChangeFrequency = ChangeFrequency.Daily,
                Location = $"https://nemestats.com/UniversalGame/Details/{id}",
                Priority = .7,
                TimeStamp = DateTime.Now
            });

            //var sitemapLocation = _configurationManager.AppSettings.Get;

            // var url = 

            //sitemap.Add(Url.CreateUrl());
        }
    }
}