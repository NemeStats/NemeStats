using System.Collections.Generic;
using BusinessLogic.Facades;
using BusinessLogic.Logic.GameDefinitions;
using BusinessLogic.Logic.GamingGroups;
using BusinessLogic.Logic.Players;
using BusinessLogic.Logic.Utilities;
using BusinessLogic.Models.Games;
using BusinessLogic.Models.GamingGroups;
using BusinessLogic.Models.Players;
using NUnit.Framework;
using Rhino.Mocks;
using StructureMap.AutoMocking;

namespace BusinessLogic.Tests.UnitTests.FacadesTests
{
    public class TopPlayersRetrieverTests
    {
        private RhinoAutoMocker<TopPlayersRetriever> _autoMocker;

        [SetUp]
        public void SetUp()
        {
            _autoMocker = new RhinoAutoMocker<TopPlayersRetriever>();
        }

        [Test]
        public void ItReturnsResultsFromThePlayerSummaryBuilderIfTheResultsArentAlreadyInTheCache()
        {
            //--arrange
            var expectedResults = new List<TopPlayer>();
            int numberOfPlayersToRetrieve = 1;
            _autoMocker.Get<IPlayerSummaryBuilder>().Expect(mock => mock.GetTopPlayers(numberOfPlayersToRetrieve))
              .Return(expectedResults);

            //--act
            var actualResults = _autoMocker.ClassUnderTest.GetFromSource(numberOfPlayersToRetrieve);

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
