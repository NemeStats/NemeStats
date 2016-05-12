using System.Runtime.Caching;
using NUnit.Framework;

namespace BusinessLogic.Tests.UnitTests.CachingTests.CacheServiceTests
{
    [TestFixture]
    public class EvictItemFromCacheTests : CacheServiceTestBase
    {
        [Test]
        public void ItDoesNothingIfItAttemptsToEvictSomethingThatIsntInTheCache()
        {
            //--act
            _autoMocker.ClassUnderTest.EvictItemFromCache("some cache key that isn't in the cache");
        }

        [Test]
        public void ItEvictsAnItemFromTheCache()
        {
            //--arrange
            string cacheKey = "key";
            _autoMocker.ClassUnderTest.AddItemToCache(cacheKey, "some value", 10000);

            //--act
            _autoMocker.ClassUnderTest.EvictItemFromCache(cacheKey);

            //--assert
            Assert.False(MemoryCache.Default.Contains(cacheKey));
        }
    }
}
