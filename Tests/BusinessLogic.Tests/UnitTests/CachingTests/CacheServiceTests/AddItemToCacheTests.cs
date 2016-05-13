using System.Runtime.Caching;
using NUnit.Framework;

namespace BusinessLogic.Tests.UnitTests.CachingTests.CacheServiceTests
{
    [TestFixture]
    public class AddItemToCacheWithAbsoluteExpirationTests : CacheServiceTestBase
    {
        [Test]
        public void ItAddsTheItemToTheCache()
        {
            //--arrange
            string cacheKey = "some cache key";
            object someObject = new object();

            //--act
            _autoMocker.ClassUnderTest.AddItemToCache(cacheKey, someObject, 123);

            //--assert
            Assert.True(MemoryCache.Default.Contains(cacheKey));
            var itemFromCache = MemoryCache.Default.Get(cacheKey);
            Assert.That(itemFromCache, Is.SameAs(someObject));        }

    }
}
