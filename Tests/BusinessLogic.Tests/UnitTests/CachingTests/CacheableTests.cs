using System;
using System.Runtime.Caching;
using BusinessLogic.Caching;
using BusinessLogic.Models.Players;
using NUnit.Framework;

namespace BusinessLogic.Tests.UnitTests.CachingTests
{
    [TestFixture]
    public class CacheableTests
    {
        private CacheableImplementation _classUnderTest;

        [SetUp]
        public void SetUp()
        {
            _classUnderTest = new CacheableImplementation();
        }

        [TearDown]
        public void TearDown()
        {
            //this is the worst. Dispose renders the memory cache unusable so have to handle it this way.
            foreach (var key in MemoryCache.Default)
            {
                MemoryCache.Default.Remove(key.Key);
            }
        }

        internal class CacheableImplementation : Cacheable<int, PlayerStatistics>
        {
            internal static readonly int CACHE_EXPIRATION_IN_SECONDS = 123;
            internal static readonly string CACHE_PREFIX = "CP";
            internal PlayerStatistics expectedPlayerStatisticsFromDb;

            internal CacheableImplementation()
            {
                expectedPlayerStatisticsFromDb = new PlayerStatistics();
            }

            internal override int GetCacheExpirationInSeconds()
            {
                return CACHE_EXPIRATION_IN_SECONDS;
            }

            internal override PlayerStatistics GetFromSource(int inputParameter)
            {
                return expectedPlayerStatisticsFromDb;
            }
        }

        [Test]
        public void ItReturnsTheItemFromTheCacheIfItExists()
        {
            //--arrange
            int inputValue = 1;
            var cacheKey = _classUnderTest.GetCacheKey(inputValue);
            var playerStatisticsIncache = new PlayerStatistics();
            var cacheItem = new CacheItem(cacheKey, playerStatisticsIncache);
            var cacheItemPolicy = new CacheItemPolicy
            {
                AbsoluteExpiration = new DateTimeOffset(
                    DateTime.UtcNow.AddSeconds(1000000))
            };
            MemoryCache.Default.Add(cacheItem, cacheItemPolicy);

            //--act
            var results = _classUnderTest.GetResults(inputValue);

            //--assert
            Assert.That(results, Is.SameAs(playerStatisticsIncache));
        }

        [Test]
        public void ItAddsTheItemToTheCacheAfterRetrievingItFromSourceIfItWasntAlreadyInTheCache()
        {
            //--arrange
            int inputValue = 1;

            //--act
            var results = _classUnderTest.GetResults(inputValue);

            //--assert
            Assert.That(results, Is.SameAs(_classUnderTest.expectedPlayerStatisticsFromDb));
        }
    }
}
