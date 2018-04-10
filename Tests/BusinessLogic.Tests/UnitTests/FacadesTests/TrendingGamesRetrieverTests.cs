using System.Collections.Generic;
using BusinessLogic.DataAccess.Repositories;
using BusinessLogic.Facades;
using BusinessLogic.Logic.GameDefinitions;
using BusinessLogic.Logic.Utilities;
using BusinessLogic.Models.Games;
using NUnit.Framework;
using Rhino.Mocks;
using StructureMap.AutoMocking;

namespace BusinessLogic.Tests.UnitTests.FacadesTests
{
    public class TrendingGamesRetrieverTests
    {
        private RhinoAutoMocker<TrendingGamesRetriever> _autoMocker;

        [SetUp]
        public void SetUp()
        {
            _autoMocker = new RhinoAutoMocker<TrendingGamesRetriever>();
        }

        [Test]
        public void ItReturnsResultsFromTheGameDefinitionRetrieverIfTheResultsArentAlreadyInTheCache()
        {
            //--arrange
            var expectedResults = new List<TrendingGame>();
            var trendingGamesRequest = new TrendingGamesRequest(1, 2);
            _autoMocker.Get<IGameDefinitionRepository>().Expect(mock => mock.GetTrendingGames(trendingGamesRequest.NumberOfTrendingGamesToShow, trendingGamesRequest.NumberOfDaysOfTrendingGames))
              .Return(expectedResults);

            //--act
            var actualResults = _autoMocker.ClassUnderTest.GetFromSource(trendingGamesRequest);

            //--assert
            Assert.That(actualResults, Is.SameAs(expectedResults));
        }

        [Test]
        public void TheCacheExpirationIsAtTheTopOfTheDay()
        {
            //--arrange
            int expectedNumberOfSeconds = 42;
            _autoMocker.Get<IDateUtilities>().Expect(mock => mock.GetNumberOfSecondsUntilEndOfDay())
                       .Return(expectedNumberOfSeconds);

            //--act
            var actualResults = _autoMocker.ClassUnderTest.GetCacheExpirationInSeconds();

            //--assert
            Assert.That(actualResults, Is.EqualTo(expectedNumberOfSeconds));
        }

    }
}
