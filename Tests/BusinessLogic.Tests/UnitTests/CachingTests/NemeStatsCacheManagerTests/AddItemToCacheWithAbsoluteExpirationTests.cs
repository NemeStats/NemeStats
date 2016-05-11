using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.Caching;
using NUnit.Framework;
using StructureMap.AutoMocking;

namespace BusinessLogic.Tests.UnitTests.CachingTests.NemeStatsCacheManagerTests
{
    [TestFixture]
    public class AddItemToCacheWithAbsoluteExpirationTests : NemeStatsCacheManagerTestBase
    {
        [Test]
        public void ItAddsTheItemToTheCache()
        {
            //--arrange
            string cacheKey = "some cache key";
            object someObject = new object();

            //--act
            _autoMocker.ClassUnderTest.AddItemToCacheWithAbsoluteExpiration(cacheKey, someObject, 123);

            //--assert
            Assert.True(MemoryCache.Default.Contains(cacheKey));
            var itemFromCache = MemoryCache.Default.Get(cacheKey);
            Assert.That(itemFromCache, Is.SameAs(someObject));        }

    }
}
