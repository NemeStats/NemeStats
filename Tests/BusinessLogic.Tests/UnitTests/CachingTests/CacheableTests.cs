using BusinessLogic.Caching;
using BusinessLogic.Models.Players;
using NUnit.Framework;
using Rhino.Mocks;
using StructureMap.AutoMocking;

namespace BusinessLogic.Tests.UnitTests.CachingTests
{
    [TestFixture]
    public class CacheableTests
    {
        private RhinoAutoMocker<CacheableImplementation> _autoMocker;

        [SetUp]
        public void SetUp()
        {
            _autoMocker = new RhinoAutoMocker<CacheableImplementation>();
        }

        internal class CacheableImplementation : Cacheable<int, PlayerStatistics>
        {
            internal static readonly int CACHE_EXPIRATION_IN_SECONDS = 123;
            internal static readonly string CACHE_PREFIX = "CP";
            internal PlayerStatistics expectedPlayerStatisticsFromDb;

            public CacheableImplementation(INemeStatsCacheManager cacheManager) : base(cacheManager)
            {
                expectedPlayerStatisticsFromDb = new PlayerStatistics();
            }

            public override int GetCacheExpirationInSeconds()
            {
                return CACHE_EXPIRATION_IN_SECONDS;
            }

            public override PlayerStatistics GetFromSource(int inputParameter)
            {
                return expectedPlayerStatisticsFromDb;
            }
        }

        [Test]
        public void ItReturnsTheItemFromTheCacheIfItExists()
        {
            //--arrange
            int inputValue = 1;
            var cacheKey = _autoMocker.ClassUnderTest.GetCacheKey(inputValue);
            var playerStatisticsIncache = new PlayerStatistics();
            _autoMocker.Get<INemeStatsCacheManager>().Expect(mock => mock.TryGetItemFromCache(
                Arg<string>.Is.Equal(cacheKey), 
                out Arg<PlayerStatistics>.Out(playerStatisticsIncache).Dummy))
                       .Return(true);

            //--act
            var results = _autoMocker.ClassUnderTest.GetResults(inputValue);

            //--assert
            Assert.That(results, Is.SameAs(playerStatisticsIncache));
        }

        [Test]
        public void ItAddsTheItemToTheCacheIfItWasntAlreadyInTheCache()
        {
            //--arrange
            int inputValue = 1;
            var cacheKey = _autoMocker.ClassUnderTest.GetCacheKey(inputValue);
            var numberOfSecondsUntilExpiration = _autoMocker.ClassUnderTest.GetCacheExpirationInSeconds();

            //--act
            _autoMocker.ClassUnderTest.GetResults(inputValue);

            //--assert
            _autoMocker.Get<INemeStatsCacheManager>().AssertWasCalled(
                mock => mock.AddItemToCacheWithAbsoluteExpiration(
                    Arg<string>.Is.Equal(cacheKey),
                    Arg<PlayerStatistics>.Is.Same(_autoMocker.ClassUnderTest.expectedPlayerStatisticsFromDb),
                    Arg<int>.Is.Equal(numberOfSecondsUntilExpiration)
                ));
        }

        [Test]
        public void ItReturnsTheItemFromTheSourceIfItWasntAlreadyInTheCache()
        {
            //--arrange
            int inputValue = 1;
            _autoMocker.Get<INemeStatsCacheManager>()
                       .Expect(mock => mock.TryGetItemFromCache(
                           Arg<string>.Is.Anything, 
                           out Arg<PlayerStatistics>.Out(new PlayerStatistics()).Dummy))
                       .Return(false);

            //--act
            var results = _autoMocker.ClassUnderTest.GetResults(inputValue);

            //--assert
            Assert.That(results, Is.SameAs(_autoMocker.ClassUnderTest.expectedPlayerStatisticsFromDb));
        }
    }
}
