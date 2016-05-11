using System.Runtime.Caching;
using BusinessLogic.Caching;
using NUnit.Framework;
using StructureMap.AutoMocking;

namespace BusinessLogic.Tests.UnitTests.CachingTests.NemeStatsCacheManagerTests
{
    public class NemeStatsCacheManagerTestBase
    {
        protected RhinoAutoMocker<NemeStatsCacheManager> _autoMocker;

        [SetUp]
        public void SetUp()
        {
            _autoMocker = new RhinoAutoMocker<NemeStatsCacheManager>();
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