using BoardGameGeekApiClient.Interfaces;
using BoardGameGeekApiClient.Service;
using NUnit.Framework;
using RollbarSharp;
using System.Configuration.Abstractions;

namespace BoardGameGeekApiClient.Tests.IntegrationTests
{
    [Ignore("Integration Test")]
    [TestFixture, Category("Integration")]
    public abstract class BaseBoardGameGeekApiClientIntegrationTest
    {
        public IBoardGameGeekApiClient ApiClient{ get; set; }

        [OneTimeSetUp]
        public virtual void FixtureSetUp()
        {
            ApiClient = new BoardGameGeekClient(new ApiDownloaderService(ConfigurationManager.Instance), new RollbarClient());
        }
    }
}
