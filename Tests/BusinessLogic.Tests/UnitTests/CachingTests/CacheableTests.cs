using BusinessLogic.Caching;
using BusinessLogic.Logic.Utilities;
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
            internal PlayerStatistics expectedPlayerStatisticsFromDb;

            public CacheableImplementation(IDateUtilities dateUtilities, ICacheService cacheService) : base(dateUtilities, cacheService)
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
            _autoMocker.Get<ICacheService>().Expect(mock => mock.TryGetItemFromCache(
                Arg<string>.Is.Equal(cacheKey), 
                out Arg<PlayerStatistics>.Out(playerStatisticsIncache).Dummy))
                       .Return(true);

            //--act
            var results = _autoMocker.ClassUnderTest.GetResults(inputValue);

            //--assert
            Assert.That(results, Is.SameAs(playerStatisticsIncache));
        }

        [Test]
        public void ItReturnsTheItemFromTheSourceIfItWasntAlreadyInTheCache()
        {
            //--arrange
            int inputValue = 1;
            _autoMocker.Get<ICacheService>()
                       .Expect(mock => mock.TryGetItemFromCache(
                           Arg<string>.Is.Anything,
                           out Arg<PlayerStatistics>.Out(new PlayerStatistics()).Dummy))
                       .Return(false);

            //--act
            var results = _autoMocker.ClassUnderTest.GetResults(inputValue);

            //--assert
            Assert.That(results, Is.SameAs(_autoMocker.ClassUnderTest.expectedPlayerStatisticsFromDb));
        }

        [Test]
        public void ItAddsTheItemToTheCacheUsingTheSpecifiedNumberOfSecondsToExpireIfItWasntAlreadyInTheCache()
        {
            //--arrange
            int inputValue = 1;
            var cacheKey = _autoMocker.ClassUnderTest.GetCacheKey(inputValue);
            var numberOfSecondsUntilExpiration = _autoMocker.ClassUnderTest.GetCacheExpirationInSeconds();

            //--act
            _autoMocker.ClassUnderTest.GetResults(inputValue);

            //--assert
            _autoMocker.Get<ICacheService>().AssertWasCalled(
                mock => mock.AddItemToCache(
                    Arg<string>.Is.Equal(cacheKey),
                    Arg<PlayerStatistics>.Is.Same(_autoMocker.ClassUnderTest.expectedPlayerStatisticsFromDb),
                    Arg<int>.Is.Equal(numberOfSecondsUntilExpiration)
                ));
        }


        internal class CacheableImplementationWithDefaultExpiration : Cacheable<int, PlayerStatistics>
        {
            public CacheableImplementationWithDefaultExpiration(IDateUtilities dateUtilities, ICacheService cacheService) : base(dateUtilities, cacheService)
            {
            }

            public override PlayerStatistics GetFromSource(int inputParameter)
            {
                return new PlayerStatistics();
            }
        }

        [Test]
        public void ItDefaultsTheCacheExpirationToTheEndOfTheDayIfNotOverridden()
        {
            //--arrange
            var automockerForThisTestOnly = new RhinoAutoMocker<CacheableImplementationWithDefaultExpiration>();
            int inputValue = 1;
            var cacheKey = automockerForThisTestOnly.ClassUnderTest.GetCacheKey(inputValue);
            var numberOfSecondsUntilExpiration = 42;
            automockerForThisTestOnly.Get<IDateUtilities>().Expect(mock => mock.GetNumberOfSecondsUntilEndOfDay())
                       .Return(numberOfSecondsUntilExpiration);

            //--act
            automockerForThisTestOnly.ClassUnderTest.GetResults(inputValue);

            //--assert
            automockerForThisTestOnly.Get<ICacheService>().AssertWasCalled(
                mock => mock.AddItemToCache(
                    Arg<string>.Is.Equal(cacheKey),
                    Arg<PlayerStatistics>.Is.Anything,
                    Arg<int>.Is.Equal(numberOfSecondsUntilExpiration)
                ));
        }
    }
}
