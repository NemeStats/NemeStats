using System.Runtime.Caching;
using BusinessLogic.Caching;
using NUnit.Framework;
using StructureMap.AutoMocking;

namespace BusinessLogic.Tests.UnitTests.CachingTests.CacheServiceTests
{
    public class CacheServiceTestBase
    {
        protected RhinoAutoMocker<CacheService> _autoMocker;

        [SetUp]
        public void SetUp()
        {
            _autoMocker = new RhinoAutoMocker<CacheService>();
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
    }
}