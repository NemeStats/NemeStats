using System.Collections.Generic;
using BusinessLogic.Facades;
using BusinessLogic.Logic.GamingGroups;
using BusinessLogic.Logic.Utilities;
using BusinessLogic.Models.GamingGroups;
using NUnit.Framework;
using Rhino.Mocks;
using StructureMap.AutoMocking;

namespace BusinessLogic.Tests.UnitTests.FacadesTests
{
    public class TopGamingGroupsRetrieverTests
    {
        private RhinoAutoMocker<TopGamingGroupsRetriever> _autoMocker;

        [SetUp]
        public void SetUp()
        {
            _autoMocker = new RhinoAutoMocker<TopGamingGroupsRetriever>();
        }

        [Test]
        public void ItReturnsResultsFromThePlayedGameRetrieverIfTheResultsArentAlreadyInTheCache()
        {
            //--arrange
            int groupsToRetrieve = 1;
            var expectedResults = new List<TopGamingGroupSummary>();
            _autoMocker.Get<IGamingGroupRetriever>().Expect(mock => mock.GetTopGamingGroups(groupsToRetrieve))
              .Return(expectedResults);

            //--act
            var actualResults = _autoMocker.ClassUnderTest.GetFromSource(groupsToRetrieve);

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
