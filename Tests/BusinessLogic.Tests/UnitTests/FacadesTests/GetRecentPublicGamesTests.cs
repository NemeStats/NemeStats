using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.Caching;
using BusinessLogic.Facades;
using BusinessLogic.Logic.PlayedGames;
using BusinessLogic.Models.Games;
using NUnit.Framework;
using Rhino.Mocks;
using StructureMap.AutoMocking;

namespace BusinessLogic.Tests.UnitTests.FacadesTests
{
    [TestFixture]
    public class GetRecentPublicGamesTests
    {
        private RhinoAutoMocker<RecentPublicGamesRetriever> _autoMocker;

        [SetUp]
        public void SetUp()
        {
            _autoMocker = new RhinoAutoMocker<RecentPublicGamesRetriever>();    
        }

        [Test]
        public void ItReturnsTheItemFromTheCacheIfItExists()
        {
            //--arrange
            int gamesToRetrieve = 1;
            var expectedResults = new List<PublicGameSummary>();
            string expectedCacheKey = RecentPublicGamesRetriever.GetCacheKey(gamesToRetrieve);
            _autoMocker.Get<ICacheRetriever>().Expect(mock => mock.TryGetItemFromCache(
                Arg<string>.Is.Equal(expectedCacheKey), 
                out Arg<List<PublicGameSummary>>.Out(expectedResults).Dummy))
                       .Return(true);

            //--act
            var actualResults = _autoMocker.ClassUnderTest.GetRecentPublicGames(gamesToRetrieve);

            //--assert
            Assert.That(actualResults, Is.SameAs(expectedResults));
        }

        [Test]
        public void ItReturnsResultsFromTheDatabaseIfTheyArentAlreadyInTheCache()
        {
            //--arrange
            int gamesToRetrieve = 1;
            var expectedResults = new List<PublicGameSummary>();
            string expectedCacheKey = RecentPublicGamesRetriever.GetCacheKey(gamesToRetrieve);
            _autoMocker.Get<ICacheRetriever>().Expect(mock => mock.TryGetItemFromCache(
                Arg<string>.Is.Equal(expectedCacheKey),
                out Arg<List<PublicGameSummary>>.Out(null).Dummy))
                       .Return(false);
            _autoMocker.Get<IPlayedGameRetriever>().Expect(mock => mock.GetRecentPublicGames(gamesToRetrieve))
                       .Return(expectedResults);

            //--act
            var actualResults = _autoMocker.ClassUnderTest.GetRecentPublicGames(gamesToRetrieve);

            //--assert
            Assert.That(actualResults, Is.SameAs(expectedResults));
        }

        [Test]
        public void ItAddsTheResultsToTheCacheIfItHadToRetrieveFromTheDatabase()
        {
            //--arrange
            int gamesToRetrieve = 1;
            var expectedResults = new List<PublicGameSummary>();
            string expectedCacheKey = RecentPublicGamesRetriever.GetCacheKey(gamesToRetrieve);
            _autoMocker.Get<ICacheRetriever>().Expect(mock => mock.TryGetItemFromCache(
                Arg<string>.Is.Equal(expectedCacheKey),
                out Arg<List<PublicGameSummary>>.Out(null).Dummy))
                       .Return(false);
            _autoMocker.Get<IPlayedGameRetriever>().Expect(mock => mock.GetRecentPublicGames(gamesToRetrieve))
                       .Return(expectedResults);

            //--act
            var actualResults = _autoMocker.ClassUnderTest.GetRecentPublicGames(gamesToRetrieve);

            //--assert
            Assert.That(actualResults, Is.SameAs(expectedResults));
        }
    }
}
