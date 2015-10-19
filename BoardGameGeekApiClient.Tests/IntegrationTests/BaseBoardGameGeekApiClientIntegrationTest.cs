using BoardGameGeekApiClient.Interfaces;
using BoardGameGeekApiClient.Service;
using NUnit.Framework;

namespace BoardGameGeekApiClient.Tests.IntegrationTests
{
    public abstract class BaseBoardGameGeekApiClientIntegrationTest
    {
        public IBoardGameGeekApiClient ApiClient{ get; set; }

        [TestFixtureSetUp]
        public virtual void FixtureSetUp()
        {
            ApiClient = new BoardGameGeekClient(new ApiDownloaderService());
        }
    }
}