using System.Collections.Generic;
using BusinessLogic.Facades;
using BusinessLogic.Logic.PlayedGames;
using BusinessLogic.Models.Games;
using BusinessLogic.Models.PlayedGames;
using NUnit.Framework;
using Rhino.Mocks;
using StructureMap.AutoMocking;

namespace BusinessLogic.Tests.UnitTests.FacadesTests
{
    [TestFixture]
    public class RecentPublicGamesRetrieverTests
    {
        private RhinoAutoMocker<RecentPublicGamesRetriever> _autoMocker;

        [SetUp]
        public void SetUp()
        {
            _autoMocker = new RhinoAutoMocker<RecentPublicGamesRetriever>();    
        }

        [Test]
        public void ItPullsFromPlayedGameRetrieverIfTheResultsArentAlreadyInTheCache()
        {
            //--arrange
            var expectedResults = new List<PublicGameSummary>();
            var recentlyPlayedGamesFilter = new RecentlyPlayedGamesFilter();
            _autoMocker.Get<IPlayedGameRetriever>().Expect(mock => mock.GetRecentPublicGames(Arg<RecentlyPlayedGamesFilter>.Is.Equal(recentlyPlayedGamesFilter)))
              .Return(expectedResults);

            //--act
            var actualResults = _autoMocker.ClassUnderTest.GetFromSource(recentlyPlayedGamesFilter);

            //--assert
            Assert.That(actualResults, Is.SameAs(expectedResults));
        }

        [Test]
        public void TheCacheExpirationIsOneHour()
        {
            //--act
            var actualResults = _autoMocker.ClassUnderTest.GetCacheExpirationInSeconds();

            //--assert
            Assert.That(actualResults, Is.EqualTo(3600));
        }
    }
}
